# ExampleMod

The StoneForge sample mod: custom items, a tonic, buffs, skills, UI panels and GML bindings. Repository: [StoneForgeTeam/ExampleMod](https://github.com/StoneForgeTeam/ExampleMod). The loader lives in [StoneForgeTeam/StoneForge](https://github.com/StoneForgeTeam/StoneForge).

## Requirements

Install StoneForge on Stoneshard's VM modbranch: the version `mod.json` names, or newer (the newest parts need the release after 0.5.0). The editor project requires the .NET 10 SDK. StoneForge compiles the mod's source itself when loading it; the editor's compiled DLL is not installed.

## Edit and build

Keep the checkout folder named `ExampleMod`: the folder name determines the generated `ExampleMod.Gml` namespace. This project does not require a StoneForge source checkout.

Point to your game installation before opening your editor or building:

```powershell
$env:STONESHARD_DIR = 'C:\Program Files (x86)\Steam\steamapps\common\Stoneshard'
dotnet build ExampleMod.csproj -c Release
```

Alternatively pass `-p:StoneForgeSdkDir="C:\path\to\Stoneshard\dotnet"`. The SDK directory must contain `StoneForge.API.dll` and `StoneForge.GmlGenerator.dll` from the same installed release. If editing directly in `<game>/mods/ExampleMod`, the project finds `<game>/dotnet` automatically.

## Install and try it

Close the game. Copy `mod.json`, the C# source folders, `Assets` and `GML` into `<game>/mods/ExampleMod`. Keep existing local edits backed up. Do not copy `.git`, `.vs`, `bin` or `obj`. Run the StoneForge installer again to patch the custom consumable, skills and GML, then start the game and enable Example Mod in the Mods window.

This mod changes game behavior: it demonstrates custom equipment and skills, displays level 99, and adds controls and panels. Use a test save. GML executes directly in the game and requires a restart after changes.

The sample has its own version in `mod.json` and the editor project. Its minimum loader version is declared in `mod.json`. See [Writing a mod](https://github.com/StoneForgeTeam/StoneForgeDocs/blob/main/docs/modding/writing-a-mod.md) and [GML bindings](https://github.com/StoneForgeTeam/StoneForgeDocs/blob/main/docs/modding/gml-bindings.md) in the StoneForge documentation for API and binding guidance.
