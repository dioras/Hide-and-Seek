using System.Collections.Generic;
using System.Linq;
using Project.Development.CameraScripts;
using Project.Development.Events;
using Project.Development.ModeScripts;
using UnityEngine;

namespace Project.Development
{
	public class Firework: MonoBehaviour
	{
		[SerializeField] private GameObject fireworkPrefab;

		private List<GameObject> fireworks;
	
	
	
		private void Awake()
		{
			this.fireworks = new List<GameObject>();
		
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnChangeLevel.AddListener(OnChangeLevel);
		}

		private void OnDestroy()
		{
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnChangeLevel.RemoveListener(OnChangeLevel);
		}



		private void InstantiateFirework(Vector3 position, Quaternion rotation)
		{
			var firework = Instantiate(this.fireworkPrefab, position, rotation);
			
			this.fireworks.Add(firework);
			
			Destroy(firework, 5f);
		}

		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			if (result == Result.Win)
			{
				var cameraController = FindObjectOfType<CameraController>();
			
				cameraController.LerpCamera(Quaternion.Euler(60f, 0, 0), new Vector3(0, 17f, -8.5f), 1.0f);
			
				var player = gameMode.Players.Single(p => p.CompareTag("Player"));
				
				player.transform.rotation = Quaternion.Euler(0, 180, 0);
				
				InstantiateFirework(player.transform.position + player.transform.right * 1.5f, this.fireworkPrefab.transform.rotation);
				InstantiateFirework(player.transform.position - player.transform.right * 1.5f, this.fireworkPrefab.transform.rotation);
			}
		}
		
		private void OnChangeLevel(Level level, int levelNum, bool bonusLevel)
		{
			foreach (var firework in this.fireworks)
			{
				if (firework == null)
				{
					continue;
				}
				
				Destroy(firework);
			}
		}
	}
}