using Project.Development.InputController;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Development.BotScripts.States
{
	public abstract class BotState
	{
		protected BotController botController;
		protected NavMeshAgent navMeshAgent;
		protected MockInputController inputController;

		public Vector3 TargetPos { get; protected set; }
		
		
		
		public virtual void Enter(BotController botController)
		{
			this.inputController = (MockInputController)botController.Character.InputController;
			this.navMeshAgent = botController.NavMeshAgent;
			this.botController = botController;
		}
		
		
		
		public abstract BotState Update();



		public void Move(Vector3 pos)
		{
			if (this.navMeshAgent == null)
			{
				return;
			}
			
			if (this.navMeshAgent.pathPending)
			{
				return;
			}
			
			if (pos == this.TargetPos && this.navMeshAgent.remainingDistance <= this.navMeshAgent.stoppingDistance)
			{
				this.inputController.RotationResultVector = Vector3.zero;
				this.inputController.MovingResultVector = Vector3.zero;
			
				return;
			}

			if (pos != this.TargetPos)
			{
				this.navMeshAgent.SetDestination(pos);
			}

			this.TargetPos = pos;

			if (this.navMeshAgent.path.corners.Length <= 1)
			{
				return;
			}

			var botPos = this.botController.transform.position;

			botPos.y = 0f;

			var motion = (this.navMeshAgent.path.corners[1] - botPos).normalized;

			motion.y = 0f;

			this.inputController.RotationResultVector = motion;
			this.inputController.MovingResultVector = motion;
		}
	}
}