using System.Collections.Generic;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using Project.Development.LocatorScripts;
using UnityEngine;

namespace Project.Development.ModeScripts
{
	public class Seeker: MonoBehaviour
	{
		public List<Hider> Hiders { get; private set; }
		
		public InvisibilityComponent InvisibilityComponent { get; private set; }
		
		private TriangularLocator<Hider> triangularLocator;
		private RoundLocator<Hider> roundLocator;

		private TriangularFieldOfView triangularFieldOfView;
		private RoundFieldOfView roundFieldOfView;

		private Cage cagePrefab;
		private GameObject playerCatchEffectPrefab;
		private GameObject notPlayerCatchEffectPrefab;
		
	
	
		private void Awake()
		{
			AddLocators();
			AddFieldsOfView();
			
			Hiders = new List<Hider>();

			InvisibilityComponent = GetComponent<InvisibilityComponent>();
			this.cagePrefab = Resources.Load<Cage>("Props/Cage");
			this.playerCatchEffectPrefab = Resources.Load<GameObject>("VFX/DustDirtyPoofWithCoins");
			this.notPlayerCatchEffectPrefab = Resources.Load<GameObject>("VFX/DustDirtyPoof");
			
			EventManager.OnHiderRescue.AddListener(OnHiderRescue);
			EventManager.OnHiderReleased.AddListener(OnHiderReleased);
		}

		private void OnDestroy()
		{
			EventManager.OnHiderRescue.RemoveListener(OnHiderRescue);
			EventManager.OnHiderReleased.RemoveListener(OnHiderReleased);

			if(this.triangularLocator != null) Destroy(this.triangularLocator.gameObject);
			if(this.roundLocator != null) Destroy(this.roundLocator.gameObject);
			if(this.triangularFieldOfView != null) Destroy(this.triangularFieldOfView.gameObject);
			if(this.roundFieldOfView != null) Destroy(this.roundFieldOfView.gameObject);
		}




		public void Catch(Hider hider)
		{
			hider.Caught = true;
			
			hider.SetInvisibility(false);

			var cagePos = hider.transform.position;

			CatchEffect(cagePos + Vector3.up * 0.04f);

			var cage = Instantiate(this.cagePrefab, cagePos, this.cagePrefab.transform.rotation);
			
			cage.Put(this, hider);
			
			Hiders.Add(hider);
			
			EventManager.OnHiderCaught?.Invoke(this, hider);
		}

		public void SetActiveFieldsOfView(bool active)
		{
			if(this.triangularFieldOfView != null) this.triangularFieldOfView.SetActive(active);
			if(this.roundFieldOfView != null) this.roundFieldOfView.SetActive(active);
		}

		public void SetActiveLocators(bool active)
		{
			this.triangularLocator.SetActive(active);
			this.roundLocator.SetActive(active);
		}

		public void AddObjectToLocators(Hider hider)
		{
			this.roundLocator.Objects.Add(hider);
			this.triangularLocator.Objects.Add(hider);
		}

		public void SetObjectsToLocators(List<Hider> hiders)
		{
			this.roundLocator.Objects = hiders;
			this.triangularLocator.Objects = hiders;
		}



		private void AddLocators()
		{
			this.triangularLocator = this.gameObject.AddComponent<HiderTriangularLocator>();
			
			this.triangularLocator.SetActive(false);
			
			this.triangularLocator.Angle = 60f;
			this.triangularLocator.Range = 3f;
			this.triangularLocator.Rate = 0.2f;
			this.triangularLocator.LayerMask = ~(1 << LayerMask.NameToLayer("Character") 
			| 1 << LayerMask.NameToLayer("Cage") 
			| 1 << LayerMask.NameToLayer("Coin")
			| 1 << LayerMask.NameToLayer("InteractionCollider"));

			this.triangularLocator.OnLocatorFound += OnLocatorFound;
			
			this.roundLocator = this.gameObject.AddComponent<HiderRoundLocator>();
			
			this.roundLocator.SetActive(false);
			
			this.roundLocator.Rate = 0.2f;
			this.roundLocator.Radius = 1f;
			this.roundLocator.LayerMask = ~(1 << LayerMask.NameToLayer("Character") 
			| 1 << LayerMask.NameToLayer("Cage") 
			| 1 << LayerMask.NameToLayer("Coin")
			| 1 << LayerMask.NameToLayer("InteractionCollider"));
			
			this.roundLocator.OnLocatorFound += OnLocatorFound;
		}

		private void AddFieldsOfView()
		{
			var triangularFieldOfView = Resources.Load<TriangularFieldOfView>("TriangularFieldOfView");

			var fovColor = !CompareTag("Player")
				? new Color(1f, 0f, 0f, 0.50f)
				: new Color(1f, 1f, 1f, 0.25f);
			
			if (this.triangularFieldOfView == null)
			{
				this.triangularFieldOfView = Instantiate(triangularFieldOfView);

				this.triangularFieldOfView.Fov = this.triangularLocator.Angle;
				this.triangularFieldOfView.RayCount = 40;
				this.triangularFieldOfView.ViewDistance = this.triangularLocator.Range;
				this.triangularFieldOfView.Offset = new Vector3(0, 0.04f, 0f);
				this.triangularFieldOfView.SetColor(fovColor);
			}

			this.triangularFieldOfView.Owner = this.transform;
			
			this.triangularFieldOfView.SetActive(false);
			
			var roundFieldOfView = Resources.Load<RoundFieldOfView>("RoundFieldOfView");
			
			if (this.roundFieldOfView == null)
			{
				this.roundFieldOfView = Instantiate(roundFieldOfView);

				this.roundFieldOfView.RayCount = 100;
				this.roundFieldOfView.ViewDistance = this.roundLocator.Radius;
				this.roundFieldOfView.Angle = this.triangularLocator.Angle;
				this.roundFieldOfView.Offset = new Vector3(0, 0.04f, 0f);
				this.roundFieldOfView.SetColor(fovColor);
			}

			this.roundFieldOfView.Owner = this.transform;
		
			this.roundFieldOfView.SetActive(false);
		}

		private void CatchEffect(Vector3 pos)
		{
			var prefab = CompareTag("Player") ? this.playerCatchEffectPrefab : this.notPlayerCatchEffectPrefab;
		
			var effect = Instantiate(prefab, pos, prefab.transform.rotation);
			
			Destroy(effect, 3.0f);
		}
		
		
		private void OnLocatorFound(Locator<Hider> locator, List<Hider> hiders)
		{
			if (locator == this.triangularLocator || locator == this.roundLocator)
			{
				foreach (var hider in hiders)
				{
					if (hider != null && !hider.Caught && hider.InvisibilityComponent.Alpha >= 1f)
					{
						Catch(hider);
					}
				}
			}
		}
		
		private void OnHiderReleased(Hider hider)
		{
			Hiders.Remove(hider);
		}

		private void OnHiderRescue(Hider rescuer, Hider rescued)
		{
			Hiders.Remove(rescued);
		}
	}
}