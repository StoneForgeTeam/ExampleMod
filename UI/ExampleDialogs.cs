using System.Linq;
using StoneForge;

namespace ExampleMod.UI;

// Windows in other frames than the Settings menu's - StoneForge's UIWindow takes any sprite.
//
// ExampleConfirmWindow: the game's confirm panel (s_skill_confirm_panel, as its "are you sure?" dialogs): no title,
// no close button, a message in the middle and two buttons where the frame has places for them (o_confirm_panel: its
// message area 26,26 256x55, its buttons' middles at 103,103 and 205,103).
public class ExampleConfirmWindow : UIWindow
{
    private readonly ModContext _context;
    private readonly ExampleSlicedWindow _sliced;

    public ExampleConfirmWindow(ModContext context, ExampleSlicedWindow sliced)
    {
        _context = context;
        _sliced = sliced;
        sliced.Under = this;
        FrameSprite = (int)Sprite.s_skill_confirm_panel;
        // (The message area's corner; the buttons reach 7 from the bottom.)
        ContentInsets = new UIInsets(26, 26, 26, 7);
        CloseButton.Visible = false;
    }

    protected override void OnOpen()
    {
        // (Two centred lines in the message area.)
        Content.Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.exampledialogs.a_window_in_the_game_s"), 0, 13) { Width = Content.Width, Align = Draw.AlignCenter }, live => { live.Text = ExampleText.Get("ui.exampledialogs.a_window_in_the_game_s"); }));
        Content.Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.exampledialogs.open_a_sliced_one_over_it"), 0, 28) { Width = Content.Width, Align = Draw.AlignCenter }, live => { live.Text = ExampleText.Get("ui.exampledialogs.open_a_sliced_one_over_it"); }));
        // (The frame's two button places, from the content's corner: 53 - 26 and 155 - 26 across, 90 - 26 down.)
        var buttons = Content.Add(new UIButtonRow(0, 64, Content.Width) { Positions = new double[] { 27, 129 } });
        ExampleText.Live(buttons.Add(ExampleText.Get("ui.exampledialogs.sliced"), _sliced.Open), live => { live.Text = ExampleText.Get("ui.exampledialogs.sliced"); });
        ExampleText.Live(buttons.Add(ExampleText.Get("ui.exampledialogs.cancel"), Close), live => { live.Text = ExampleText.Get("ui.exampledialogs.cancel"); });
    }

    protected override void OnClosed() => _context.Log("Confirm window closed");
}

// ExampleSlicedWindow: the same confirm panel 9-sliced to a bigger window (Slice: its corners and edges kept, its middle
// stretched), with a title, the close button and a scrolling page - opened over the confirm window. The panel's two
// button plates are part of its picture, in its bottom 40 pixels: that strip is the slice's bottom border, so they
// keep their height, and they stretch sideways with the middle - the buttons go where they land, as wide.
public class ExampleSlicedWindow : UIWindow
{
    // The confirm panel: its size, the slice's side borders, its buttons' left edges and top (o_confirm_panel's
    // buttons' middles at 103,103 and 205,103, 100x26).
    private const double PanelWidth = 308, PanelHeight = 123, Side = 24, Bottom = 40;
    private static readonly double[] ButtonLefts = { 53, 155 };
    private const double ButtonTop = 90, ButtonWidth = 100;

    public ExampleSlicedWindow() : base(ExampleText.Get("ui.exampledialogs.sliced_frame"))
    {
        FrameSprite = (int)Sprite.s_skill_confirm_panel;
        Slice = new UIInsets(Side, 26, Side, Bottom);
        FrameWidth = 360;
        FrameHeight = 220;
        ContentInsets = new UIInsets(26, 30, 26, 7);
ExampleText.Live(this, live => live.Title = ExampleText.Get("ui.exampledialogs.sliced_frame"));    }

    /// <summary>The window it opens over (its "Close both" closes that too).</summary>
    public UIWindow? Under { get; set; }

    protected override void OnOpen()
    {
        // Where the plates land: the middle column (between the side borders) stretched by this much.
        double stretch = (FrameWidth - Side * 2) / (PanelWidth - Side * 2);
        double Across(double x) => Side + (x - Side) * stretch;
        double buttonY = FrameHeight - (PanelHeight - ButtonTop) - ContentInsets.Top;
        var page = Content.Add(new UIScrollArea(0, 0, Content.Width, buttonY - 10));
        ExampleText.Live(page.AddText(ExampleText.Get("ui.exampledialogs.the_confirm_panel_s_frame_sliced")), live => { live.Text = ExampleText.Get("ui.exampledialogs.the_confirm_panel_s_frame_sliced"); });
        ExampleText.Live(page.AddText(ExampleText.Get("ui.exampledialogs.its_button_plates_are_part_of"), Draw.Muted), live => { live.Text = ExampleText.Get("ui.exampledialogs.its_button_plates_are_part_of"); });
        ExampleText.Live(page.AddText(ExampleText.Get("ui.exampledialogs.escape_closes_this_one_first_then"), Draw.Muted), live => { live.Text = ExampleText.Get("ui.exampledialogs.escape_closes_this_one_first_then"); });
        for (int i = 1; i <= 4; i++)
            ExampleText.Live(page.AddText(ExampleText.Get("ui.exampledialogs.line_to_make_it_scroll", i)), live => { live.Text = ExampleText.Get("ui.exampledialogs.line_to_make_it_scroll", i); });
        var buttons = Content.Add(new UIButtonRow(0, buttonY, Content.Width)
        {
            ButtonWidth = ButtonWidth * stretch,
            Positions = ButtonLefts.Select(x => Across(x) - ContentInsets.Left).ToArray(),
        });
        ExampleText.Live(buttons.Add(ExampleText.Get("ui.exampledialogs.back"), Close), live => { live.Text = ExampleText.Get("ui.exampledialogs.back"); });
        ExampleText.Live(buttons.Add(ExampleText.Get("ui.exampledialogs.close_both"), () =>
        {
            Close();
            Under?.Close();
        }), live => { live.Text = ExampleText.Get("ui.exampledialogs.close_both"); });
    }
}
