# ExampleMod changes

## Unreleased

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
