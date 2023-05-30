using Project.Development.CharacterScripts;
using Project.Development.Events;
using UnityEngine;

namespace Project.Development
{
	public class Coin: MonoBehaviour
	{
		[field:SerializeField]
		public int Amount { get; set; }
		[field:SerializeField]
		public bool RandomStartRotation { get; set; }
		[field:SerializeField]
		public GameObject PickupEffect { get; set; }




		private void Awake()
		{
			if (this.RandomStartRotation)
			{
				this.transform.rotation = Quaternion.Euler(Random.Range(0, 360), 0, 0);
			}
		}



		private void PlayPickupEffect()
		{
			var effect = Instantiate(this.PickupEffect, this.transform.position, this.PickupEffect.transform.rotation);
			
			Destroy(effect, 3.0f);
		}
		
		
		

		private void OnTriggerEnter(Collider other)
		{
			var coinCollector = other.transform.root.GetComponentInChildren<CoinCollector>();

			if (coinCollector != null)
			{
				coinCollector.Coins += this.Amount;
				
				EventManager.OnCoinPickuped?.Invoke(this, coinCollector);
				
				PlayPickupEffect();
				
				Destroy(this.gameObject);
			}
		}
	}
}