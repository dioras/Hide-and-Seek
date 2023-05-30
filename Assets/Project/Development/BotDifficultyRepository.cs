using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Development
{
	[Serializable]
	public class HiderTimeToRunParam
	{
		public int FromLvl;
		public int ToLvl;
		public float TimeToRunToSeeker;
	}
	
	[Serializable]
	public class SeekerTimeToAvoidParam
	{
		public int FromLvl;
		public int ToLvl;
		public float TimeToAvoidPlayer;
	}

	public class BotDifficultyRepository: MonoBehaviour
	{
		public static BotDifficultyRepository Instance { get; private set; }


		[field: SerializeField] public int TimeToRunToSeekerAddRandomTime { get; set; }
		public List<HiderTimeToRunParam> HiderTimeToRunParams;
		public List<SeekerTimeToAvoidParam> SeekerTimeToAvoidParams;
		
		


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
	}
}