using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Development.Perks
{
	public class ThroughWallsActivePerk: ActivePerk
	{
		public GameObject Effect { get; set; }
	
		private List<Collider> wallsCollider;
		private ThroughWalls throughWalls;

		private GameObject effect;



		protected override void Awake()
		{
			base.Awake();

			Name = ActivePerkName.ThroughWalls;
		}
		
		
		

		public override void Activate()
		{
			IsActive = true;

			this.throughWalls = this.gameObject.GetComponent<ThroughWalls>();

			if (this.throughWalls == null)
			{
				this.throughWalls = this.gameObject.AddComponent<ThroughWalls>();
			}
			else
			{
				this.throughWalls.enabled = true;
			}

			this.effect = Instantiate(this.Effect, this.transform);
			
			this.effect.transform.localPosition = new Vector3(0, 0.95f, 0);

			PerkActivation?.Invoke();

			StartCoroutine(ThroughWallProcess());
		}

		public override void Stop()
		{
			IsActive = false;

			this.throughWalls.enabled = false;
			
			Destroy(this.effect);

			CurrentDuration = 0f;
			CurrentCooldown = 0f;
		
			PerkDeactivated?.Invoke();
		}


		private IEnumerator ThroughWallProcess()
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