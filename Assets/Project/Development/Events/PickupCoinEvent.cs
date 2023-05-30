using Project.Development.CharacterScripts;
using UnityEngine.Events;

namespace Project.Development.Events
{
	public class PickupCoinEvent: UnityEvent<Coin, CoinCollector>
	{
		
	}
}