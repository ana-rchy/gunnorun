using Godot;
using System;

public partial class SettingsPanel : Panel {
    [Export] VBoxContainer Binds;

    public override void _Input(InputEvent e) {
        var mousePos = GetGlobalMousePosition();
        if (Input.IsMouseButtonPressed(MouseButton.Left) &&
            (mousePos.X < Position.X || mousePos.X > Position.X + Size.X || mousePos.Y < Position.Y || mousePos.Y > Position.Y + Size.Y)) { // outside the panel
            GetParent<ColorRect>().Hide();
        }
    }

    void _OnResetToDefaults() {
		InputMap.LoadFromProjectSettings();
		DirAccess.RemoveAbsolute("user://config.cfg");

		foreach (Button button in Binds.GetChildren()) {
			button.Text = InputMap.ActionGetEvents(button.Name)[0].AsText();
        }
	}
}