using Godot;
using System;

namespace Pincushion.LD54
{
	public partial class OverlayController : Node
	{

		[Export] private Label BulletsCooldownLabel;
		[Export] private Label AoeCooldownLabel;
		[Export] private Label SpeedCooldownLabel;
		[Export] private Label ShrinkCooldownLabel;
		[Export] private Label ShieldCooldownLabel;



		[Export] private Button BulletsButton;
		[Export] private Button AoEButton;
		[Export] private Button SpeedButton;
		[Export] private Button ShrinkButton;
		[Export] private Button ShieldButton;
		[Export] private Texture2D _HiddenAbilityIcon;

		[Export] private Texture2D _BulletsButtonIcon;
		[Export] private Texture2D _AoEButtonIcon;
		[Export] private Texture2D _SpeedButtonIcon;
		[Export] private Texture2D _ShrinkButtonIcon;
		[Export] private Texture2D _ShieldButtonIcon;



		[Export] private Control _playerWonPanel;
		[Export] private Button _playerWonPanelCloseButton;
		


[Export] private Control _introPanel;
		[Export] private Button _introPanelCloseButton;


		private SceneController Scene;
		private SubmarineController Player;
		
		// Called when the node enters the scene tree for the first time.
		public override void _EnterTree()
		{
			
		}

        public override void _Ready()
        {
			Scene = GetNode<SceneController>("/root/Scene");
			Player = GetNode<SubmarineController>("/root/Scene/Submarine");

			BulletsButton.Icon = _HiddenAbilityIcon;
			AoEButton.Icon = _HiddenAbilityIcon;
			SpeedButton.Icon = _HiddenAbilityIcon;
			ShrinkButton.Icon = _HiddenAbilityIcon;
			ShieldButton.Icon = _HiddenAbilityIcon;

			BulletsButton.Pressed += () => Player.FireBullets();
			AoEButton.Pressed += () => Player.StartAoe();
			SpeedButton.Pressed += () => Player.StartSpeedBoost();
			ShrinkButton.Pressed += () => Player.StartShrink();
			ShieldButton.Pressed += () => Player.StartShield();

			Player.ReceivedRelic += OnReceivedRelic;



			Scene.PlayerWon += OnPlayerWon;
			_playerWonPanelCloseButton.Pressed += OnPlayerWonClosePressed;
			_playerWonPanel.Hide();


			_introPanelCloseButton.Pressed += OnIntroPanelCloseButtonPressed;
			_introPanel.Show();
        }

        private void OnIntroPanelCloseButtonPressed()
        {
            _introPanel.Hide();
        }


        private void OnPlayerWonClosePressed()
        {
            _playerWonPanel.Hide();
        }


        private void OnPlayerWon()
        {
            _playerWonPanel.Show();
        }


        private void OnReceivedRelic(RelicController relic)
        {
            
        	if(relic.IsBulletRelic) {
				BulletsButton.Icon = _BulletsButtonIcon;
			}
			if(relic.IsAoeRelic) {
				AoEButton.Icon = _AoEButtonIcon;
			}
			if(relic.IsSpeedRelic) {
				SpeedButton.Icon = _SpeedButtonIcon;
			}
			if(relic.IsShieldRelic) {
				ShieldButton.Icon = _ShieldButtonIcon;
			}
			if(relic.IsShrinkRelic) {
				ShrinkButton.Icon = _ShrinkButtonIcon;
			}
		}

        public override void _Process(double delta)
        {
			if(Player.HasBullets) {
				// Never has a cooldown
			}
			if(Player.HasAoe) {
				AoeCooldownLabel.Text = Player.AoeActive? 
					Player.AoeTimeRemaining.ToString("#0") : Player.AoeCooldownTime.ToString("#0");
			}
			if(Player.HasSpeed) {
				SpeedCooldownLabel.Text = Player.SpeedActive?
					Player.SpeedTimeRemaining.ToString("#0") : Player.SpeedCooldownTime.ToString("#0");
			}
			if(Player.HasShield) {
				ShieldCooldownLabel.Text = Player.ShieldActive?
					Player.ShieldTimeRemaining.ToString("#0") : Player.ShieldCooldownTime.ToString("#0");
			}
			if(Player.HasShrink) {
				ShrinkCooldownLabel.Text = Player.ShrinkActive?
					Player.ShrinkTimeRemaining.ToString("#0") : Player.ShrinkCooldownTime.ToString("#0");
			}
        }
    }
}