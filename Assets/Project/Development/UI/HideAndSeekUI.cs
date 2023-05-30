using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Development.Events;
using Project.Development.ModeScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class HideAndSeekUI: MonoBehaviour
	{
		[SerializeField] 
		private TextMeshProUGUI timeForHideText;
		[SerializeField] 
		private TextMeshPro timeForHidePrefab;
		[Header("Players Caught Panel")]
		[SerializeField] 
		private RectTransform playersCaughtPanel;
		[SerializeField] 
		private CaughtIndicatorUI caughtIndicatorPrefab;
		[SerializeField] 
		private Sprite activeCaughtIndicatorSpriteHideMode;
		[SerializeField] 
		private Sprite activeCaughtIndicatorSpriteSeekMode;
		[SerializeField] 
		private Sprite inactiveCaughtIndicatorSprite;
		[SerializeField] private TextMeshProUGUI timer;
		[SerializeField] private Image timeProgressImage;
		[SerializeField] private GameObject timerPanel;

		[SerializeField] private Color caughtIndicatorSeekModeAnimColor;
		[SerializeField] private Color caughtIndicatorHideModeAnimColor;

		[SerializeField] private GameObject onlyOneLeftPanel;
		
		private HideAndSeekGameMode hideAndSeek;
		private TextMeshPro timeForHide;

		private List<CaughtIndicatorUI> playersCaughtIndicators;
		
		
	
	
		private void Awake()
		{
			this.hideAndSeek = FindObjectOfType<HideAndSeekGameMode>();
		
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnStartMatch.AddListener(OnStartMatch);
			EventManager.OnMatchStarted.AddListener(OnMatchStarted);
			EventManager.OnHiderCaught.AddListener(OnHiderCaught);
			EventManager.OnExtraTimeStarted.AddListener(OnExtraTimeStarted);
			EventManager.OnHiderReleased.AddListener(OnHiderReleased);
			EventManager.OnHiderRescue.AddListener(OnHiderRescue);
			
			this.hideAndSeek.OnTimerLeft += OnTimerLeft;
			
			this.gameObject.SetActive(false);
		}

		private void Start()
		{
			InitPlayersCaughtPanel();
		}

		private void Update()
		{
			this.timeForHideText.SetText(Mathf.RoundToInt(this.hideAndSeek.CurrTimeForHide).ToString());
			
			if (this.timeForHide != null)
			{
				this.timeForHide.SetText(Mathf.RoundToInt(this.hideAndSeek.CurrTimeForHide).ToString());
			}

			var t = TimeSpan.FromSeconds(Mathf.RoundToInt(this.hideAndSeek.CurrentTime));

			this.timer.SetText(t.ToString("m':'ss''"));

			this.timeProgressImage.fillAmount = this.hideAndSeek.CurrentTime / this.hideAndSeek.MatchTime;
		}
		
		private void OnDestroy()
		{
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnStartMatch.RemoveListener(OnStartMatch);
			EventManager.OnMatchStarted.RemoveListener(OnMatchStarted);
			EventManager.OnHiderCaught.RemoveListener(OnHiderCaught);
			EventManager.OnExtraTimeStarted.RemoveListener(OnExtraTimeStarted);
			EventManager.OnHiderReleased.RemoveListener(OnHiderReleased);
			EventManager.OnHiderRescue.RemoveListener(OnHiderRescue);
			
			this.hideAndSeek.OnTimerLeft -= OnTimerLeft;
			
			if(this.timeForHide != null) Destroy(this.timeForHide.gameObject);
		}
		
		


		private void OnStartMatch(GameMode gameMode)
		{
			this.gameObject.SetActive(true);
		
			var seekerTr = this.hideAndSeek.Seeker.transform;
		
			this.timeForHide = Instantiate(this.timeForHidePrefab, 
				seekerTr.position + Vector3.up * 1.3f + Vector3.right * 0.65f, 
			this.timeForHidePrefab.transform.rotation);
		
			this.timeForHideText.gameObject.SetActive(true);
		}
		
		private void OnTimerLeft()
		{
			this.timerPanel.SetActive(false);
			this.playersCaughtPanel.gameObject.SetActive(false);
		}

		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			this.timerPanel.SetActive(false);
			this.playersCaughtPanel.gameObject.SetActive(false);
		}
		
		private void OnMatchStarted(GameMode gameMode)
		{
			this.timeForHideText.gameObject.SetActive(false);
			this.timeForHide.gameObject.SetActive(false);
			this.timerPanel.SetActive(true);
			this.playersCaughtPanel.gameObject.SetActive(true);
		}
		
		private void OnExtraTimeStarted(float time)
		{
			this.timerPanel.SetActive(true);
			this.playersCaughtPanel.gameObject.SetActive(true);
		}
		
		private void OnHiderCaught(Seeker seeker, Hider hider)
		{
			UpdatePlayerCaughtIndicators();

			if (hider.CompareTag("Player"))
			{
				this.timerPanel.SetActive(false);
				this.playersCaughtPanel.gameObject.SetActive(false);
			}

			if (seeker.CompareTag("Player"))
			{
				if (this.hideAndSeek.Hiders.Count - seeker.Hiders.Count == 1)
				{
					ShowOnlyOneLeftPanel();
				
					StartCoroutine(HideOnlyOneLeftPanelWithDelay());
				}
			}
		}
		
		private void OnHiderReleased(Hider hider)
		{
			UpdatePlayerCaughtIndicators();
			
			if (hider.CompareTag("Player"))
			{
				this.timerPanel.SetActive(true);
				this.playersCaughtPanel.gameObject.SetActive(true);
			}
		}
		
		private void OnHiderRescue(Hider rescuer, Hider rescued)
		{
			if (rescued.CompareTag("Player"))
			{
				this.timerPanel.SetActive(true);
				this.playersCaughtPanel.gameObject.SetActive(true);
			}
		}


		private void InitPlayersCaughtPanel()
		{
			this.playersCaughtIndicators = new List<CaughtIndicatorUI>();
		
			for (var i = 0; i < this.hideAndSeek.PlayersCount; i++)
			{
				var caughtIndicator = Instantiate(this.caughtIndicatorPrefab, this.playersCaughtPanel);

				SetInactiveIndicatorSprite(caughtIndicator);

				if (i > 3)
				{
					caughtIndicator.PlusImage.enabled = true;
				}
				
				this.playersCaughtIndicators.Add(caughtIndicator);
			}
		}

		private void UpdatePlayerCaughtIndicators()
		{
			var caughtCount = this.hideAndSeek.Hiders.Count(h => h.Caught);
		
			for(var i = 0; i < this.playersCaughtIndicators.Count; i++)
			{
				if (i < caughtCount)
				{
					SetActiveIndicatorSprite(this.playersCaughtIndicators[i]);
				}
				else
				{
					SetInactiveIndicatorSprite(this.playersCaughtIndicators[i]);
				}
			}
		}


		private void SetActiveIndicatorSprite(CaughtIndicatorUI indicator)
		{
			var activeIndicatorSprite = this.hideAndSeek.Seeker.CompareTag("Player") ? this.activeCaughtIndicatorSpriteSeekMode : this.activeCaughtIndicatorSpriteHideMode;

			indicator.BackgroundImage.sprite = activeIndicatorSprite;
			indicator.AnimImage.color = this.hideAndSeek.Seeker.CompareTag("Player")
				? this.caughtIndicatorSeekModeAnimColor
				: this.caughtIndicatorHideModeAnimColor;
			indicator.AnimImage.gameObject.SetActive(true);
		}
		
		private void SetInactiveIndicatorSprite(CaughtIndicatorUI indicator)
		{
			indicator.BackgroundImage.sprite = this.inactiveCaughtIndicatorSprite;
		}

		private void ShowOnlyOneLeftPanel()
		{
			this.onlyOneLeftPanel.SetActive(true);
		}

		private void HideOnlyOneLeftPanel()
		{
			this.onlyOneLeftPanel.SetActive(false);
		}

		private IEnumerator HideOnlyOneLeftPanelWithDelay()
		{
			yield return new WaitForSeconds(5f);
			
			HideOnlyOneLeftPanel();
		}
	}
}