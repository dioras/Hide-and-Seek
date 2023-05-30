using Project.Development.SkinScripts;

namespace Project.Development
{
	public interface IPlayerProfile
	{
		string Name { get; set; }
		Country Country { get; set; }
		int Coins { get; set; }
		ISkinService SkinService { get; set; }
		bool NeedEasyBots { get; set; }
	}
}