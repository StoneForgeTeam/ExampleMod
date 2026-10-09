using StoneForge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExampleMod;

// Three small jobs, assigned to distinct Osbrook townspeople once per saved character.
// The giver's stable id_name is saved in the quest, never an instance ID from a room.
internal sealed class ExampleNpcJobs : ITickable
{
    private static readonly string[] Givers =
    {
        "osbrook_smith", "osbrook_tailor", "osbrook_herbalist", "osbrook_carpenter",
        "osbrook_butcher", "osbrook_fruittrader", "osbrook_foodtrader",
        "osbrook_merchant", "osbrook_innkeeper", "osbrook_stableman",
    };
    private const string GiverKey = "example_giver";
    private static ExampleNpcJobs? _current;
    private readonly Dictionary<string, RegisteredDialogue> _dialogues = new();
    private readonly ModContext _context;
    private readonly Job[] _jobs;
    private bool _asking;

    private sealed record Job(string Key, string? Item, int Count, int Crowns, int Xp, CustomQuest Quest);

    internal ExampleNpcJobs(ModContext context)
    {
        _context = context;
        _current = this;
        _jobs = new[]
        {
            Add("herbs", "peppermint", 3, 90, 30),
            Add("food", "bread", 2, 80, 25),
            Add("brigands", null, 3, 150, 75),
        };
        context.AddTickable(this);
        foreach (var job in _jobs) AddDialogue(job);
        // A small ledger makes the random employers easy to find, even after loading a save.
        ExampleActions.Add("npcjobs.ledger.button", ShowLedger);
        Units.OnDied(context, (unit, killer) =>
        {
            var job = _jobs[2];
            if (!job.Quest.IsStarted || job.Quest.IsCompleted || job.Quest.IsFailed ||
                !killer.Equals(Player.Instance) || WorldMap.Floor != 0 || WorldMap.PlayerCell is not { } cell) return;
            var data = job.Quest.Data!.Value;
            if (!data.Has("example_osbrook_x") || !data.Has("example_osbrook_y")) return;
            int distance = Math.Max(Math.Abs(cell.X - data["example_osbrook_x"].AsInt),
                Math.Abs(cell.Y - data["example_osbrook_y"].AsInt));
            string objectName = Gm.ObjectGetName(unit.Get("object_index").AsInt);
            if (distance <= 3 && objectName.StartsWith("o_bandit_", StringComparison.Ordinal) &&
                !objectName.StartsWith("o_bandit_dog", StringComparison.Ordinal)) job.Quest.Advance("work");
        });
    }

    private Job Add(string key, string? item, int count, int crowns, int xp)
    {
        string prefix = "npcjobs." + key;
        var quest = _context.Quests.Add(new QuestDefinition("osbrook_" + key,
            ExampleText.Get(prefix + ".title"), ExampleText.Get(prefix + ".description"))
        {
            TitleKey = prefix + ".title", DescriptionKey = prefix + ".description",
            AutoComplete = false, RewardCrowns = crowns, RewardExperience = xp,
            Objectives =
            {
                new QuestObjective("work", ExampleText.Get(prefix + ".objective"), count)
                    { TextKey = prefix + ".objective" },
                new QuestObjective("return", "Return to your employer in Osbrook") { TextKey = "npcjobs.return" },
            },
            OnReward = _ => _context.Log(ExampleText.Get("npcjobs.rewarded", ExampleText.Get(prefix + ".title"))),
        });
        return new Job(key, item, count, crowns, xp, quest);
    }

    private void AssignGivers()
    {
        if (!WorldMap.Available || !Player.Exists || _jobs.Any(job => !job.Quest.IsAvailable)) return;
        if (_jobs.All(job => job.Quest.Data!.Value.Get(GiverKey, "").AsString.Length > 0)) return;
        var remaining = new List<string>(Givers);
        foreach (var job in _jobs)
            if (job.Quest.Data is { } data && data.Get(GiverKey, "").AsString is { Length: > 0 } saved)
                remaining.Remove(saved);
        foreach (var job in _jobs)
        {
            var data = job.Quest.Data!.Value;
            if (data.Get(GiverKey, "").AsString.Length != 0) continue;
            int pick = Gm.Irandom(remaining.Count - 1);
            data[GiverKey] = remaining[pick];
            remaining.RemoveAt(pick);
        }
    }

    public void Tick(double deltaTime) => AssignGivers();

