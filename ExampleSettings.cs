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
        Sparks = settings.Toggle("sparks", "Sparks on crits", true, "Whether the Example Blade's crits spark - for 5 more damage.");
        ShockChance = settings.Slider("shockChance", "Shock chance", 33, min: 0, max: 100, step: 1, tooltip: "How often a hit with the Example Blade shocks its target.");
        ShockChance.Format = value => $"{value:0}%";
        NotesSide = settings.Choice("notesSide", "Notepad side", new[] { "Left", "Right" }, tooltip: "Which side of the screen the notepad (F8 in game) is on.");
        ShowClock = settings.Toggle("showClock", "Show the clock", true, "The world's time, time of day, date and turn at the top of the screen in game.");
        Greeting = settings.Text("greeting", "Greeting", "Hello from C#!", tooltip: "Logged when the mod loads, and by the Example window's Say hello.");
        CheatDeath = settings.Toggle("cheatDeath", "Cheat death once", false, "The first time you'd die, you're left at a quarter of your health instead (Player.OnDying).");
    }
}
