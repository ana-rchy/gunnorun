using Godot;
using System;

public partial class KeybindButton : Button {
	void _OnPressed() {
		var oldText = Text;
		Text = "...";
	}
}
