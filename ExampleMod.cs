using ExampleMod.Skills;
using ExampleMod.Buffs;
using ExampleMod.Items;
using ExampleMod.UI;
using System;
using System.Linq;
using StoneForge;
using StoneForge.Objects;

// The scripts this mod hooks (the loader makes them hookable) - by the generated names, no strings.
[assembly: HookScript(nameof(Scripts.scr_atr))]
[assembly: HookScript(nameof(Scripts.scr_player_move))]

namespace ExampleMod;

// An example on the typed API: an "Example" button on the main menu opening panels built from UI classes
// (ExamplePanel, FpsGraph, and ControlsPanel with the game's own controls), a weapon of its own (ExampleBlade:
// F7 gives it) with buffs of its own (ExampleBuffs), the player's health read in its Step event, every move
// logged, and the character's level shown as 99 (a replaced script call).
public class ExampleMod : IStoneMod, ITickable
{
    private int _playerSteps;
    private ModContext _context = null!;
    private ExampleBlade _blade = null!;
    private ExampleShirt _shirt = null!;
    private ExampleTonic _tonic = null!;
    private NotesPanel _notes = null!;

    public void Load(ModContext context)
    {
        _context = context;
        // Its settings (ExampleSettings): on its page in the Mods window.
        var settings = new ExampleSettings(context.Settings);
        context.Log(settings.Greeting.Value);
        // Its own GML functions (GML\*.gml), called through the generated Gml class: Twice calls Add.
        context.Log($"GML: Twice(21) = {Gml.Twice(21)}, Add(20, 22) = {Gml.Add(20, 22)}");
        // Saves (SaveSlots): each time the game saves, the character folder's info counts the saves made with the Example
        // Mod on, and the save menu's header shows that count after the character's name.
        SaveSlots.OnInfoSaving(context, (slot, info) =>
            info["example_saves"] = (slot.Info?["example_saves"] is { Kind: GmKind.Real } count ? count.AsInt : 0) + 1);
        SaveSlots.SetTitle(context, slot => slot.Info is { } info && info["example_saves"] is { Kind: GmKind.Real } count
            ? $"{info.CharacterName ?? "?"} - {count.AsInt} save(s) with Example Mod" : null);

        // The panels, on the main menu's screen - shown (side by side) by the main menu button, and closed again
        // when the main menu goes.
        var panel = context.UI.MainMenu.Add(new ExamplePanel(context.Manifest, context.LoadSprite("icon.png")));
        var controls = context.UI.MainMenu.Add(new ControlsPanel());
        // (The two go together: the button opens both unless both are open, the panel's Close closes both.)
        panel.Closed += () => controls.Visible = false;
        // And a window in the game's look (ExampleWindow), on the main menu's screen too.
        var window = context.UI.MainMenu.Add(new ExampleWindow(context, settings));
        // And windows in other frames (ExampleDialogs): the game's confirm panel, and the same frame 9-sliced bigger.
        var sliced = context.UI.MainMenu.Add(new ExampleSlicedWindow());
        var confirm = context.UI.MainMenu.Add(new ExampleConfirmWindow(context, sliced));

        // (Placed by name: just under the game's Play button - "Start" names it too, or the text shown on a button.)

        MainMenu.AddAfter(context, VanillaButton.Credits, "Example Button", () =>
        {
            MainMenu.ClearButtons(context);
            MainMenu.AddButton(context, VanillaButton.Play);
            MainMenu.AddAfter(context, VanillaButton.Play, "Example Window", window.Open);
            MainMenu.AddButton(context, "Example", () => controls.Visible = panel.Visible = !(panel.Visible && controls.Visible));
            MainMenu.AddButton(context, "Window Styles", confirm.Open);
            // A menu in this one: the game's play screen buttons, each doing what it does there.
            MainMenu.AddButton(context, "Play Options", () =>
            {
                MainMenu.ClearButtons(context);
                MainMenu.AddButton(context, VanillaButton.Continue);
                MainMenu.AddButton(context, VanillaButton.NewGame);
                MainMenu.AddButton(context, VanillaButton.LoadGame);
                MainMenu.AddButton(context, VanillaButton.Prologue);
                MainMenu.AddButton(context, VanillaButton.Adventure);
                MainMenu.AddButton(context, VanillaButton.Back);
            });
            // (The game's Back: the main menu as it started.)
            MainMenu.AddButton(context, VanillaButton.Back);
        });

        context.UI.MainMenu.Hidden += () => controls.Visible = panel.Visible = false;
        // And one over the game world, on the in-game screen, toggled with F8 (NotesPanel).
        _notes = context.UI.InGame.Add(new NotesPanel());
        // The world's clock at the top of the screen (Time), unless the setting turns it off.
        var clock = context.UI.InGame.Add(new ClockPanel());
        clock.Visible = settings.ShowClock.Value;
        settings.ShowClock.Changed += show => clock.Visible = show;
        // (On the side its setting says - and moved when it's changed.)
        _notes.Anchor = settings.NotesSide.Value == 0 ? UIAnchor.Left : UIAnchor.Right;
        settings.NotesSide.Changed += side => _notes.Anchor = side == 0 ? UIAnchor.Left : UIAnchor.Right;

        // Two effects of our own (ExampleBuffs) and an item that uses them (ExampleBlade); F7 in game gives the
        // player one.
        var shocked = new Shocked();
        var focus = new BattleFocus();
        context.Buffs.Add(shocked);
        context.Buffs.Add(focus);
        _blade = new ExampleBlade(shocked, focus, settings);
        context.Items.Add(_blade);
        // And armour with its own look on the character (ExampleShirt); F6 gives it.
        _shirt = new ExampleShirt();
        context.Items.Add(_shirt);
        // And a consumable (ExampleTonic): a drink of our own; F5 gives one.
        _tonic = new ExampleTonic(focus);
        context.Items.Add(_tonic);
        // And skills (ExampleSkill, ExampleSkill2, StaticCharge): Shock Bolt, on its tab of the skills menu, and Storm Ward, learnt
        // once Shock Bolt is (and more); F4 gives an ability point.
        var shockBolt = new ShockBolt(shocked);
        context.Skills.Add(shockBolt);
        context.Skills.Add(new StormWard(focus, shockBolt));
        // And a passive (StaticCharge): +5% Crit Chance, and weapon hits may shock.
        context.Skills.Add(new StaticCharge(shocked));

        // An object event, with the instance already as its class: player.HP, not player.Get("HP").
        Events.o_player.Step_0.After(context, player =>
        {
            if (++_playerSteps % 600 == 0)
                context.Log($"player: HP {(double)player.HP:0.#}/{(double)player.max_hp:0.#}, MP {(double)player.MP:0.#}/{(double)player.max_mp:0.#}");
        });

        // Script hooks.
        Scripts.scr_player_move.Before(context, call =>
        {
            context.Log($"{call.Name}({string.Join(", ", call.Args)}) by {call.Self}");
            return false;
        });
        /*Scripts.scr_atr.Before(context, call =>
        {
            // Replacing a call: the game asking for the character's level gets 99.
            if (call.Args.Length > 0 && call.Args[0].AsString == "LVL")
            {
                call.Result = 99;
                return true;
            }
            return false;
        });*/
    }

