using System.Linq;
using StoneForge;

namespace ExampleMod.UI;

// A badge on the game's HUD (ModUI.Hud: drawn with the HUD, so the game's windows - the inventory, the map - cover it,
// and hidden with the HUD in cutscenes), at the right edge: your level and whether enemies are after you (Player), the
// effects on you as the game shows them (UnitEffects), and the cell under the mouse with who stands there (Mouse) -
// drawn with Draw's shapes, sprites and the game's fonts.
public class HudBadge : UIElement
{
    private const int MaxIcons = 6;
    private static readonly int Gold = Draw.Rgb(214, 186, 120), Calm = Draw.Rgb(120, 200, 120), Fighting = Draw.Rgb(220, 90, 70);

    public HudBadge()
    {
        Anchor = UIAnchor.Right;
        X = 8;
        Width = 130;
        Height = 54;
        // (Clicks go through to the game.)
        HitTest = false;
    }

    protected override void OnDraw(double x, double y)
    {
        if (!Player.Exists)
            return;
        Draw.Frame(x, y, Width, Height, alpha: 0.9);
        // Level, and a dot: green calm, red when enemies are after you.
        Draw.Text(x + 8, y + 7, ExampleText.Get("ui.hudbadge.lv", Player.Level), Gold, Draw.AlignLeft, Draw.AlignTop, GameFont.Digits);
        bool fighting = Player.InCombat;
        Draw.Circle(x + Width - 12, y + 11, 4, fighting ? Fighting : Calm);
        Draw.Circle(x + Width - 12, y + 11, 4, Draw.Black, outline: true);
        Draw.Text(x + Width - 20, y + 7, fighting ? ExampleText.Get("ui.hudbadge.in_combat") : ExampleText.Get("ui.hudbadge.calm"), Draw.Muted, Draw.AlignRight, Draw.AlignTop);
        // The effects on you that show: their icons, half size.
        var effects = UnitEffects.On(Player.Instance).Where(effect => effect.Shown).Take(MaxIcons).ToList();
        for (int i = 0; i < effects.Count; i++)
            Draw.SpriteExt(UnitEffects.IconOf(effects[i].Name), 0, x + 10 + i * 18, y + 30, 0.5, 0.5);
        if (effects.Count == 0)
            Draw.Text(x + 8, y + 22, ExampleText.Get("ui.hudbadge.no_effects"), Draw.Muted);
        // The cell under the mouse, and who's on it.
        Cell cell = Mouse.Cell;
        Instance unit = Mouse.Unit;
        string who = unit.IsNone ? "" : Units.IsPlayer(unit) ? " - you" : " - " + Gm.ObjectGetName(unit.Get("object_index").AsInt).Replace("o_", "");
        Draw.Text(x + 8, y + Height - 14, ExampleText.Get("ui.hudbadge.value", cell, who), Draw.Muted);
    }
}
