using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class DevUI: MonoBehaviour
	{
		[SerializeField] private InputField levelIF;



		public void Go()
		{
			if (int.TryParse(this.levelIF.text, out var level))
			{
				LevelRepository.Instance.CurrentLevelNumber = level;
			
				Spawner.Instance.ChangeLevel(LevelRepository.Instance.CurrentLevel, level);
			}
		}
	}
}