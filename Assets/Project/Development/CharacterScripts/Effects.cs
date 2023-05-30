using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Development.Events;
using Project.Development.ModeScripts;
using UnityEngine;

namespace Project.Development.CharacterScripts
{
	[RequireComponent(typeof(MovementController))]
	public class Effects: MonoBehaviour
	{
		private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
		private static readonly int Outline = Shader.PropertyToID("_Outline");
		private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");
		private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
	
		[SerializeField] private GameObject dustUnderfootPrefab;
		[SerializeField] private GameObject waterSplashPrefab;
		[SerializeField] private float waterSplashDuration;
		[SerializeField] private Footprints footprintsPrefab;
		[SerializeField] private float outlineWidth;
	
		public Footprints Footprints { get; private set; }
	
		private MovementController movementController;
		private InvisibilityComponent invisibilityComponent;
		private ParticleSystem waterSplashPS;

		private Vector3 lastVelocity;

		private bool needDustUnderfootCooldown;

		private float currentDustUnderfootCD;

		[SerializeField] private float dustUnderfootCdOnFirstLevels;
		private float dustUnderfootCD;
		private Coroutine waterSplashCoroutine;

		private bool prevInWater;
		public bool IsRendererEnabled { get; private set; }
		public float Alpha { get; private set; }
		public List<Renderer> Renderers { get; private set; }
		public List<Renderer> NicknameRenderers { get; set; }
		


		private void Awake()
		{
			this.movementController = GetComponent<MovementController>();
			this.invisibilityComponent = GetComponent<InvisibilityComponent>();
			this.Renderers = GetComponentsInChildren<Renderer>().ToList();
			
			AddSplashWaterEffect();
			AddFootprintsEffect();
			Alpha = 1f;
			IsRendererEnabled = true;
			
			EventManager.OnMatchStarted.AddListener(OnMatchStarted);
			EventManager.OnSkinChanged.AddListener(OnSkinChanged);
		}

		private void Update()
		{
			var velocity = this.movementController.Velocity;

			if (this.currentDustUnderfootCD > 0f)
			{
				this.currentDustUnderfootCD -= Time.deltaTime;
			}
			
			if (this.movementController.IsGrounded && velocity != Vector3.zero && this.lastVelocity == Vector3.zero)
			{
				if (this.movementController.InWater)
				{
					WaterSplash(this.waterSplashDuration);
				}
				else
				{
					DustUnderfoot();
				}
			}

			if (!this.prevInWater && this.movementController.InWater)
			{
				WaterSplash(this.waterSplashDuration);
			}

			if (this.movementController.InWater && Footprints.IsPlaying)
			{
				Footprints.Stop();
			}
		}

		private void LateUpdate()
		{
			this.lastVelocity = this.movementController.Velocity;
			this.prevInWater = this.movementController.InWater;
		}

		private void OnDestroy()
		{
			if(Footprints != null) Destroy(Footprints.gameObject);
			if(this.waterSplashPS != null) Destroy(this.waterSplashPS.gameObject);
		
			EventManager.OnMatchStarted.RemoveListener(OnMatchStarted);
			EventManager.OnSkinChanged.RemoveListener(OnSkinChanged);
		}


		public void SetAlpha(float alpha)
		{
			Alpha = alpha;
			
			SetBaseColorAlpha(alpha);
			SetOutlineAlpha(alpha);
		}

		public void SetRenderState(bool state)
		{
			foreach (var renderer in Renderers)
			{
				if (renderer == null)
				{
					continue;
				}
			
				renderer.enabled = state;
			}

			if (NicknameRenderers != null)
			{
				foreach (var renderer in NicknameRenderers)
				{
					if (renderer == null)
					{
						continue;
					}

					renderer.enabled = state;
				}
			}
		}

		public void SetBaseColorAlpha(float alpha)
		{
			foreach (var renderer in this.Renderers)
			{
				for (var i = 0; i < renderer.materials.Length; i++)
				{
					SetBaseColorAlphaToMaterial(renderer.materials[i], alpha);
				}
			}
		}

		public void SetOutlineAlpha(float alpha)
		{
			foreach (var renderer in this.Renderers)
			{
				for (var i = 0; i < renderer.materials.Length; i++)
				{
					SetOutlineAlphaToMaterial(renderer.materials[i], alpha);
				}
			}
		}

		public void SetOutlineWidth(float width)
		{
			foreach (var renderer in this.Renderers)
			{
				for (var i = 0; i < renderer.materials.Length; i++)
				{
					SetOutlineWidthToMaterial(renderer.materials[i], width);
				}
			}
		}

