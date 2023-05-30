using System.Collections.Generic;
using UnityEngine;

namespace Project.Development.CharacterScripts
{
	public class InvisibilityComponent: MonoBehaviour
	{
		public float Alpha
		{
			get => this.effects.Alpha;
			set => this.effects.SetAlpha(value);
		}
	
		private Effects effects;
		
		



		private void Awake()
		{
			this.effects = GetComponent<Effects>();
		}
		



		public void SetInvisibility(bool invisible)
		{
			this.effects.SetRenderState(!invisible);
		}
	}
}