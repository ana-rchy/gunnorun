using System;
using Godot;

public partial class KeybindButton : Button {
	bool ChangingBind;

    public override void _Ready() {
		Text = InputMap.ActionGetEvents(Name)[0].AsText();
    }

    public override void _Input(InputEvent e) {
        if (ChangingBind && e.IsPressed()) {
			ChangeBind(e);
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
			Text = InputMap.ActionGetEvents(Name)[0].AsText();
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

	void SaveKeybindConfig() {
		var config = new ConfigFile();

		var bind = InputMap.ActionGetEvents(Name)[0];
		GD.Print(Name + "\t" + bind);

		if (bind is InputEventMouseButton) {
			var mouseBind = (InputEventMouseButton) bind;
			config.SetValue("Keybinds", Name, "m" + mouseBind.ButtonIndex.ToString());
		} else if (bind is InputEventKey) {
			var keyBind = (InputEventKey) bind;
			config.SetValue("Keybinds", Name, "k" + keyBind.Keycode.ToString());
		}

		config.Save("user://config.cfg");
	}

	#endregion
}