    private Job? JobFor(Instance npc)
    {
        if (!Player.Exists || !npc.Exists || !npc.Get("is_life").AsBool || !WorldMap.Available || WorldMap.InPrologue) return null;
        string id = npc.Get("id_name") is { Kind: GmKind.String } value ? value.AsString : "";
        return _jobs.FirstOrDefault(job => job.Quest.Data?.Get(GiverKey, "").AsString == id && id.Length > 0);
    }
    private static bool Nearby(Instance npc) => Player.Exists && npc.Exists &&
        Units.CellOf(npc).DistanceTo(Units.CellOf(Player.Instance)) <= 2;
    private static bool Finished(Job job) => job.Quest.IsCompleted || job.Quest.IsFailed;

    private void AddDialogue(Job job)
    {
        var dialogue = _context.Dialogues.Add(new DialogueDefinition("osbrook_" + job.Key, "offer")
        {
            Nodes =
            {
                new DialogueNode("offer", "A small job")
                {
                    TextKey = "npcjobs.dialogue.offer",
                    TextArguments = _ => new object?[] {
                        ExampleText.Get("npcjobs." + job.Key + ".request"), job.Crowns, job.Xp },
                    Choices =
                    {
                        new DialogueChoice("accept", "I'll do it.", "accepted")
                        {
                            TextKey = "npcjobs.dialogue.accept",
                            EnabledWhen = conversation => JobFor(conversation.Speaker) == job && !Finished(job) && !job.Quest.IsStarted && Nearby(conversation.Speaker),
                            OnSelected = conversation => Accept(job, conversation.Speaker),
                        },
                        new DialogueChoice("decline", "Not right now.") { TextKey = "npcjobs.dialogue.decline" },
                    },
                },
                new DialogueNode("accepted", "Thank you. Come back when you're done.")
                {
                    TextKey = "npcjobs.dialogue.accepted",
                    Choices = { new DialogueChoice("leave", "Goodbye.") { TextKey = "npcjobs.dialogue.leave" } },
                },
                new DialogueNode("report", "How is the job going?")
                {
                    TextKey = job.Item == null ? "npcjobs.dialogue.hunt_report" : "npcjobs.dialogue.delivery_report",
                    TextArguments = _ => new object?[] { job.Quest.Progress("work"), job.Count, ReportHint(job) },
                    Choices =
                    {
                        new DialogueChoice("submit", "Here's what you asked for.")
                        {
                            TextProvider = _ => ExampleText.Get(job.Quest.Progress("work") >= job.Count ? "npcjobs.dialogue.claim_reward" :
                                job.Item == null ? "npcjobs.dialogue.report_progress" : "npcjobs.dialogue.hand_over"),
                            EnabledWhen = conversation => CanTurnIn(job, conversation.Speaker),
                            OnSelected = conversation => TurnIn(job, conversation),
                        },
                        new DialogueChoice("later", "I'm still working on it.") { TextKey = "npcjobs.dialogue.later" },
                    },
                },
                new DialogueNode("partial", "Thank you. I still need a little more.")
                {
                    TextKey = "npcjobs.dialogue.partial",
                    TextArguments = _ => new object?[] { job.Quest.Progress("work"), job.Count },
                    Choices = { new DialogueChoice("leave", "Goodbye.") { TextKey = "npcjobs.dialogue.leave" } },
                },
                new DialogueNode("thanks", "Thanks again for your help.")
                {
                    TextKey = "npcjobs.dialogue.thanks",
                    Choices = { new DialogueChoice("leave", "Goodbye.") { TextKey = "npcjobs.dialogue.leave" } },
                },
                new DialogueNode("paid", "Thank you! Here is your payment.")
                {
                    TextKey = "npcjobs.dialogue.paid",
                    TextArguments = _ => new object?[] { job.Crowns, job.Xp },
                    Choices = { new DialogueChoice("leave", "Goodbye.") { TextKey = "npcjobs.dialogue.leave" } },
                },
            },
        });
        _dialogues[job.Key] = dialogue;
        dialogue.AddTopic(npc =>
        {
            AssignGivers();
            return !_asking && JobFor(npc) == job && !job.Quest.IsFailed;
        }, _ => ExampleText.Get(job.Quest.IsCompleted ? "npcjobs.menu.completed" : job.Quest.IsStarted ? "npcjobs.menu.report" : "npcjobs.menu.ask"),
            _ => job.Quest.IsCompleted ? "thanks" : job.Quest.IsStarted ? "report" : "offer");
    }

