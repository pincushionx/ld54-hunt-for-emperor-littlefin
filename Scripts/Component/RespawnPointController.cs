using Godot;
using System;

namespace Pincushion.LD54
{
	public partial class RespawnPointController : Node3D
	{
		[Export] public bool IsSilent = false; // That's for the original respawn point


		public SceneController Scene;

		//private 
		// Called when the node enters the scene tree for the first time.
		public override void _EnterTree()
		{
			Area3D area = GetNode<Area3D>("Area3D");
			area.BodyEntered += OnBodyEntered;

			Scene = GetNode<SceneController>("/root/Scene");
		}

        private void OnBodyEntered(Node3D node)
        {
			SubmarineController player = node is SubmarineController? node as SubmarineController : node.GetParentOrNull<SubmarineController>();
			if(player != null)
			{
				Scene.RespawnPoint = this;
			}
        }
	}
}