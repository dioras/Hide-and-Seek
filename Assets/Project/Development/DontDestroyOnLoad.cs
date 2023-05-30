using UnityEngine;

namespace Project.Development
{
	public class DontDestroyOnLoad: MonoBehaviour
	{
		private static DontDestroyOnLoad Instance { get; set; }
		
		
	
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else 
			{
				if (Instance != this)
				{
					Destroy(this.gameObject);
				}
			}
		}
	}
}