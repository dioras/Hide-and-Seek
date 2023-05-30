using System;
using Project.Development.Events;
using UnityEngine;

namespace Project.Development.Perks
{
	public class ActivePerkPickup: MonoBehaviour
	{
		public ActivePerkName ActivePerkName => this.activePerkName;
	
		[SerializeField] private ActivePerkName activePerkName;
		
		
		
	
		private void OnTriggerEnter(Collider other)
		{
			var activePerkController = other.GetComponent<ActivePerkController>();

			if (activePerkController != null && activePerkController.CompareTag("Player"))
			{
				activePerkController.ApplyPerk(ActivePerk.GetTypeByName(activePerkName));
				
				EventManager.OnActivePerkPickedUp?.Invoke(ActivePerkName, activePerkController);
				
				this.gameObject.SetActive(false);
			}
		}
	}
}