using System;
using Project.Development.Events;
using Project.Development.ModeScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class BonusLevelUI: MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI timer;
		[SerializeField] private Image timeProgressImage;
		[SerializeField] private GameObject timerPanel;
		
		
		private BonusLevelGameMode bonusLevelGameMode;
		
		
	
	
		private void Awake()
		{
			this.bonusLevelGameMode = FindObjectOfType<BonusLevelGameMode>();
		
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnStartMatch.AddListener(OnStartMatch);
			EventManager.OnMatchStarted.AddListener(OnMatchStarted);
			
			this.bonusLevelGameMode.OnTimerLeft += OnTimerLeft;
			
			this.gameObject.SetActive(false);
		}

		private void Update()
		{
			var t = TimeSpan.FromSeconds(Mathf.RoundToInt(this.bonusLevelGameMode.CurrentTime));

			this.timer.text = t.ToString("m':'ss''");

			this.timeProgressImage.fillAmount = this.bonusLevelGameMode.CurrentTime / this.bonusLevelGameMode.MatchTime;
		}
		
		private void OnDestroy()
		{
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnStartMatch.RemoveListener(OnStartMatch);
			EventManager.OnMatchStarted.RemoveListener(OnMatchStarted);
			
			this.bonusLevelGameMode.OnTimerLeft -= OnTimerLeft;
		}
		
		


		private void OnStartMatch(GameMode gameMode)
		{
			this.gameObject.SetActive(true);
		}
		
		private void OnTimerLeft()
		{
			this.timerPanel.SetActive(false);
		}

		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			this.timerPanel.SetActive(false);
		}
		
		private void OnMatchStarted(GameMode gameMode)
		{
			this.timerPanel.SetActive(true);
		}
	}
}