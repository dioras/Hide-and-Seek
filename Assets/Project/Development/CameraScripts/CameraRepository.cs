using UnityEngine;

namespace Project.Development.CameraScripts
{
	public class CameraRepository: MonoBehaviour
	{
		public static CameraRepository Instance { get; private set; }


		[field: SerializeField]
		public Vector3 Offset { set; get; }
		
		[field: SerializeField]
		public Vector3 Rotation { set; get; }




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
		}

		private void InitDefault()
		{
		}
	}
}