using System;
using System.IO.Pipes;
using Godot;


namespace Pincushion.LD54 {
	public partial class SubmarineController : CharacterBody3D
	{
    	[Signal] public delegate void SubmarineDamagedEventHandler();
		[Signal] public delegate void SubmarineDestroyedEventHandler();
		[Signal] public delegate void SubmarineRespawnedEventHandler();
		[Signal] public delegate void SubmarineHealedEventHandler();


		[Signal] public delegate void ReceivedRelicEventHandler(RelicController relic);

		// prefabs
		[Export] private PackedScene BulletPrefab;
		[Export] private PackedScene AoePrefab;

		// stats
		public int Health = 100;
		private readonly int MaxHealth = 100;
		private readonly int BaseCrashDamage = 15;

		// Buffs
		public bool HasBullets = false;
		public bool HasAoe = false;
		public bool HasShield = false;
		public bool HasSpeed = false;
		public bool HasShrink = false;


		public bool AoeActive = false;
		public bool ShieldActive = false;
		public bool SpeedActive = false;
		public bool ShrinkActive = false;

		public float AoeMaxTime = 5;
		public float ShieldMaxTime = 5;
		public float SpeedMaxTime = 5;
		public float ShrinkMaxTime = 5;

		public float AoeTimeRemaining = 0;
		public float ShieldTimeRemaining = 0;
		public float SpeedTimeRemaining = 0;
		public float ShrinkTimeRemaining = 0;

		public float AoeMaxCooldownTime = 10;
		public float ShieldMaxCooldownTime = 10;
		public float SpeedMaxCooldownTime = 10;
		public float ShrinkMaxCooldownTime = 10;

		public float AoeCooldownTime = 0;
		public float ShieldCooldownTime = 0;
		public float SpeedCooldownTime = 0;
		public float ShrinkCooldownTime = 0;

		



		private float _shrinkFactor = 0.5f;
		private float _shieldBonus = 2f;
		private float _currentShieldBonus = 1f;


		// Internal
		private ProgressBar _healthBar;
		private AnimationPlayer _animationPlayer;
		private Camera3D _camera;

		private float _gravity = 0.1f;
		private float _drag = 0.25f;

		private Vector2 _velocity;
		private Vector2 _acceleration = new Vector2(0.25f, 0.5f);
		private Vector2 _currentMaxSpeed = new Vector2(1f, 1f);
		private Vector2 _currentBuffSpeed = new Vector2(1f, 1f);
		private Vector2 _baseMaxSpeed = new Vector2(1f, 1f);
		private Vector2 _speedBonus = new Vector2(2f, 2f);

		private float _targetYRotation = 0;
		private Vector2 _rotationSpeed = new Vector2(2f, 10f);
		private Vector2 _bounceForce = new Vector2(0.5f, 0.5f);

		private float _bounceTime = 0.025f;
		private float _bounceTimeLeft = 0f;


		private Node3D _shieldSphere;


		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			//_camera = GetNode<Camera3D>("Camera3D");
			_healthBar = GetNode<ProgressBar>("HealthBarViewport/HealthBar");
			_healthBar.MaxValue = MaxHealth;
			_healthBar.Value = Health;
			

			_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

			_shieldSphere =  GetNode<Node3D>("Shield");
			_shieldSphere.Hide();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			// process cooldowns
			if(AoeCooldownTime > 0) {
				AoeCooldownTime -= (float) delta;

				if(AoeCooldownTime <= 0) {
					AoeCooldownTime = 0;
					// ready indicator
				}
			}
			if(SpeedCooldownTime > 0) {
				SpeedCooldownTime -= (float) delta;

				if(SpeedCooldownTime <= 0) {
					// ready indicator
					SpeedCooldownTime = 0;
				}
			}
			if(ShrinkCooldownTime > 0) {
				ShrinkCooldownTime -= (float) delta;

				if(ShrinkCooldownTime <= 0) {
					// ready indicator
					ShrinkCooldownTime = 0;
				}
			}
			if(ShieldCooldownTime > 0) {
				ShieldCooldownTime -= (float) delta;

				if(ShieldCooldownTime <= 0) {
					// ready indicator
					ShieldCooldownTime = 0;
				}
			}


