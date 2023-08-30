using Godot;
using System;

public partial class PlayerPreview : TextureRect {
    public override void _Ready() {
        ((ShaderMaterial) Material).SetShaderParameter("color", Global.PlayerData.Color);
    }

    void _OnColorChanged(Color color) {
        ((ShaderMaterial) Material).SetShaderParameter("color", new Vector3(color.R, color.G, color.B));
    }
}