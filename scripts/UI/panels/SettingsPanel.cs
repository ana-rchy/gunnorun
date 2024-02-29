using Godot;
using System;

public partial class SettingsPanel : Panel {
    [Export] VBoxContainer _binds;
    [Export] ColorPickerButton _crosshairColorPicker;

    public override void _Ready() {
        SettingsClosed += this.GetNodeConst<Settings>("SETTINGS")._OnSettingsPanelClosed;

        _crosshairColorPicker.Color = Settings.CrosshairColor;
    }

    public override void _Input(InputEvent e) {
        var mousePos = GetGlobalMousePosition();
        if (IsClickedOutsidePanel(mousePos)) {
            EmitSignal(SignalName.SettingsClosed);
            GetParent<ColorRect>().Hide();
        }
    }

    //---------------------------------------------------------------------------------//
    #region | funcs

    // pure
    bool IsClickedOutsidePanel(Vector2 mousePos) {
        return Input.IsMouseButtonPressed(MouseButton.Left) &&
        (mousePos.X < Position.X || mousePos.X > Position.X + Size.X || mousePos.Y < Position.Y || mousePos.Y > Position.Y + Size.Y);
    }

    #endregion

    //---------------------------------------------------------------------------------//
    #region | signals

    [Signal] public delegate void SettingsClosedEventHandler();

    void _OnCrosshairColorChanged(Color color) {
        Settings.CrosshairColor = color;
    }

    void _OnResetToDefaults() {
        InputMap.LoadFromProjectSettings();
        DirAccess.RemoveAbsolute("user://config.cfg");

        foreach (Button button in _binds.GetChildren()) {
            button.Text = InputMap.ActionGetEvents(button.Name)[0].AsText();
        }
    }

    #endregion
}
