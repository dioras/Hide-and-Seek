using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Linq;
using Project.Development;
using Project.Development.SkinScripts;
using Project.Development.UI;
using UnityEngine.Events;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FortuneWheelManager : MonoBehaviour
{
	[Header("Game Objects for some elements")]
	public Button PaidTurnButton; 				// This button is showed when you can turn the wheel for coins
	public Button FreeTurnButton;				// This button is showed when you can turn the wheel for free
	public GameObject Circle; 					// Rotatable GameObject on scene with reward objects

	private bool _isStarted;					// Flag that the wheel is spinning

	[Header("Params for each sector")]
	public FortuneWheelSector[] Sectors;		// All sectors objects

	private float _finalAngle;					// The final angle is needed to calculate the reward
	private float _startAngle;    				// The first time start angle equals 0 but the next time it equals the last final angle
	private float _currentLerpRotationTime;		

	// Flag that player can turn the wheel for free right now
	private bool _isFreeTurnAvailable;

	private FortuneWheelSector _finalSector;
	[SerializeField] private MainMenuUI mainMenuUi;
	[SerializeField] private Button nextLevelButton;

	private PlayerProfile playerProfile;


	private void OnEnable()
	{
		HideTurnButtons();
	
		SetRandomSkinsForSkinSectors();

		TurnWheel();
		
		StartCoroutine(ShowFreeButtonWithDelay());
	}

	private void Awake ()
	{
		this.playerProfile = FindObjectOfType<PlayerProfile>();
	
		// Show sector reward value in text object if it's set
		foreach (var sector in Sectors)
		{
			if (sector.ValueText != null)
				sector.ValueText.text = sector.RewardValue.ToString();
		}
	}

	private void TurnWheel ()
	{
		SetNextLevelButtonActive(false);
		
		SetRandomSkinsForSkinSectors();
	
		_currentLerpRotationTime = 0f;

		// All sectors angles
		int[] sectorsAngles = new int[Sectors.Length];

		// Fill the necessary angles (for example if we want to have 12 sectors we need to fill the angles with 30 degrees step)
		// It's recommended to use the EVEN sectors count (2, 4, 6, 8, 10, 12, etc)
		for (int i = 1; i <= Sectors.Length; i++)
		{
			sectorsAngles[i - 1] =  360 / Sectors.Length * i;
		}

		//int cumulativeProbability = Sectors.Sum(sector => sector.Probability);

		double rndNumber = UnityEngine.Random.Range (1, Sectors.Sum(sector => sector.Probability));

		// Calculate the propability of each sector with respect to other sectors
		int cumulativeProbability = 0;
		// Random final sector accordingly to probability
		int randomFinalAngle = sectorsAngles [0];
		_finalSector = Sectors[0];

		for (int i = 0; i < Sectors.Length; i++) {
			cumulativeProbability += Sectors[i].Probability;

			if (rndNumber <= cumulativeProbability) {
				// Choose final sector
				randomFinalAngle = sectorsAngles [i];
				_finalSector = Sectors[i];
				break;
			}
		}

		int fullTurnovers = 5;

		// Set up how many turnovers our wheel should make before stop
		_finalAngle = fullTurnovers * 360 + randomFinalAngle - 90f - 18f;

		// Stop the wheel
		_isStarted = true;
	}

	public void TurnWheelButtonClick ()
	{
		TurnWheel();
	}

	public void TurnWheelForAdsButtonClick()
	{
		HideTurnButtons();
		
	}

	private void TurnWheelForAdsRewardAction()
	{
		TurnWheel();
	}

	private IEnumerator ShowFreeButtonWithDelay()
	{
		yield return new WaitForSeconds(3f);
		
		ShowFreeTurnButton();
	}

	public void SetNextLevelButtonActive(bool active)
	{
		this.nextLevelButton.gameObject.SetActive(active);
	}

	private void Update ()
	{
		if (!_isStarted)
			return;

		// Animation time
		float maxLerpRotationTime = 4f;

		// increment animation timer once per frame
		_currentLerpRotationTime += Time.deltaTime * 1.5f;

		// If the end of animation
		if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle) {
			_currentLerpRotationTime = maxLerpRotationTime;
			_isStarted = false;
			_startAngle = _finalAngle % 360;

			_finalSector.RewardCallback.Invoke();
		} else {
			// Calculate current position using linear interpolation
			float t = _currentLerpRotationTime / maxLerpRotationTime;

			// This formulae allows to speed up at start and speed down at the end of rotation.
			// Try to change this values to customize the speed
			t = t * t * t * (t * (6f * t - 15f) + 10f);

			float angle = Mathf.Lerp (_startAngle, _finalAngle, t);
			Circle.transform.eulerAngles = new Vector3 (0, 0, angle);	
		}
	}

	private void EnableButton (Button button)
	{
		button.interactable = true;
		button.GetComponent<Image> ().color = new Color(255, 255, 255, 1f);
	}

	private void DisableButton (Button button)
	{
		button.interactable = false;
		button.GetComponent<Image> ().color = new Color(255, 255, 255, 0.5f);
	}

	// Function for more readable calls
	private void EnableFreeTurnButton () { EnableButton (FreeTurnButton); }
	private void DisableFreeTurnButton () {	DisableButton (FreeTurnButton);	}
	private void EnablePaidTurnButton () { EnableButton (PaidTurnButton); }
	private void DisablePaidTurnButton () { DisableButton (PaidTurnButton); }

	private void ShowFreeTurnButton ()
	{
		FreeTurnButton.gameObject.SetActive(true); 
		PaidTurnButton.gameObject.SetActive(false);
	}

	private void ShowPaidTurnButton ()
	{
		PaidTurnButton.gameObject.SetActive(true); 
		FreeTurnButton.gameObject.SetActive(false);
	}

	private void HideTurnButtons()
	{
		PaidTurnButton.gameObject.SetActive(false); 
		FreeTurnButton.gameObject.SetActive(false);
	}

	private void SetRandomSkinsForSkinSectors()
	{
		for (var i = 0; i < this.Sectors.Length; i++)
		{
			var skins = SkinRepository.Instance.Skins.Where(s => 
				!this.playerProfile.SkinService.IsSkinInProfile(s.Name) && !s.ForAds
				&& this.Sectors.All(sec => sec.Skin != s)).ToList();
		
			if (this.Sectors[i].SkinImage != null && 
				(this.Sectors[i].Skin == null || this.playerProfile.SkinService.IsSkinInProfile(this.Sectors[i].Skin.Name)))
			{
				if (skins.Count > 0)
				{
					var randomSkin = skins[Random.Range(0, skins.Count)];

					SetSkinToSector(this.Sectors[i], randomSkin);
				}
				else
				{
					SetCoinsToSector(this.Sectors[i], 1000);
				}
			}
		}
	}

	private void SetSkinToSector(FortuneWheelSector sector, Skin skin)
	{
		sector.Skin = skin;
		sector.SkinImage.gameObject.SetActive(true);
		sector.CoinsImage.gameObject.SetActive(false);
		sector.ValueText.gameObject.SetActive(false);
		sector.RewardCallback.RemoveAllListeners();
					
		sector.RewardCallback.AddListener(() =>
		{
			this.playerProfile.SkinService.AddSkinInProfile(sector.Skin);
			SetNextLevelButtonActive(true);
		});
	}

	private void SetCoinsToSector(FortuneWheelSector sector, int coins)
	{
		sector.ValueText.text = coins.ToString();
		sector.SkinImage.gameObject.SetActive(false);
		sector.CoinsImage.gameObject.SetActive(true);
		sector.ValueText.gameObject.SetActive(true);
		sector.RewardCallback.RemoveAllListeners();

		sector.RewardCallback.AddListener(() =>
		{
			this.mainMenuUi.AddCoinsToBonusLevelCollectedCoins(coins);
			SetNextLevelButtonActive(true);
		});
	}
}

/**
 * One sector on the wheel
 */
[Serializable]
public class FortuneWheelSector : System.Object
{
	[Tooltip("Text object where value will be placed (not required)")]
	public Text ValueText;

	[Tooltip("Value of reward")]
	public string RewardValue = string.Empty;

	public Skin Skin
	{
		get => this.skin;
		set
		{
			this.skin = value;

			if (SkinImage != null)
			{
				SkinImage.sprite = this.skin.Sprite;
			}
		}
	}

	private Skin skin;

	public Image SkinImage;
	public Image CoinsImage;

	[Tooltip("Chance that this sector will be randomly selected")]
	[RangeAttribute(0, 100)]
	public int Probability = 100;

	[Tooltip("Method that will be invoked if this sector will be randomly selected")]
	public UnityEvent RewardCallback;
}