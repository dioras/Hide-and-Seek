using Project.Development.SkinScripts;
using UnityEngine;

namespace Project.Development
{
	public class MockPlayerProfile: MonoBehaviour, IPlayerProfile
	{
		public string Name { get; set; }
		public Country Country { get; set; }
		
		public int Coins { get; set; }
		public ISkinService SkinService { get; set; }
		public bool NeedEasyBots { get; set; }
	}
}