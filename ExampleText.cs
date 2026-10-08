using StoneForge;

namespace ExampleMod;

// One catalog belongs to this mod. Initialize it before constructing settings, content or UI.
// IDs, sprite paths and saved option values stay separate from translated display text.
internal static class ExampleText
{
    private static ModLocalization _texts = null!;
    internal static void Initialize(ModContext context) => _texts = context.Localization;
    internal static T Live<T>(T target, System.Action<T> refresh) where T : class
    {
        _texts.Bind(target, refresh);
        return target;
    }
    internal static string Get(string key, params object?[] arguments) => _texts.Get(key, arguments);
}
