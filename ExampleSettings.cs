using StoneForge;

namespace ExampleMod;

// The mod's settings (context.Settings): saved for it, and on its page in the Mods window for the player to
// change. Used by the Example Blade (sparks, shock chance), the notepad (its side), the clock, the greeting, and
// cheating death (ExampleInventory).
public sealed class ExampleSettings
{
    public ToggleSetting Sparks { get; }
    public SliderSetting ShockChance { get; }
    public ChoiceSetting NotesSide { get; }
    public ToggleSetting ShowClock { get; }
    public TextSetting Greeting { get; }
    public ToggleSetting CheatDeath { get; }

    public ExampleSettings(ModSettings settings)
    {
        Sparks = settings.Toggle("sparks", ExampleText.Get("examplesettings.sparks_on_crits"), true, ExampleText.Get("examplesettings.whether_the_example_blade_s_crits"));
        ShockChance = settings.Slider("shockChance", ExampleText.Get("examplesettings.shock_chance"), 33, min: 0, max: 100, step: 1, tooltip: ExampleText.Get("examplesettings.how_often_a_hit_with_the"));
        ShockChance.Format = value => $"{value:0}%";
        NotesSide = settings.Choice("notesSide", ExampleText.Get("examplesettings.notepad_side"), new[] { "Left", "Right" }, tooltip: ExampleText.Get("examplesettings.which_side_of_the_screen_the"));
        ShowClock = settings.Toggle("showClock", ExampleText.Get("examplesettings.show_the_clock"), true, ExampleText.Get("examplesettings.the_world_s_time_time_of"));
        Greeting = settings.Text("greeting", ExampleText.Get("examplesettings.greeting"), ExampleText.Get("examplesettings.hello_from_c"), tooltip: ExampleText.Get("examplesettings.logged_when_the_mod_loads_and"));
        CheatDeath = settings.Toggle("cheatDeath", ExampleText.Get("examplesettings.cheat_death_once"), false, ExampleText.Get("examplesettings.the_first_time_you_d_die"));
    }
}
