using UnityEngine;

namespace Project.Development.UI
{
	public class TalkMesh: MonoBehaviour
	{
		public Transform Target { get; set; }
		[field:SerializeField]
		public Vector3 Offset { get; set; }




		private void LateUpdate()
		{
			if (this.Target != null)
			{
				this.transform.position = this.Target.position + this.Offset;
			}
		}
	}
}