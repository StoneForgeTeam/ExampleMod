using System;
using System.Linq;
using StoneForge;

namespace ExampleMod;

// The game's world from C#, through the game's own menus and the mouse rather than more keys:
// - the Esc menu gets "Rest an Hour" (EscMenu): it asks with the game's own confirmation (GameDialogs), then holds the
//   screen black with a line of text (Blackout) while an hour passes (Time), and fades back;
// - and "Mark This Spot": a flag on the world map where you are (MapMarkers, WorldMap.PlayerCell) - or, if there's one
//   on that cell already, it comes off; the markers you place or take off on the map yourself are logged (OnPlaced,
//   OnRemoved);
// - and some of the game's events, logged: each room you go into (Rooms.OnEntered), each unit you kill
//   (Units.OnDied, its killer you), each level you gain (Player.OnLevelUp) and each item you come to carry
//   (Inventory.OnAdded);
// - an enemy's right-click menu gets Inspect, Stun and Push (ContextMenus): Inspect logs where it stands (Units) and the
//   effects on it (UnitEffects), Stun puts the game's stun on it from you (UnitEffects.Create), Push moves it a cell
//   away from you, onto the nearest free one (Units.NearestFreeCell, Units.Move);
// - a middle click on the world (Mouse.ClickedWorld - not on any window, and in the game's window) logs the cell and
//   who stands on it (Mouse.Cell, Mouse.Unit), and walks you there if it's empty (Player.WalkTo); with Ctrl, you go out
//   by the nearest way out instead (Exits).
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
        EscMenu.AddBefore(context, EscButton.Settings, ExampleText.Get("exampleworld.rest_an_hour"), AskToRest);
        EscMenu.AddBefore(context, EscButton.Settings, ExampleText.Get("exampleworld.mark_this_spot"), MarkThisSpot);
        MapMarkers.OnPlaced(context, marker => context.Log($"You placed a {marker.Sprite} marker on {marker.Tile}"));
        MapMarkers.OnRemoved(context, marker => context.Log($"You took the {marker.Sprite} marker off {marker.Tile}"));
        Rooms.OnEntered(context, room => context.Log($"Entered {Rooms.CurrentName}"));
        Player.OnLevelUp(context, level => context.Log($"Level {level}!"));
        Inventory.OnAdded(context, item => context.Log($"You have {item.Name} x{item.Stack}"));
        Units.OnDied(context, (unit, killer) =>
        {
            if (!killer.IsNone && killer.Equals(Player.Instance))
                context.Log($"You killed {unit.Get("name")}");
        });
        ContextMenus.Add(context, ExampleText.Get("exampleworld.inspect"), IsEnemy, Inspect, hover: ExampleText.Get("exampleworld.example_mod_log_where_it_stands"));
        ContextMenus.Add(context, ExampleText.Get("exampleworld.stun"), IsEnemy, Stun, hover: ExampleText.Get("exampleworld.example_mod_stun_it_for_turns"));
        ContextMenus.Add(context, ExampleText.Get("exampleworld.push"), IsEnemy, Push, hover: ExampleText.Get("exampleworld.example_mod_push_it_a_cell"));
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
            Instance door = Exits.Nearest(Units.CellOf(Player.Instance));
            _context.Log(door.IsNone ? "No way out of here" : $"Leaving by {Gm.ObjectGetName(door.Get("object_index").AsInt)}");
            Exits.Use(door);
            return;
        }
        Cell cell = Mouse.Cell;
        Instance unit = Mouse.Unit;
        if (unit.IsNone)
        {
            _context.Log($"Cell {cell} is empty: walking there");
            Player.WalkTo(cell);
        }
        else
            _context.Log($"Cell {cell}: {Gm.ObjectGetName(unit.Get("object_index").AsInt)}{(Units.IsPlayer(unit) ? " (you)" : "")}");
    }

    private void MarkThisSpot()
    {
        if (WorldMap.PlayerCell is not { } here)
            return;
        var markers = MapMarkers.All();
        if (markers.FirstOrDefault(m => m.Tile == here) is { Sprite: not null } mine)
        {
            MapMarkers.Remove(mine);
            _context.Log($"Took the {mine.Sprite} marker off {here}");
            return;
        }
        // (The cell's middle, in world-map pixels.)
        var flag = new MapMarker(MapMarkers.Sprites[4], 0, new Point((here.X + 0.5) * MapMarkers.CellSize, (here.Y + 0.5) * MapMarkers.CellSize));
        MapMarkers.Add(flag);
        _context.Log($"Marked {here} on the world map ({markers.Count + 1} markers)");
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
        Cell cell = Units.CellOf(target);
        var effects = UnitEffects.On(target).Where(effect => effect.Shown)
            .Select(effect => $"{effect.Name} ({effect.Duration}{(effect.Harmful ? ", harmful" : "")})");
        _context.Log($"{Gm.ObjectGetName(target.Get("object_index").AsInt)} at cell {cell}: HP {target.Get("HP").AsReal:0.#}, "
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
        Cell you = Units.CellOf(Player.Instance), from = Units.CellOf(target);
        Cell away = from.Offset(Math.Sign(from.X - you.X), Math.Sign(from.Y - you.Y));
        if (Units.NearestFreeCell(target, away) is not { } to || to == from || !Units.CanTake(target, to))
        {
            _context.Log("Nowhere to push it");
            return;
        }
        Units.Move(target, to);
        _context.Log($"Pushed it from {from} to {to}");
    }
}
