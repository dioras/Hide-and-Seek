using System;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using Project.Development.ModeScripts;
using Project.Development.SkinScripts;
using UnityEngine;

namespace Project.Development
{
	public class PlayerProfile: MonoBehaviour, IPlayerProfile
	{
		private static string CoinsKey = "Coins";
		private static string NicknameKey = "Nickname";
		private static string NeedEasyBotsKey = "NeedEasyBots";

		public Action<int, int> OnCoinsChanged { get; set; }


		public string Name
		{
			get => PlayerPrefs.GetString(NicknameKey, "Player");
			set
			{
				PlayerPrefs.SetString(NicknameKey, value);
				PlayerPrefs.Save();
			}
		}
		[field:SerializeField]
		public Country Country { get; set; }
		
		public ISkinService SkinService { get; set; }
	
		public int Coins
		{
			get => PlayerPrefs.GetInt(CoinsKey, 0);
			set
			{
				var prevValue = this.Coins;
			
				PlayerPrefs.SetInt(CoinsKey, value);
				PlayerPrefs.Save();
				
				OnCoinsChanged?.Invoke(value, prevValue);
			}
		}
		
		
		public bool NeedEasyBots
		{
			get => Convert.ToBoolean(PlayerPrefs.GetString(NeedEasyBotsKey, false.ToString()));
			set
			{
				PlayerPrefs.SetString(NeedEasyBotsKey, value.ToString());
				PlayerPrefs.Save();
			}
		}



		private void Awake()
		{
			this.SkinService = new PlayerSkinService();
			
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnCoinPickuped.AddListener(OnCoinPickuped);
			EventManager.OnHiderCaught.AddListener(OnHiderCaught);
			EventManager.OnHiderRescue.AddListener(OnHiderRescue);
		}

		private void OnDestroy()
		{
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnCoinPickuped.RemoveListener(OnCoinPickuped);
			EventManager.OnHiderCaught.RemoveListener(OnHiderCaught);
			EventManager.OnHiderRescue.RemoveListener(OnHiderRescue);
			
		}




		private void OnCoinPickuped(Coin coin, CoinCollector coinCollector)
		{
			if (coinCollector.CompareTag("Player"))
			{
				this.Coins += coin.Amount;
			}
		}
		
		private void OnHiderCaught(Seeker seeker, Hider hider)
		{
			if (seeker.CompareTag("Player"))
			{
				this.Coins += RewardRepository.Instance.CatchReward;
			}
		}
		
		private void OnHiderRescue(Hider rescuer, Hider rescued)
		{
			if (rescuer != null && rescuer.CompareTag("Player"))
			{
				this.Coins += RewardRepository.Instance.RescueReward;
			}
		}
		
		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			if (result == Result.Win)
			{
				this.Coins += RewardRepository.Instance.WinReward;
			}
		}
		
	}
}