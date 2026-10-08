# ExampleMod changes

## Unreleased

- **No GML any more**: the `GML` folder (Twice, Add, CountNearbyEnemies) and the log line calling them are gone. A mod with GML can't load on the game's native build, which StoneForge is moving to.
- Logs the other mods loaded before it (`context.Mods`), and has a little API for mods that require it: `Hello(from)`, `Greetings` and `GiveTonic()` (see the ExampleAddon mod).
- The worn blade's mark of ours is a `ModData` value (`blade.ModData(context)["given"]`), under a key no other mod's can meet.
- **Loot tables** (`ExampleInventory.cs`): every container's loot table may give an Example Tonic, a quarter of the time (`LootTables.EditAll`), and **Make It a Boss Chest** on an unopened container's right-click menu has it roll a tier 5 crypt boss chest's loot (`Containers.SetLootTable`).
- **Items, containers and the game's events to try** (`ExampleInventory.cs`):
  - A chest's right-click menu (any container in the world) has **Peek** (what's in it, read without opening it), **Stash a Tonic**, **Stash a Worn Blade** and **Take a Tonic**. They work open or closed: a closed chest has them saved in it, and a never-opened one stays unopened, with them joining its loot as it first opens.
  - The Esc menu has **Give a Worn Blade**: the Example Blade at a quarter of its condition, marked with a value of ours (`Inventory.Add` with `setup`).
  - Logged: containers opened and closed, items put in and taken out, gear put on and taken off, your skills used, and quests started, moving on, done and failed.
  - The **Cheat death once** setting: the first time you'd die, you're left at a quarter of your health (`Player.OnDying`).
- Logs each level you gain (`Player.OnLevelUp`) and each item you come to carry (`Inventory.OnAdded`).
- Logs the rooms you go into (`Rooms.OnEntered`) and the units you kill (`Units.OnDied`). Needs the StoneForge release with these events.
- The Esc menu has **Mark This Spot** (`MapMarkers`): a flag on the world map on the cell you're on, or, if one's there already, it comes off. The markers you place or take off on the map yourself are logged (`MapMarkers.OnPlaced`, `OnRemoved`). Needs the StoneForge release with `MapMarkers`.
- Uses StoneForge's `Cell` (and `Point`) in place of `(x, y)` tuples: `Mouse.Cell`, `Units.CellOf`, `Units.NearestFreeCell`, `Player.WalkTo(cell)` and `Exits.Nearest(cell)`. Needs the StoneForge release with `Cell`.
- **The world through the game's own menus and the mouse** (`ExampleWorld.cs`):
  - The Esc menu has **Rest an Hour** (`EscMenu`). It asks with the game's own confirmation (`GameDialogs.Confirm`),
    holds the screen black with "Resting..." (`Blackout`) while an hour passes (`Time.Advance`), then fades back.
  - An enemy's right-click menu has **Inspect**, **Stun** and **Push** (`ContextMenus`):
    - Inspect logs the cell it stands on (`Units.CellOf`), its health, whether it's after you (`Player.IsHuntedBy`)
      and the effects on it (`UnitEffects.On`).
    - Stun puts the game's stun on it from you (`UnitEffects.Create`).
    - Push moves it a cell away from you, onto the nearest free cell (`Units.NearestFreeCell`, `Units.Move`).
  - A **middle click on the world** (`Mouse.ClickedWorld`: not on any window) logs the cell and who stands on it
    (`Mouse.Cell`, `Mouse.Unit`), and walks you there if it's empty (`Player.WalkTo`). **Ctrl+middle click** goes out by
    the nearest way out (`Exits`, which was `Doors`).
- **A badge on the game's HUD** (`UI/HudBadge.cs`, `ModUI.Hud`), drawn under the game's windows and hidden with its HUD,
  at the right edge. It shows your level and whether enemies are after you (`Player`), the icons of the effects on you
  (`UnitEffects`), and the cell under the mouse with who's on it. It's drawn with `Draw.Frame`, `Draw.Circle`,
  `Draw.SpriteExt` and the game's digit font (`GameFont.Digits`).
