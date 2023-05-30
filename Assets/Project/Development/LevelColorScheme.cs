using UnityEngine;

namespace Project.Development
{
	[CreateAssetMenu(fileName = "New color scheme", menuName = "LevelColorScheme", order = 55)]
	public class LevelColorScheme: ScriptableObject
	{
		public Material[] FloorMaterials;
		public Material[] WallMaterials;
		public Material[] BridgeMaterials;
		public Material[] BackgroundMaterials;
	}
}