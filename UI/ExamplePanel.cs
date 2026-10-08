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
        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.examplepanel.v_by", mod.Version, mod.Author), 66, 30, Draw.Muted), live => { live.Text = ExampleText.Get("ui.examplepanel.v_by", mod.Version, mod.Author); }));
        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.examplepanel.built_from_ui_classes_this_panel"), 10, 66) { Wrap = true, Width = Width - 20 }, live => { live.Text = ExampleText.Get("ui.examplepanel.built_from_ui_classes_this_panel"); }));
        _time = Add(new UILabel("", 10, 112, Draw.Muted));
        _clicks = Add(new UILabel("", 120, 112, Draw.Muted));
        Add(new FpsGraph { X = 10, Y = 128, Width = Width - 20, Height = 50 });
        var check = Add(ExampleText.Live(new UICheckbox(ExampleText.Get("ui.examplepanel.lock_the_click_counter"), 10, 186), live => { live.Text = ExampleText.Get("ui.examplepanel.lock_the_click_counter"); }));
        var clickMe = Add(ExampleText.Live(new UIButton(ExampleText.Get("ui.examplepanel.click_me"), 10, 0, 85) { Anchor = UIAnchor.BottomLeft, Y = 10 }, live => { live.Text = ExampleText.Get("ui.examplepanel.click_me"); }));
        clickMe.Clicked += _ => _clickCount++;
        check.Changed += locked => clickMe.Enabled = !locked;
        Add(ExampleText.Live(new UIButton(ExampleText.Get("ui.examplepanel.close"), 10, 0, 85, onClick: Close) { Anchor = UIAnchor.BottomRight, Y = 10 }, live => { live.Text = ExampleText.Get("ui.examplepanel.close"); }));
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
        _time.Text = ExampleText.Get("ui.examplepanel.time_hh_mm_ss", DateTime.Now);
        _clicks.Text = ExampleText.Get("ui.examplepanel.clicks", _clickCount);
    }
}
