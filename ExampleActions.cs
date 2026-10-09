using StoneForge;
using System;
using System.Collections.Generic;

namespace ExampleMod;

// One entry on the Esc menu; example features register their actions inside it.
internal static class ExampleActions
{
    private static ActionsWindow _window = null!;
    internal static void Initialize(ModContext context)
    {
        _window = context.UI.InGame.Add(new ActionsWindow());
        EscMenu.AddButton(context, () => ExampleText.Get("exampleactions.title"), _window.Open);
    }
    internal static void Add(string textKey, Action action) => _window.Actions.Add((textKey, action));

    private sealed class ActionsWindow : UIWindow
    {
        internal readonly List<(string Key, Action Action)> Actions = new();
        internal ActionsWindow() : base(ExampleText.Get("exampleactions.title"))
        {
            ContentInsets = new UIInsets(16, 32, 16, 16);
            ExampleText.Live(this, live => live.Title = ExampleText.Get("exampleactions.title"));
        }
        protected override void OnFit()
        {
            FrameWidth = Math.Min(340, Draw.Width - 24);
            FrameHeight = Math.Min(250, Draw.Height - 24);
        }
        protected override void OnOpen()
        {
            var list = Content.Add(new UIScrollArea(0, 0, Content.Width, Content.Height) { Padding = 0, Spacing = 8 });
            foreach (var entry in Actions)
            {
                var button = list.Add(new UIButton { Height = 26 });
                ExampleText.Live(button, live => live.Text = ExampleText.Get(entry.Key));
                button.Clicked += _ => { Close(); entry.Action(); };
            }
        }
    }
}
