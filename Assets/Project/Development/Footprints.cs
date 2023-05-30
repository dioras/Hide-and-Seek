using System.Collections;
using UnityEngine;

namespace Project.Development
{
	public class Footprints: MonoBehaviour
	{
		[field: SerializeField]
		public Transform Target { get; set; }
		[field: SerializeField]
		public Vector3 Offset { get; set; }

		public Material Material
		{
			get => this.footprintsRenderer.material;
			set => this.footprintsRenderer.material = value;
		}

		public bool IsPlaying { get; private set; }

		private ParticleSystem footprintsPS;
		private ParticleSystemRenderer footprintsRenderer;

		private Coroutine playCoroutine;

		private float startLifetime;




		private void Awake()
		{
			this.footprintsPS = GetComponent<ParticleSystem>();

			this.footprintsRenderer = GetComponent<ParticleSystemRenderer>();

			this.startLifetime = this.footprintsPS.main.startLifetime.constant;
		}

		private void LateUpdate()
		{
			if (Target == null)
			{
				return;
			}
		
			if (!IsPlaying)
			{
				return;
			}
			
			this.transform.position = Target.position + Offset;
			this.transform.forward = Target.forward;
		}




		public void Play()
		{
			IsPlaying = true;
		
			var footprintsPsMain = this.footprintsPS.main;
			footprintsPsMain.startLifetime = this.startLifetime;
		}

		public void Play(float duration)
		{
			if (this.playCoroutine != null)
			{
				StopCoroutine(this.playCoroutine);
			}
		
			this.playCoroutine = StartCoroutine(PlayProcess(duration));
		}

		public void Stop()
		{
			IsPlaying = false;
		
			var footprintsPsMain = this.footprintsPS.main;
			footprintsPsMain.startLifetime = 0f;
		}


		private IEnumerator PlayProcess(float duration)
		{
			Play();
		
			yield return new WaitForSeconds(duration);
			
			Stop();

			this.playCoroutine = null;
		}
	}
}