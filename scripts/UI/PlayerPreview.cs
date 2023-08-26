using Godot;
using System;

public partial class PlayerPreview : TextureRect {
    void _OnColorChanged(Color color) {
        ((ShaderMaterial) Material).SetShaderParameter("color", new Vector3(color.R, color.G, color.B));
    }
}