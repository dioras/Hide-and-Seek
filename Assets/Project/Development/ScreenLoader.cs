using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development
{
	public class ScreenLoader: MonoBehaviour
	{
		[SerializeField] private Image progressbar;
		[SerializeField] private float loadingTime;




		private void Start()
		{
			StartCoroutine(LoadingProcess(this.loadingTime));
		}




		private IEnumerator LoadingProcess(float loadingTime)
		{
			var startLoadingTime = loadingTime;
		
			while (loadingTime > 0)
			{
				this.progressbar.fillAmount = 1 - loadingTime / startLoadingTime;

				loadingTime -= Time.deltaTime;

				yield return null;
			}
			
			this.gameObject.SetActive(false);
		}
	}
}