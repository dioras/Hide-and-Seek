using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Development.LocatorScripts
{
	public class TriangularLocator<T>: Locator<T>
	{
		public float Angle { get; set; }
		public float Range { get; set; }



		public override bool Find(T t)
		{
			var comp = t as Component;
		
			var dir = ((comp.transform.position + Vector3.up) - (this.transform.position + Vector3.up)).normalized;
			
			var dist = ((comp.transform.position + Vector3.up) - (this.transform.position + Vector3.up)).magnitude;

			var obstacle = Physics.Raycast(this.transform.position + Vector3.up, dir, dist, this.LayerMask);

			var angleBetween = Vector3.Angle(dir, this.transform.forward);

			var inLocator = angleBetween <= Angle * 0.5f;

			return inLocator && !obstacle && dist <= this.Range;
		}
	}
}