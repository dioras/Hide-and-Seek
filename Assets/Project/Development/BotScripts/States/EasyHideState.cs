using UnityEngine;

namespace Project.Development.BotScripts.States
{
	public class EasyHideState: BotState
	{
		public Transform HidePlace { get; set; }
	
	
	
		public override BotState Update()
		{
			if (this.HidePlace != null)
			{
				Move(this.HidePlace.position);
			}

			return null;
		}
	}
}