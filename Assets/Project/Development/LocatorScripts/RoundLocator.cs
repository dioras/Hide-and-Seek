using UnityEngine;

namespace Project.Development.LocatorScripts
{
	public class RoundLocator<T>: Locator<T>
	{
		public float Radius { get; set; }
	
	
	
	
		public override bool Find(T t)
		{
			var comp = t as Component;
		
			var dist = ((comp.transform.position + Vector3.up) - (this.transform.position + Vector3.up)).magnitude;
			
			var dir = ((comp.transform.position + Vector3.up) - (this.transform.position + Vector3.up)).normalized;
			
			var obstacle = Physics.Raycast(this.transform.position + Vector3.up, dir, dist, this.LayerMask);

			return dist <= this.Radius && !obstacle;
		}
	}
}