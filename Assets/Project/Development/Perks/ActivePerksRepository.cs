using System;
using UnityEngine;

namespace Project.Development.Perks
{
	[Serializable]
	public abstract class ActivePerksParameters
	{
		[field:SerializeField]
		public float Duration { get; set; }
		[field:SerializeField]
		public float Cooldown { get; set; }
	}
	
	[Serializable]
	public class ThroughWallsActivePerkParameters: ActivePerksParameters
	{
		[field:SerializeField]
		public GameObject Effect { get; set; }
	}
	
	[Serializable]
	public class InvisibilityActivePerkParameters: ActivePerksParameters
	{
		
	}
	
	[Serializable]
	public class AccelerationActivePerkParameters: ActivePerksParameters
	{
		[field:SerializeField]
		public float AccelerationMultiplier { get; set; }
		[field:SerializeField]
		public GameObject Effect { get; set; }
	}


	public class ActivePerksRepository: MonoBehaviour
	{
		public static ActivePerksRepository Instance { get; private set; }

		[SerializeField] private ThroughWallsActivePerkParameters throughWallsActivePerkParameters;
		[SerializeField] private InvisibilityActivePerkParameters invisibilityActivePerkParameters;
		[SerializeField] private AccelerationActivePerkParameters accelerationActivePerkParameters;
		
		
		


		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(this);
			}
			else 
			{
				if (Instance != this)
				{
					Destroy(this.gameObject);
                    
					return;
				}
                
				return;
			}
		}


		public void SetPerkParameters(ActivePerk activePerk)
		{
			switch (activePerk)
			{
				case ThroughWallsActivePerk throughWallsActivePerk:
					throughWallsActivePerk.Duration = this.throughWallsActivePerkParameters.Duration;
					throughWallsActivePerk.Cooldown = this.throughWallsActivePerkParameters.Cooldown;
					throughWallsActivePerk.Effect = this.throughWallsActivePerkParameters.Effect;
					
					break;
				case InvisibilityActivePerk invisibilityActivePerk:
					invisibilityActivePerk.Duration = this.invisibilityActivePerkParameters.Duration;
					invisibilityActivePerk.Cooldown = this.invisibilityActivePerkParameters.Cooldown;
					
					break;
				case AccelerationActivePerk accelerationActivePerk:
					accelerationActivePerk.Duration = this.accelerationActivePerkParameters.Duration;
					accelerationActivePerk.Cooldown = this.accelerationActivePerkParameters.Cooldown;
					accelerationActivePerk.AccelerationMultiplier =
						this.accelerationActivePerkParameters.AccelerationMultiplier;
					accelerationActivePerk.Effect = this.accelerationActivePerkParameters.Effect;
					
					break;
			}
		}
	}
}