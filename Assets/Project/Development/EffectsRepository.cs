using UnityEngine;

namespace Project.Development
{
	public class EffectsRepository: MonoBehaviour
	{
		private const string LowLevelsDustUnderfootCDKey = "lowlevels_dustunderfoot_cd";
		
		private const string HighLevelsDustUnderfootMinCDKey = "highlevels_dustunderfoot_min_cd";
		private const string HighLevelsDustUnderfootMaxCDKey = "highlevels_dustunderfoot_max_cd";
	
		public static EffectsRepository Instance { get; private set; }


		public float LowLevelsDustUnderfootCD { set; get; }
		public float HighLevelsDustUnderfootMinCD { set; get; }
		public float HighLevelsDustUnderfootMaxCD { set; get; }

		[SerializeField] private float lowLevelsDustUnderfootCD;
		[SerializeField] private float highLevelsDustUnderfootMinCD;
		[SerializeField] private float highLevelsDustUnderfootMaxCD;



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
					Destroy(this);
                    
					return;
				}
                
				return;
			}
		
			InitDefault();
		
			RemoteSettings.Completed += RemoteSettingsOnCompleted;
		}

		private void OnDestroy()
		{
			RemoteSettings.Completed -= RemoteSettingsOnCompleted;
		}



		private void RemoteSettingsOnCompleted(bool arg1, bool arg2, int arg3)
		{
			this.LowLevelsDustUnderfootCD = RemoteSettings.GetFloat(LowLevelsDustUnderfootCDKey, this.lowLevelsDustUnderfootCD);
			this.HighLevelsDustUnderfootMinCD = RemoteSettings.GetFloat(HighLevelsDustUnderfootMinCDKey, this.highLevelsDustUnderfootMinCD);
			this.HighLevelsDustUnderfootMaxCD = RemoteSettings.GetFloat(HighLevelsDustUnderfootMaxCDKey, this.highLevelsDustUnderfootMaxCD);
		}

		private void InitDefault()
		{
			this.LowLevelsDustUnderfootCD = this.lowLevelsDustUnderfootCD;
			this.HighLevelsDustUnderfootMinCD = this.highLevelsDustUnderfootMinCD;
			this.HighLevelsDustUnderfootMaxCD = this.highLevelsDustUnderfootMaxCD;
		}
	}
}