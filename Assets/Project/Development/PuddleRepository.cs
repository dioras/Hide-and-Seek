using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Development
{
	[Serializable]
	public class PuddleFootstep
	{
		[field: SerializeField]
		public Material PuddleMaterial { get; set; }
		[field: SerializeField]
		public Material FootstepMaterial { get; set; }
	}

	public class PuddleRepository: MonoBehaviour
	{
		public static PuddleRepository Instance { get; private set; }


		[SerializeField] private PuddleFootstep[] puddlesFootsteps;




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




		public PuddleFootstep GetRandomPuddleFootstep()
		{
			return this.puddlesFootsteps[Random.Range(0, this.puddlesFootsteps.Length)];
		}
	}
}