using Godot;
using System;
using System.Collections.Generic;

namespace Pincushion.LD54
{
	public partial class SceneController : Node3D
	{
		[Signal] public delegate void PlayerWonEventHandler();

		public RespawnPointController RespawnPoint;
		public SubmarineController Player;

		public FishController LittleFin;

		//public List<FishController> fishControllers = new List<FishController>();

		// Called when the node enters the scene tree for the first time.
		public override void _EnterTree()
		{
			Player = GetNode<SubmarineController>("Submarine");
			RespawnPoint = GetNode<RespawnPointController>("RespawnPoints/StartingRespawnPoint");
			//RespawnPoint = GetNode<RespawnPointController>("RespawnPoints/RespawnPoint3");

			Player.SubmarineDestroyed += OnSubmarineDestroyed;

			LittleFin  = GetNode<FishController>("BossFight/LittleFin");
			LittleFin.FishDied += OnWinCondition;
		}
        public override void _Ready()
        {
            //Player.Respawn(RespawnPoint);
        }

		public void OnWinCondition() {
			EmitSignal(SignalName.PlayerWon);
		}


        private void OnSubmarineDestroyed() {
			Respawn();
		}
		private void Respawn()
		{
			Player.Respawn(RespawnPoint);
		}
	}
}