using UnityEngine;

namespace Project.Development.CharacterScripts
{
	public class CharacterRepository: MonoBehaviour
	{
		private const string MoveSpeedKey = "move_speed";
		private const string TurnSpeedKey = "turn_speed";
	
		public static CharacterRepository Instance { get; private set; }


		public float MoveSpeed { set; get; }
		
		public float TurnSpeed { set; get; }

		[SerializeField] private float moveSpeedDefault;
		[SerializeField] private float turnSpeedDefault;




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
			this.MoveSpeed = RemoteSettings.GetFloat(MoveSpeedKey, this.moveSpeedDefault);
			this.TurnSpeed = RemoteSettings.GetFloat(TurnSpeedKey, this.turnSpeedDefault);
		}

		private void InitDefault()
		{
			this.MoveSpeed = this.moveSpeedDefault;
			this.TurnSpeed = this.turnSpeedDefault;
		}
	}
}