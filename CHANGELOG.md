# ExampleMod changes

## Unreleased

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
