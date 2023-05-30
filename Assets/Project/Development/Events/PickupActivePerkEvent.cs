using Project.Development.Perks;
using UnityEngine.Events;

namespace Project.Development.Events
{
	public class PickupActivePerkEvent: UnityEvent<ActivePerkName, ActivePerkController>
	{
		
	}
}