using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class SettingsUI: MonoBehaviour
	{
		[SerializeField] private InputField nicknameIF;

		private PlayerProfile playerProfile;

		private Coroutine nicknameEditCoroutine;




		private void OnEnable()
		{
			this.nicknameIF.text = this.playerProfile.Name;
		}

		private void Awake()
		{
			this.playerProfile = FindObjectOfType<PlayerProfile>();
			
			this.nicknameIF.onEndEdit.AddListener(onNicknameEndEdit);
		}

		private void OnDestroy()
		{
			this.nicknameIF.onEndEdit.RemoveListener(onNicknameEndEdit);
		}




		public void OnClickPenButton()
		{
			this.nicknameIF.interactable = true;
			this.nicknameIF.Select();
		}


		private void onNicknameEndEdit(string value)
		{
			if (this.nicknameEditCoroutine != null)
			{
				return;
			}
		
			this.nicknameEditCoroutine = StartCoroutine(NicknameEditProcess());

			this.playerProfile.Name = value;
		}

		private IEnumerator NicknameEditProcess()
		{
			this.nicknameIF.DeactivateInputField();
		
			yield return null;
			
			this.nicknameIF.interactable = false;

			this.nicknameEditCoroutine = null;
		}
	}
}