using ExampleMod.Buffs;
using StoneForge;
using StoneForge.GameDamageTypes;

namespace ExampleMod.DamageTypes;

// A kind of damage of our own: Overcharge, dealt as the game's shock (shock resistance, its effects), but half as
// hard again on a target that's already Shocked (our Shocked buff). Shock Bolt deals it.
public class Overcharge : Shock
{
    private readonly Shocked _shocked;

    public Overcharge(Shocked shocked)
    {
        _shocked = shocked;
        Name = "Overcharge";
    }

    // (StoneForge.Buffs in full: in this mod, plain "Buffs" is its own ExampleMod.Buffs namespace.)
    protected override double Modify(DamageHit hit) => global::StoneForge.Buffs.Has(hit.Target, _shocked) ? hit.Amount * 1.5 : hit.Amount;
}
