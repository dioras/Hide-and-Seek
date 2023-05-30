using Project.Development.Events;
using Project.Development.SkinScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class SkinUI: MonoBehaviour
	{
		[SerializeField] private Image skinImage;
		[SerializeField] private Text priceText;
		[SerializeField] private RectTransform boughtState;
		[SerializeField] private RectTransform notBoughtState;
		[SerializeField] private RectTransform buyForAds;
		[SerializeField] private RectTransform buyForCoins;
		[SerializeField] private Image backgroundImage;
		[SerializeField] private Color selectedColor;
		[SerializeField] private Color unselectedColor;

		public Skin Skin { get; private set; }
		
		private PlayerProfile playerProfile;




		private void OnEnable()
		{
			UpdateState();
		}

		private void Awake()
		{
			this.playerProfile = FindObjectOfType<PlayerProfile>();
		}



		public void Buy()
		{
			if (this.playerProfile.SkinService.IsSkinInProfile(this.Skin))
			{
				return;
			}
			
			if (this.Skin.Price > this.playerProfile.Coins)
			{
				EventManager.OnNotEnoughCoins?.Invoke();
			
				return;
			}

			this.playerProfile.Coins -= this.Skin.Price;
			
			this.playerProfile.SkinService.AddSkinInProfile(this.Skin);
			
			Select();
			
			UpdateState();
			
			EventManager.OnSkinBought?.Invoke(this.Skin);
		}
		
		public void BuyForAds()
		{
			if (this.playerProfile.SkinService.IsSkinInProfile(this.Skin))
			{
				return;
			}
		}

		private void BuyForAdsRewardAction()
		{
			this.playerProfile.SkinService.AddSkinInProfile(this.Skin);
			
			Select();
			
			UpdateState();
			
			EventManager.OnSkinBought?.Invoke(this.Skin);
		}

		public void SetSkin(Skin skin)
		{
			this.skinImage.sprite = skin.Sprite;
			this.priceText.text = skin.Price.ToString();
			
			this.Skin = skin;
			
			UpdateState();
		}

		public void Select()
		{
			if (!this.playerProfile.SkinService.IsSkinInProfile(this.Skin))
			{
				return;
			}
		
			this.playerProfile.SkinService.CurrentSkin = this.Skin;
			
			SetBackgroundImageColor(this.selectedColor);
		
			EventManager.OnSkinSelected.Invoke(this);
		}

		public void Deselect()
		{
			if (!this.playerProfile.SkinService.IsSkinInProfile(this.Skin))
			{
				return;
			}
			
			SetBackgroundImageColor(this.unselectedColor);
		}

		private void SetBackgroundImageColor(Color color)
		{
			this.backgroundImage.color = color;
		}

		private void UpdateState()
		{
			if (this.playerProfile == null)
			{
				this.playerProfile = FindObjectOfType<PlayerProfile>();
			}

			if (this.Skin == null)
			{
				return;
			}
		
			if (this.playerProfile.SkinService.IsSkinInProfile(this.Skin))
			{
				this.boughtState.gameObject.SetActive(true);
				this.notBoughtState.gameObject.SetActive(false);
			}
			else
			{
				this.boughtState.gameObject.SetActive(false);
				this.notBoughtState.gameObject.SetActive(true);
				this.buyForAds.gameObject.SetActive(this.Skin.ForAds);
				this.buyForCoins.gameObject.SetActive(!this.Skin.ForAds);
			}
		}
	}
}