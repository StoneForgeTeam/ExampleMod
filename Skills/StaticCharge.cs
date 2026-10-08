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
        DisplayName = ExampleText.Get("skills.staticcharge.static_charge");
        Description = ExampleText.Get("skills.staticcharge.a_passive_made_in_c_crit");
        Icon = "static_charge.png";
        Tab = ExampleText.Get("skills.exampleskill.stormcalling");
        Group = ExampleText.Get("examplemod.example");
        Set(BuffStat.CRT, 5);
    }

    protected override void OnHit(Attack attack)
    {
        if (Random.Shared.NextDouble() < 0.25 && Context.Buffs.Apply(_shocked, attack.Target, 3, attack.Attacker) != null)
            Context.Log("Static Charge: shocked the target");
    }
}
