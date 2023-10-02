using Godot;
using System;

namespace Pincushion.LD54 {
	public partial class CameraController : Camera3D
	{
		private SubmarineController _player;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_player = GetNode<SubmarineController>("/root/Scene/Submarine");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			Vector3 position = Position;
			Vector3 playerPosition = _player.Position;

			position.X = playerPosition.X;
			position.Y = playerPosition.Y;

			Position = position;
			//_player.Velocity.X
			//_player
		}
	}
}