using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Project.Development.BotScripts;
using Project.Development.BotScripts.States;
using Project.Development.CameraScripts;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using UnityEngine;

namespace Project.Development.ModeScripts
{
	public class HideAndSeekGameMode: GameMode
	{
		[field:SerializeField]
		public float TimeForHide { get; set; }
		[field:SerializeField]
		public int CaughtHidersCountForWin { get; set; }
	
		public Seeker Seeker { get; private set; }

		public List<Hider> Hiders { get; private set; }
		
		public float CurrTimeForHide { get; private set; }

		[SerializeField] 
		private AnimatorOverrideController seekerAC;
		[SerializeField] 
		private AnimatorOverrideController hiderAC;




		protected override void Awake()
		{
			base.Awake();
			
			Name = ModeName.HideAndSeek;
			
			Hiders = new List<Hider>();

			CurrTimeForHide = TimeForHide;
			
		}
		
		protected override void Update()
		{
			base.Update();

			if (Pause || !IsStarted || IsFinished)
			{
				return;
			}

			if (Seeker == null)
			{
				return;
			}
			
			if (Seeker.Hiders.Count == Hiders.Count && Seeker.CompareTag("Player"))
			{
				FinishMatch();
			}
		}



		public override void StartMatch()
		{
			EventManager.OnStartMatch?.Invoke(this);
			
			StartCoroutine(MatchProcess());
		}

		public override void FinishMatch()
		{
			if (IsFinished)
			{
				return;
			}
		
			base.FinishMatch();
			
			Seeker.SetActiveLocators(false);

			Result result;

			if (Seeker.CompareTag("Player"))
			{
				result = Hiders.Count(h => h.Caught) >= CaughtHidersCountForWin ? Result.Win : Result.Lose;
			}
			else
			{
				var player = Hiders.Single(h => h.CompareTag("Player"));
			
				result = player.Caught == false ? Result.Win : Result.Lose;
			}
			
			EventManager.OnMatchFinished?.Invoke(this, result);
		}

		public override void Join(Character character, Dictionary<string, object> settings)
		{
			base.Join(character, settings);

			switch (settings["role"])
			{
				case "hider":
					var hider = AddHider(character.transform);

					if (Seeker != null)
					{
						Seeker.AddObjectToLocators(hider);
					}

					break;
				case "seeker":
					AddSeeker(character.transform);
					
					break;
			}
		}
		
		public Role PlayerRole()
		{
			return Seeker.CompareTag("Player") ? Role.Seeker : Role.Hider;
		}



		private Hider AddHider(Transform tr)
		{
			var hider = tr.gameObject.AddComponent<Hider>();

			var animationController = hider.GetComponent<CharacterAnimationController>();
			
			animationController.SetAnimatorController(this.hiderAC);
			
			Hiders.Add(hider);

			return hider;
		}

		private Seeker AddSeeker(Transform tr)
		{
			if (Seeker != null)
			{
				var seekerComp = Seeker.GetComponent<Seeker>();

				if (seekerComp != null)
				{
					Destroy(seekerComp);
				}
			}
		
			Seeker = tr.gameObject.AddComponent<Seeker>();
			
			Seeker.SetObjectsToLocators(Hiders);
			
			var animationController = Seeker.GetComponent<CharacterAnimationController>();
			
			animationController.SetAnimatorController(this.seekerAC);
			animationController.SetRunAnimSpeedMultiplier(1.5f);

			return Seeker;
		}
		

		private void SetHidersInvisibility(bool invisible)
		{
			foreach (var hider in Hiders)
			{
				hider.SetInvisibility(invisible);
			}
		}

		private IEnumerator MatchProcess()
		{
			var cameraController = FindObjectOfType<CameraController>();

			if (PlayerRole() == Role.Hider)
			{
				var bot = Seeker.GetComponent<BotController>();

				var seekState = new SeekState
				{
					Player = Hiders.Single(h => h.CompareTag("Player"))
				};

				bot.MovingState = seekState;
				bot.MovingState.Enter(bot);
			}
			else
			{
				cameraController.LerpCamera(Quaternion.Euler(CameraRepository.Instance.Rotation),  new Vector3(0, 7.3f, -1.7f), 1f);
			}

			var hidePlaces = GameObject.FindGameObjectsWithTag("HidePlace");
			
			hidePlaces.Shuffle();
			
			foreach (var hider in Hiders)
			{
				var bot = hider.gameObject.GetComponent<BotController>();
				
				var movementController = hider.GetComponent<MovementController>();

				movementController.CanMove = true;

				if (bot == null)
				{
					continue;
				}
				
				bot.MovingState = new HardHideState();
				bot.MovingState.Enter(bot);

				var movingState = (HardHideState) bot.MovingState;
				
				movingState.Seeker = Seeker;
				movingState.PlayerHider = Hiders.FirstOrDefault(h => h.CompareTag("Player"));
			}

			while (CurrTimeForHide > 0f)
			{
				CurrTimeForHide -= Time.deltaTime;

				yield return null;
			}

			CurrTimeForHide = 0f;

			if (PlayerRole() == Role.Seeker)
			{
				cameraController.LerpCamera(Quaternion.Euler(CameraRepository.Instance.Rotation),  CameraRepository.Instance.Offset, 1f);
			
				SetHidersInvisibility(true);
			}
			
			Seeker.SetActiveLocators(true);
			Seeker.SetActiveFieldsOfView(true);

			Seeker.GetComponent<MovementController>().CanMove = true;
			
			EventManager.OnMatchStarted?.Invoke(this);
			
			IsStarted = true;
		}

		protected override void TimerLeft()
		{
			SetPause(true);

			if (PlayerRole() == Role.Hider)
			{
				FinishMatch();
			}
		}
	}
}