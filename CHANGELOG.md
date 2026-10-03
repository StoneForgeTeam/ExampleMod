# ExampleMod changes

## Unreleased

- Added examples of using the new MainMenu button api.
- Static Charge (`Skills/StaticCharge.cs`), a passive on the Stormcalling tab: +5% Crit Chance, and weapon hits have a 25% chance to Shock their target for 3 turns. Needs the StoneForge release with passive skills.
- Shock Bolt deals 10 damage with `Combat.Damage`, as Overcharge (`DamageTypes/Overcharge.cs`): a kind of damage of the mod's own, dealt as the game's shock but half as hard again on a target already Shocked. Needs the StoneForge release with damage types.

## 0.1.0 — Initial public release

- Sample items, consumables, buffs, skills, UI panels and GML bindings for StoneForge.
- Standalone .NET 10 editor project referencing an installed StoneForge SDK.
- Requires StoneForge 0.1.0 or newer.
