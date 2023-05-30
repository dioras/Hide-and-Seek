using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Development.Perks
{
	[RequireComponent(typeof(Collider))]
	public class ThroughWalls: MonoBehaviour
	{
		private List<Collider> wallsCollider;

		private new Collider collider;




		private void OnDisable()
		{
			foreach (var wallCollider in this.wallsCollider)
			{
				Physics.IgnoreCollision(this.collider, wallCollider, false);
			}
			
			PopOutIfObjectIsInsideWall();
		}

		private void Awake()
		{
			this.collider = GetComponent<Collider>();

			this.wallsCollider = GameObject.FindGameObjectsWithTag("Wall").Select(o => o.GetComponent<Collider>())
				.ToList();

			this.wallsCollider.AddRange(GameObject.FindGameObjectsWithTag("Bridge")
				.Select(o => o.GetComponent<Collider>())
				.ToList());
		}



		private void OnCollisionEnter(Collision other)
		{
			if (!this.isActiveAndEnabled)
			{
				return;
			}
		
			if (this.wallsCollider.Contains(other.collider))
			{
				if (other.collider.CompareTag("Bridge"))
				{
					if (other.contacts[0].normal.y < 0.1f && other.contacts[0].normal.y > -0.1f)
					{
						Physics.IgnoreCollision(this.collider, other.collider, true);
					}
				}
				else
				{
					Physics.IgnoreCollision(this.collider, other.collider, true);
				}
			}
		}

		private void OnCollisionExit(Collision other)
		{
			if (!this.isActiveAndEnabled)
			{
				return;
			}
		
			if (this.wallsCollider.Contains(other.collider) && other.collider.CompareTag("Bridge"))
			{
				StartCoroutine(IgnoreCollisionWithDelay(other.collider, false, 0.2f));
			}
		}


		private IEnumerator IgnoreCollisionWithDelay(Collider collider, bool ignore, float delay)
		{
			yield return new  WaitForSeconds(delay);
			
			Physics.IgnoreCollision(this.collider, collider, ignore);
		}

		private void PopOutIfObjectIsInsideWall()
		{
			Physics.Raycast(this.collider.transform.position + Vector3.up * 0.2f,
				this.collider.transform.forward, out var fwd, 100f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Cage"));
			Physics.Raycast(this.collider.transform.position + Vector3.up * 0.2f,
				-this.collider.transform.forward, out var bwd, 100f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Cage"));
			Physics.Raycast(this.collider.transform.position + Vector3.up * 0.2f,
				this.collider.transform.right, out var rgt, 100f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Cage"));
			Physics.Raycast(this.collider.transform.position + Vector3.up * 0.2f,
				-this.collider.transform.right, out var lft, 100f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Cage"));
			Physics.Raycast(this.collider.transform.position,
				this.collider.transform.up, out var up, 3f, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Cage"));

			if (up.transform != null)
			{
				if (fwd.transform == bwd.transform == rgt.transform == lft.transform)
				{
					var hits = new List<RaycastHit>();

					if (fwd.transform != null && this.wallsCollider.Any(c => c.transform == fwd.transform))
					{
						hits.Add(fwd);
					}

					if (bwd.transform != null && this.wallsCollider.Any(c => c.transform == bwd.transform))
					{
						hits.Add(bwd);
					}

					if (rgt.transform != null && this.wallsCollider.Any(c => c.transform == rgt.transform))
					{
						hits.Add(rgt);
					}

					if (lft.transform != null && this.wallsCollider.Any(c => c.transform == lft.transform))
					{
						hits.Add(lft);
					}

					if (hits.Count > 0)
					{
						var minDist = hits.Min(hit => hit.distance);
						var closest = hits.FirstOrDefault(h => h.distance == minDist);

						this.collider.transform.position = closest.point;
					}
				}
			}
		}
	}
}