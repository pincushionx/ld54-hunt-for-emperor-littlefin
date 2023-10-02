using Godot;
using System;

namespace Pincushion.LD54 {
	public partial class BulletController : Area3D
	{
		[Export] public int Damage = 10;

		private float _timeToLive = 3f;
		private float _speed = 50f;

		
        public override void _PhysicsProcess(double delta)
        {		
			Transform3D transform = Transform;
			Position += transform.Basis.X * _speed * (float) delta;

            _timeToLive -= (float) delta;
			if(_timeToLive < 0f) {
				QueueFree();
			}
        }
    }
}