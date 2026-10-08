using StoneForge;

namespace ExampleMod.UI;

// The world's clock at the top of the screen in game (Time): the time, the time of day NPCs follow, the date and the
// turn - and where on the world map you are (WorldMap: the location, the cell, the dungeon floor) - read twice a second.
// It lets clicks through to the game (HitTest off), so it never gets in the way of walking. The "Show the clock" setting
// turns it off; F9 lets an hour pass, to watch it move.
public class ClockPanel : UIPanel
{
    private readonly UILabel _time;
    private readonly UILabel _date;
    private readonly UILabel _where;
    private double _sinceRead = double.MaxValue;

    public ClockPanel() : base(0, 8, 190, 58, UIAnchor.Top)
    {
        Framed = true;
        HitTest = false;
        _time = Add(new UILabel("", 0, 7) { Width = 190, Align = Draw.AlignCenter, HitTest = false });
        _date = Add(new UILabel("", 0, 23, Draw.Muted) { Width = 190, Align = Draw.AlignCenter, HitTest = false });
        _where = Add(new UILabel("", 0, 39, Draw.Muted) { Width = 190, Align = Draw.AlignCenter, HitTest = false });
    }

    protected override void OnUpdate(double deltaTime)
    {
        if ((_sinceRead += deltaTime) < 0.5)
            return;
        _sinceRead = 0;
        if (!Time.Available)
        {
            _time.Text = "";
            _date.Text = "";
            _where.Text = "";
            return;
        }
        GameTime now = Time.Now;
        _time.Text = ExampleText.Get("ui.clockpanel.value", now.Hours, now.Minutes, ExampleText.Get("time." + now.OfDay.ToString().ToLowerInvariant()), (Time.IsFrozen ? ExampleText.Get("time.frozen") : ""));
        _date.Text = ExampleText.Get("ui.clockpanel.month_day_turn", now.Months + 1, now.Days + 1, Time.Turns);
        // (The prologue has a map of its own: no world-map cell.)
        _where.Text = WorldMap.Here is { } here
            ? ExampleText.Get("ui.clockpanel.value_2", here.Location ?? ExampleText.Get("world.wilds"), here.X, here.Y, (WorldMap.Floor > 0 ? ExampleText.Get("world.floor", WorldMap.Floor) : ""))
            : ExampleText.Get("ui.clockpanel.the_prologue");
    }
}
