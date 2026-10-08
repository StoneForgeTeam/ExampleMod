using ExampleMod.Buffs;
using StoneForge;
using StoneForge.GameItems;

namespace ExampleMod.Items;

// A consumable of our own: the game's wine (its sips, sounds and what it does - it's still wine, and goes to
// your head) as a teal tonic that also heals a little and, as it's drunk, focuses you for 5 turns (our Battle
// Focus buff). Its key, written in the base(...) call, gives it its own object in the game - o_inv_examplemod__tonic -
// at the next start. F5 in game gives one.
public class ExampleTonic : Wine
{
    private readonly BattleFocus _focus;

    public ExampleTonic(BattleFocus focus) : base("tonic")
    {
        _focus = focus;
        DisplayName = ExampleText.Get("items.exampletonic.example_tonic");
        Description = ExampleText.Get("items.exampletonic.a_teal_tonic_made_in_c");
        InventorySprite = "tonic_inv.png";
        LootSprite = "tonic_loot.png";
        Set(ConsumableColumn.Health_Restoration, 15);
        Set(ConsumableColumn.Price, 120);
    }

    protected override void OnUse(Instance item)
    {
        if (Instances.First<GameInstance>(GameObjectId.o_player) is { } player)
            Context.Buffs.Apply(_focus, player, 5);
        Context.Log("Example Tonic drunk: focused for 5 turns");
    }
}
