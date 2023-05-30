using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Development
{
	[Serializable]
	public class CountrySprite
	{
		[field:SerializeField]
		public Country Country { get; set; }
		[field:SerializeField]
		public Sprite Sprite { get; set; }
	}

	public class SpriteRepository: MonoBehaviour
	{
		public static SpriteRepository Instance { get; private set; }


		[SerializeField] private List<CountrySprite> countrySprites;


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
		}



		public Sprite GetCountrySprite(Country country)
		{
			return this.countrySprites.FirstOrDefault(cs => cs.Country == country)?.Sprite;
		}
	}
}