using System.Collections;
using System.Linq;
using Project.Development.ModeScripts;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Project.Development.BotScripts.States
{
	public class HardHideState: BotState
	{
		public Seeker Seeker { get; set; }

		public Hider PlayerHider { get; set; }
	
		private Vector3 targetPos;

		private float timer;

		private bool hide;

		private float afkTimer;
		private float currentTimeForAfk;

		private float TimeForAfk;
		private float AfkTime;
		private float TimeToRunToSeeker;
		private float currentTimeToRunToSeeker;

		private Coroutine randomMoveCoroutine;



		public override void Enter(BotController botController)
		{
			base.Enter(botController);

			this.TimeForAfk = Random.Range(2.5f, 3.5f);
			this.AfkTime = Random.Range(2.5f, 4f);

			TimeToRunToSeeker = 35;

			if (Spawner.Instance.PlayerProfile.NeedEasyBots || LevelRepository.Instance.CurrentLevelNumber <= 5)
			{
				TimeToRunToSeeker = 3 + BotDifficultyRepository.Instance.HiderTimeToRunParams.Single(p =>
						                    LevelRepository.Instance.CurrentLevelNumber >= p.FromLvl &&
						                    (p.ToLvl == -1 || LevelRepository.Instance.CurrentLevelNumber <= p.ToLvl))
					                    .TimeToRunToSeeker + Random.Range(0,
					                    BotDifficultyRepository.Instance.TimeToRunToSeekerAddRandomTime);
			}
		}

		public override BotState Update()
		{
			if (this.Seeker == null)
			{
				return null;
			}

			if (this.Seeker.InvisibilityComponent.Alpha < 1f)
			{
				if (this.targetPos == Vector3.zero || this.navMeshAgent.remainingDistance < 1f)
				{
					this.targetPos = GetRandomPos(10, Spawner.CurrLocationPos);
				}
			}
			else
			{
				if (Seeker.CompareTag("Player") && this.currentTimeToRunToSeeker >= this.TimeToRunToSeeker)
				{
					this.targetPos = this.Seeker.transform.position;
				}
				else
				{
					var dist = (this.Seeker.transform.position - this.botController.transform.position).magnitude;
				
					var angle = Vector3.SignedAngle(
						(this.Seeker.transform.position - this.botController.transform.position).normalized,
						this.Seeker.transform.forward, Vector3.up);

					this.currentTimeForAfk += Time.deltaTime;

					if (this.timer <= 0 && dist < 4f)
					{
						if (!this.hide || (this.hide &&
						                   (this.targetPos == Vector3.zero ||
						                    this.navMeshAgent.remainingDistance < 1f)))
						{
							var pos = this.Seeker.transform.position + Quaternion.Euler(0, Mathf.Sign(angle) *
							                                                               Random.Range(30, 90), 0) *
							          (this.Seeker.transform.forward * Random.Range(6f, 9f));

							NavMesh.SamplePosition(pos, out var hit, 8f, NavMesh.AllAreas);

							this.targetPos = hit.position;

							this.timer = 0.3f;

							this.hide = true;
						}
					}
					else
					{
						this.hide = false;

						if (this.currentTimeForAfk < this.TimeForAfk)
						{
							if (this.PlayerHider != null && this.PlayerHider.Caught &&
							    Vector3.Distance(this.PlayerHider.transform.position,
								    this.botController.transform.position) <= 3f)
							{
								this.targetPos = this.PlayerHider.transform.position;
							}
							else
							{
								if (this.targetPos == Vector3.zero || this.navMeshAgent.remainingDistance < 1f)
								{
									this.targetPos = GetRandomPos(Random.Range(4f, 10f), Spawner.CurrLocationPos);
								}
							}
						}
						else
						{
							this.afkTimer += Time.deltaTime;

							if (this.afkTimer > this.AfkTime)
							{
								this.afkTimer = 0f;

								this.currentTimeForAfk = 0f;
							}
						}
					}

					this.timer -= Time.deltaTime;
					this.currentTimeToRunToSeeker += Time.deltaTime;
				}
			}

			Move(this.targetPos);

			return null;
		}
		
		private Vector3 GetRandomPos(float walkRadius, Vector3 origin)
		{
			Vector3 randomDir = Random.insideUnitCircle * walkRadius;

			randomDir.z = randomDir.y;
			randomDir.y = 0;

			randomDir += origin;

			NavMesh.SamplePosition(randomDir, out var hit, walkRadius, NavMesh.AllAreas);

			return hit.position;
		}

		private IEnumerator RandomMove()
		{
			var t = 0f;

			var randomTime = Random.Range(1.5f, 2.5f);
		
			while (true)
			{
				t += Time.deltaTime;
			
				if (t > randomTime)
				{
					t = 0f;
					
					randomTime = Random.Range(1.5f, 2.5f);

					this.targetPos = GetRandomPos(Random.Range(2f, 3f), this.botController.transform.position);
				}

				yield return null;
			}
		}
	}
}