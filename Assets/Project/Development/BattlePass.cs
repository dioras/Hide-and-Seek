using System;
using System.Collections.Generic;
using System.Linq;
using Project.Development.Events;
using Project.Development.ModeScripts;
using Project.Development.SkinScripts;
using UnityEngine;

namespace Project.Development
{
	public class BattlePass: MonoBehaviour
	{
		public static BattlePass Instance { get; private set; }
	
	
		public static string BPCurrWinCountKey = "BPCurrWinCount";
		public static string BPWinCountForSkinKey = "BPWinCountForSkin";
		public static string BPNextSkinKey = "BPNextSkin";


		public Action<int> OnCurrentWinCountChangedEvent { get; set; }
		public Action<int> OnWinCountForRewardChangedEvent { get; set; }
		public Action<Skin> OnRewardEvent { get; set; }
		public Action<Skin> OnNextRewardChangedEvent { get; set; }

		public bool IsActive => SkinRepository.Instance.Skins.Any(s => !this.playerProfile.SkinService.IsSkinInProfile(s));

		[field:SerializeField]
		public List<Skin> Skins { get; set; }

		public int WinCountForSkinReward
		{
			get => PlayerPrefs.GetInt(BPWinCountForSkinKey, 2);
			private set
			{
				if (value > 5)
				{
					return;
				}
				
				PlayerPrefs.SetInt(BPWinCountForSkinKey, value);
				PlayerPrefs.Save();
				
				OnWinCountForRewardChangedEvent?.Invoke(value);
			}
		}

		public int CurrentWinCount
		{
			get => PlayerPrefs.GetInt(BPCurrWinCountKey, 0);
			private set
			{
				PlayerPrefs.SetInt(BPCurrWinCountKey, value);
				PlayerPrefs.Save();
				
				OnCurrentWinCountChangedEvent?.Invoke(value);
			}
		}

		public Skin NextSkin
		{
			get
			{
				var skinName = PlayerPrefs.GetString(BPNextSkinKey, Skins[0].Name);

				return SkinRepository.Instance.Skins.Single(s => s.Name.Equals(skinName));
			}
			private set
			{
				PlayerPrefs.SetString(BPNextSkinKey, value.Name);
				PlayerPrefs.Save();
				
				OnNextRewardChangedEvent?.Invoke(value);
			}
		}

		private PlayerProfile playerProfile;
		private HideAndSeekGameMode hideAndSeekGameMode;



		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(this);
			}
			else 
			{
				if (Instance != this)
				{
					Destroy(this.gameObject);
                    
					return;
				}
                
				return;
			}
		
			OnCurrentWinCountChangedEvent += OnCurrentWinCountChanged;

			this.playerProfile = FindObjectOfType<PlayerProfile>();
			
			this.playerProfile.SkinService.OnSkinAdded += OnSkinAddedToProfile;
			
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnGameModeInit.AddListener(OnGameModeInit);
		}

		private void OnDestroy()
		{
			this.playerProfile.SkinService.OnSkinAdded -= OnSkinAddedToProfile;
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnGameModeInit.RemoveListener(OnGameModeInit);
		}



		private void OnCurrentWinCountChanged(int newValue)
		{
			if (newValue >= WinCountForSkinReward)
			{
				NextSkin.AvailableInShop = true;
			
				SkinRepository.Instance.AddOrUpdateSkin(NextSkin);
				
				OnRewardEvent?.Invoke(NextSkin);
				
				SetNextSkin();
				
				CurrentWinCount = 0;
				WinCountForSkinReward++;
			}
		}

		public void SetNextSkin()
		{
			foreach (var s in Skins)
			{
				if (!s.AvailableInShop)
				{
					NextSkin = s;
					
					return;
				}
			}

			var skins = SkinRepository.Instance.Skins.Where(s => s != NextSkin && !this.playerProfile.SkinService.IsSkinInProfile(s) && s.AvailableInShop && s.ForAds).ToList();

			if (skins.Count > 0)
			{
				NextSkin = skins[UnityEngine.Random.Range(0, skins.Count)];
				
				return;
			}
			
			skins = SkinRepository.Instance.Skins.Where(s => s != NextSkin && !this.playerProfile.SkinService.IsSkinInProfile(s) && s.AvailableInShop).ToList();

			if (skins.Count > 0)
			{
				NextSkin = skins[UnityEngine.Random.Range(0, skins.Count)];
				
				return;
			}
		}
		
		private void OnSkinAddedToProfile(Skin skin)
		{
			if (NextSkin == skin)
			{
				SetNextSkin();
				CurrentWinCount = 0;
			}
		}
		
		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			if (gameMode is HideAndSeekGameMode)
			{
				if (result == Result.Win)
				{
					CurrentWinCount++;
				}
			}
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

		private void OnTimerLeft()
		{
			var win = this.hideAndSeekGameMode.PlayerRole() == Role.Seeker && this.hideAndSeekGameMode.Seeker.Hiders.Count >=
			           this.hideAndSeekGameMode.CaughtHidersCountForWin;

			if (win)
			{
				CurrentWinCount++;
			}
		}
	}
}