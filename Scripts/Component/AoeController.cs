using Godot;
using System;

namespace Pincushion.LD54 {
	public partial class AoeController : Node3D
	{
		//private float _timeToLive = 5f;
		private float _speed = 10f;

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			Rotate(new Vector3(0,0,1), _speed * (float) delta);
		}
	}
}