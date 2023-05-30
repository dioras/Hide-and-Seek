using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Development
{
	[Serializable]
	public class LevelBackground
	{
		public int FromLevel;
		public int ToLevel;
		public GameObject BackgroundPrefab;
		public Sprite Icon;
	}

	public class Backgrounds: MonoBehaviour
	{
		public static Backgrounds Instance { get; private set; }

		public List<LevelBackground> LevelsBackgrounds => this.levelsBackgrounds;
	
		[SerializeField] private List<LevelBackground> levelsBackgrounds;
		
		
		
		
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



		public GameObject GetBackgroundByLevel(int level)
		{
			return this.levelsBackgrounds.FirstOrDefault(l => level >= l.FromLevel && level <= l.ToLevel)?
				.BackgroundPrefab;
		}
	}
}