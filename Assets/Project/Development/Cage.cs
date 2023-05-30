using Project.Development.BotScripts;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using Project.Development.ModeScripts;
using Project.Development.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Development
{
	public class Cage: MonoBehaviour
	{
		[SerializeField] private Transform parent;
	
		public Hider Hider { get; private set; }

		public bool InWater { get; private set; }

		[SerializeField] private TalkMesh talkMeshPrefab;
		private new Rigidbody rigidbody;

		private TalkMesh talkMesh;




		private void Awake()
		{
			this.rigidbody = GetComponent<Rigidbody>();
		}

		private void FixedUpdate()
		{
			InWater = false;
		}
		
		private void OnTriggerStay(Collider other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
			{
				InWater = true;
			}
		}

		private void Update()
		{
			var meshZ = this.parent.localPosition.z;

			meshZ = Mathf.MoveTowards(meshZ, InWater ? -0.0082f : 0f, Time.deltaTime * 0.05f);
            
			this.parent.localPosition = new Vector3(this.parent.localPosition.x, this.parent.localPosition.y, meshZ);
		}

		private void OnDestroy()
		{
			if (this.talkMesh != null)
			{
				Destroy(this.talkMesh.gameObject);
			}
		}


		public void Put(Seeker seeker, Hider hider)
		{
			var character = hider.GetComponent<Character>();

			character.InCage = true;
		
			var navMeshObstacle = hider.GetComponent<NavMeshObstacle>();

			if (!hider.CompareTag("Player"))
			{
				navMeshObstacle.enabled = true;
			}
			
			foreach (var col in hider.GetComponentsInChildren<Collider>())
			{
				col.enabled = false;
			}

			if (Spawner.Instance.CharacterMeshNicknames.ContainsKey(hider.gameObject))
			{
				Spawner.Instance.CharacterMeshNicknames[hider.gameObject].SetActive(false);
			}

			if (!seeker.CompareTag("Player"))
			{
				this.talkMesh = Instantiate(this.talkMeshPrefab, this.transform.position,
					this.talkMeshPrefab.transform.rotation);

				this.talkMesh.Target = this.transform;
			}

			character.MovementController.enabled = false;
			character.MovementController.SetKinematic(true);

			var botController = hider.GetComponent<BotController>();

			if (botController != null)
			{
				botController.enabled = false;
				botController.NavMeshAgent.enabled = false;
			}
		
			hider.transform.SetParent(this.parent);
			
			hider.transform.localPosition = Vector3.zero;

			this.Hider = hider;
		}
		
		public Hider Open(Hider rescuer)
		{
			if (this.Hider == null)
			{
				return null;
			}
		
			this.Hider.transform.SetParent(null);

			this.Hider.transform.position = this.transform.position;
			
			var character = this.Hider.GetComponent<Character>();

			character.InCage = false;

			var navMeshObstacle = this.Hider.GetComponent<NavMeshObstacle>();

			if (!this.Hider.CompareTag("Player"))
			{
				navMeshObstacle.enabled = false;
			}
			
			foreach (var col in this.Hider.GetComponentsInChildren<Collider>())
			{
				col.enabled = true;
			}

			if (Spawner.Instance.CharacterMeshNicknames.ContainsKey(this.Hider.gameObject))
			{
				Spawner.Instance.CharacterMeshNicknames[this.Hider.gameObject].SetActive(true);
			}

			character.MovementController.enabled = true;
			character.MovementController.SetKinematic(false);

			var botController = this.Hider.GetComponent<BotController>();

			if (botController != null)
			{
				botController.enabled = true;
				botController.NavMeshAgent.enabled = true;
			}

			this.Hider.Caught = false;
			
			EventManager.OnHiderReleased?.Invoke(this.Hider);

			if (rescuer != null)
			{
				rescuer.RescuedCount++;
			}

			EventManager.OnHiderRescue?.Invoke(rescuer, this.Hider);
			
			var rescued = this.Hider;

			this.Hider = null;

			return rescued;
		}

		public void SetKinematic(bool kinematic)
		{
			this.rigidbody.isKinematic = kinematic;
		}

		private void OnTriggerEnter(Collider other)
		{
			var hider = other.GetComponent<Hider>();

			if (hider != null && hider != this.Hider && (hider.CompareTag("Player") || this.Hider.CompareTag("Player")))
			{
				Open(hider);
				
				Destroy(this.gameObject);
			}
		}
	}
}