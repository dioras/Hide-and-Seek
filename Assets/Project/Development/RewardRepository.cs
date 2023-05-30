using UnityEngine;

namespace Project.Development
{
	public class RewardRepository: MonoBehaviour
	{
		public static RewardRepository Instance { get; private set; }


		[field: SerializeField] public int WinReward { get; set; }
		[field: SerializeField] public int RescueReward { get; set; }
		[field: SerializeField] public int CatchReward { get; set; }


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
	}
}