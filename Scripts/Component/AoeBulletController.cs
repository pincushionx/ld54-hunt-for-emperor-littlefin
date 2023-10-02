using Godot;
using System;
namespace Pincushion.LD54 {
	public partial class AoeBulletController : Area3D
	{
		[Export] public int Damage = 3;

		private float _speed = 5f;

		
        public override void _PhysicsProcess(double delta)
        {		
			Rotate(new Vector3(0,0,1), _speed * (float) delta);
        }
	}
}