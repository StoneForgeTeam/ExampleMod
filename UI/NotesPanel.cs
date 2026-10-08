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
        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.notespanel.notes"), 10, 8), live => { live.Text = ExampleText.Get("ui.notespanel.notes"); }));
        var note = Add(ExampleText.Live(new UITextBox(10, 28, 200, placeholder: ExampleText.Get("ui.notespanel.write_a_note")) { MaxLength = 40, Tooltip = ExampleText.Get("ui.notespanel.type_here_the_game_s_hotkeys") }, live => { live.Placeholder = ExampleText.Get("ui.notespanel.write_a_note"); live.Tooltip = ExampleText.Get("ui.notespanel.type_here_the_game_s_hotkeys"); }));
        _saved = Add(new UILabel("", 10, 50, Draw.Muted));
        note.Submitted += text => _saved.Text = text.Length == 0 ? "" : ExampleText.Get("ui.notespanel.noted", text);
        _loot = Add(ExampleText.Live(new UILabel("", 10, 68, Draw.Muted) { Tooltip = ExampleText.Get("ui.notespanel.every_item_on_the_ground_in") }, live => { live.Tooltip = ExampleText.Get("ui.notespanel.every_item_on_the_ground_in"); }));
        Add(ExampleText.Live(new UIButton(ExampleText.Get("ui.examplepanel.close"), 10, 0, 80, onClick: () => Visible = false) { Anchor = UIAnchor.BottomRight, Y = 8 }, live => { live.Text = ExampleText.Get("ui.examplepanel.close"); }));
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
        _loot.Text = ExampleText.Get("ui.notespanel.ground_loot_here_off_screen", loot.Count, offScreen);
    }
}
