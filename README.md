# ExampleMod

The StoneForge sample mod: custom items, a tonic, buffs, skills and UI panels. Repository: [StoneForgeTeam/ExampleMod](https://github.com/StoneForgeTeam/ExampleMod). The loader lives in [StoneForgeTeam/StoneForge](https://github.com/StoneForgeTeam/StoneForge).

## Requirements

Install StoneForge on Stoneshard's VM modbranch. `mod.json` says `"stoneforge": "latest"` while the sample's in development: it's built against StoneForge's latest source, and any StoneForge loads it - its newest parts need the newest StoneForge. The editor project requires the .NET 10 SDK. StoneForge compiles the mod's source itself when loading it; the editor's compiled DLL is not installed.

## Edit and build

This project does not require a StoneForge source checkout.

Point to your game installation before opening your editor or building:

```powershell
$env:STONESHARD_DIR = 'C:\Program Files (x86)\Steam\steamapps\common\Stoneshard'
dotnet build ExampleMod.csproj -c Release
```

Alternatively pass `-p:StoneForgeSdkDir="C:\path\to\Stoneshard\dotnet"`. The SDK directory must contain `StoneForge.API.dll` from the installed release. If editing directly in `<game>/mods/ExampleMod`, the project finds `<game>/dotnet` automatically.

## Install and try it

Close the game. Copy `mod.json`, the C# source folders and `Assets` into `<game>/mods/ExampleMod`. Keep existing local edits backed up. Do not copy `.git`, `.vs`, `bin` or `obj`. Run the StoneForge installer again to patch the custom consumable and skills, then start the game and enable Example Mod in the Mods window.

This mod changes game behavior: it demonstrates custom equipment and skills, displays level 99, and adds controls and panels. Use a test save.

The sample has its own version in `mod.json` and the editor project. Its minimum loader version is declared in `mod.json`. See [Writing a mod](https://github.com/StoneForgeTeam/StoneForgeDocs/blob/main/docs/modding/writing-a-mod.md) in the StoneForge documentation for API guidance.

## Localization

Requires the StoneForge 0.8.0 build with the localization API. Install the `Localization`
folder with this mod's source and Assets. US English lives in `Localization/en-US.json`;
add another culture file (for example `fr.json`) with the same keys to translate it.

`ExampleText` delegates to `context.Localization.Get`, initialized at the beginning of
Load before any settings, items, buffs, skills or panels are constructed. Display text
uses translation keys; identifiers, asset paths, logs and saved choice values stay stable.
Numbered placeholders can be reordered without changing the C# code.

`ExampleWindow` demonstrates `context.Localization.TranslationsChanged`: it rebuilds an
open window while preserving its selected tab. Text drawn each frame (such as FPS)
uses the current language automatically. Bound panels and main-menu buttons refresh when you change the game language or
save an edit to a loaded translation file (normally after about 150 ms). `ExampleText.Live`
shows how to bind control text and tooltips without resetting their values.
Registered content and settings definitions still take their text when the mod loads;
reload the mod to update those. Invalid JSON while editing keeps the last valid text.
The Left/Right setting values remain English because StoneForge saves choices by text.

The English catalog preserves the existing wording. This sample does not translate
third-party mods, engine object names, user-entered notes or diagnostic log messages.