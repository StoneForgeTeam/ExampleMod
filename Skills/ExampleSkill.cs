using ExampleMod.Buffs;
using StoneForge;
using StoneForge.GameSkills;

namespace ExampleMod.Skills;

// A skill of our own: the game's Chain Lightning (aimed at an enemy, its cast and sounds) as Shock Bolt - violet,
// cheaper and quicker, and in place of the lightning, it shocks its target for 4 turns (our Shocked buff). Its
// key, written in the base(...) call, gives it its own objects in the game at the next start. It's on its own tab of
// the skills menu - Stormcalling, under an EXAMPLE header at the bottom of the list - learnt with an ability point:
// F4 gives one.
public class ShockBolt : ChainLightning
{
    private readonly Shocked _shocked;

    public ShockBolt(Shocked shocked) : base("shock_bolt")
    {
        _shocked = shocked;
        DisplayName = "Shock Bolt";
        Description = "A bolt made in C#: it leaves its target Shocked for 4 turns.";
        Icon = "shock_bolt.png";
        // Its place in the skills menu (by default: a tab named after the mod, under MODS).
        Tab = "Stormcalling";
        Group = "Example";
        Cooldown = 8;
        EnergyCost = 20;
    }

    protected override void OnCast(SkillCast cast)
    {
        Fx.Play(cast.Target, Sprite.s_weapondamage_electricity, new FxOptions { OffsetY = -14, Light = Draw.Rgb(190, 110, 255) });
        bool shocked = Context.Buffs.Apply(_shocked, cast.Target, 4, cast.Caster) != null;
        Context.Log(shocked ? "Shock Bolt: shocked its target for 4 turns" : "Shock Bolt: its target couldn't be shocked");
    }
}
