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
        Add(new UILabel("Notes", 10, 8));
        var note = Add(new UITextBox(10, 28, 200, placeholder: "Write a note...") { MaxLength = 40, Tooltip = "Type here: the game's hotkeys stay quiet while you do." });
        _saved = Add(new UILabel("", 10, 50, Draw.Muted));
        note.Submitted += text => _saved.Text = text.Length == 0 ? "" : $"Noted: {text}";
        Add(new UIButton("Close", 10, 0, 80, onClick: () => Visible = false) { Anchor = UIAnchor.BottomRight, Y = 8 });
    }
}
