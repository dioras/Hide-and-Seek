using UnityEngine;

namespace Project.Development.BotScripts.States
{
	public class MovingState: BotState
	{
		public Vector3 TargetPos { get; set; }
	
	
	
	
		public override BotState Update()
		{
			Move(this.TargetPos);

			return null;
		}
	}
}