using System;
using Godot;

public partial class KeybindButton : Button {
	bool ChangingBind;

    public override void _Ready() {
		if (InputMap.ActionGetEvents(Name).Count != 0) {
			Text = InputMap.ActionGetEvents(Name)[0].AsText();
		} else {
			Text = " ";
		}
    }

    public override void _Input(InputEvent e) {
        if (ChangingBind && e.IsPressed()) {
			if (InputMap.ActionGetEvents(Name).Count == 0 ||
				InputMap.ActionGetEvents(Name)[0].AsText() != e.AsText()) {
				ChangeBind(e);
			} else {
				Unbind();
			}

			SaveKeybindConfig();
			
			ButtonPressed = false;
			_OnToggled(false);
		}
    }

    //---------------------------------------------------------------------------------//
    #region | signals

    void _OnToggled(bool pressed) {
		if (pressed) {
			Text = "...";
			ChangingBind = true;
		} else {
			Text = InputMap.ActionGetEvents(Name).Count != 0 ? InputMap.ActionGetEvents(Name)[0].AsText() : " ";
			ChangingBind = false;
		}
	}

	#endregion

	//---------------------------------------------------------------------------------//
    #region | funcs
	
	void ChangeBind(InputEvent e) {
		if (e is InputEventMouseButton) {
			((InputEventMouseButton) e).DoubleClick = false;
		}
		
		InputMap.ActionEraseEvents(Name);
		InputMap.ActionAddEvent(Name, e);
	}

	void Unbind() {
		Input.ActionRelease(Name); // otherwise, action isnt released, shoots continuously after unbinding
		InputMap.ActionEraseEvents(Name);
	}

	void SaveKeybindConfig() {
		var config = new ConfigFile();
		if (InputMap.ActionGetEvents(Name).Count == 0) {
			config.SetValue("Keybinds", Name, "None");
			config.Save("user://config.cfg");
			return;
		}

		var bind = InputMap.ActionGetEvents(Name)[0];

		if (bind is InputEventMouseButton) {
			var mouseBind = (InputEventMouseButton) bind;
			config.SetValue("Keybinds", Name, $"m{mouseBind.ButtonIndex}");
		} else if (bind is InputEventKey) {
			var keyBind = (InputEventKey) bind;
			config.SetValue("Keybinds", Name, $"k{keyBind.Keycode}");
		}

		config.Save("user://config.cfg");
	}

	#endregion
}
