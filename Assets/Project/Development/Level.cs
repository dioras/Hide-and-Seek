using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Development
{
	[Serializable]
	public class LevelInfo
	{
		public int LevelNumber;
		public string ActivePerkName;
		public int IsActivePerkAvailable;
		public int IsActivePerkShown;
		public string ActivePerkPlace;
		public string ColorSchemeName;
		public string BackgroundMaterialName;
		public float Rotation;
	}

	[Serializable]
	public class LevelInfoWrapper
	{
		public List<LevelInfo> LevelsInfo;
	}
	

	[CreateAssetMenu(fileName = "New level", menuName = "Level", order = 51)]
	public class Level: ScriptableObject
	{
		public string Name => this.name;
		public GameObject Location => this.location;
		public float[] Rotations => this.rotations;
		
		[SerializeField] private string name;
		[SerializeField] private GameObject location;
		[SerializeField] private LevelColorScheme[] colorSchemes;
		[SerializeField] private float[] rotations;

		public LevelColorScheme ColorScheme { get; set; }
		public LevelColorScheme[] ColorSchemes => this.colorSchemes;
		public Material BackgroundMaterial { get; set; }
	}
}