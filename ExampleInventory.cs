using ExampleMod.Items;
using StoneForge;

namespace ExampleMod;

// Items, containers and the game's events, to try in game:
// - a chest's right-click menu (any container in the world: a chest, a barrel, a tomb) gets Peek - what's in it, read
//   without opening it (Containers.ContentsJson; a never-opened one has nothing yet: its loot is rolled as it opens) -
//   Stash a Tonic and Stash a Worn Blade - put in it, open or closed (Containers.AddItem: a closed one's saved in it,
//   in the first free cell as it opens; a never-opened one stays so, and they join its loot as it first opens) - and
//   Take a Tonic (RemoveItem);
// - the Esc menu gets Give a Worn Blade: the Example Blade at a quarter of its condition, marked as ours
//   (Inventory.Add, setup: its own data, saved with it);
// - logged: containers opened and closed, items put in and taken out while they're open, gear put on and taken off,
//   your skills used, and quests started, moving on, done and failed;
// - with the "Cheat death once" setting, the first time you'd die you're left at a quarter of your health
//   (Player.OnDying).
public sealed class ExampleInventory
{
    private const string GivenKey = "examplemod:given";

    private readonly ModContext _context;
    private readonly ExampleTonic _tonic;
    private int _container = -2;
    private bool _cheated;

    public ExampleInventory(ModContext context, ExampleSettings settings, ExampleTonic tonic)
    {
        _context = context;
        _tonic = tonic;

        ContextMenus.Add(context, "Peek", IsContainer, Peek, hover: "Example Mod: log what's in it, without opening it");
        ContextMenus.Add(context, "Stash a Tonic", IsContainer, chest => Log(Containers.AddItem(chest, _tonic), "Stashed an Example Tonic", "Couldn't stash a tonic"),
            hover: "Example Mod: put an Example Tonic in it, open or closed");
        ContextMenus.Add(context, "Stash a Worn Blade", IsContainer,
            chest => Log(Containers.AddItem<ExampleBlade>(chest, setup: Wear), "Stashed a worn Example Blade", "Couldn't stash a blade"),
            hover: "Example Mod: put an Example Blade at a quarter of its condition in it");
        ContextMenus.Add(context, "Take a Tonic", IsContainer, chest => context.Log($"Took {Containers.RemoveItem(chest, _tonic)} tonic(s) out"),
            hover: "Example Mod: take an Example Tonic out of it");
        EscMenu.AddBefore(context, EscButton.Settings, "Give a Worn Blade", () =>
        {
            if (Inventory.Add<ExampleBlade>(setup: Wear) is { } blade)
                context.Log($"Gave a worn Example Blade: {blade.Durability:0}/{blade.MaxDurability:0}, ours: {blade.Data(GivenKey)}");
            else
                context.Log("Couldn't give a blade (no game, or no room: it's at your feet)");
        });

        Containers.OnOpened(context, open => context.Log($"Opened {Name(open.Container)}: {open.Items().Count} item(s)"));
        Containers.OnClosed(context, chest => context.Log($"Closed {Name(chest)}"));
        Containers.OnItemAdded(context, (open, item) => context.Log($"Put {item.Name} x{item.Stack} in {Name(open.Container)}"));
        Containers.OnItemRemoved(context, (open, item) => context.Log($"Took {item.Name} out of {Name(open.Container)}"));
        Inventory.OnEquipped(context, (item, on) => context.Log($"{(on ? "Put on" : "Took off")} {item.Name} ({item.DurabilityPercent:0}%)"));
        StoneForge.Skills.OnUsed(context, cast =>
        {
            if (cast.Caster.Instance.Equals(Player.Instance))
                context.Log($"Used {Name(cast.Skill.Instance)}{(cast.IsCrit ? " - a miracle!" : "")}");
        });
        Quests.OnStarted(context, quest => context.Log($"Quest started: {quest}"));
        Quests.OnProgress(context, (quest, task, value) => context.Log($"Quest {quest}: {task} now {value}"));
        Quests.OnCompleted(context, quest => context.Log($"Quest done: {quest}"));
        Quests.OnFailed(context, quest => context.Log($"Quest failed: {quest}"));
        Player.OnDying(context, () =>
        {
            if (!settings.CheatDeath.Value || _cheated)
                return false;
            _cheated = true;
            Instance player = Player.Instance;
            player["HP"] = player.Get("max_hp").AsReal / 4;
            context.Log("Cheated death - once");
            return true;
        });
    }

    // A worn blade: a quarter of its condition, and marked as given by us (its own data: kept and saved with it).
    private static void Wear(InventoryItem blade)
    {
        blade.DurabilityPercent = 25;
        blade.SetData(GivenKey, true);
    }

    private void Peek(Instance chest)
    {
        string? json = Containers.ContentsJson(chest);
        if (json == null)
            _context.Log($"{Name(chest)} is open: look in it");
        else if (!Containers.HasBeenOpened(chest))
            // (Never opened: its loot is rolled as it first opens - what's been stashed in it joins it then.)
            _context.Log($"{Name(chest)} hasn't been opened: its loot is rolled as it is; stashed in it so far: {json}");
        else
            _context.Log($"{Name(chest)} holds: {json}");
    }

    private bool IsContainer(Instance target)
    {
        if (_container == -2)
            _container = Gm.AssetGetIndex("c_container");
        int obj = target.Get("object_index").AsInt;
        return _container >= 0 && (obj == _container || Gm.ObjectIsAncestor(obj, _container));
    }

    private static string Name(Instance instance) => instance.IsNone ? "?" : Gm.ObjectGetName(instance.Get("object_index").AsInt);

    private void Log(bool done, string yes, string no) => _context.Log(done ? yes : no);
}
