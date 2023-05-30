using Project.Development.CameraScripts;
using Project.Development.Events;
using Project.Development.ModeScripts;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class HUD: MonoBehaviour
	{
		[SerializeField] private GameObject coinsPanel;
		[SerializeField] private Joystick joystick;
		[SerializeField] private OffScreenIndicator offScreenIndicator;

		private Camera camera;

		private HideAndSeekGameMode hideAndSeekGameMode;



		private void Awake()
		{
			EventManager.OnStartMatch.AddListener(OnStartMatch);
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnLevelChanged.AddListener(OnLevelChanged);
			EventManager.OnGameModeInit.AddListener(OnGameModeInit);
			EventManager.OnExtraTimeStarted.AddListener(OnExtraTimeStarted);
			EventManager.OnMatchStarted.AddListener(OnMatchStarted);
			EventManager.OnHiderCaught.AddListener(OnHiderCaught);
			EventManager.OnHiderReleased.AddListener(OnHiderReleased);
			EventManager.OnHiderRescue.AddListener(OnHiderRescue);
		}

		private void Start()
		{
			this.joystick.enabled = false;
		}

		private void OnDestroy()
		{
			EventManager.OnStartMatch.RemoveListener(OnStartMatch);
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnLevelChanged.RemoveListener(OnLevelChanged);
			EventManager.OnGameModeInit.RemoveListener(OnGameModeInit);
			EventManager.OnExtraTimeStarted.RemoveListener(OnExtraTimeStarted);
			EventManager.OnMatchStarted.RemoveListener(OnMatchStarted);
			EventManager.OnHiderCaught.RemoveListener(OnHiderCaught);
			EventManager.OnHiderReleased.RemoveListener(OnHiderReleased);
			EventManager.OnHiderRescue.RemoveListener(OnHiderRescue);
		}
		

		
		private void OnTimerLeft()
		{
			this.joystick.enabled = false;
			this.coinsPanel.gameObject.SetActive(false);
		}
		
		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			this.joystick.enabled = false;
			this.coinsPanel.gameObject.SetActive(false);
		}
		
		private void OnStartMatch(GameMode gameMode)
		{
			this.joystick.ResetJoystick();
			this.joystick.enabled = true;
		}
		
		private void OnExtraTimeStarted(float time)
		{
			this.joystick.ResetJoystick();
			this.joystick.enabled = true;
			this.coinsPanel.gameObject.SetActive(true);
		}
		
		private void OnLevelChanged(Level newLevel, int levelNum, bool bonusLevel)
		{
			this.joystick.enabled = false;
			this.coinsPanel.gameObject.SetActive(true);
		}
		
		private void OnGameModeInit(GameMode gameMode)
		{
			if (gameMode is HideAndSeekGameMode hideAndSeekGameMode)
			{
				if (this.hideAndSeekGameMode != null)
				{
					this.hideAndSeekGameMode.OnTimerLeft -= OnTimerLeft;
				}

				this.hideAndSeekGameMode = hideAndSeekGameMode;

				this.hideAndSeekGameMode.OnTimerLeft += OnTimerLeft;
			}
		}
		
		private void OnMatchStarted(GameMode gameMode)
		{
			this.joystick.ResetJoystick();
			this.joystick.enabled = true;
		
			if (gameMode is HideAndSeekGameMode)
			{
				if (!this.hideAndSeekGameMode.Seeker.CompareTag("Player"))
				{
					if (this.camera == null)
					{
						this.camera = FindObjectOfType<CameraController>().GetComponent<Camera>();
					}

					var indicatorTarget = this.hideAndSeekGameMode.Seeker.gameObject.AddComponent<Target>();

					indicatorTarget.SetNeedArrowIndicator(true);

					this.offScreenIndicator.SetCamera(this.camera);
				}
			}
		}
		
		private void OnHiderRescue(Hider rescuer, Hider rescued)
		{
			if (rescued.CompareTag("Player"))
			{
				this.joystick.ResetJoystick();
				this.joystick.enabled = true;
			}
			
			this.coinsPanel.gameObject.SetActive(true);
		}

		private void OnHiderReleased(Hider hider)
		{
			if (hider.CompareTag("Player"))
			{
				this.joystick.ResetJoystick();
				this.joystick.enabled = true;
				this.coinsPanel.gameObject.SetActive(true);
			}
		}

		private void OnHiderCaught(Seeker seeker, Hider hider)
		{
			if (hider.CompareTag("Player"))
			{
				this.joystick.enabled = false;
				this.coinsPanel.gameObject.SetActive(false);
			}
		}
	}
}