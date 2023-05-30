using Project.Development.BotScripts.States;
using Project.Development.CharacterScripts;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Development.BotScripts
{
	public class BotController: MonoBehaviour
	{
		public Character Character { get; set; }
		public NavMeshAgent NavMeshAgent { get; set; }
		
		public BotState MovingState { get; set; }




		private void Awake()
		{
			this.Character = GetComponent<Character>();
			
			AddNavMeshAgent();
		}

		private void Update()
		{
			var	movingState = this.MovingState?.Update();

			if (movingState != null)
			{
				this.MovingState = movingState;
				
				this.MovingState.Enter(this);
			}
		}

		private void OnDrawGizmos()
		{
			if (MovingState is SeekState seekState)
			{
				Gizmos.DrawLine(this.transform.position, seekState.TargetPos);
			}
		}


		private void AddNavMeshAgent()
		{
			this.NavMeshAgent = this.gameObject.AddComponent<NavMeshAgent>();
			
			this.NavMeshAgent.speed = 0f;
			this.NavMeshAgent.angularSpeed = 0f;
			this.NavMeshAgent.acceleration = 0f;
			this.NavMeshAgent.autoBraking = false;
			this.NavMeshAgent.baseOffset = 0f;
			this.NavMeshAgent.height = 1.2f;
			this.NavMeshAgent.radius = 0.05f;
			this.NavMeshAgent.stoppingDistance = 0.1f;
			this.NavMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		}
	}
}