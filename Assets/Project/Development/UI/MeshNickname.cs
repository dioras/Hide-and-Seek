using TMPro;
using UnityEngine;

namespace Project.Development.UI
{
	public class MeshNickname: MonoBehaviour
	{
		[SerializeField] private TextMeshPro textMesh;
		
		public Transform Target { get; set; }
		public Vector3 Offset { get; set; }




		private void LateUpdate()
		{
			if (this.Target != null)
			{
				this.transform.position = this.Target.position + this.Offset;
			}
		}




		public void SetNickname(string text)
		{
			this.textMesh.text = text;
		}

		public void SetActive(bool active)
		{
			this.gameObject.SetActive(active);
		}
	}
}