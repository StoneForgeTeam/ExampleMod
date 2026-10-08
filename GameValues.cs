using System.Linq;
using StoneForge;

namespace ExampleMod;

// GameMaker arrays and structs from C# (GmArray, GmStruct): made here, changed in place, through JSON, and the game's
// own - shown once in the log when the game first runs, and once in game.
internal static class GameValues
{
    // New ones of our own.
    public static void ShowMade(ModContext context)
    {
        using GmArray numbers = GmArray.From(new GmValue[] { 1, 2, 3 });
        numbers.Push(4);
        numbers[0] = 10;
        using GmStruct hero = GmStruct.Create();
        hero["name"] = "Felice";
        hero["level"] = 3;
        hero["kit"] = numbers;
        using GmStruct? back = GmStruct.FromJson(hero.ToJson());
        context.Log($"Arrays and structs: [{string.Join(", ", numbers)}] (length {numbers.Length}), "
            + $"struct {{{string.Join(", ", hero.Names.OrderBy(n => n))}}} as JSON {hero.ToJson()}, "
            + $"read back: {back?["name"]} level {back?["level"]}");
    }

    // The game's own: the player's sprite rows (global.playerSpriteArray), straight from the game.
    public static void ShowGames(ModContext context)
    {
        if (Game.Global["playerSpriteArray"].AsArray is not { } sprites)
        {
            context.Log("The game's arrays: global.playerSpriteArray isn't an array here");
            return;
        }
        using (sprites)
            context.Log($"The game's arrays: global.playerSpriteArray has {sprites.Length} sprite rows: "
                + string.Join(", ", sprites.Select(sprite => Game.CallBuiltin("sprite_get_name", sprite).AsString)));
        // The world's clock.
        context.Log($"The time: {Time.Now} ({Time.OfDay}), turn {Time.Turns}. F9 lets an hour pass.");
        // And an instance's alarms (alarm[0] to alarm[11]): steps until each goes off, -1 when it's off.
        if (Instances.First<GameInstance>(GameObjectId.o_player) is { } player)
            context.Log("The player's alarms: " + string.Join(", ", Enumerable.Range(0, Alarms.Count).Select(i => $"[{i}] {player.Alarm[i]}")));
    }
}