    // Switched off in the Mods window. What it registered through its context - panels, main menu buttons, items,
    // buffs, hooks, settings - is taken back for it; it changed nothing else in the game, so there's nothing to undo.
    public void Unload() => _context.Log("Switched off - goodbye!");

    private bool _shownValues, _shownGameValues;

    // Every frame (ITickable): F4 gives the player an ability point, F5 an Example Tonic, F6 an Example Shirt, F7 an
    // Example Blade; F8 shows or hides the notes; F2 clears every Example Tonic off the ground (ClearTonics); F9 lets an
    // hour of game time pass; F10 logs where you are on the world map; F11 logs this location's saved state.
    public void Tick(double deltaTime)
    {
        // GameMaker arrays and structs from C#, once each: our own, then the game's once we're in it.
        if (!_shownValues)
        {
            _shownValues = true;
            GameValues.ShowMade(_context);
        }
        if (!_shownGameValues && Gm.InGame)
        {
            _shownGameValues = true;
            GameValues.ShowGames(_context);
        }
        if (Keyboard.Pressed(Keyboard.F8) && _context.UI.InGame.IsActive)
            _notes.Visible = !_notes.Visible;
        if (Keyboard.Pressed(Keyboard.F7))
            _context.Log(_context.Items.Give(_blade) ? "Gave the Example Blade" : "Couldn't give the Example Blade (no player, or no room)");
        if (Keyboard.Pressed(Keyboard.F4) && Instances.First<GameInstance>(GameObjectId.o_player) is { } player)
        {
            Game.CallScript("scr_atr_incr", player.Instance, "SP", 1);
            _context.Log("Gave an ability point");
        }
        if (Keyboard.Pressed(Keyboard.F5))
            _context.Log(_context.Items.Give(_tonic) ? "Gave an Example Tonic" : "Couldn't give the Example Tonic (no player, or no room)");
        if (Keyboard.Pressed(Keyboard.F6))
            _context.Log(_context.Items.Give(_shirt) ? "Gave the Example Shirt" : "Couldn't give the Example Shirt (no player, or no room)");
        if (Keyboard.Pressed(Keyboard.F2) && Gm.InGame)
            ClearTonics();
        if (Keyboard.Pressed(Keyboard.F10) && WorldMap.Here is { } here)
        {
            // Where you are: the place as one string, the cell's location and seeds, and its dungeon if it has one.
            var seeds = here.Seeds;
            _context.Log($"World map: {WorldMap.Place}, cell {here.Tag} ({here.Location ?? "no location"}) of {WorldMap.Width} x {WorldMap.Height}; "
                + $"seeds: layout {seeds.Layout}, mobs {seeds.Mobs}, preset {seeds.Preset}");
            if (here.Dungeon is { } dungeon)
                _context.Log($"  its dungeon: boss alive {dungeon["boss_alive"]}, open {dungeon["dungeon_is_open"]}, resets in {dungeon["dungeon_reset"]}, "
                    + $"{(dungeon.GetMap("saveGraphMap") is { } graphs ? graphs.Count : 0)} saved floor graph(s), values: {string.Join(", ", dungeon.Keys)}");
        }
        if (Keyboard.Pressed(Keyboard.F11) && Locations.Here is var (locationTag, roomTag))
            LogLocation(locationTag, roomTag);
        if (Keyboard.Pressed(Keyboard.F3) && SaveData.Available)
            LogSaves();
        if (Keyboard.Pressed(Keyboard.F12) && !Game.IsBusy && Instances.First<GameInstance>(GameObjectId.o_player) is { } me)
            CopyNearestItem(me.Instance["x"].AsReal, me.Instance["y"].AsReal);
        if (Keyboard.Pressed(Keyboard.F9) && Time.Available && !Game.IsBusy)
        {
            // (As play lets time pass: the hour's upkeep, and NPCs following the new time of day.)
            GameTime before = Time.Now;
            Time.Advance(60);
            _context.Log($"An hour passed: {before} ({before.OfDay}) -> {Time.Now} ({Time.OfDay})");
        }
    }