			// Process Durations
			if(AoeActive) {
				AoeTimeRemaining -= (float) delta;

				if(AoeTimeRemaining <= 0) {
					StopAoe();
				}
			}
			if(SpeedActive) {
				SpeedTimeRemaining -= (float) delta;

				if(SpeedTimeRemaining <= 0) {
					StopSpeedBoost();
				}
			}
			if(ShrinkActive) {
				ShrinkTimeRemaining -= (float) delta;

				if(ShrinkTimeRemaining <= 0) {
					StopShrink();
				}
			}
			if(ShieldActive) {
				ShieldTimeRemaining -= (float) delta;

				if(ShieldTimeRemaining <= 0) {
					StopShield();
				}
			}
		}

		public void RececeivedRelic(RelicController relic) {
			bool update = false;

			if(relic.IsBulletRelic && !HasBullets) {
				HasBullets = true;
				update = true;
			}
			if(relic.IsAoeRelic && !HasAoe) {
				HasAoe = true;
				update = true;
			}
			if(relic.IsSpeedRelic && !HasSpeed) {
				HasSpeed = true;
				update = true;
			}
			if(relic.IsShieldRelic && !HasShield) {
				HasShield = true;
				update = true;
			}
			if(relic.IsShrinkRelic && !HasShrink) {
				HasShrink = true;
				update = true;
			}

			if(update) {
				EmitSignal(SignalName.ReceivedRelic, relic);
				GetNode<GpuParticles3D>("Sparkles").Emitting = true;
			}
		}

        public override void _Input(InputEvent @event)
        {
			if(@event.IsActionPressed("FireBullets")) {
				FireBullets();
			}
			else if(@event.IsActionPressed("FireAOE")) {
				StartAoe();
			}
			else if(@event.IsActionPressed("SpeedBoost")) {
				StartSpeedBoost();
			}
			else if(@event.IsActionPressed("Shield")) {
				StartShield();
			}
			else if(@event.IsActionPressed("Shrink")) {
				StartShrink();
			}
        }

		public void FireBullets() {
			if(HasBullets) {
				BulletController bullet = BulletPrefab.Instantiate<BulletController>();
				AddChild(bullet);
			}
		}

		public void StartAoe()
		{
			if(HasAoe && AoeCooldownTime <= 0) {
					
				AoeCooldownTime = AoeMaxCooldownTime;
				AoeTimeRemaining = AoeMaxTime;
				
				AoeController bullet = AoePrefab.Instantiate<AoeController>();
				bullet.Name = "AOE";
				AddChild(bullet);
				AoeActive = true;
			}
		}
		public void StartSpeedBoost()
		{
			if(HasSpeed && SpeedCooldownTime <= 0) {

				//SpeedCooldownTime = SpeedMaxCooldownTime;
				//_currentBuffSpeed = _speedBonus;
				//_currentMaxSpeed = _baseMaxSpeed * _speedBonus;


				// Speed is now a heal
				Health = MaxHealth;
				_healthBar.Value = Health;
				EmitSignal(SignalName.SubmarineHealed);

				SpeedTimeRemaining = SpeedMaxTime; 
				// SpeedActive =  true; 
			}
		}
		public void StartShield()
		{
			if(HasShield && ShieldCooldownTime <= 0) {
					
					
				_currentShieldBonus = _shieldBonus;

				// Show the fresnel sphere
				_shieldSphere.Show();

				ShieldTimeRemaining = ShieldMaxTime;
				ShieldActive = true;
				ShieldCooldownTime = ShieldMaxCooldownTime;
			}
		}
		public void StartShrink()
		{
			if(HasShrink && ShrinkCooldownTime <= 0) {
				
				Scale *= _shrinkFactor;
				ShrinkTimeRemaining = ShrinkMaxTime; 
				ShrinkActive = true;
				ShrinkCooldownTime = ShrinkMaxCooldownTime;
			}
		}


