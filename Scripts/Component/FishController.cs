using Godot;
using System;

namespace Pincushion.LD54 {
	public partial class FishController : CharacterBody3D
	{
		[Signal] public delegate void FishDiedEventHandler();

		[Export] public float Speed = 5.0f;
		[Export] public int Damage = 5;

		[Export] public int Health = 50;
		[Export] public int MaxHealth = 50;

		private Vector3 _lastKnownPlayerLocation;
		private Vector3 _currentRotation;
		private float _timeSinceAction = 0;
		private float _timeBetweenActions = 2f;

		private float _timeSinceBite = 0;
private float _timeBetweenBites = 1;

		private float _targetYRotation = 0;

		private Area3D _area;


		private ProgressBar _healthBar;
		private AnimationPlayer _animationPlayer;


		private bool _dying = false;

        public override void _EnterTree()
        {
			_lastKnownPlayerLocation = GlobalPosition;
			_healthBar = GetNode<ProgressBar>("HealthBarViewport/HealthBar");
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = Health;

			_area = GetNode<Area3D>("Area3D");
			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer1");

			_targetYRotation = Rotation.Y;
        }


private bool stopAnimations = false;
        public override void _Process(double delta)
        {
			if(_dying) {
				return;
			}

			if(_area.HasOverlappingBodies()) {
				foreach(Node3D node in _area.GetOverlappingBodies())
				{
					if(node is SubmarineController) {
						if(_timeBetweenBites < _timeSinceBite) {
							// Bite the player
							(node as SubmarineController).HitByCreature(this);

							// Animation
						if(!stopAnimations)	_animationPlayer.Play("Bite");

							_timeSinceBite = 0;
						}
						break;
					}
				}
			}
			_timeSinceBite += (float) delta;
        }

        public void _on_area_3d_area_entered(Area3D area)
		{
			if(_dying) {
				return;
			}

			if(area is BulletController) {
				Health -= (area as BulletController).Damage;
				_healthBar.Value = Health;
			}
			else if(area is AoeBulletController) {
				Health -= (area as AoeBulletController).Damage;
				_healthBar.Value = Health;
			}
			
			if(Health <= 0)
			{
				_dying = true;

				// Maybe play an animation?
				if(!stopAnimations) _animationPlayer.Play("Die");

								// Where to notify?
				EmitSignal(SignalName.FishDied);
			}
		}

        public override void _PhysicsProcess(double delta)
		{

			if(_dying) {
				return;
			}

			if((_lastKnownPlayerLocation - GlobalPosition).Length() > 0.5f) {
				// move toward the player

				Vector3 velocity = Velocity;
				Vector3 direction = (_lastKnownPlayerLocation - GlobalPosition).Normalized();
				if (direction != Vector3.Zero)
				{
					velocity.X = direction.X * Speed;
					velocity.Y = direction.Y * Speed;
				}
				else
				{
					velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
					velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
				}
				Velocity = velocity;
				MoveAndSlide();

				
				//_currentRotation.Y = (direction.X < 0)? Mathf.Pi : 0;





Vector3 rot = Rotation;
				if(rot.Y < -0.1f)
				{
					rot.Y += 2*Mathf.Pi;
				}
				if(_targetYRotation < -0.1f)
				{
					_targetYRotation += 2*Mathf.Pi;
				}

				if(velocity.X > 0 && rot.Y != 0) {
					_targetYRotation = 0;
				}
				else if(velocity.X < 0 && rot.Y == 0) {
					_targetYRotation = Mathf.Pi;
				}

				if(_targetYRotation != rot.Y) {
					rot.Y = Mathf.MoveToward(rot.Y, _targetYRotation, (float) delta);
				}


Rotation = rot;




				_timeSinceAction = 0;
			}
			else 
			{ 

				//_currentRotation = Rotation;
				if(_currentRotation.Y < -0.1f)
				{
					_currentRotation.Y += 2*Mathf.Pi;
				}

				if(_timeSinceAction > _timeBetweenActions) {
					// Randomly rotate around to search for the player

	//-Mathf.Pi, Mathf.Pi

					if(Random.Shared.Next() % 2 == 0)
					{
						_currentRotation.Y = (_currentRotation.Y == 0)? Mathf.Pi : 0;
					}
					//else
					//{
					//	float randomAngle = Random.Shared.NextSingle() % Mathf.Pi;
				//		randomAngle = Mathf.Clamp(randomAngle, -Mathf.Pi/3, Mathf.Pi/3);
			//			_currentRotation = Rotation.Rotated(Vector3.Forward, randomAngle);
		//			}
					//Rotate(Vector3.Forward, Ra)
					_timeSinceAction = 0;

					
				}
				
			}
		if(!Rotation.IsEqualApprox(_currentRotation)) 
			{
				// Do the random rotation
				Rotation = Rotation.MoveToward(_currentRotation, (float) delta*10f);
				_timeSinceAction = 0;
			}
			else
			{
				_timeSinceAction += (float) delta;
			}
			
			

			LookForPlayer();
		}


		private void LookForPlayer() {

			ShapeCast3D shapeCast3D = GetNode<ShapeCast3D>("ShapeCast3D");

			if(shapeCast3D.IsColliding()) {
				GodotObject obj = shapeCast3D.GetCollider(0);
				if(obj is Node3D) 
				{
					Node3D node = obj as Node3D;
					SubmarineController controller = node is SubmarineController? node as SubmarineController : node.GetParentOrNull<SubmarineController>();

					if(controller != null)
					{
						//GD.Print("Found the ship");
						_lastKnownPlayerLocation = controller.GlobalPosition;
					}
				}
			}
		}
	}
}