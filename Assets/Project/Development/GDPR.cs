using System;
using UnityEngine;

namespace Project.Development
{
	public class GDPR: MonoBehaviour
	{
		public const string GDPRAcceptedKey = "gdpr_accepted";
	
		public static GDPR Instance { get; private set; }
		
		public bool IsGDPRAccepted
		{
			get => Convert.ToBoolean(PlayerPrefs.GetInt(GDPRAcceptedKey, 0));
			set
			{
				PlayerPrefs.SetInt(GDPRAcceptedKey, Convert.ToInt32(value));
				PlayerPrefs.Save();
			}
		}

		[SerializeField] private GameObject gdprWindow;
		
		
		
		
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
		}




		public void SetActiveWindow(bool state)
		{
			this.gdprWindow.SetActive(state);
		}

		public void OnClickAcceptButton()
		{
			AcceptGDPR();
			SetActiveWindow(false);
		}

		public void OpenPrivacyPolicyLink()
		{
			Application.OpenURL("https://privacy.azurgames.com/");
		}

		public void AcceptGDPR()
		{
			if (IsGDPRAccepted)
			{
				return;
			}

			IsGDPRAccepted = true;
		}
	}
}