		private void StopAoe()
		{
			AoeController bullet = GetNode<AoeController>("AOE");
			bullet.QueueFree();
			AoeActive = false;
		}
		private void StopSpeedBoost()
		{
			// Speed is now heal. There's no need to stop.

			//_currentBuffSpeed = Vector2.One;
			//_currentMaxSpeed = _baseMaxSpeed;
			SpeedActive = false;
		}
		private void StopShield()
		{
			_currentShieldBonus = 1f;
			_shieldSphere.Hide();
			ShieldActive = false;
		}
		private void StopShrink()
		{
			Scale = Vector3.One;
			ShrinkActive = false;
		}




        /// <summary>
        /// This should be called only by the scene
        /// </summary>
        /// <param name="respawnPoint"></param>
        public void Respawn(RespawnPointController respawnPoint)
		{
			Position = respawnPoint.Position;

			// Reset damage
			Health = MaxHealth;
			_healthBar.Value = Health;

			// Notify others
			EmitSignal(SignalName.SubmarineHealed);
			EmitSignal(SignalName.SubmarineRespawned);
		}
		
		private void HitCave() {
			DamageReceived(BaseCrashDamage);
		}
		public void HitByCreature(FishController fish)
		{
			DamageReceived(fish.Damage);
		}
		private void DamageReceived(int damage)
		{
			_animationPlayer.Play("Damaged");

			damage = Mathf.FloorToInt(damage / _currentShieldBonus);

			if(damage >= Health) {
				Health = 0;
				_healthBar.Value = Health;
				EmitSignal(SignalName.SubmarineDestroyed);
			}
			else {
				Health -= damage;
				_healthBar.Value = Health;
				EmitSignal(SignalName.SubmarineDamaged);
			}
		}

