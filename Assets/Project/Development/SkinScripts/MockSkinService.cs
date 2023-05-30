using System;

namespace Project.Development.SkinScripts
{
	public class MockSkinService: ISkinService
	{
		public Action<Skin> OnSkinAdded { get; set; }
		public Skin CurrentSkin { get; set; }
		
		
		public int GetSkinsInProfileAmount()
		{
			throw new System.NotImplementedException();
		}

		public bool IsSkinInProfile(string skin)
		{
			throw new System.NotImplementedException();
		}

		public bool IsSkinInProfile(Skin skin)
		{
			throw new System.NotImplementedException();
		}

		public void BuySkin(string skin)
		{
			throw new System.NotImplementedException();
		}

		public void RemoveSkin(string skin)
		{
			throw new System.NotImplementedException();
		}

		public string[] GetBoughtSkins()
		{
			throw new System.NotImplementedException();
		}

		public void AddSkinInProfile(string skin)
		{
			throw new System.NotImplementedException();
		}

		public void AddSkinInProfile(Skin skin)
		{
			throw new System.NotImplementedException();
		}

		public string GetSkinsInProfileString()
		{
			throw new System.NotImplementedException();
		}
	}
}