using System.Collections;
using Project.Development.CharacterScripts;
using UnityEngine;

namespace Project.Development.Perks
{
	public class AccelerationActivePerk: ActivePerk
	{
		public float AccelerationMultiplier { get; set; }
		public GameObject Effect { get; set; }

		private float deltaSpeed;
		private Character character;

		private GameObject rightLegEffect;
		private GameObject leftLegEffect;
		
	
	
		protected override void Awake()
		{
			base.Awake();

			Name = ActivePerkName.Acceleration;
			
			this.character = this.activePerkController.GetComponent<Character>();
		}
		
		
		

		public override void Activate()
		{
			IsActive = true;

			var wishSpeed = this.character.MovementController.MoveSpeed * AccelerationMultiplier;

			this.deltaSpeed = wishSpeed - this.character.MovementController.MoveSpeed;

			if (this.character.MovementController != null)
			{
				this.character.MovementController.MoveSpeed += this.deltaSpeed;
			}

			this.rightLegEffect = Instantiate(Effect, this.character.RightLeg);
			this.rightLegEffect.transform.localPosition = Vector3.zero;
			
			this.leftLegEffect = Instantiate(Effect, this.character.LeftLeg);
			this.leftLegEffect.transform.localPosition = Vector3.zero;
			
			PerkActivation?.Invoke();

			StartCoroutine(AccelerationProcess());
		}

		public override void Stop()
		{
			IsActive = false;

			CurrentDuration = 0f;
			CurrentCooldown = 0f;
			
			Destroy(this.rightLegEffect);
			Destroy(this.leftLegEffect);

			if (this.character.MovementController != null)
			{
				this.character.MovementController.MoveSpeed -= this.deltaSpeed;
			}
		
			PerkDeactivated?.Invoke();
		}


		private IEnumerator AccelerationProcess()
		{
			while (CurrentDuration <= Duration)
			{
				CurrentDuration += Time.deltaTime;
			
				yield return null;
			}
			
			Stop();
		}
	}
}