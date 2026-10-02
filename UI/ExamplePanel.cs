using System;
using StoneForge;

namespace ExampleMod.UI;

// The panel: a UIPanel holding the loader's elements and one of our own (FpsGraph).
public class ExamplePanel : UIPanel
{
    private readonly UILabel _time;
    private readonly UILabel _clicks;
    private int _clickCount;

    public ExamplePanel(ModManifest mod, int icon) : base(6, 0, 200, 244, UIAnchor.Left)
    {
        Visible = false;
        Add(new UIImage(icon, 10, 10, scale: 1.5));
        Add(new UILabel(mod.Name, 66, 14));
        Add(new UILabel($"v{mod.Version} by {mod.Author}", 66, 30, Draw.Muted));
        Add(new UILabel("Built from UI classes: this panel is a UIPanel subclass, the graph below its own UIElement.", 10, 66) { Wrap = true, Width = Width - 20 });
        _time = Add(new UILabel("", 10, 112, Draw.Muted));
        _clicks = Add(new UILabel("", 120, 112, Draw.Muted));
        Add(new FpsGraph { X = 10, Y = 128, Width = Width - 20, Height = 50 });
        var check = Add(new UICheckbox("Lock the click counter", 10, 186));
        var clickMe = Add(new UIButton("Click me", 10, 0, 85) { Anchor = UIAnchor.BottomLeft, Y = 10 });
        clickMe.Clicked += _ => _clickCount++;
        check.Changed += locked => clickMe.Enabled = !locked;
        Add(new UIButton("Close", 10, 0, 85, onClick: Close) { Anchor = UIAnchor.BottomRight, Y = 10 });
    }

    /// <summary>Closed with its Close button (the mod closes the controls panel beside it too).</summary>
    public event Action? Closed;

    private void Close()
    {
        Visible = false;
        Closed?.Invoke();
    }

    protected override void OnUpdate(double deltaTime)
    {
        _time.Text = $"Time: {DateTime.Now:HH:mm:ss}";
        _clicks.Text = $"Clicks: {_clickCount}";
    }
}
