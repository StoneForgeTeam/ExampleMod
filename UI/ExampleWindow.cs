using StoneForge;

namespace ExampleMod.UI;

// A window as the game's Settings menu (a UIWindow): tabs down the left, a scrolling page for each, and buttons
// along the bottom. Built when it opens (OnOpen), its page filled for the tab opened (OnTabOpened) - with any
// UI element, our own FpsGraph too. On the main menu's screen, opened by its "Example Window" button.
public class ExampleWindow : UIWindow
{
    private readonly ModContext _context;
    private readonly ExampleSettings _settings;
    private int _icon = -1;

    public ExampleWindow(ModContext context, ExampleSettings settings) : base("Example Mod")
    {
        _context = context;
        _settings = settings;
    }

    protected override void OnOpen()
    {
        SetTabs("About", "Options");
        AddButton(0, "Say hello").Clicked += _ => _context.Log(_settings.Greeting.Value);
        AddCloseButton();
        Tabs[0].Open();
    }

    protected override void OnTabOpened(UITab tab)
    {
        Page.Clear();
        Page.AddHeader(tab.Text);
        if (tab.Index == 0)
        {
            if (_icon < 0)
                _icon = _context.LoadSprite("icon.png");
            Page.AddImage(_icon);
            Page.AddText("This window is drawn by StoneForge in the game's look - its Settings menu's frame, tabs, scrolling page and buttons - and any UI element can go on it.");
            Page.AddText("Made by the Example Mod.", Draw.Muted);
        }
        else
        {
            // (One of the mod's settings - the same as on its page in the Mods window.)
            var sparks = Page.AddCheckbox(_settings.Sparks.Label, _settings.Sparks.Value, _settings.Sparks.Tooltip);
            sparks.Changed += on => _settings.Sparks.Value = on;
            Page.AddText("Frames per second:", Draw.Muted);
            Page.Add(new FpsGraph { X = 5, Width = 280, Height = 50 });
        }
    }

    protected override void OnClosed() => _context.Log("Example window closed");
}
