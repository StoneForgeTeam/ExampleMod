using StoneForge;

namespace ExampleMod.Buffs;

// Two effects of our own, like the game's: an icon by the unit's others, a name and description on hover,
// a duration, stat changes while they last - and anything else in their On... methods.

// A debuff the Example Blade's blows can leave: worse at hitting and dodging, and it hurts every turn.
public class Shocked : ModBuff
{
    public Shocked() : base("shocked", BuffKind.Debuff)
    {
        DisplayName = ExampleText.Get("buffs.examplebuffs.shocked");
        Description = ExampleText.Get("buffs.examplebuffs.sparks_from_an_example_blade_less");
        Icon = "shocked.png";
        Set(BuffStat.Hit_Chance, -15);
        Set(BuffStat.EVS, -10);
        // Lightning crackling over it while it lasts, with a blue glow: one of the game's own animations.
        SetAura(Sprite.s_lightinge_twohand, new FxOptions { Light = Draw.Rgb(110, 170, 255) });
    }

    protected override void OnTurn(Effect effect) => effect.DealDamage(3);
}

// A buff for the player after a kill with the blade.
public class BattleFocus : ModBuff
{
    public BattleFocus() : base("battle_focus")
    {
        DisplayName = ExampleText.Get("buffs.examplebuffs.battle_focus");
        Description = ExampleText.Get("buffs.examplebuffs.the_last_kill_sharpened_your_aim");
        Icon = "focus.png";
        Set(BuffStat.Hit_Chance, 10);
        Set(BuffStat.CRT, 5);
        // The game's red banner glow on the ground at your feet while it lasts (Under: behind you, as the game draws it).
        SetAura(Sprite.s_crimsonBannerBuff, new FxOptions { Under = true, Light = Draw.Rgb(255, 90, 60) });
    }

    protected override void OnApplied(Effect effect) => Context.Log($"Battle Focus on for {effect.Duration} turns");

    protected override void OnRemoved(Effect effect) => Context.Log("Battle Focus wore off");
}
