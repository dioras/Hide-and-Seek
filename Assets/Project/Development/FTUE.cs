using System;
using System.Collections;
using System.Linq;
using Project.Development.Events;
using Project.Development.InputController.Interface;
using Project.Development.ModeScripts;
using UnityEngine;

namespace Project.Development
{
	public class FTUE: MonoBehaviour
	{
		private const string FTUEKey = "FTUE";
	
		public bool NeedFTUE
		{
			get => Convert.ToBoolean(PlayerPrefs.GetInt(FTUEKey, 1));
			private set
			{
				PlayerPrefs.SetInt(FTUEKey, Convert.ToInt32(value));
				PlayerPrefs.Save();
			}
		}

		[SerializeField] private GameObject finger;

		private HideAndSeekGameMode gameMode;

		private IInputController playerInputController;




		private void Start()
		{
			EventManager.OnMatchStarted.AddListener(OnMatchStarted);
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnChangeLevel.AddListener(OnChangeLevel);
		}

		private void OnDestroy()
		{
			EventManager.OnMatchStarted.RemoveListener(OnMatchStarted);
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnChangeLevel.RemoveListener(OnChangeLevel);
		}

		


		private IEnumerator FTUEProcess()
		{
			this.finger.SetActive(true);
		
			while (this.playerInputController.GetMotion() == Vector3.zero)
			{
				yield return null;
			}
			
			this.finger.SetActive(false);
		}
		
		private void OnMatchStarted(GameMode gameMode)
		{
			if (NeedFTUE)
			{
				this.gameMode = gameMode as HideAndSeekGameMode;

				if (this.gameMode != null)
				{
					this.playerInputController =
						this.gameMode.Players.Single(p => p.CompareTag("Player")).InputController;
					this.gameMode.OnTimerLeft += OnTimerLeft;

					StartCoroutine(FTUEProcess());
				}
			}
		}
		
		private void OnChangeLevel(Level level, int levelNum, bool bonus)
		{
			if (level != null)
			{
				NeedFTUE = false;
			}
		}

		private void OnTimerLeft()
		{
			if (NeedFTUE)
			{
				NeedFTUE = this.gameMode.Seeker.Hiders.Count < this.gameMode.CaughtHidersCountForWin;
			
				this.finger.SetActive(false);
			}
		}

		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			if (NeedFTUE)
			{
				if (result == Result.Win)
				{
					NeedFTUE = false;
				}
			
				this.finger.SetActive(false);
			}
		}
	}
}