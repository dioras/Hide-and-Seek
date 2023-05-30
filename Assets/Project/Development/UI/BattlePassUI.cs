using System.Collections;
using Project.Development.SkinScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class BattlePassUI: MonoBehaviour
	{
		[SerializeField] private Image skinIcon;

		[SerializeField] private Image progressbarImage;
		[SerializeField] private GameObject parent;
		[SerializeField] private GameObject[] frames;
		[SerializeField] private GameObject skinRewardPanel;
		[SerializeField] private Image skinRewardIcon;
		[SerializeField] private MainMenuUI mainMenuUi;
		[SerializeField] private Button openRewardPanelButton;
		[SerializeField] private Button[] buttons;
		
		private PlayerProfile playerProfile;

		private Skin skinReward;



		private void Awake()
		{
			this.playerProfile = FindObjectOfType<PlayerProfile>();
		
			BattlePass.Instance.OnRewardEvent += OnReward;
		}

		private void Start()
		{
			this.progressbarImage.fillAmount = (float) BattlePass.Instance.CurrentWinCount / BattlePass.Instance.WinCountForSkinReward;
		}

		private void OnDisable()
		{
			this.skinReward = null;
			this.openRewardPanelButton.interactable = false;
		}

		private void OnDestroy()
		{
			BattlePass.Instance.OnRewardEvent -= OnReward;
		}




		public void Open()
		{
			if (!BattlePass.Instance.IsActive)
			{
				this.gameObject.SetActive(false);
				
				return;
			}
		
			UpdateProgressbar();
			SetSkinIcon(this.skinReward != null ? this.skinReward.Sprite : BattlePass.Instance.NextSkin.Sprite);
			UpdateFrame(BattlePass.Instance.WinCountForSkinReward);
			
			this.parent.SetActive(true);

			if (this.skinReward != null)
			{
				foreach (var nextLevelButton in this.buttons)
				{
					nextLevelButton.interactable = false;
				}
			
				this.openRewardPanelButton.interactable = true;
			
				StartCoroutine(OpenRewardPanelWithDelay());
			}
		}

		public void Hide()
		{
			this.skinReward = null;
			this.openRewardPanelButton.interactable = false;
		
			this.parent.SetActive(false);
		}

		public void OpenRewardPanel()
		{
			SetSkinRewardIcon(this.skinReward.Sprite);
		
			this.skinRewardPanel.SetActive(true);
		}

		public void HideSkinRewardPanel()
		{
			foreach (var nextLevelButton in this.buttons)
			{
				nextLevelButton.interactable = true;
			}
			
			this.skinRewardPanel.SetActive(false);
		}

		public void MissOutSkinReward()
		{
			HideSkinRewardPanel();
		}

		private void SkinRewardForAdsAction()
		{
			foreach (var nextLevelButton in this.buttons)
			{
				nextLevelButton.interactable = true;
			}
		
			HideSkinRewardPanel();
		
			this.playerProfile.SkinService.AddSkinInProfile(this.skinReward != null ? this.skinReward : BattlePass.Instance.NextSkin);
			this.playerProfile.SkinService.CurrentSkin = this.skinReward;

			this.mainMenuUi.ChangeLevel();
		}

		private IEnumerator OpenRewardPanelWithDelay()
		{
			yield return new WaitForSeconds(1.2f);
		
			OpenRewardPanel();
		}

		private void UpdateProgressbar()
		{
			StartCoroutine(AnimProgressbar(1f,
				this.skinReward != null
					? 1f
					: (float) BattlePass.Instance.CurrentWinCount / BattlePass.Instance.WinCountForSkinReward));
		}

		private IEnumerator AnimProgressbar(float time, float targetValue)
		{
			yield return new WaitForSeconds(0.2f);
		
			var t = time;

			var startValue = this.progressbarImage.fillAmount;

			if (targetValue < startValue)
			{
				startValue = 0f;
				
				this.progressbarImage.fillAmount = startValue;
			}

			while (t > 0)
			{
				var lerp = 1 - (t / time);

				this.progressbarImage.fillAmount = Mathf.Lerp(startValue, targetValue, lerp);

				t -= Time.deltaTime;

				yield return null;
			}

			this.progressbarImage.fillAmount = targetValue;
		}

		private void SetSkinIcon(Sprite icon)
		{
			this.skinIcon.sprite = icon;
			this.skinIcon.preserveAspect = true;
		}
		
		private void SetSkinRewardIcon(Sprite icon)
		{
			this.skinRewardIcon.sprite = icon;
			this.skinRewardIcon.preserveAspect = true;
		}

		private void UpdateFrame(int count)
		{
			if (this.skinReward != null)
			{
				return;
			}
			
			for (var i = 0; i < this.frames.Length; i++)
			{
				this.frames[i].SetActive(i == count - 2);
			}
		}
		
		private void OnReward(Skin skin)
		{
			this.skinReward = skin;
		}
	}
}