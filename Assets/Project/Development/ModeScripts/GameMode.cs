using System;
using System.Collections.Generic;
using System.Linq;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using UnityEngine;

namespace Project.Development.ModeScripts
{
	public abstract class GameMode: MonoBehaviour
	{
		public Action OnTimerLeft { get; set; }
	
		public ModeName Name { get; set; }
		
		[field: SerializeField]
		public int PlayersCount { get; set; }
		[field: SerializeField]
		public float MatchTime { get; set; }
		
		public bool IsStarted { get; protected set; }
		public bool IsFinished { get; protected set; }

		public float CurrentTime { get; private set; }
		
		public float CurrentMatchDuration { get; private set; }
		
		public List<Character> Players { get; protected set; }
		
		private bool timerLeft;

		public bool Pause { get; protected set; }
		public bool PauseTimer { get; set; }

		[SerializeField] private Canvas canvasPrefab;

		private Canvas canvas;




		protected virtual void Awake()
		{
			this.canvas = Instantiate(this.canvasPrefab);
		
			EventManager.OnGameModeInit?.Invoke(this);
			EventManager.OnChangeLevel?.AddListener(OnChangeLevel);
		
			this.CurrentTime = this.MatchTime;
			
			this.Players = new List<Character>();
			
			OnTimerLeft += TimerLeft;
			
			SetPause(false);
		}

		
		protected virtual void Start()
		{
			
		}

		protected virtual void Update()
		{
			if (Pause)
			{
				return;
			}

			Timer();
			
			if (!Pause && IsStarted && !IsFinished)
			{
				CurrentMatchDuration += Time.deltaTime;
			}
		}

		protected virtual void OnDestroy()
		{
			OnTimerLeft -= TimerLeft;

			if (this.canvas != null)
			{
				Destroy(this.canvas.gameObject);
			}
			
			EventManager.OnChangeLevel?.RemoveListener(OnChangeLevel);
		}
		

		public virtual void StartMatch()
		{
			
		}

		public virtual void FinishMatch()
		{
			if (this.IsFinished)
			{
				return;
			}
		
			this.IsFinished = true;
			
			SetPlayersMoveState(false);
		}

		public virtual void Join(Character character, Dictionary<string, object> settings)
		{
			this.Players.Add(character);
		}

		public virtual void SetPause(bool pause, bool playerOnly = false)
		{
			Pause = pause;

			if (playerOnly)
			{
				var player = this.Players.SingleOrDefault(p => p.CompareTag("Player"));

				if (player != null)
				{
					player.MovementController.CanMove = !pause;
				}
			}
			else
			{
				SetPlayersMoveState(!pause);
			}
		}

		public void ExtraTime(float time)
		{
			this.timerLeft = false;
			this.CurrentTime = time;
			
			EventManager.OnExtraTimeStarted?.Invoke(time);
		}


		private void Timer()
		{
			if (this.PauseTimer || this.timerLeft || !this.IsStarted || this.IsFinished)
			{
				return;
			}
		
			this.CurrentTime -= Time.deltaTime;

			if (this.CurrentTime <= 0f)
			{
				this.timerLeft = true;

				OnTimerLeft?.Invoke();
			}
		}
		
		protected virtual void TimerLeft()
		{
			FinishMatch();
		}

		protected void SetPlayersMoveState(bool state)
		{
			foreach (var player in Players)
			{
				player.MovementController.CanMove = state;
			}
		}
		
		private void OnChangeLevel(Level level, int levelNum, bool bonus)
		{
			SetPlayersMoveState(false);
		}

	}
}