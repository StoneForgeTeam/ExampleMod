using System.Linq;
using StoneForge;

namespace ExampleMod.UI;

// The game's own controls, from the loader: sliders, bars, a dropdown, a text box and a scroll list, each
// with a tooltip, in a panel with the game's hover-window frame.
public class ControlsPanel : UIPanel
{
    private readonly UILabel _status;

    public ControlsPanel() : base(212, 0, 220, 290, UIAnchor.Left)
    {
        Visible = false;
        Framed = true;
        Add(new UILabel("The game's controls", 12, 10));

        Add(new UILabel("Health", 12, 32, Draw.Muted));
        var health = Add(new UISlider(70, 31, value: 0.75, width: 120) { Tooltip = "A slider like the volume sliders. Drag it, click the bar or use the wheel." });
        Add(new UILabel("Energy", 12, 52, Draw.Muted));
        var energy = Add(new UISlider(70, 51, min: 0, max: 10, value: 4, width: 120) { Step = 1, Tooltip = "This one moves in steps of 1, from 0 to 10." });

        var healthBar = Add(new UIProgressBar(12, 74, health.Value, style: UIBarStyle.Health, width: 196) { Tooltip = "The game's health bar, following the Health slider." });
        var energyBar = Add(new UIProgressBar(12, 86, energy.Value, max: 10, style: UIBarStyle.Energy, width: 196) { Tooltip = "And its energy bar, following Energy." });
        health.Changed += value => healthBar.Value = value;
        energy.Changed += value => energyBar.Value = value;

        Add(new UILabel("Difficulty", 12, 104, Draw.Muted));
        var difficulty = Add(new UIDropdown(new[] { "Story", "Easy", "Normal", "Hard", "Nightmare", "Legend", "Mythic", "Impossible", "Unfair", "Why", "Please stop" }, 70, 102, 140, selected: 2)
        {
            MaxVisibleRows = 6,
            Tooltip = "The Settings menu's dropdown. Its list draws over everything; the wheel scrolls it.",
        });
        difficulty.Changed += index => Say($"Difficulty: {difficulty.Selected}");

        Add(new UILabel("Name", 12, 126, Draw.Muted));
        var name = Add(new UITextBox(70, 124, 140, placeholder: "Type a name...") { MaxLength = 24, Tooltip = "Click to type; Enter submits, Esc lets go." });
        name.Submitted += text => Say(text.Length == 0 ? "No name given" : $"Hello, {text}!");

        var list = Add(new UIScrollList(12, 148, 196, 104) { Tooltip = "A scroll list: the wheel, the arrows or the thumb." });
        foreach (int row in Enumerable.Range(1, 20))
        {
            var button = list.Add(new UIButton($"Row {row}", 0, 0, height: 14));
            button.Clicked += _ => Say($"Clicked row {row}");
        }

        _status = Add(new UILabel("", 12, 262, Draw.Muted));
    }

    private void Say(string text) => _status.Text = text;
}
