using ExampleMod.Buffs;
using StoneForge;
using StoneForge.GameSkills;

namespace ExampleMod.Skills;

// A skill with requirements: the game's Static Field (cast on yourself) as Storm Ward - amber, and in place of the
// field, it focuses you for 5 turns (our Battle Focus buff). It's next to Shock Bolt on the Stormcalling tab, but
// locked until you've learnt Shock Bolt, reached level 2 and put 2 points into Perception and Willpower - its
// tooltip says what's missing.
public class StormWard : StaticField
{
    private readonly BattleFocus _focus;

    public StormWard(BattleFocus focus, ShockBolt shockBolt) : base("storm_ward")
    {
        _focus = focus;
        DisplayName = ExampleText.Get("skills.exampleskill2.storm_ward");
        Description = ExampleText.Get("skills.exampleskill2.gathers_the_storm_around_you_battle");
        Icon = "storm_ward.png";
        Tab = ExampleText.Get("skills.exampleskill.stormcalling");
        Group = ExampleText.Get("skills.exampleskill2.sorcery");
        Cooldown = 12;
        EnergyCost = 15;
        // What it takes to learn it (besides an ability point).
        RequireSkill(shockBolt);
        RequiredLevel = 2;
        RequireAttributes(2, CharacterAttribute.Perception, CharacterAttribute.Willpower);
    }

    protected override void OnCast(SkillCast cast)
    {
        Fx.Play(cast.Caster, Sprite.s_lighting_enchansment_startcast, new FxOptions { OffsetY = -18, Light = Draw.Rgb(255, 190, 90) });
        Context.Buffs.Apply(_focus, cast.Caster, 5);
        Context.Log("Storm Ward: focused for 5 turns");
    }
}
