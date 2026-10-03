using System;
using ExampleMod.Buffs;
using StoneForge;

namespace ExampleMod.Skills;

// A passive of our own: always on once learnt (with an ability point, beside Shock Bolt on the Stormcalling tab).
// It gives +5% Crit Chance - in the character sheet, as the game's passives' stats are - and the player's weapon hits
// a 1 in 4 chance to Shock their target for 3 turns (our Shocked buff, so Shock Bolt's overcharge hits it harder).
public class StaticCharge : ModPassive
{
    private readonly Shocked _shocked;

    public StaticCharge(Shocked shocked) : base("static_charge")
    {
        _shocked = shocked;
        DisplayName = "Static Charge";
        Description = "A passive made in C#: +5% Crit Chance, and weapon hits have a 25% chance to Shock their target for 3 turns.";
        Icon = "static_charge.png";
        Tab = "Stormcalling";
        Group = "Example";
        Set(BuffStat.CRT, 5);
    }

    protected override void OnHit(Attack attack)
    {
        if (Random.Shared.NextDouble() < 0.25 && Context.Buffs.Apply(_shocked, attack.Target, 3, attack.Attacker) != null)
            Context.Log("Static Charge: shocked the target");
    }
}