- The mod's per-frame work is timed in StoneForge's profiler (Ctrl+Shift+P) as "world" (`Profiler.Measure`).
- The twin (Shift+F12) is drawn with `Draw.SpriteExt`, and F4's ability point goes to `Player.Instance`.
- `"stoneforge": "latest"` in mod.json while in development: it's built against StoneForge as it is now. A release
  names the StoneForge it was built against.
- A twin (`CharacterLook`): Shift+F12 reads your look and draws a character built from it a tile to your right, its sprites made by the game's own compositor (`Build`). Shift+F12 again removes it. Needs the StoneForge release with `CharacterLook`.
- Needs StoneForge 0.4.0.
- Moving between screens (`Rooms`): F1 goes into the room you're in again, as a door to it would. Example Button -> Play Options has Load Newest Save (`Rooms.LoadSave`) and New Adventure (`Rooms.StartNew`). Needs the StoneForge release with `Rooms`.
- Saves (`SaveData`, `SaveSlots`): F3 logs the save data's sections, the character's apart from the world's, then this game's character folder with its saves, and every other folder. Each save made with the mod on is counted in the folder's info (`SaveSlots.OnInfoSaving`), and the save menu's header shows the count after the character's name (`SaveSlots.SetTitle`). Needs the StoneForge release with `SaveData` / `SaveSlots`.
- F12 copies the ground item nearest you (`GroundItems`): its save in the game's format, and a copy made from it lying right where it lies. Then it puts a wine at your feet with the game's hop and logs its flight. Needs the StoneForge release with `GroundItems`.
- F11 logs the saved state of the location you're in (`Locations`): its rooms, and each preset's flags and the kinds of entities it has saved. Needs the StoneForge release with `Locations`.
- Where you are on the world map (`WorldMap`): the clock's third line shows the location, the cell and the dungeon floor, and F10 logs the place string, the cell's seeds and its dungeon's values. Needs the StoneForge release with `WorldMap`.
- A clock at the top of the screen in game (`UI/ClockPanel.cs`): the time, the time of day, the date and the turn, from `Time`. It lets clicks through, and the "Show the clock" setting turns it off.
- F9 lets an hour of game time pass (`Time.Advance`), logging the time before and after; the in-game log line shows the time and turn (`Time.Now`, `Time.OfDay`, `Time.Turns`). Needs the StoneForge release with `Time`.
- Needs StoneForge 0.3.0.
- Off-screen instances (StoneForge's culled instances):
  - The notepad (F8) counts the room's ground loot twice a second, the off-screen items too: `Instances.All(..., includeCulled: true)` and `IsCulled`.
  - F2 clears every Example Tonic off the ground in the room, off-screen ones included. Each is listed by its own ground object, which is told apart by `object_index` even while it's culled, and removed with the safe `Destroy()`.
- GameMaker arrays and structs from C# (`GameValues.cs`): the log shows one of each made, changed in place and sent through JSON when the game starts, and the game's own `global.playerSpriteArray` once you're in game, with the player's alarms (`Alarm[n]`). Needs the StoneForge release with `GmArray` / `GmStruct` and alarms.
- Example Button's menu has the game's Back, and a Play Options menu inside it with the game's Continue, New Game, Load Game, Prologue, Adventure and Back.
- Window Styles (main menu): a dialog in the game's confirm-panel frame, with no title, no close button and its two buttons in the frame's places. Its Sliced button opens the same frame 9-sliced to a bigger window over it (`UI/ExampleDialogs.cs`).
- Added examples of using the new MainMenu button api.
- Static Charge (`Skills/StaticCharge.cs`), a passive on the Stormcalling tab: +5% Crit Chance, and weapon hits have a 25% chance to Shock their target for 3 turns. Needs the StoneForge release with passive skills.
- Shock Bolt deals 10 damage with `Combat.Damage`, as Overcharge (`DamageTypes/Overcharge.cs`): a kind of damage of the mod's own, dealt as the game's shock but half as hard again on a target already Shocked. Needs the StoneForge release with damage types.

## 0.1.0 — Initial public release

- Sample items, consumables, buffs, skills, UI panels and GML bindings for StoneForge.
- Standalone .NET 10 editor project referencing an installed StoneForge SDK.
- Requires StoneForge 0.1.0 or newer.
