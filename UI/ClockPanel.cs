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
        _time.Text = $"{now.Hours:00}:{now.Minutes:00}  {now.OfDay}{(Time.IsFrozen ? " (frozen)" : "")}";
        _date.Text = $"Month {now.Months + 1}, day {now.Days + 1}  -  turn {Time.Turns}";
        // (The prologue has a map of its own: no world-map cell.)
        _where.Text = WorldMap.Here is { } here
            ? $"{here.Location ?? "Wilds"} ({here.X}, {here.Y}){(WorldMap.Floor > 0 ? $", floor {WorldMap.Floor}" : "")}"
            : "The prologue";
    }
}
