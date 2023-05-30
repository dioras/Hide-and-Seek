using System.Linq;
using Project.Development.ModeScripts;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Development.BotScripts.States
{
	public class SeekState: BotState
	{
		public Hider Player;
		private Vector3 targetPos;
		private bool hide;
		private float TimeToAvoidPlayer;
		private float currentTimeToAvoidPlayer;



		public override void Enter(BotController botController)
		{
			base.Enter(botController);

			TimeToAvoidPlayer = 0;

			if (Spawner.Instance.PlayerProfile.NeedEasyBots || LevelRepository.Instance.CurrentLevelNumber <= 5)
			{
				TimeToAvoidPlayer = BotDifficultyRepository.Instance.SeekerTimeToAvoidParams.Single(p =>
						LevelRepository.Instance.CurrentLevelNumber >= p.FromLvl &&
						(p.ToLvl == -1 || LevelRepository.Instance.CurrentLevelNumber <= p.ToLvl))
					.TimeToAvoidPlayer;
			}
		}

		public override BotState Update()
		{
			var dist = (Player.transform.position - this.botController.transform.position).magnitude;

			if (this.currentTimeToAvoidPlayer < TimeToAvoidPlayer && dist < 4.5f)
			{
				if (!this.hide || (this.hide &&
				                   (this.targetPos == Vector3.zero ||
				                    this.navMeshAgent.remainingDistance < 1f)))
				{
					var angle = Vector3.SignedAngle(
						(Player.transform.position - this.botController.transform.position).normalized,
						Player.transform.forward, Vector3.up);

					var pos = Player.transform.position + Quaternion.Euler(0, Mathf.Sign(angle) *
					                                                          Random.Range(30, 90), 0) *
					          (Player.transform.forward * 6);

					NavMesh.SamplePosition(pos, out var hit, 8f, NavMesh.AllAreas);

					this.targetPos = hit.position;

					this.hide = true;
				}
			}
			else
			{
				if (this.targetPos == Vector3.zero || this.navMeshAgent.remainingDistance < 1f)
				{
					this.hide = false;
				
					this.targetPos = GetRandomPos(10);
				}
				
				this.currentTimeToAvoidPlayer += Time.deltaTime;
			}

			Move(this.targetPos);

			return null;
		}



		private Vector3 GetRandomPos(float walkRadius)
		{
			Vector3 randomDir = Random.insideUnitCircle * walkRadius;

			randomDir.z = randomDir.y;
			randomDir.y = 0;

			randomDir += Spawner.CurrLocationPos;

			NavMesh.SamplePosition(randomDir, out var hit, walkRadius, NavMesh.AllAreas);

			return hit.position;
		}
	}
}