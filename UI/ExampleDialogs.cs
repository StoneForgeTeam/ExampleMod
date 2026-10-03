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
        Content.Add(new UILabel("A window in the game's confirm panel.", 0, 13) { Width = Content.Width, Align = Draw.AlignCenter });
        Content.Add(new UILabel("Open a sliced one over it?", 0, 28) { Width = Content.Width, Align = Draw.AlignCenter });
        // (The frame's two button places, from the content's corner: 53 - 26 and 155 - 26 across, 90 - 26 down.)
        var buttons = Content.Add(new UIButtonRow(0, 64, Content.Width) { Positions = new double[] { 27, 129 } });
        buttons.Add("Sliced", _sliced.Open);
        buttons.Add("Cancel", Close);
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

    public ExampleSlicedWindow() : base("Sliced Frame")
    {
        FrameSprite = (int)Sprite.s_skill_confirm_panel;
        Slice = new UIInsets(Side, 26, Side, Bottom);
        FrameWidth = 360;
        FrameHeight = 220;
        ContentInsets = new UIInsets(26, 30, 26, 7);
    }

    /// <summary>The window it opens over (its "Close both" closes that too).</summary>
    public UIWindow? Under { get; set; }

    protected override void OnOpen()
    {
        // Where the plates land: the middle column (between the side borders) stretched by this much.
        double stretch = (FrameWidth - Side * 2) / (PanelWidth - Side * 2);
        double Across(double x) => Side + (x - Side) * stretch;
        double buttonY = FrameHeight - (PanelHeight - ButtonTop) - ContentInsets.Top;
        var page = Content.Add(new UIScrollArea(0, 0, Content.Width, buttonY - 10));
        page.AddText("The confirm panel's frame, 9-sliced to 360 x 220: its corners stay as drawn, its edges stretch along and its middle both ways.");
        page.AddText("Its button plates are part of the picture: they keep their height and stretch sideways, and the buttons sit on them.", Draw.Muted);
        page.AddText("Escape closes this one first, then the one under it.", Draw.Muted);
        for (int i = 1; i <= 4; i++)
            page.AddText($"Line {i}, to make it scroll.");
        var buttons = Content.Add(new UIButtonRow(0, buttonY, Content.Width)
        {
            ButtonWidth = ButtonWidth * stretch,
            Positions = ButtonLefts.Select(x => Across(x) - ContentInsets.Left).ToArray(),
        });
        buttons.Add("Back", Close);
        buttons.Add("Close both", () =>
        {
            Close();
            Under?.Close();
        });
    }
}
