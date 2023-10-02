using Godot;
using System;

namespace Pincushion.LD54 {
	public partial class HealthUIController : RichTextLabel
	{
		public SceneController Scene;

        public override void _Ready()
        {
			Scene = GetNode<SceneController>("/root/Scene");
			//SubmarineController Player = GetNode<SubmarineController>("/root/Scene/Submarine");
			Scene.Player.SubmarineDamaged += OnSubmarineHealthChanged;
			Scene.Player.SubmarineRespawned += OnSubmarineHealthChanged;
        }

		public void OnSubmarineHealthChanged() {
			Text = "Health: " + Scene.Player.Health;
		}

    }
}