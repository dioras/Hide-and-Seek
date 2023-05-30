using System;
using UnityEngine;

namespace Project.Development.Perks
{
	[RequireComponent(typeof(ActivePerkController))]
	public abstract class ActivePerk: MonoBehaviour
	{
		public Action PerkActivation;
		public Action PerkDeactivated;
		
		public ActivePerkName Name { get; protected set; }
		
		public bool IsAvailable => CurrentCooldown <= 0f;
		public bool IsActive { get; protected set; }
		public float Duration { get; set; }
		public float CurrentDuration { get; protected set; }
		
		public float Cooldown { get; set; }
		public float CurrentCooldown { get; protected set; }
		

		protected ActivePerkController activePerkController;




		protected virtual void Awake()
		{
			this.activePerkController = GetComponent<ActivePerkController>();
		}
		
		


		public abstract void Activate();
		public abstract void Stop();



		public static Type GetTypeByName(ActivePerkName activePerkName)
		{
			Type activePerkType = null;

			switch (activePerkName)
			{
				case ActivePerkName.ThroughWalls:
					activePerkType = typeof(ThroughWallsActivePerk);
					break;
				case ActivePerkName.Invisibility:
					activePerkType = typeof(InvisibilityActivePerk);
					break;
				case ActivePerkName.Acceleration:
					activePerkType = typeof(AccelerationActivePerk);
					break;
			}

			return activePerkType;
		}
	}
}