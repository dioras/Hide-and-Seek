using System.Collections;
using Project.Development.CharacterScripts;
using UnityEngine;

namespace Project.Development.Perks
{
	public class InvisibilityActivePerk: ActivePerk
	{
		private InvisibilityComponent invisibilityComponent;
		private Coroutine invisibilityProcess;
	
	
		protected override void Awake()
		{
			base.Awake();

			Name = ActivePerkName.Invisibility;
		}
		
		
		

		public override void Activate()
		{
			if (IsActive)
			{
				if (this.invisibilityProcess != null)
				{
					StopCoroutine(this.invisibilityProcess);
				}

				CurrentDuration = 0f;
			}
			
			IsActive = true;

			if (this.invisibilityComponent == null)
			{
				this.invisibilityComponent = this.activePerkController.GetComponent<InvisibilityComponent>();
			}

			if (this.invisibilityComponent != null)
			{
				this.invisibilityComponent.Alpha = 0.3f;
			}

			PerkActivation?.Invoke();

			this.invisibilityProcess = StartCoroutine(InvisibilityProcess());
		}

		public override void Stop()
		{
			IsActive = false;

			CurrentDuration = 0f;
			CurrentCooldown = 0f;

			if (this.invisibilityComponent == null)
			{
				this.invisibilityComponent = this.activePerkController.GetComponent<InvisibilityComponent>();
			}

			if (this.invisibilityComponent != null)
			{
				this.invisibilityComponent.Alpha = 1f;
			}
		
			PerkDeactivated?.Invoke();
		}


		private IEnumerator InvisibilityProcess()
		{
			while (CurrentDuration <= Duration - 2)
			{
				CurrentDuration += Time.deltaTime;
			
				yield return null;
			}

			var sign = 1;
			
			while (CurrentDuration <= Duration)
			{
				if (this.invisibilityComponent.Alpha >= 0.95f)
				{
					sign = -1;
				}
				
				if (this.invisibilityComponent.Alpha <= 0.3f)
				{
					sign = 1;
				}
				
				this.invisibilityComponent.Alpha += sign * (0.65f * 4f) * Time.deltaTime;
			
				CurrentDuration += Time.deltaTime;
			
				yield return null;
			}
			
			Stop();

			this.invisibilityProcess = null;
		}
	}
}