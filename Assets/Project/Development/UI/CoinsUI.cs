using TMPro;
using UnityEngine;

namespace Project.Development.UI
{
	public class CoinsUI: MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI coinsCountText;
		
		private PlayerProfile playerProfile;




		private void OnEnable()
		{
			UpdateCoinsCountText();
		}

		private void Awake()
		{
			this.playerProfile = FindObjectOfType<PlayerProfile>();
		
			this.playerProfile.OnCoinsChanged += OnCoinsChanged;
		
			UpdateCoinsCountText();
		}

		private void OnDestroy()
		{
			this.playerProfile.OnCoinsChanged -= OnCoinsChanged;
		}



		private void OnCoinsChanged(int newCoins, int prevCoins)
		{
			UpdateCoinsCountText();
		}

		private void UpdateCoinsCountText()
		{
			this.coinsCountText.text = this.playerProfile.Coins.ToString();
		}
	}
}