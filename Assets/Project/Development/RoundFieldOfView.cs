using UnityEngine;

namespace Project.Development
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class RoundFieldOfView: MonoBehaviour
	{
		private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
	
		[field: SerializeField]
		public int RayCount { get; set; }
		[field: SerializeField]
		public float ViewDistance { get; set; }
		[field: SerializeField]
		public Transform Owner { get; set; }
		[field: SerializeField]
		public Vector3 Offset { get; set; }
		[field: SerializeField]
		public float Angle { get; set; }
		[field: SerializeField]
		public LayerMask LayerMask { get; set; }
		
		private Mesh mesh;
		private MeshFilter meshFilter;
		private new Renderer renderer;
		


		private void Awake()
		{
			this.meshFilter = this.gameObject.GetComponent<MeshFilter>();
			this.renderer = GetComponent<Renderer>();

			this.renderer.material = Instantiate(this.renderer.material);
		}

		private void Start()
		{
			this.mesh = new Mesh();

			this.meshFilter.mesh = mesh;
		}

		private void LateUpdate()
		{
			if (this.Owner == null)
			{
				return;
			}
		
			var origin = this.Owner.position + this.Offset;

			var angle = this.Angle;
			
			var angleIncrease = (360 - angle) / this.RayCount;
			
			var vertices = new Vector3[this.RayCount + 1 + 1];

			var uv = new Vector2[vertices.Length];

			var triangles = new int[this.RayCount * 3];

			vertices[0] = origin;

			var vertexIdx = 1;
			var triangleIdx = 0;
			
			for (var i = 0; i <= this.RayCount; i++)
			{
				Vector3 vertex;

				var dir = this.Owner.TransformDirection(GetVectorFromAngle(angle));

				if (Physics.Raycast(origin + Vector3.up * 0.3f, dir, out var hit, this.ViewDistance, this.LayerMask))
				{
					vertex = new Vector3(hit.point.x, origin.y, hit.point.z);
				}
				else
				{
					vertex = origin + dir * this.ViewDistance;
				}
				
				vertices[vertexIdx] = vertex;

				if (i > 0)
				{
					triangles[triangleIdx] = 0;
					triangles[triangleIdx + 1] = vertexIdx - 1;
					triangles[triangleIdx + 2] = vertexIdx;

					triangleIdx += 3;
				}

				vertexIdx++;

				angle -= angleIncrease;
			}

			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			
			mesh.RecalculateBounds();
		}



		public void SetColor(Color color)
		{
			this.renderer.material.SetColor(BaseColor, color);
		}

		public void SetActive(bool active)
		{
			this.gameObject.SetActive(active);
		}


		private Vector3 GetVectorFromAngle(float angle)
		{
			var angleRad = angle * (Mathf.PI / 180);
			
			return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
		}
	}
}