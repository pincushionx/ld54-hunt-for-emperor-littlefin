using Godot;
using System;

namespace Pincushion.LD54
{
	public partial class RelicController : Node3D
	{
		public SceneController Scene;
		public bool RelicConsumed = false;

		[Export] public bool IsBulletRelic = false;
		[Export] public bool IsAoeRelic = false;
		[Export] public bool IsSpeedRelic = false;
		[Export] public bool IsShieldRelic = false;
		[Export] public bool IsShrinkRelic = false;

		private GpuParticles3D _particles;

        public override void _EnterTree()
        {
  			Area3D area = GetNode<Area3D>("Area3D");
			area.BodyEntered += OnBodyEntered;

			Scene = GetNode<SceneController>("/root/Scene");

			_particles = GetNode<GpuParticles3D>("Sparkles");
			_particles.Emitting = false;
        }

		private void OnBodyEntered(Node3D node)
        {
			SubmarineController player = node is SubmarineController? node as SubmarineController : node.GetParentOrNull<SubmarineController>();
			if(player != null)
			{
				GD.Print("Hit relic " + Name);
				RelicConsumed = true;
				_particles.Emitting = true;
				player.RececeivedRelic(this);
			}
        }
	}
}