    // The game being played (SaveData): its save data's sections, the character's apart from the world's - and the saves
    // on disk (SaveSlots): this character's folder and its saves, and every other folder.
    private void LogSaves()
    {
        string? json = SaveData.ToJson();
        _context.Log($"Save data: {json?.Length ?? 0} characters of JSON; the character's sections: {string.Join(", ", SaveData.CharacterSections)} "
            + $"({SaveData.CharacterJson()?.Length ?? 0} characters); the world's: {string.Join(", ", SaveData.WorldSections)}");
        if (SaveSlots.Current is { } slot)
        {
            _context.Log($"  This game's folder: {slot.Name} ({slot.Info?.CharacterName}), last save {SaveSlots.CurrentSave?.Name}");
            foreach (var save in slot.Saves)
                _context.Log($"    {save.Name} ({save.Kind}): {save.Info?.LocationTitleKey}, {save.Info?.SavedAt}");
        }
        else
            _context.Log("  This game has no folder yet (never saved)");
        foreach (var other in SaveSlots.All)
            _context.Log($"  {other.Name}: {other.Info?.CharacterName ?? "?"}, saved {other.Info?.SavedAt}, {other.Saves.Count} save(s)");
    }

    // Items on the ground (GroundItems): the item nearest you that the game saves (not one placed with the location)
    // goes out in the game's save format and a copy is made from it, landed right where the first lies - as another game
    // or a stash would make it. Then a wine is put at your feet with the game's hop, and its flight logged: what another
    // game needs to fly its copy along the same arc (GroundItem.Fly).
    private void CopyNearestItem(double x, double y)
    {
        var nearest = GroundItems.All()
            .Where(item => !item.IsStatic)
            .OrderBy(item => Math.Pow(item.X - x, 2) + Math.Pow(item.Y - y, 2))
            .FirstOrDefault();
        if (nearest.Instance.IsNone || nearest.ToJson() is not { } json)
            _context.Log("No item on the ground here to copy (drop one first)");
        else if (GroundItems.Create(json) is { } copy)
            _context.Log($"Copied {copy.ObjectName} at ({copy.X}, {copy.Y}) from its save: {json}");
        else
            _context.Log($"Couldn't make an item from: {json}");
        if (GroundItems.Spawn("wine", x, y, hop: true) is { Flight: { } flight })
            _context.Log($"A wine hops from ({flight.X}, {flight.Y}) to ({flight.TargetX}, {flight.TargetY}): {flight.ToJson()}");
    }

