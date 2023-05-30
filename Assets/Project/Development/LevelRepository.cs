using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Development
{
	public class LevelRepository: MonoBehaviour
	{
		public static LevelRepository Instance { get; private set; }
	
	
		private const string CurrentLevelKey = "CurrLvl";
		private const string LastLevelKey = "LastLvl";
		
		public const string LevelsKey = "levels";
		
		public int[] RSLevels { get; set; }
		
		[SerializeField] private string levels;
		public Dictionary<int, Level> Levels { get; set; }
		public List<Level> AllLevels => this.allLevels;
		[SerializeField] private List<Level> allLevels;
		
		public List<Level> BonusLevels => this.bonusLevels;
		[SerializeField] private List<Level> bonusLevels;

		public Level CurrentLevel
		{
			get
			{
				var mod = CurrentLevelNumber % Levels.Count;
				
				return Levels[mod != 0 ? mod : Levels.Count];
			}
		}

		public Level NextLevel
		{
			get
			{
				var mod = (CurrentLevelNumber + 1) % Levels.Count;
				
				return Levels[mod != 0 ? mod : Levels.Count];
			}
		}

		public Level LastLevel => Levels[this.LastLevelNumber];

		public int CurrentLevelNumber 
		{
			get => PlayerPrefs.GetInt(CurrentLevelKey, 1);
			set
			{
				PlayerPrefs.SetInt(CurrentLevelKey, value);
				PlayerPrefs.Save();
			}
		}
		
		public int LastLevelNumber 
		{
			get => PlayerPrefs.GetInt(LastLevelKey, 1);
			set
			{
				PlayerPrefs.SetInt(LastLevelKey, value);
				PlayerPrefs.Save();
			}
		}




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
			
			Init();
			
			InitLevels();
			
			RemoteSettings.Completed += RemoteSettingsOnCompleted;
		}

		private void OnDestroy()
		{
			RemoteSettings.Completed -= RemoteSettingsOnCompleted;
		}




		private void InitLevels()
		{
			Levels = new Dictionary<int, Level>();

			var lvlNum = 1;

			foreach (var level in this.RSLevels)
			{
				Levels.Add(lvlNum, this.allLevels.First(l => l.Name.Equals("Level" + level)));
				
				lvlNum++;
			}
		}
		
		private void RemoteSettingsOnCompleted(bool arg1, bool arg2, int arg3)
		{
			this.RSLevels = DeserializeLevels(RemoteSettings.GetString(LevelsKey, this.levels));
			
			InitLevels();
		}

		private void Init()
		{
			this.RSLevels = DeserializeLevels(this.levels);
		}

		private string SerializeLevels(int[] levels)
		{
			var str = this.levels.Aggregate(string.Empty, (current, level) => current + (level + ","));

			str = str.Remove(str.Length - 1);

			return str;
		}
		
		private int[] DeserializeLevels(string levels)
		{
			try
			{
				return levels.Split(',').Select(int.Parse).ToArray();
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
			
				return DeserializeLevels(this.levels);
			}
		}
	}
}