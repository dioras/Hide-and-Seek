using System.Linq;
using UnityEngine;

namespace Project.Development.SkinScripts
{
	public class SkinRepository: MonoBehaviour
	{
		public static SkinRepository Instance { get; private set; }

		public const string SkinsDataKey = "skinsdata";


		public Skin[] DefaultSkins => this.defaultSkins;
		public Skin[] Skins => this.skins;
	
		[SerializeField] private Skin[] skins;
		[SerializeField] private Skin[] defaultSkins;
		
		private SkinsData skinsData;
		
		
		
		
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

			this.skinsData = LoadData();
			
			foreach (var skin in Skins)
			{
				var skinData = this.skinsData.Skins.SingleOrDefault(s => s.Name.Equals(skin.Name));

				if (skinData == null)
				{
					continue;
				}
				
				skin.UpdateData(skinData);
			}
		}
		
		private void OnApplicationQuit()
		{
			SaveData();
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				SaveData();
			}
		}




		public void SaveData()
		{
			var json = JsonUtility.ToJson(this.skinsData);
		
			PlayerPrefs.SetString(SkinsDataKey, json);
			PlayerPrefs.Save();
		}
		
		private SkinsData LoadData()
		{
			SkinsData data;
		
			if (PlayerPrefs.HasKey(SkinsDataKey))
			{
				data = JsonUtility.FromJson<SkinsData>(PlayerPrefs.GetString(SkinsDataKey));
			}
			else
			{
				data = new SkinsData();
			}
			
			return data;
		}


		public void AddOrUpdateSkin(Skin skin)
		{
			var skinData = this.skinsData.Skins.SingleOrDefault(s => s.Name.Equals(skin.Name));
		
			if (skinData == null)
			{
				skinData = new SkinData
				{
					Name = skin.Name
				};

				this.skinsData.Skins.Add(skinData);
			}
			
			skinData.ForAds = skin.ForAds;
			skinData.AvailableInShop = skin.AvailableInShop;
		}
	}
}