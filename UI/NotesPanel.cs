using StoneForge;

namespace ExampleMod.UI;

// A notepad over the game world (F8 in game): the loader keeps the game's input off mod UI - a click on it
// doesn't walk or attack, and typing in it doesn't set off the game's hotkeys (try "i" for the inventory).
// Under the note, the room's ground loot, counted twice a second - the off-screen items too, which the game has
// culled (deactivated) and GameMaker's own with skips: Instances.All(..., includeCulled: true) and IsCulled.
public class NotesPanel : UIPanel
{
    private readonly UILabel _saved;
    private readonly UILabel _loot;
    private double _sinceCount;

    public NotesPanel() : base(16, 0, 220, 116, UIAnchor.Left)
    {
        Visible = false;
        Framed = true;
        Add(new UILabel("Notes", 10, 8));
        var note = Add(new UITextBox(10, 28, 200, placeholder: "Write a note...") { MaxLength = 40, Tooltip = "Type here: the game's hotkeys stay quiet while you do." });
        _saved = Add(new UILabel("", 10, 50, Draw.Muted));
        note.Submitted += text => _saved.Text = text.Length == 0 ? "" : $"Noted: {text}";
        _loot = Add(new UILabel("", 10, 68, Draw.Muted) { Tooltip = "Every item on the ground in this room. Off screen ones are culled by the game - deactivated, skipped by its own with - but still there." });
        Add(new UIButton("Close", 10, 0, 80, onClick: () => Visible = false) { Anchor = UIAnchor.BottomRight, Y = 8 });
    }

    protected override void OnUpdate(double deltaTime)
    {
        if (!Visible || (_sinceCount += deltaTime) < 0.5)
            return;
        _sinceCount = 0;
        var loot = Instances.All(GameObjectId.o_loot, includeCulled: true);
        int offScreen = 0;
        foreach (var item in loot)
            if (item.IsCulled)
                offScreen++;
        _loot.Text = $"Ground loot here: {loot.Count} ({offScreen} off screen)";
    }
}
