using System;
using System.Linq;
using StoneForge;

namespace ExampleMod;

// The game's world from C#, through the game's own menus and the mouse rather than more keys:
// - the Esc menu gets "Rest an Hour" (EscMenu): it asks with the game's own confirmation (GameDialogs), then holds the
//   screen black with a line of text (Blackout) while an hour passes (Time), and fades back;
// - an enemy's right-click menu gets Inspect, Stun and Push (ContextMenus): Inspect logs where it stands (Units) and the
//   effects on it (UnitEffects), Stun puts the game's stun on it from you (UnitEffects.Create), Push moves it a cell
//   away from you, onto the nearest free one (Units.NearestFreeCell, Units.Move);
// - a middle click on the world (Mouse.ClickedWorld - not on any window, and in the game's window) logs the cell and
//   who stands on it (Mouse.Cell, Mouse.Unit), and walks you there if it's empty (Player.WalkTo); with Ctrl, you go out
//   by the nearest way out instead (Doors).
public sealed class ExampleWorld
{
    private readonly ModContext _context;
    private int _enemy = -2;
    // (The rest under way: when the black screen's held long enough to read.)
    private DateTime _restUntil;
    private bool _resting;

    public ExampleWorld(ModContext context)
    {
        _context = context;
        EscMenu.AddBefore(context, EscButton.Settings, "Rest an Hour", AskToRest);
        ContextMenus.Add(context, "Inspect", IsEnemy, Inspect, hover: "Example Mod: log where it stands and its effects");
        ContextMenus.Add(context, "Stun", IsEnemy, Stun, hover: "Example Mod: stun it for 2 turns");
        ContextMenus.Add(context, "Push", IsEnemy, Push, hover: "Example Mod: push it a cell away from you");
    }

    // Every frame (ExampleMod's Tick, timed in the profiler as "world").
    public void Tick()
    {
        if (_resting && DateTime.UtcNow >= _restUntil)
        {
            _resting = false;
            GameTime before = Time.Now;
            Time.Advance(60);
            Blackout.Hide();
            _context.Log($"Rested an hour: {before} -> {Time.Now}");
        }
        if (!Gm.InGame || !Mouse.ClickedWorld(Mouse.Middle))
            return;
        if (Keyboard.Down(Keyboard.Control))
        {
            var (px, py) = Units.CellOf(Player.Instance);
            Instance door = Doors.Nearest(Units.PositionOf(px), Units.PositionOf(py));
            _context.Log(door.IsNone ? "No way out of here" : $"Leaving by {Gm.ObjectGetName(door.Get("object_index").AsInt)}");
            Doors.Use(door);
            return;
        }
        var (x, y) = Mouse.Cell;
        Instance unit = Mouse.Unit;
        if (unit.IsNone)
        {
            _context.Log($"Cell {x}, {y} is empty: walking there");
            Player.WalkTo(Units.PositionOf(x), Units.PositionOf(y));
        }
        else
            _context.Log($"Cell {x}, {y}: {Gm.ObjectGetName(unit.Get("object_index").AsInt)}{(Units.IsPlayer(unit) ? " (you)" : "")}");
    }

    private void AskToRest()
    {
        if (!Time.Available || _resting)
            return;
        GameDialogs.Confirm(_context, "Rest for an hour here?", () =>
        {
            _resting = true;
            _restUntil = DateTime.UtcNow + TimeSpan.FromSeconds(1.5);
            Blackout.Show("Resting...");
        }, () => _context.Log("Didn't rest"));
    }

    // An enemy (any o_enemy - an animal, a bandit...), not the player.
    private bool IsEnemy(Instance target)
    {
        if (_enemy == -2)
            _enemy = Gm.AssetGetIndex("o_enemy");
        return _enemy >= 0 && target.Exists && !Units.IsPlayer(target)
            && Gm.ObjectIsAncestor(target.Get("object_index").AsInt, _enemy);
    }

    private void Inspect(Instance target)
    {
        var (x, y) = Units.CellOf(target);
        var effects = UnitEffects.On(target).Where(effect => effect.Shown)
            .Select(effect => $"{effect.Name} ({effect.Duration}{(effect.Harmful ? ", harmful" : "")})");
        _context.Log($"{Gm.ObjectGetName(target.Get("object_index").AsInt)} at cell {x}, {y}: HP {target.Get("HP").AsReal:0.#}, "
            + $"{(Player.IsHuntedBy(target) ? "after you" : "not after you")}; effects: {string.Join(", ", effects.DefaultIfEmpty("none"))}");
    }

    private void Stun(Instance target)
    {
        Instance stun = UnitEffects.Create("o_db_stun", target, 2, owner: Player.Instance);
        _context.Log(stun.IsNone ? "It wasn't stunned (immune?)" : "Stunned it for 2 turns");
    }

    // A cell further from you, the nearest free one to it (walls and other units count), as a knockback would.
    private void Push(Instance target)
    {
        var (px, py) = Units.CellOf(Player.Instance);
        var (tx, ty) = Units.CellOf(target);
        int dx = Math.Sign(tx - px), dy = Math.Sign(ty - py);
        if (Units.NearestFreeCell(target, tx + dx, ty + dy) is not var (fx, fy) || (fx, fy) == (tx, ty) || !Units.CanTake(target, fx, fy))
        {
            _context.Log("Nowhere to push it");
            return;
        }
        Units.Move(target, fx, fy);
        _context.Log($"Pushed it from {tx}, {ty} to {fx}, {fy}");
    }
}
