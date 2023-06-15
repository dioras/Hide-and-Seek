using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using Project.Development.CameraScripts;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using Project.Development.ModeScripts;
using Project.Development.Perks;
using Project.Development.SkinScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class MainMenuUI: MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI currentLevelText;
		[SerializeField] private Button hideModeButton;
		[SerializeField] private Button seekModeButton;
		[SerializeField] private RectTransform playersResultPanel;
		[SerializeField] private PlayerResultUI playerResultUiPrefab;
		[SerializeField] private GameObject currentLevelPanel;
		[SerializeField] private GameObject seekLosePanel;
		[SerializeField] private GameObject hideLosePanel;
		[SerializeField] private GameObject seekNoExtraTimePanel;
		[SerializeField] private List<TextMeshProUGUI> matchCoinsTexts;
		[SerializeField] private GameObject seekVictoryPanel;
		[SerializeField] private GameObject hideVictoryPanel;
		[SerializeField] private TextMeshProUGUI rescuedCoinsText;
		[SerializeField] private TextMeshProUGUI rescuedCountText;
		[SerializeField] private TextMeshProUGUI collectedCoinsText;
		[SerializeField] private SkinShopUI skinShop;
		[SerializeField] private Button settingsButton;
		[SerializeField] private GameObject panel;
		[SerializeField] private GameObject noFtuePanel;
		[SerializeField] private GameObject wheelOfFortunePanel;
		[SerializeField] private FortuneWheelManager fortuneWheelManager;
		[SerializeField] private GameObject coinsPanel;
		[SerializeField] private GameObject leftButtonsPanel;
		[SerializeField] private TextMeshProUGUI bonusLevelCoinsText;
        [SerializeField] private GameObject matchResultsShadePanel;
        //[SerializeField] private BattlePassUI battlePassUi;
	
		private HideAndSeekGameMode hideAndSeekGameMode;

		private PlayerProfile playerProfile;
		public int MatchCoins { get; private set; }
		private int playerCoinsBeforeMatch;
		private int boughtSkinsCoins;

		private int caughtCount;
		private bool loseInSeekMode;
		private bool win;
		private FTUE ftue;
		private bool isFTUEMatch;
		private int bonusLevelCoinsCollected;
		private int bonusLevelCoinsAfterWheelOfFortune;
		

		private bool firstLevel;
		private int winCount;




		private void OnEnable()
		{
			this.noFtuePanel.SetActive(true);
		}

		private void Awake()
		{

			this.playerProfile = FindObjectOfType<PlayerProfile>();
			
			this.ftue = FindObjectOfType<FTUE>();

			this.firstLevel = LevelRepository.Instance.CurrentLevelNumber == 1;
			
		
			EventManager.OnGameModeInit.AddListener(OnGameModeInit);
			EventManager.OnLevelChanged.AddListener(OnLevelChanged);
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnChangeLevel.AddListener(OnChangeLevel);
			EventManager.OnStartMatch.AddListener(OnStartMatch);
			EventManager.OnHiderCaught.AddListener(OnHiderCaught);
			EventManager.OnHiderReleased.AddListener(OnHiderReleased);
			EventManager.OnExtraTimeStarted.AddListener(OnExtraTimeStarted);
			EventManager.OnHiderRescue.AddListener(OnHiderRescue);
			EventManager.OnSkinBought.AddListener(OnSkinBought);
		}

		private void Start()
		{
			this.skinShop.UpdateSkinsScrollRect();
			UpdateCurrentLevelText();

			Input.multiTouchEnabled = false;
		}

		private void OnDestroy()
		{
			EventManager.OnGameModeInit.RemoveListener(OnGameModeInit);
			EventManager.OnLevelChanged.RemoveListener(OnLevelChanged);
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnChangeLevel.RemoveListener(OnChangeLevel);
			EventManager.OnStartMatch.RemoveListener(OnStartMatch);
			EventManager.OnHiderCaught.RemoveListener(OnHiderCaught);
			EventManager.OnHiderReleased.RemoveListener(OnHiderReleased);
			EventManager.OnExtraTimeStarted.RemoveListener(OnExtraTimeStarted);
			EventManager.OnHiderRescue.RemoveListener(OnHiderRescue);
			EventManager.OnSkinBought.RemoveListener(OnSkinBought);
		}



		public void StartSeekMode()
		{
			var bots = FindObjectsOfType<Character>().Where(ch => !ch.CompareTag("Player")).ToList();

			if (bots.Count == 0)
			{
				return;
			}
			
			var character = FindObjectsOfType<Character>().SingleOrDefault(ch => ch.CompareTag("Player"));
			
			if (character == null)
			{
				return;
			}
			
			foreach (var bot in bots)
			{
				this.hideAndSeekGameMode.Join(bot, new Dictionary<string, object>()
				{
					{"role", "hider"}
				});
			}
			
			this.hideAndSeekGameMode.Join(character, new Dictionary<string, object>()
			{
				{"role", "seeker"}
			});
			
			this.hideAndSeekGameMode.StartMatch();
			
			Spawner.Instance.ShowActivePerks();
			
			this.leftButtonsPanel.SetActive(false);
			
			SetActive(false);
			
			this.loseInSeekMode = false;
			this.win = false;
		}

		public void StartHideMode()
		{
			var bots = FindObjectsOfType<Character>().Where(ch => !ch.CompareTag("Player")).ToList();

			if (bots.Count == 0)
			{
				return;
			}

			var character = FindObjectsOfType<Character>().SingleOrDefault(ch => ch.CompareTag("Player"));

			if (character == null)
			{
				return;
			}
			
			for(var i = 1; i < bots.Count; i++)
			{
				this.hideAndSeekGameMode.Join(bots[i], new Dictionary<string, object>()
				{
					{"role", "hider"}
				});
			}
			
			this.hideAndSeekGameMode.Join(character, new Dictionary<string, object>()
			{
				{"role", "hider"}
			});
			
			this.hideAndSeekGameMode.Join(bots[0], new Dictionary<string, object>()
			{
				{"role", "seeker"}
			});
			
			this.hideAndSeekGameMode.StartMatch();
			
			Spawner.Instance.ShowActivePerks();
			
			this.leftButtonsPanel.SetActive(false);
			
			SetActive(false);

			this.win = false;
		}

		private bool bonusLevel;

		public void NextLevel()
		{
			EventManager.OnClickNextLevelButton?.Invoke();

			ChangeLevel();
		}

		public void ChangeLevel()
		{
			this.playerProfile.NeedEasyBots = false;
		
			if ((LevelRepository.Instance.CurrentLevelNumber + 1) % Spawner.Instance.BonusLevelNum == 0 && !this.bonusLevel)
			{
				this.bonusLevel = true;
			
				Spawner.Instance.ChangeLevel(LevelRepository.Instance.BonusLevels[Random.Range(0, LevelRepository.Instance.BonusLevels.Count)], LevelRepository.Instance.CurrentLevelNumber + 1,true);
			}
			else
			{
				this.bonusLevel = false;
			
				Spawner.Instance.ChangeLevel(LevelRepository.Instance.NextLevel, LevelRepository.Instance.CurrentLevelNumber + 1);
				
				LevelRepository.Instance.CurrentLevelNumber++;
			}
		}

		public void OnClickStartInvisible()
		{
		}

		private void AdsStartInvisibleRewardAction()
		{
			StartHideMode();
			
			var player = this.hideAndSeekGameMode.Players.Single(p => p.CompareTag("Player"));

			var activePerkController = player.GetComponent<ActivePerkController>();
			
			activePerkController.ApplyPerk(ActivePerk.GetTypeByName(ActivePerkName.Invisibility));
			activePerkController.CurrentActivePerk.Duration = 10;
			
		}

		public void OnClickWheelOfFortuneNextLevel()
		{
			Spawner.Instance.ChangeLevel(LevelRepository.Instance.NextLevel, LevelRepository.Instance.CurrentLevelNumber + 1);
			
			LevelRepository.Instance.CurrentLevelNumber++;

			this.playerProfile.Coins += this.bonusLevelCoinsAfterWheelOfFortune - this.bonusLevelCoinsCollected;
		}

		public void AdsNextLevel()
		{
		}

		private void AdsNextLevelRewardAction()
		{
			EventManager.OnClickNextLevelButton?.Invoke();
		
			ChangeLevel();
			
		}
		
		

		public void PlayAgain()
		{
			if ((this.caughtCount >= 2) || (this.loseInSeekMode && !this.ftue.NeedFTUE))
			{

				this.caughtCount = 0;
			}
			else if ((this.firstLevel && this.winCount > 1) || (!this.firstLevel && this.winCount > 0))
			{

				this.winCount = 0;
			}
			
			if (this.win)
			{
				this.winCount++;
			}

			this.playerProfile.NeedEasyBots = !this.win;

			this.win = false;
			this.loseInSeekMode = false;
		
			EventManager.OnClickPlayAgainButton?.Invoke();
		
			Spawner.Instance.RestartLevel();

			var cameraController = FindObjectOfType<CameraController>();
			
			cameraController.LerpCamera(Quaternion.Euler(CameraRepository.Instance.Rotation), CameraRepository.Instance.Offset, 0.5f);
		}

		public void SetActive(bool active)
		{
			this.panel.SetActive(active);
		}
		
		public void ExtraTime()
		{
			var result = this.hideAndSeekGameMode.Seeker.Hiders.Count >= this.hideAndSeekGameMode.CaughtHidersCountForWin ? 
			Result.Win : Result.Lose;
			
		}

		private void ExtraTimeRewardAction()
		{
			this.hideAndSeekGameMode.ExtraTime(15f);
			
			this.hideAndSeekGameMode.SetPause(false);
			
			SetActive(false);
			
		}

		public void OpenShop()
		{
			this.skinShop.Open();
		}

		public void CloseShop()
		{
			this.skinShop.Close();
		}

		public void AddCoinsToBonusLevelCollectedCoins(int coins)
		{
			this.bonusLevelCoinsAfterWheelOfFortune += coins;

			this.bonusLevelCoinsText.text = this.bonusLevelCoinsAfterWheelOfFortune.ToString();
			
			this.fortuneWheelManager.SetNextLevelButtonActive(true);
		}
		
		public void MultiplyCoinsBonusLevelCollectedCoins(int multiplier)
		{
			this.bonusLevelCoinsAfterWheelOfFortune *= multiplier;
			
			this.bonusLevelCoinsText.text = this.bonusLevelCoinsAfterWheelOfFortune.ToString();
			
			this.fortuneWheelManager.SetNextLevelButtonActive(true);
		}
		
		public void AddSkinToProfile(string skinName)
		{
			this.playerProfile.SkinService.AddSkinInProfile(SkinRepository.Instance.Skins.Single(s => s.Name.Equals(skinName)));
		}

		private void OnClickDoubleMatchCoinsButton()
		{
		}

		private void DoubleMatchCoinsRewardAction()
		{
			this.playerProfile.Coins += this.MatchCoins;

			SetMatchCoins(this.MatchCoins * 2);
			
		}

		public void Claim()
		{
			this.hideAndSeekGameMode.SetPause(true);
			
		}

		private void ClaimRewardAction()
		{
			var player = this.hideAndSeekGameMode.Hiders.First(h => h.CompareTag("Player"));
		
			var cage = player.transform.root
				.GetComponent<Cage>();

			if (cage != null)
			{
				cage.Open(null);

				Destroy(cage.gameObject);
			}

			this.hideAndSeekGameMode.PauseTimer = false;
			this.hideAndSeekGameMode.SetPause(false);

			var activePerkController = player.GetComponent<ActivePerkController>();
			
			activePerkController.ApplyPerk(ActivePerk.GetTypeByName(ActivePerkName.Invisibility));
			activePerkController.CurrentActivePerk.Duration = 7;
			
			SetActive(false);
		}


		private void UpdateCurrentLevelText()
		{
			this.currentLevelText.text = LevelRepository.Instance.CurrentLevelNumber.ToString();
		}
		
		private void OnGameModeInit(GameMode gameMode)
		{
			if (gameMode is HideAndSeekGameMode hideAndSeekGameMode)
			{
				if (this.hideAndSeekGameMode != null)
				{
					this.hideAndSeekGameMode.OnTimerLeft -= OnTimerLeft;
				}

				this.hideAndSeekGameMode = hideAndSeekGameMode;

				this.hideAndSeekGameMode.OnTimerLeft += OnTimerLeft;
			}
		}
		
		private void OnStartMatch(GameMode gameMode)
		{
			Input.multiTouchEnabled = true;
		
			this.isFTUEMatch = this.ftue.NeedFTUE;
			this.playerCoinsBeforeMatch = this.playerProfile.Coins;
			this.boughtSkinsCoins = 0;
			this.bonusLevelCoinsCollected = 0;
			this.bonusLevelCoinsAfterWheelOfFortune = 0;
			
			this.settingsButton.gameObject.SetActive(false);

			var cameraController = FindObjectOfType<CameraController>();
			
			cameraController.GetComponent<Animator>().enabled = false;
			
			cameraController.enabled = true;
			
			cameraController.MotionTarget = gameMode.Players.Single(p => p.CompareTag("Player")).transform;
			cameraController.LerpCamera(Quaternion.Euler(CameraRepository.Instance.Rotation), CameraRepository.Instance.Offset, 0.5f);
		}

		private void OnTimerLeft()
		{
			Input.multiTouchEnabled = false;
		
			UpdatePlayersResultPanel();

			SetMatchCoins(this.playerProfile.Coins + this.boughtSkinsCoins - this.playerCoinsBeforeMatch);

			if (this.hideAndSeekGameMode.PlayerRole() == Role.Hider)
			{
				UpdateCollectedCoinsText();
				UpdateRescuedCoinsText();
			}

			this.currentLevelPanel.SetActive(false);
			this.hideModeButton.gameObject.SetActive(false);
			this.seekModeButton.gameObject.SetActive(false);
			this.leftButtonsPanel.gameObject.SetActive(false);

			this.loseInSeekMode = this.hideAndSeekGameMode.PlayerRole() == Role.Seeker && this.hideAndSeekGameMode.Seeker.Hiders.Count <
			                      this.hideAndSeekGameMode.CaughtHidersCountForWin;

			this.win = this.hideAndSeekGameMode.PlayerRole() == Role.Seeker && this.hideAndSeekGameMode.Seeker.Hiders.Count >=
			                     this.hideAndSeekGameMode.CaughtHidersCountForWin;

			this.playerProfile.NeedEasyBots = !this.win;
			
			if (!this.skinShop.IsCoinsForAdsAvailable)
			{
				this.skinShop.IsCoinsForAdsAvailable = this.win;
			}
			
			                                  
			this.seekNoExtraTimePanel.SetActive(this.hideAndSeekGameMode.PlayerRole() == Role.Seeker && 
			                                    this.hideAndSeekGameMode.Seeker.Hiders.Count == this.hideAndSeekGameMode.Hiders.Count);
			
			this.settingsButton.gameObject.SetActive(false);
			
			this.playersResultPanel.gameObject.SetActive(this.hideAndSeekGameMode.PlayerRole() == Role.Seeker);
			this.playersResultPanel.anchoredPosition = new Vector2(this.playersResultPanel.anchoredPosition.x, this.hideAndSeekGameMode.Seeker.Hiders.Count < 4 ? -109.6f : -77.5f);
			
			if (this.hideAndSeekGameMode.PlayerRole() == Role.Seeker)
			{
				var win = this.hideAndSeekGameMode.Seeker.Hiders.Count >=
				          this.hideAndSeekGameMode.CaughtHidersCountForWin;
			
				this.seekVictoryPanel.gameObject.SetActive(win);
				this.seekLosePanel.gameObject.SetActive(!win);
				
				this.hideVictoryPanel.SetActive(false);
				this.hideLosePanel.SetActive(false);
			}
			else
			{
				this.seekVictoryPanel.gameObject.SetActive(false);
				this.seekLosePanel.gameObject.SetActive(false);

				var player = this.hideAndSeekGameMode.Hiders.FirstOrDefault(h => h.CompareTag("Player"));
				
				this.hideVictoryPanel.SetActive(!player.Caught);
				this.hideLosePanel.SetActive(player.Caught);
			}
			
			this.coinsPanel.SetActive(false);

            this.matchResultsShadePanel.SetActive(true);
            
            SetActive(true);
            
            // if (this.win)
            // {
	           //  this.battlePassUi.Open();
            // }
            // else
            // {
	           //  this.battlePassUi.Hide();
            // }
		}

		private IEnumerator SetCoinsWithDelay()
		{
			yield return new WaitForSeconds(0.1f);
			
			SetMatchCoins(this.playerProfile.Coins + this.boughtSkinsCoins - this.playerCoinsBeforeMatch);
		}

		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			Input.multiTouchEnabled = false;
		
			if (gameMode is HideAndSeekGameMode)
			{
				this.win = result == Result.Win;
				
				this.playerProfile.NeedEasyBots = !this.win;
			
				UpdatePlayersResultPanel();

				if (this.gameObject.activeInHierarchy)
				{
					StartCoroutine(SetCoinsWithDelay());
				}

				if (this.hideAndSeekGameMode.PlayerRole() == Role.Hider)
				{
					UpdateCollectedCoinsText();
					UpdateRescuedCoinsText();
				}

				if (!this.skinShop.IsCoinsForAdsAvailable)
				{
					this.skinShop.IsCoinsForAdsAvailable = result == Result.Win;
				}

                this.currentLevelPanel.SetActive(false);
				this.hideModeButton.gameObject.SetActive(false);
				this.seekModeButton.gameObject.SetActive(false);
				this.leftButtonsPanel.gameObject.SetActive(false);

				this.seekLosePanel.gameObject.SetActive(false);
				this.playersResultPanel.gameObject.SetActive(this.hideAndSeekGameMode.PlayerRole() == Role.Seeker);
				this.playersResultPanel.anchoredPosition =
					new Vector2(this.playersResultPanel.anchoredPosition.x, -77.5f);
				this.settingsButton.gameObject.SetActive(false);

				this.noFtuePanel.SetActive(true);
				
				if (this.hideAndSeekGameMode.PlayerRole() == Role.Seeker)
				{
					this.hideVictoryPanel.SetActive(false);
					this.hideLosePanel.SetActive(false);
					
					this.seekNoExtraTimePanel.SetActive(true);

					this.seekVictoryPanel.gameObject.SetActive(result == Result.Win);
					this.seekLosePanel.gameObject.SetActive(result == Result.Lose);
				}
				else
				{
					this.seekVictoryPanel.gameObject.SetActive(false);
					this.seekLosePanel.gameObject.SetActive(false);

					this.hideVictoryPanel.SetActive(result == Result.Win);
					this.hideLosePanel.SetActive(result == Result.Lose);
				}
			}
			else
			{
				this.bonusLevelCoinsCollected = this.playerProfile.Coins - this.playerCoinsBeforeMatch;
				this.bonusLevelCoinsText.text = this.bonusLevelCoinsCollected.ToString();

				this.bonusLevelCoinsAfterWheelOfFortune = this.bonusLevelCoinsCollected;
			
				this.hideModeButton.gameObject.SetActive(false);
				this.seekModeButton.gameObject.SetActive(false);
				this.coinsPanel.SetActive(false);
				this.currentLevelPanel.SetActive(false);
				this.wheelOfFortunePanel.SetActive(true);
				this.leftButtonsPanel.SetActive(false);
			}
			
			this.coinsPanel.SetActive(false);
            this.matchResultsShadePanel.SetActive(true);
            SetActive(true);

            // if (gameMode is HideAndSeekGameMode)
            // {
	           //  if (result == Result.Win)
	           //  {
		          //   this.battlePassUi.Open();
	           //  }
	           //  else
	           //  {
		          //   this.battlePassUi.Hide();
	           //  }
            // }
		}
		
		private void OnLevelChanged(Level newLevel, int levelNum, bool bonusLevel)
		{
			Input.multiTouchEnabled = false;
		
			UpdateCurrentLevelText();
			
			this.currentLevelPanel.SetActive(true);
		
			this.hideModeButton.gameObject.SetActive(true);
			this.seekModeButton.gameObject.SetActive(true);
			this.leftButtonsPanel.gameObject.SetActive(true);
			
			this.noFtuePanel.SetActive(true);
			
			this.playersResultPanel.gameObject.SetActive(false);
			this.seekVictoryPanel.SetActive(false);
			this.seekLosePanel.SetActive(false);
			this.hideVictoryPanel.SetActive(false);
			this.hideLosePanel.SetActive(false);
			this.settingsButton.gameObject.SetActive(true);
			this.coinsPanel.SetActive(true);
			this.wheelOfFortunePanel.SetActive(false);
            this.matchResultsShadePanel.SetActive(false);

            SetActive(!bonusLevel);
            
            this.skinShop.UpdateSkinsScrollRect();
		}
		
		private void OnExtraTimeStarted(float time)
		{
			Input.multiTouchEnabled = true;
		
			this.settingsButton.gameObject.SetActive(false);
		}
		
		private void OnHiderCaught(Seeker seeker, Hider hider)
		{
			if (hider.CompareTag("Player"))
			{
				Input.multiTouchEnabled = false;
			
				this.hideAndSeekGameMode.PauseTimer = true;
			
				this.currentLevelPanel.SetActive(false);
				this.hideModeButton.gameObject.SetActive(false);
				this.seekModeButton.gameObject.SetActive(false);
				this.leftButtonsPanel.gameObject.SetActive(false);
				this.playersResultPanel.gameObject.SetActive(false);
				this.seekVictoryPanel.SetActive(false);
				this.seekLosePanel.SetActive(false);
				this.hideVictoryPanel.SetActive(false);
				this.hideLosePanel.SetActive(this.hideAndSeekGameMode.PlayerRole() == Role.Hider && hider.CompareTag("Player"));
				this.settingsButton.gameObject.SetActive(false);
                this.matchResultsShadePanel.SetActive(true);

                this.caughtCount++;
				
				SetActive(true);
			}
		}
		
		private void OnHiderRescue(Hider rescuer, Hider rescued)
		{
			if (rescued.CompareTag("Player"))
			{
				Input.multiTouchEnabled = true;
			}

			this.settingsButton.gameObject.SetActive(false);
		}
		
		private void OnChangeLevel(Level level, int levelNum, bool bonusLevel)
		{
			if (level != null)
			{
				this.winCount = 0;
			}

			//this.battlePassUi.Hide();
			
			this.firstLevel = levelNum == 1;
		
			SetActive(false);
		}
		
		private void OnSkinBought(Skin skin)
		{
			this.boughtSkinsCoins += skin.Price;
		}
		
		private void OnHiderReleased(Hider hider)
		{
			if (hider.CompareTag("Player"))
			{
				Input.multiTouchEnabled = true;
			
				this.hideAndSeekGameMode.PauseTimer = false;
				
				this.settingsButton.gameObject.SetActive(false);
			
				SetActive(false);
			}
		}

		private void SetMatchCoins(int coins)
		{
			this.MatchCoins = coins;
		
			UpdateMatchCoinsTexts();
		}

		private void UpdateMatchCoinsTexts()
		{
			foreach (var matchCoinsText in this.matchCoinsTexts)
			{
				matchCoinsText.text = this.MatchCoins.ToString();
			}
		}

		private void UpdateCollectedCoinsText()
		{
			var player = this.hideAndSeekGameMode.Hiders.FirstOrDefault(h => h.CompareTag("Player"));
		
			var rescuedCoins = player.RescuedCount * RewardRepository.Instance.RescueReward;

			this.collectedCoinsText.text = (this.MatchCoins - rescuedCoins + (player.Caught ? 0 : RewardRepository.Instance.WinReward)).ToString();
		}

		private void UpdateRescuedCoinsText()
		{
			var player = this.hideAndSeekGameMode.Hiders.FirstOrDefault(h => h.CompareTag("Player"));

			this.rescuedCountText.text = player.RescuedCount.ToString();
			this.rescuedCoinsText.text = (player.RescuedCount * RewardRepository.Instance.RescueReward).ToString();
		}

		private void UpdatePlayersResultPanel()
		{
			if (this.hideAndSeekGameMode == null)
			{
				return;
			}

			foreach (Transform tr in this.playersResultPanel)
			{
				Destroy(tr.gameObject);
			}

			foreach (var hider in this.hideAndSeekGameMode.Hiders)
			{
				var playerResult = Instantiate(this.playerResultUiPrefab, this.playersResultPanel);
				
				playerResult.SetResult(hider.Caught);

				var playerProfile = hider.CompareTag("Player") ? this.playerProfile : hider.GetComponent<IPlayerProfile>();
				
				playerResult.SetNickname(playerProfile.Name);
				playerResult.SetCountrySprite(SpriteRepository.Instance.GetCountrySprite(playerProfile.Country));
			}
		}

		private IEnumerator ShowBanner2WithDelay()
		{
			yield return new WaitForSeconds(7f);
		}
	}
}