    private int Carried(Job job) => job.Item == null ? 0 : Inventory.Items()
        .Where(item => string.Equals(item.Name, job.Item, StringComparison.OrdinalIgnoreCase)).Sum(item => item.Stack);
    private bool CanTurnIn(Job job, Instance speaker) => JobFor(speaker) == job && Nearby(speaker) &&
        job.Quest.IsStarted && !Finished(job) && (job.Quest.Progress("work") >= job.Count || Carried(job) > 0);
    private string ReportHint(Job job) => job.Quest.Progress("work") >= job.Count ? ExampleText.Get("npcjobs.dialogue.ready") :
        job.Item == null ? ExampleText.Get("npcjobs.dialogue.hunt_missing", job.Count - job.Quest.Progress("work")) :
        Carried(job) > 0 ? ExampleText.Get("npcjobs.dialogue.can_deliver", Carried(job)) :
        ExampleText.Get("npcjobs.dialogue.supplies_missing", job.Count - job.Quest.Progress("work"), ExampleText.Get("npcjobs." + job.Key + ".supply"));
    private void Accept(Job job, Instance speaker)
    {
        if (JobFor(speaker) != job || !Nearby(speaker) || Finished(job) || job.Quest.IsStarted) return;
        if (job.Item == null && WorldMap.PlayerCell is { } cell)
        {
            var data = job.Quest.Data!.Value;
            data["example_osbrook_x"] = cell.X; data["example_osbrook_y"] = cell.Y;
        }
        job.Quest.Start();
    }
    private static ExampleNpcJobs? Owner(DialogOptionContext dialogue) => _current is { } current &&
        current._context == dialogue.Mod ? current : null;
    [DialogCondition("accept_job_condition")]
    public static DialogConditionResult AcceptJobCondition(DialogOptionContext dialogue) => Owner(dialogue) is { } owner &&
        owner.JobFor(dialogue.Speaker) is { } job && Nearby(dialogue.Speaker) && !Finished(job) && !job.Quest.IsStarted
        ? DialogConditionResult.Enabled : DialogConditionResult.Visible;
    [DialogCondition("turn_in_job_condition")]
    public static DialogConditionResult TurnInJobCondition(DialogOptionContext dialogue) => Owner(dialogue) is { } owner &&
        owner.JobFor(dialogue.Speaker) is { } job && owner.CanTurnIn(job, dialogue.Speaker)
        ? DialogConditionResult.Enabled : DialogConditionResult.Visible;
    [DialogOption("accept_job")]
    public static void AcceptJob(DialogOptionContext dialogue)
    {
        if (AcceptJobCondition(dialogue) != DialogConditionResult.Enabled || Owner(dialogue) is not { } owner) return;
        var job = owner.JobFor(dialogue.Speaker)!;
        owner.Accept(job, dialogue.Speaker);
        if (dialogue.Conversation is { } active && active.Id == owner._dialogues[job.Key].Id) active.GoTo("accepted");
        else dialogue.Open(owner._dialogues[job.Key], "accepted");
    }
    [DialogOption("turn_in_job")]
    public static void TurnInJob(DialogOptionContext dialogue)
    {
        if (TurnInJobCondition(dialogue) != DialogConditionResult.Enabled || Owner(dialogue) is not { } owner) return;
        var job = owner.JobFor(dialogue.Speaker)!;
        var conversation = dialogue.Conversation;
        if (conversation?.Id != owner._dialogues[job.Key].Id) conversation = dialogue.Open(owner._dialogues[job.Key], "report");
        if (conversation != null) owner.TurnIn(job, conversation);
    }

    private void TurnIn(Job job, DialogueConversation conversation)
    {
        if (!CanTurnIn(job, conversation.Speaker)) return;
        if (job.Item != null)
        {
            int remaining = job.Count - job.Quest.Progress("work");
            int delivered = Inventory.Remove(job.Item, remaining);
            if (delivered > 0) job.Quest.Advance("work", delivered);
        }
        if (job.Quest.Progress("work") >= job.Count)
        {
            job.Quest.SetProgress("return", 1);
            job.Quest.Complete();
        }
        conversation.GoTo(job.Quest.IsCompleted ? "paid" : "partial");
    }

    private string Giver(Job job) => ExampleText.Get("npcjobs.giver." + job.Quest.Data?.Get(GiverKey, "").AsString);
    private void ShowLedger()
    {
        AssignGivers();
        if (_asking || Dialogues.Active != null || _jobs.Any(job => !job.Quest.IsAvailable) || !Player.Exists || !WorldMap.Available) return;
        string body = ExampleText.Get("npcjobs.ledger.intro") + "\n\n" + string.Join("\n\n", _jobs.Select(job =>
            ExampleText.Get("npcjobs.ledger.entry", ExampleText.Get("npcjobs." + job.Key + ".title"), Giver(job),
                ExampleText.Get(job.Quest.IsCompleted ? "npcjobs.status.done" : job.Quest.IsFailed ? "npcjobs.status.failed" :
                    job.Quest.IsStarted ? "npcjobs.status.active" : "npcjobs.status.available"))));
        _asking = GameDialogs.Confirm(_context, body, () => _asking = false, () => _asking = false);
    }
}
