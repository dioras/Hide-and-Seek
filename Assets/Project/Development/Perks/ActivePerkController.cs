using System;
using UnityEngine;

namespace Project.Development.Perks
{
	public class ActivePerkController: MonoBehaviour
	{
		public ActivePerk CurrentActivePerk { get; private set; }




		public void Apply(ActivePerk activePerk)
		{
			CurrentActivePerk = activePerk;
		}
		
		public void ApplyPerk(Type activePerkType)
		{
			var activePerkComp = this.gameObject.GetComponent(activePerkType);

			if (activePerkComp == null)
			{
				activePerkComp = this.gameObject.AddComponent(activePerkType);
			}
			
			ActivePerksRepository.Instance.SetPerkParameters((ActivePerk)activePerkComp);
			
			Apply((ActivePerk)activePerkComp);
			CurrentActivePerk.Activate();
		}
	}
}