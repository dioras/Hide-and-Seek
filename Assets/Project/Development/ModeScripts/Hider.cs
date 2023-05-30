using Project.Development.CharacterScripts;
using Project.Development.Events;
using UnityEngine;

namespace Project.Development.ModeScripts
{
	public class Hider: MonoBehaviour
	{
		public bool Caught { get; set; }
		
		public int RescuedCount { get; set; }

		public InvisibilityComponent InvisibilityComponent { get; private set; }
		
		private GameObject playerRescueEffectPrefab;
		private GameObject notPlayerRescueEffectPrefab;



		private void Awake()
		{
			this.InvisibilityComponent = GetComponent<InvisibilityComponent>();
			
			this.playerRescueEffectPrefab = Resources.Load<GameObject>("VFX/RingWithCoins");
			this.notPlayerRescueEffectPrefab = Resources.Load<GameObject>("VFX/DustDirtyPoof");
			
			EventManager.OnHiderRescue.AddListener(OnHiderRescue);
		}

		private void OnDestroy()
		{
			EventManager.OnHiderRescue.RemoveListener(OnHiderRescue);
		}


		public void SetInvisibility(bool invisible)
		{
			this.InvisibilityComponent.SetInvisibility(invisible);
		}
		
		
		
		private void OnHiderRescue(Hider rescuer, Hider rescued)
		{
			if (rescuer == this)
			{
				RescueEffect(rescued.transform.position + Vector3.up * 0.04f);
			}
		}
		
		private void RescueEffect(Vector3 pos)
		{
			var prefab = CompareTag("Player") ? this.playerRescueEffectPrefab : this.notPlayerRescueEffectPrefab;
		
			var effect = Instantiate(prefab, pos, prefab.transform.rotation);
			
			Destroy(effect, 3.0f);
		}
	}
}