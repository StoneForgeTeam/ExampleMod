using ExampleMod.Buffs;
using ExampleMod.DamageTypes;
using StoneForge;
using StoneForge.GameSkills;

namespace ExampleMod.Skills;

// A skill of our own: the game's Chain Lightning (aimed at an enemy, its cast and sounds) as Shock Bolt - violet,
// cheaper and quicker, and in place of the lightning, it deals 10 Overcharge (our own kind of shock damage, with
// Combat.Damage) and shocks its target for 4 turns (our Shocked buff). Its key, written in the base(...) call, gives it
// its own objects in the game at the next start. It's on its own tab of the skills menu - Stormcalling, under an
// EXAMPLE header at the bottom of the list - learnt with an ability point: F4 gives one.
public class ShockBolt : ChainLightning
{
    private readonly Shocked _shocked;
    private readonly Overcharge _overcharge;

    public ShockBolt(Shocked shocked) : base("shock_bolt")
    {
        _shocked = shocked;
        _overcharge = new Overcharge(shocked);
        DisplayName = "Shock Bolt";
        Description = "A bolt made in C#: 10 overcharge damage (shock, half as hard again on the Shocked), and its target Shocked for 4 turns.";
        Icon = "shock_bolt.png";
        // Its place in the skills menu (by default: a tab named after the mod, under MODS).
        Tab = "Stormcalling";
        Group = "Example";
        Cooldown = 3;
        EnergyCost = 20;
    }

    protected override void OnCast(SkillCast cast)
    {
        Fx.Play(cast.Target, Sprite.s_weapondamage_electricity, new FxOptions { OffsetY = -14, Light = Draw.Rgb(190, 110, 255) });
        // Overcharge - our own kind of shock damage (DamageTypes\Overcharge) - as the game deals shock: the target's
        // protection and shock resistance apply, the number shows over it, the combat log says so, and it's the
        // caster's hit. Half as hard again on a target already Shocked (by an earlier bolt).
        int damage = Combat.Damage(cast.Target, _overcharge, 10, cast.Caster, new DamageOptions { Name = DisplayName });
        bool shocked = Context.Buffs.Apply(_shocked, cast.Target, 4, cast.Caster) != null;
        Context.Log($"Shock Bolt: {damage} overcharge damage" + (shocked ? ", and shocked for 4 turns" : ", couldn't shock it"));
    }
}
