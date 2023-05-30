using Project.Development.CharacterScripts;
using UnityEngine;

namespace Project.Development
{
	public class Puddle: MonoBehaviour
	{
		[field:SerializeField]
		public Material FootprintsMaterial { get; set; }
		[field:SerializeField]
		public float FootprintsDuration { get; set; }

		public Material Material
		{
			get
			{
				if (this.renderer == null)
				{
					this.renderer = GetComponent<Renderer>();
				}
				
				return this.renderer.material;
			}
			set
			{
				if (this.renderer == null)
				{
					this.renderer = GetComponent<Renderer>();
				}
				
				this.renderer.material = value;
			}
		}

		private new Renderer renderer;




		private void Awake()
		{
			this.renderer = GetComponent<Renderer>();
		}




		private void OnTriggerEnter(Collider other)
		{
			var effects = other.GetComponent<Effects>();

			if (effects != null)
			{
				if (effects.Footprints != null)
				{
					Destroy(effects.Footprints.gameObject, 10f);
					effects.Footprints.Stop();
					effects.UnsetFootprints();
				}

				if (effects.Footprints == null)
				{
					effects.AddFootprintsEffect();
				}

				effects.Footprints.Play(FootprintsDuration);
				effects.Footprints.Material = FootprintsMaterial;
			}
		}
	}
}