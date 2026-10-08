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
        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.controlspanel.the_game_s_controls"), 12, 10), live => { live.Text = ExampleText.Get("ui.controlspanel.the_game_s_controls"); }));

        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.controlspanel.health"), 12, 32, Draw.Muted), live => { live.Text = ExampleText.Get("ui.controlspanel.health"); }));
        var health = Add(ExampleText.Live(new UISlider(70, 31, value: 0.75, width: 120) { Tooltip = ExampleText.Get("ui.controlspanel.a_slider_like_the_volume_sliders") }, live => { live.Tooltip = ExampleText.Get("ui.controlspanel.a_slider_like_the_volume_sliders"); }));
        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.controlspanel.energy"), 12, 52, Draw.Muted), live => { live.Text = ExampleText.Get("ui.controlspanel.energy"); }));
        var energy = Add(ExampleText.Live(new UISlider(70, 51, min: 0, max: 10, value: 4, width: 120) { Step = 1, Tooltip = ExampleText.Get("ui.controlspanel.this_one_moves_in_steps_of") }, live => { live.Tooltip = ExampleText.Get("ui.controlspanel.this_one_moves_in_steps_of"); }));

        var healthBar = Add(ExampleText.Live(new UIProgressBar(12, 74, health.Value, style: UIBarStyle.Health, width: 196) { Tooltip = ExampleText.Get("ui.controlspanel.the_game_s_health_bar_following") }, live => { live.Tooltip = ExampleText.Get("ui.controlspanel.the_game_s_health_bar_following"); }));
        var energyBar = Add(ExampleText.Live(new UIProgressBar(12, 86, energy.Value, max: 10, style: UIBarStyle.Energy, width: 196) { Tooltip = ExampleText.Get("ui.controlspanel.and_its_energy_bar_following_energy") }, live => { live.Tooltip = ExampleText.Get("ui.controlspanel.and_its_energy_bar_following_energy"); }));
        health.Changed += value => healthBar.Value = value;
        energy.Changed += value => energyBar.Value = value;

        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.controlspanel.difficulty"), 12, 104, Draw.Muted), live => { live.Text = ExampleText.Get("ui.controlspanel.difficulty"); }));
        var difficulty = Add(ExampleText.Live(new UIDropdown(new[] { ExampleText.Get("ui.controlspanel.story"), ExampleText.Get("ui.controlspanel.easy"), ExampleText.Get("ui.controlspanel.normal"), ExampleText.Get("ui.controlspanel.hard"), ExampleText.Get("ui.controlspanel.nightmare"), ExampleText.Get("ui.controlspanel.legend"), ExampleText.Get("ui.controlspanel.mythic"), ExampleText.Get("ui.controlspanel.impossible"), ExampleText.Get("ui.controlspanel.unfair"), ExampleText.Get("ui.controlspanel.why"), ExampleText.Get("ui.controlspanel.please_stop") }, 70, 102, 140, selected: 2)
        {
            MaxVisibleRows = 6,
            Tooltip = ExampleText.Get("ui.controlspanel.the_settings_menu_s_dropdown_its"),
        }, live => { live.Options.Clear(); live.Options.AddRange(new[] { ExampleText.Get("ui.controlspanel.story"), ExampleText.Get("ui.controlspanel.easy"), ExampleText.Get("ui.controlspanel.normal"), ExampleText.Get("ui.controlspanel.hard"), ExampleText.Get("ui.controlspanel.nightmare"), ExampleText.Get("ui.controlspanel.legend"), ExampleText.Get("ui.controlspanel.mythic"), ExampleText.Get("ui.controlspanel.impossible"), ExampleText.Get("ui.controlspanel.unfair"), ExampleText.Get("ui.controlspanel.why"), ExampleText.Get("ui.controlspanel.please_stop") }); live.Tooltip = ExampleText.Get("ui.controlspanel.the_settings_menu_s_dropdown_its"); }));
        difficulty.Changed += index => Say(ExampleText.Get("ui.controlspanel.difficulty_2", difficulty.Selected));

        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.controlspanel.name"), 12, 126, Draw.Muted), live => { live.Text = ExampleText.Get("ui.controlspanel.name"); }));
        var name = Add(ExampleText.Live(new UITextBox(70, 124, 140, placeholder: ExampleText.Get("ui.controlspanel.type_a_name")) { MaxLength = 24, Tooltip = ExampleText.Get("ui.controlspanel.click_to_type_enter_submits_esc") }, live => { live.Placeholder = ExampleText.Get("ui.controlspanel.type_a_name"); live.Tooltip = ExampleText.Get("ui.controlspanel.click_to_type_enter_submits_esc"); }));
        name.Submitted += text => Say(text.Length == 0 ? ExampleText.Get("ui.controlspanel.no_name_given") : ExampleText.Get("ui.controlspanel.hello", text));

        var list = Add(ExampleText.Live(new UIScrollList(12, 148, 196, 104) { Tooltip = ExampleText.Get("ui.controlspanel.a_scroll_list_the_wheel_the") }, live => { live.Tooltip = ExampleText.Get("ui.controlspanel.a_scroll_list_the_wheel_the"); }));
        foreach (int row in Enumerable.Range(1, 20))
        {
            var button = list.Add(ExampleText.Live(new UIButton(ExampleText.Get("ui.controlspanel.row", row), 0, 0, height: 14), live => { live.Text = ExampleText.Get("ui.controlspanel.row", row); }));
            button.Clicked += _ => Say(ExampleText.Get("ui.controlspanel.clicked_row", row));
        }

        _status = Add(new UILabel("", 12, 262, Draw.Muted));
    }

    private void Say(string text) => _status.Text = text;
}