        public override void _PhysicsProcess(double delta)
        {
			/*if(_bounceTimeLeft > 0f) {
					Vector3 velocity31 = new Vector3(_velocity.X, _velocity.Y, 0);
					//Vector3 velocity31 = new Vector3(1, 1, 0);
					MoveAndCollide(velocity31 * (float) delta);
					_bounceTimeLeft -= (float) delta;
				return;
			}*/

			Vector2 inputDirection = Input.GetVector("Right", "Left", "Down", "Up");
			float gravityComponent = 0;

			if(inputDirection.IsZeroApprox()) {
				// Apply gravity and drag
				//gravityComponent = _gravity * Mathf.Sign(_velocity.Y);
				//_velocity.Y -= gravityComponent;
				//if(_velocity.Y > 0) {
			//		_velocity.Y -= _drag * Mathf.Sign(_velocity.Y);
		//		}

				_velocity.Y -= _drag * Mathf.Sign(_velocity.Y);
				if(Mathf.Abs(_velocity.Y) < _drag) {
					_velocity.Y = 0;
				}

				_velocity.X -= _drag * Mathf.Sign(_velocity.X);
				if(Mathf.Abs(_velocity.X) < _drag) {
					_velocity.X = 0;
				}
			}
			else
			{
				// If switching direction, apply drag
				if(Mathf.Sign(inputDirection.Y) != Mathf.Sign(_velocity.Y))
				{
					_velocity.Y -= _drag * Mathf.Sign(_velocity.Y);
				}

				if(Mathf.Sign(inputDirection.X) != Mathf.Sign(_velocity.X))
				{
					_velocity.X -= _drag * Mathf.Sign(_velocity.X);
				}

				// Apply speed
				_velocity += inputDirection * _acceleration * _currentBuffSpeed;
			}

			// clamp the velocity
			_velocity.Clamp(-_currentMaxSpeed, _currentMaxSpeed);

			Vector3 velocity3 = new Vector3(_velocity.X, _velocity.Y, 0);

			//MoveAndSlide();
			KinematicCollision3D collision = MoveAndCollide(velocity3 * (float) delta);

			if(collision != null )
			{
				GodotObject colliderObject = collision.GetCollider();

				if(colliderObject is Node3D)
				{
					Node3D collider = colliderObject as Node3D;
					GD.Print("Collided with " + collider.Name);

					Vector3 normal = collision.GetNormal();
					//_velocity.X *= -normal.X;
					//_velocity.Y *= -normal.Y;

					normal.Z = 0;
					normal = normal.Normalized();

					velocity3 = velocity3.Bounce(normal);
					_velocity.X = velocity3.X;
					_velocity.Y = velocity3.Y;
					_velocity *= _bounceForce;
					//velocity3 = new Vector3(_velocity.X, _velocity.Y, 0);
					//MoveAndCollide(velocity3);
					//_bounceTimeLeft = _bounceTime;

					//Position += new Vector3(10f,  10f, 0);

					//GetNode<CollisionShape3D>("CollisionShape3D").Disabled = true;
					//GetNode<CollisionShape3D>("CollisionShape3D").Disabled = false;

					if(collider is FishController)
					{
						//HitByCreature(collider as FishController);
					}
					else
					{
						HitCave();
					}
				}
			}
			else {//} if(!inputDirection.IsZeroApprox()) {

				// Update rotation
				//Rotate(new Vector3(0,0,1), )
				Vector3 rot = Rotation;
				if(rot.Y < -0.1f)
				{
					rot.Y += 2*Mathf.Pi;
				}

				if(_velocity.X > 0 && rot.Y != 0) {
					_targetYRotation = 0;
				}
				else if(_velocity.X < 0 && rot.Y == 0) {
					_targetYRotation = Mathf.Pi;
				}

				if(_targetYRotation != rot.Y) {
					rot.Y = Mathf.MoveToward(rot.Y, _targetYRotation, (float) delta * _rotationSpeed.Y);
				}

				Vector2 velocityWithoutGravity = _velocity;
				velocityWithoutGravity.Y -= gravityComponent;

				if(velocityWithoutGravity.Length() > 0.5f)  {
					if(velocityWithoutGravity.X < 0) {
						//rot.Z = Mathf.Clamp(-(-velocityWithoutGravity).Angle(), -Mathf.Pi/3f, Mathf.Pi/3f);
						rot.Z = -(-velocityWithoutGravity).Angle();
					}
					else {

						//rot.Z = Mathf.Clamp(velocityWithoutGravity.Angle(), -Mathf.Pi/3f, Mathf.Pi/3f);
						rot.Z = velocityWithoutGravity.Angle();
					}
					rot.Z = Mathf.MoveToward(Rotation.Z, rot.Z, (float) delta * _rotationSpeed.X);
				}
				Rotation = rot;
				//Rotation = Rotation.MoveToward(rot, (float) delta);
			}

			//DetectCollisions();
			//UpdateCamera();
        }

		
		


		/*private void DetectCollisions()
		{
			int count = GetSlideCollisionCount();
			for(int i = 0; i < count; i++)
			{
				KinematicCollision3D collision = GetSlideCollision(i);
				GodotObject colliderObject = collision.GetCollider();

				if(colliderObject is Node3D)
				{
					Node3D collider = colliderObject as Node3D;
					GD.Print("Collided with " + collider.Name);

					Vector3 normal = collision.GetNormal();
					//_velocity.X *= -normal.X;
					//_velocity.Y *= -normal.Y;

					Velocity = Velocity.Bounce(normal);
					_velocity.X = Velocity.X;
					_velocity.Y = Velocity.Y;
					
				}
			}
		}

		private void UpdateCamera() {
			//_camera
		}*/
    }
}