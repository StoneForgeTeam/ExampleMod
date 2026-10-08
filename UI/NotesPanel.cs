using StoneForge;

namespace ExampleMod.UI;

// A notepad over the game world (F8 in game): the loader keeps the game's input off mod UI - a click on it
// doesn't walk or attack, and typing in it doesn't set off the game's hotkeys (try "i" for the inventory).
public class NotesPanel : UIPanel
{
    private readonly UILabel _saved;

    public NotesPanel() : base(16, 0, 220, 96, UIAnchor.Left)
    {
        Visible = false;
        Framed = true;
        Add(ExampleText.Live(new UILabel(ExampleText.Get("ui.notespanel.notes"), 10, 8), live => { live.Text = ExampleText.Get("ui.notespanel.notes"); }));
        var note = Add(ExampleText.Live(new UITextBox(10, 28, 200, placeholder: ExampleText.Get("ui.notespanel.write_a_note")) { MaxLength = 40, Tooltip = ExampleText.Get("ui.notespanel.type_here_the_game_s_hotkeys") }, live => { live.Placeholder = ExampleText.Get("ui.notespanel.write_a_note"); live.Tooltip = ExampleText.Get("ui.notespanel.type_here_the_game_s_hotkeys"); }));
        _saved = Add(new UILabel("", 10, 50, Draw.Muted));
        note.Submitted += text => _saved.Text = text.Length == 0 ? "" : ExampleText.Get("ui.notespanel.noted", text);
        Add(ExampleText.Live(new UIButton(ExampleText.Get("ui.examplepanel.close"), 10, 0, 80, onClick: () => Visible = false) { Anchor = UIAnchor.BottomRight, Y = 8 }, live => { live.Text = ExampleText.Get("ui.examplepanel.close"); }));
    }
}
