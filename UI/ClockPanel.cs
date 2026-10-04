using StoneForge;

namespace ExampleMod.UI;

// The world's clock at the top of the screen in game (Time): the time, the time of day NPCs follow, the date and the
// turn - read twice a second. It lets clicks through to the game (HitTest off), so it never gets in the way of walking.
// The "Show the clock" setting turns it off; F9 lets an hour pass, to watch it move.
public class ClockPanel : UIPanel
{
    private readonly UILabel _time;
    private readonly UILabel _date;
    private double _sinceRead = double.MaxValue;

    public ClockPanel() : base(0, 8, 170, 42, UIAnchor.Top)
    {
        Framed = true;
        HitTest = false;
        _time = Add(new UILabel("", 0, 7) { Width = 170, Align = Draw.AlignCenter, HitTest = false });
        _date = Add(new UILabel("", 0, 23, Draw.Muted) { Width = 170, Align = Draw.AlignCenter, HitTest = false });
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
            return;
        }
        GameTime now = Time.Now;
        _time.Text = $"{now.Hours:00}:{now.Minutes:00}  {now.OfDay}{(Time.IsFrozen ? " (frozen)" : "")}";
        _date.Text = $"Month {now.Months + 1}, day {now.Days + 1}  -  turn {Time.Turns}";
    }
}
