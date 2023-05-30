using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class BackgroundProgressUI : MonoBehaviour
	{
		[SerializeField] private Image background;
		[SerializeField] private Image progress;
		[SerializeField] private Image backgroundIcon;
		private int fromLevel;
		private int toLevel;
		[SerializeField] private Color notAvailableLevelBackgroundColor;
		[SerializeField] private Color availableLevelBackgroundColor;




		private void OnEnable()
		{
			if (this.background != null)
			{
				if (LevelRepository.Instance.CurrentLevelNumber >= this.fromLevel)
				{
					this.background.color = this.availableLevelBackgroundColor;
				
					if (this.progress != null)
					{
						this.progress.fillAmount = (float) (LevelRepository.Instance.CurrentLevelNumber - this.fromLevel) / (this.toLevel - this.fromLevel);
					}
				}
				else
				{
					this.background.color = this.notAvailableLevelBackgroundColor;
				
					if (this.progress != null)
					{
						this.progress.fillAmount = 0;
					}
				}
			}
		}
		


		public void SetBackgroundIcon(Sprite icon)
		{
			if (icon == null)
			{
				return;
			}
		
			this.backgroundIcon.sprite = icon;
			this.backgroundIcon.preserveAspect = true;
		}

		public void SetFromLevel(int fromLevel)
		{
			this.fromLevel = fromLevel;
		}

		public void SetToLevel(int toLevel)
		{
			this.toLevel = toLevel;
		}
	}
}