    // The saved state of the location you're in (Locations): each room it has, and each room's presets - what will spawn
    // afresh next time (its flags) and which kinds of entities it has saved. (The room you're in is saved as you leave.)
    private void LogLocation(string locationTag, GmValue roomTag)
    {
        if (Locations.Get(locationTag) is not { } location)
        {
            _context.Log($"Location {locationTag}: nothing saved yet (you're in {roomTag}, saved as you leave it)");
            return;
        }
        _context.Log($"Location {locationTag} (you're in {roomTag}): {location.Rooms.Count} room(s) saved");
        foreach (GmValue tag in location.Rooms)
        {
            if (location.Room(tag) is not { } room)
                continue;
            foreach (GmValue presetTag in room.Presets)
            {
                if (room.Preset(presetTag) is not { } preset)
                    continue;
                string kinds = "nothing saved";
                if (preset.Entities is { } entities)
                {
                    kinds = string.Join(", ", entities.Keys.Select(key => key.AsString));
                    entities.Destroy();
                }
                _context.Log($"  {tag} / {presetTag}: flags {preset.Flags}; {kinds}");
            }
        }
    }

    // Every Example Tonic lying on the ground in the room - the off-screen ones too, which the game has culled
    // (deactivated): listed with includeCulled, and destroyed safely - a culled one leaves the game's culling list
    // first. A tonic on the ground is its own object (o_loot_ and its game key), told apart by object_index even
    // while it's culled - its own variables can't be read then.
    private void ClearTonics()
    {
        int tonic = Gm.AssetGetIndex($"o_loot_{_context.Id}__{_tonic.Key}");
        if (tonic < 0)
            return;
        int cleared = 0, offScreen = 0;
        foreach (var item in Instances.All(tonic, includeCulled: true))
        {
            if (item.IsCulled)
                offScreen++;
            item.Destroy();
            cleared++;
        }
        _context.Log($"Cleared {cleared} Example Tonic(s) off the ground ({offScreen} of them off screen)");
    }
}
