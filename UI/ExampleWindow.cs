using StoneForge;

namespace ExampleMod.UI;

// A window as the game's Settings menu (a UISettingsWindow): tabs down the left, a scrolling page for each, and buttons
// along the bottom. Built when it opens (OnOpen), its page filled for the tab opened (OnTabOpened) - with any
// UI element, our own FpsGraph too. On the main menu's screen, opened by its "Example Window" button.
public class ExampleWindow : UISettingsWindow
{
    private readonly ModContext _context;
    private readonly ExampleSettings _settings;
    private int _icon = -1;

    public ExampleWindow(ModContext context, ExampleSettings settings) : base(ExampleText.Get("ui.examplewindow.example_mod"))
    {
        _context = context;
        _settings = settings;
        context.Localization.TranslationsChanged += RefreshLanguage;
ExampleText.Live(this, live => live.Title = ExampleText.Get("ui.examplewindow.example_mod"));    }

    protected override void OnOpen()
    {
        SetTabs(ExampleText.Get("ui.examplewindow.about"), ExampleText.Get("ui.examplewindow.options"));
        ExampleText.Live(AddButton(ExampleText.Get("ui.examplewindow.say_hello")), live => { live.Text = ExampleText.Get("ui.examplewindow.say_hello"); }).Clicked += _ => _context.Log(_settings.Greeting.Value);
        ExampleText.Live(AddCloseButton(ExampleText.Get("common.close")), live => { live.Text = ExampleText.Get("common.close"); });
        Tabs.Tabs[0].Open();
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
            ExampleText.Live(Page.AddText(ExampleText.Get("ui.examplewindow.this_window_is_drawn_by_stoneforge")), live => { live.Text = ExampleText.Get("ui.examplewindow.this_window_is_drawn_by_stoneforge"); });
            ExampleText.Live(Page.AddText(ExampleText.Get("ui.examplewindow.made_by_the_example_mod"), Draw.Muted), live => { live.Text = ExampleText.Get("ui.examplewindow.made_by_the_example_mod"); });
        }
        else
        {
            // (One of the mod's settings - the same as on its page in the Mods window.)
            var sparks = ExampleText.Live(Page.AddCheckbox(ExampleText.Get("examplesettings.sparks_on_crits"), _settings.Sparks.Value, ExampleText.Get("examplesettings.whether_the_example_blade_s_crits")), live => { live.Text = ExampleText.Get("examplesettings.sparks_on_crits"); live.Tooltip = ExampleText.Get("examplesettings.whether_the_example_blade_s_crits"); });
            sparks.Changed += on => _settings.Sparks.Value = on;
            ExampleText.Live(Page.AddText(ExampleText.Get("ui.examplewindow.frames_per_second"), Draw.Muted), live => { live.Text = ExampleText.Get("ui.examplewindow.frames_per_second"); });
            Page.Add(new FpsGraph { X = 5, Width = 280, Height = 50 });
        }
    }

    // Text assigned to controls is a snapshot; rebuild this window when the game language changes.
    private void RefreshLanguage()
    {
        Title = ExampleText.Get("ui.examplewindow.example_mod");
        if (!IsOpen) return;
        int tab = SelectedTab?.Index ?? 0;
        Close();
        Open();
        Tabs.Tabs[System.Math.Clamp(tab, 0, Tabs.Tabs.Count - 1)].Open();
    }
    protected override void OnClosed() => _context.Log("Example window closed");
}
