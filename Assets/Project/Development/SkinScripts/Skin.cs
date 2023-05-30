using UnityEngine;

namespace Project.Development.SkinScripts
{
	[CreateAssetMenu(fileName = "New skin", menuName = "Skin", order = 52)]
	public class Skin: ScriptableObject
	{
		[field: SerializeField] public string Name { get; set; }
		[field: SerializeField] public GameObject SkinPrefab { get; set; }
		[field: SerializeField] public Sprite Sprite { get; set; }
		[field: SerializeField] public int Price { get; set; }
		[field: SerializeField] public bool ForAds { get; set; }
		[field: SerializeField] public bool AvailableInShop { get; set; }


		public void UpdateData(SkinData skinData)
		{
			if (!skinData.Name.Equals(Name))
			{	
				return;
			}

			ForAds = skinData.ForAds;
			AvailableInShop = skinData.AvailableInShop;
		}
	}
}