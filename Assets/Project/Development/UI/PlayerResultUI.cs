using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class PlayerResultUI: MonoBehaviour
	{
		[SerializeField] 
		private TextMeshProUGUI nicknameText;
		[SerializeField] 
		private Image countryImage;
		[SerializeField] 
		private Image backgroundImage;

		[SerializeField] private Sprite loseBackgroundSprite;
		[SerializeField] private Sprite winBackgroundSprite;
		



		public void SetNickname(string nick)
		{
			this.nicknameText.text = nick;
		}

		public void SetCountrySprite(Sprite sprite)
		{
			this.countryImage.sprite = sprite;
		}

		public void SetResult(bool win)
		{
			this.backgroundImage.sprite = win ? this.winBackgroundSprite : this.loseBackgroundSprite;
		}
	}
}