		public void SetDefaultOutlineWidth()
		{
			SetOutlineWidth(this.outlineWidth);
		}

		private void SetOutlineWidthToMaterial(Material material, float width)
		{
			if (material.HasProperty(OutlineWidth))
			{
				material.SetFloat(OutlineWidth, width);
			}

			if (material.HasProperty(Outline))
			{
				material.SetFloat(Outline, width * 0.25f);
			}
		}

		private void SetOutlineAlphaToMaterial(Material material, float alpha)
		{
			if (material.HasProperty(OutlineColor))
			{
				var color = material.GetColor(OutlineColor);

				color.a = alpha;

				material.SetColor(OutlineColor, color);
			}
		}
		
		private void SetBaseColorAlphaToMaterial(Material material, float alpha)
		{
			if (material.HasProperty(BaseColor))
			{
				var color = material.GetColor(BaseColor);

				color.a = alpha;

				material.SetColor(BaseColor, color);
			}
		}		

		private void DustUnderfoot()
		{
			if (this.needDustUnderfootCooldown && this.currentDustUnderfootCD > 0f)
			{
				return;
			}
			
			var dustUnderfoot = Instantiate(this.dustUnderfootPrefab, this.transform.position, Quaternion.identity);
			
			Destroy(dustUnderfoot, 3.0f);

			this.currentDustUnderfootCD = this.dustUnderfootCD;
		}

		private void AddSplashWaterEffect()
		{
			var waterSplash = Instantiate(this.waterSplashPrefab);

			this.waterSplashPS = waterSplash.GetComponent<ParticleSystem>();
			
			this.waterSplashPS.Stop();
		}

		public void AddFootprintsEffect()
		{
			Footprints = Instantiate(this.footprintsPrefab);
			
			Footprints.Stop();

			Footprints.Target = this.transform;
			Footprints.Offset += Vector3.up * 0.05f;
		}

		public void UnsetFootprints()
		{
			Footprints = null;
		}

		private void WaterSplash(float duration)
		{
			if (this.needDustUnderfootCooldown && this.currentDustUnderfootCD > 0f)
			{
				return;
			}
		
			if (this.waterSplashCoroutine != null)
			{
				StopCoroutine(this.waterSplashCoroutine);
			}
				
			this.waterSplashCoroutine = StartCoroutine(PlayWaterSplashEffect(duration));
			
			this.currentDustUnderfootCD = this.dustUnderfootCD;
		}

		private IEnumerator PlayWaterSplashEffect(float duration)
		{
			this.waterSplashPS.Play();

			while (duration > 0)
			{
				this.waterSplashPS.transform.position = this.transform.position + Vector3.up * 0.04f;
			
				duration -= Time.deltaTime;

				yield return null;
			}
			
			this.waterSplashPS.Stop();
		}
		
		
		private void OnMatchStarted(GameMode gameMode)
		{
			if (gameMode is HideAndSeekGameMode hideAndSeek)
			{
				this.needDustUnderfootCooldown = hideAndSeek.Seeker.CompareTag("Player") && !CompareTag("Player");

				if (this.needDustUnderfootCooldown)
				{
					var cd = EffectsRepository.Instance.LowLevelsDustUnderfootCD;

					if (LevelRepository.Instance.CurrentLevelNumber > 10)
					{
						cd = ((LevelRepository.Instance.CurrentLevelNumber - 10) / 290f)
						     * (EffectsRepository.Instance.HighLevelsDustUnderfootMaxCD -
						        EffectsRepository.Instance.HighLevelsDustUnderfootMinCD)
						     + EffectsRepository.Instance.HighLevelsDustUnderfootMinCD;
					}

					if (LevelRepository.Instance.CurrentLevelNumber <= 5)
					{
						cd = this.dustUnderfootCdOnFirstLevels;
					}

					this.dustUnderfootCD = cd;
				}
			}
		}
		
		private void OnSkinChanged(Character character, GameObject skin)
		{
			if (character.gameObject == this.gameObject)
			{
				this.Renderers = skin.GetComponentsInChildren<Renderer>().ToList();

				if (character.CompareTag("Player"))
				{
					foreach (var renderer in this.Renderers)
					{
						for (var i = 0; i < renderer.materials.Length; i++)
						{
							renderer.materials[i] = Instantiate(renderer.materials[i]);
							
							SetOutlineWidthToMaterial(renderer.materials[i], this.outlineWidth);
						}
					}
				}
			}
		}
	}
}