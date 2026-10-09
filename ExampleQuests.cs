using StoneForge;
namespace ExampleMod;

// A saved journal quest, started from Esc, and a naturally generated dungeon contract.
internal sealed class ExampleQuests
{
    internal ExampleQuests(ModContext context)
    {
        var bounty = context.Quests.Add(new QuestDefinition("local_bounty", "A local bounty", "Defeat three enemies")
        {
            TitleKey = "quests.bounty.title", DescriptionKey = "quests.bounty.description",
            RewardCrowns = 100, RewardExperience = 50,
            Objectives = { new QuestObjective("hunt", "Defeat enemies", 3) { TextKey = "quests.bounty.hunt" } },
            OnReward = _ => context.Log(ExampleText.Get("quests.bounty.rewarded")),
        });
        ExampleActions.Add("quests.bounty.start", () => bounty.Start());

        // The base template supplies the dungeon faction, boss setup and settlement reward rules.
        var job = context.Contracts.Add(new ContractDefinition("dungeon_bounty", "bastion_Clearing",
            "A brigand bounty", "Defeat three foes inside %dungeon_name%, then return for payment")
        {
            TitleKey = "contracts.bounty.title", DescriptionKey = "contracts.bounty.description",
            RewardCrowns = 250, DeadlineHours = 72,
            TravelTextKey = "contracts.bounty.travel", ReturnTextKey = "contracts.bounty.return",
            Objectives = { new QuestObjective("hunt", "Defeat foes in the assigned dungeon", 3) { TextKey = "contracts.bounty.hunt" } },
            OnReward = _ => context.Log(ExampleText.Get("contracts.bounty.rewarded")),
        });
        Units.OnDied(context, (unit, killer) =>
        {
            if (!killer.Equals(Player.Instance)) return;
            if (bounty.IsStarted && !bounty.IsCompleted && !bounty.IsFailed) bounty.Advance("hunt");
            if (WorldMap.Floor <= 0 || WorldMap.PlayerCell is not { } cell) return;
            string position = cell.X + "/" + cell.Y;
            foreach (var contract in job.Instances)
                if (contract.IsTaken && !contract.IsReady && !contract.IsFailed &&
                    contract.Data["Dungeon_Coordinate"].AsString == position) contract.Advance("hunt");
        });
    }
}
