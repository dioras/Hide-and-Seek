using UnityEngine.Events;

namespace Project.Development.Events
{
	public static class EventManager
	{
		public static SceneLoadedEvent OnSceneLoaded { get; set; }
		public static SceneLoadEvent OnSceneLoad { get; set; }
		public static StartMatchEvent OnStartMatch { get; set; }
		public static StartMatchEvent OnMatchStarted { get; set; }
		public static FinishMatchEvent OnMatchFinished { get; set; }
		public static GameModeInitEvent OnGameModeInit { get; set; }
		public static CatchEvent OnHiderCaught { get; set; }
		public static PickupCoinEvent OnCoinPickuped { get; set; }
		public static UnityEvent OnRemoteSettingCompleted { get; set; }
		/// <summary>
		/// Called when hider was rescued (params: newLevel, levelNumb, bonusLevel?))
		/// if newLevel = null then it's restart
		/// </summary>
		public static ChangeLevelEvent OnChangeLevel { get; set; }
		/// <summary>
		/// Called when hider was rescued (params: newLevel, levelNumb, bonusLevel?))
		/// if newLevel = null then it's restart
		/// </summary>
		public static ChangeLevelEvent OnLevelChanged { get; set; }
		public static ExtraTimeEvent OnExtraTimeStarted { get; set; }
		public static ReleaseHiderEvent OnHiderReleased { get; set; } 
		/// <summary>
		/// Called when hider was rescued (params: rescuer, rescued)
		/// </summary>
		public static HiderRescueEvent OnHiderRescue { get; set; }
		public static ChangeSkinEvent OnSkinChanged { get; set; }
		public static SelectSkinEvent OnSkinSelected { get; set; }
		public static UnityEvent OnClickNextLevelButton { get; set; }
		public static UnityEvent OnClickPlayAgainButton { get; set; }
		public static BuySkinEvent OnSkinBought { get; set; }
		/// <summary>
		/// Called on purchase (params: revenue, currency, contentId)
		/// </summary>
		public static PurchaseEvent OnPurchase { get; set; }
		/// <summary>
		/// Called when not enough coins for purchase.
		/// </summary>
		public static UnityEvent OnNotEnoughCoins { get; set; }
		public static PickupActivePerkEvent OnActivePerkPickedUp { get; set; }
	
	
		static EventManager()
		{
			OnSceneLoaded = new SceneLoadedEvent();
			OnSceneLoad = new SceneLoadEvent();
			OnStartMatch = new StartMatchEvent();
			OnMatchStarted = new StartMatchEvent();
			OnMatchFinished = new FinishMatchEvent();
			OnGameModeInit = new GameModeInitEvent();
			OnHiderCaught = new CatchEvent();
			OnCoinPickuped = new PickupCoinEvent();
			OnRemoteSettingCompleted = new UnityEvent();
			OnLevelChanged = new ChangeLevelEvent();
			OnExtraTimeStarted = new ExtraTimeEvent();
			OnChangeLevel = new ChangeLevelEvent();
			OnHiderReleased = new ReleaseHiderEvent();
			OnHiderRescue = new HiderRescueEvent();
			OnSkinChanged = new ChangeSkinEvent();
			OnSkinSelected = new SelectSkinEvent();
			OnClickNextLevelButton = new UnityEvent();
			OnClickPlayAgainButton = new UnityEvent();
			OnSkinBought = new BuySkinEvent();
			OnPurchase = new PurchaseEvent();
			OnNotEnoughCoins = new UnityEvent();
			OnActivePerkPickedUp = new PickupActivePerkEvent();
		}
	}
}