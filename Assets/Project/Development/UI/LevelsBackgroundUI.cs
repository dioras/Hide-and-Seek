using UnityEngine;

namespace Project.Development.UI
{
	public class LevelsBackgroundUI: MonoBehaviour
	{
		[SerializeField] private BackgroundProgressUI[] backgroundsProgress;




		private void Awake()
		{
			for (var i = 0; i < this.backgroundsProgress.Length; i++)
			{
				SetBackground(Backgrounds.Instance.LevelsBackgrounds[i], this.backgroundsProgress[i]);
			}
		}
		
		


		private void SetBackground(LevelBackground levelBackground, BackgroundProgressUI backgroundProgress)
		{
			backgroundProgress.SetBackgroundIcon(levelBackground.Icon);
			backgroundProgress.SetFromLevel(levelBackground.FromLevel);
			backgroundProgress.SetToLevel(levelBackground.ToLevel);
		}
	}
}