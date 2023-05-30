using System;
using System.Collections;
using System.Collections.Generic;
using Project.Development.Events;
using Project.Development.SkinScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class SkinShopUI: MonoBehaviour
	{
		[SerializeField] private ScrollRect skinsScrollRect;
		[SerializeField] private SkinUI skinPrefab;
		[SerializeField] private GameObject skinShop;
		[SerializeField] private string coinsForAdsKey;
		[SerializeField] private Button coinsForAdsButton;
		[SerializeField] private int coinsForAds;
		[SerializeField] private GameObject notEnoughCoinsPanel;

		public bool IsCoinsForAdsAvailable
		{
			get => Convert.ToBoolean(PlayerPrefs.GetInt(this.coinsForAdsKey, 1));
			set
			{
				PlayerPrefs.SetInt(this.coinsForAdsKey, Convert.ToInt32(value));
				PlayerPrefs.Save();
			}
		}
	
		private PlayerProfile playerProfile;
		private FTUE ftue;

		private List<SkinUI> skinsUI = new List<SkinUI>();




		private void Awake()
		{
			this.playerProfile = FindObjectOfType<PlayerProfile>();
			this.ftue = FindObjectOfType<FTUE>();
			
			EventManager.OnSkinSelected.AddListener(OnSkinSelected);
			EventManager.OnNotEnoughCoins.AddListener(OnNotEnoughCoins);
		}

		private void Start()
		{
			IsCoinsForAdsAvailable = true;
		}

		private void OnDestroy()
		{
			EventManager.OnSkinSelected.RemoveListener(OnSkinSelected);
			EventManager.OnNotEnoughCoins.RemoveAllListeners();
		}


		public void Open()
		{
			this.coinsForAdsButton.gameObject.SetActive(!this.ftue.NeedFTUE && IsCoinsForAdsAvailable);
			
			this.skinShop.SetActive(true);
			ScrollToCurrentSkin();
		}

		public void Close()
		{
			this.skinShop.SetActive(false);
		}

		public void GetCoinsForAds()
		{
		}

		private void GetCoinsForAdsRewardAction()
		{
			this.playerProfile.Coins += this.coinsForAds;

			IsCoinsForAdsAvailable = false;
			
			this.coinsForAdsButton.gameObject.SetActive(false);
			
		}


		public void UpdateSkinsScrollRect()
		{
			if (this.gameObject.activeInHierarchy)
			{
				StartCoroutine(UpdateSkinsScrollRectProcess());
			}
		}

		private IEnumerator UpdateSkinsScrollRectProcess()
		{
			foreach (Transform tr in this.skinsScrollRect.content)
			{
				Destroy(tr.gameObject);
			}
			
			this.skinsUI.Clear();
		
			var skins = SkinRepository.Instance.Skins;
		
			foreach (var skin in skins)
			{
				if (!skin.AvailableInShop)
				{
					continue;
				}
			
				var skinUi = Instantiate(this.skinPrefab, this.skinsScrollRect.content);
				
				skinUi.SetSkin(skin);
				
				this.skinsUI.Add(skinUi);
				
				yield return null;
			}

			var prevState = this.skinShop.activeInHierarchy;

			yield return null;
			
			this.skinShop.SetActive(true);
			ScrollToCurrentSkin();
			this.skinShop.SetActive(prevState);
			SelectCurrentSkin();
		}

		private void SelectCurrentSkin()
		{
			var currSkin = GetCurrentSkin();
			
			if (currSkin != null)
			{
				currSkin.Select();
			}
		}

		private SkinUI GetCurrentSkin()
		{
			SkinUI currSkin = null;
		
			foreach (var skinUi in this.skinsUI)
			{
				if (skinUi.Skin.Name.Equals(this.playerProfile.SkinService.CurrentSkin.Name))
				{
					currSkin = skinUi;
					
					break;
				}
			}

			return currSkin;
		}

		private void ScrollToCurrentSkin()
		{
			var currSkin = GetCurrentSkin();

			if (currSkin != null)
			{
				this.skinsScrollRect.ScrollToCenter(currSkin.GetComponent<RectTransform>());
			}
		}
		
		private void OnSkinSelected(SkinUI skinUI)
		{
			foreach (var skinUi in this.skinsUI)
			{
				if (skinUi == skinUI)
				{
					continue;
				}
				
				skinUi.Deselect();
			}
		}
		
		private void OnNotEnoughCoins()
		{
			this.notEnoughCoinsPanel.SetActive(true);
		}
	}
}