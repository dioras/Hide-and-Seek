using System.Collections;
using UnityEngine;

namespace Project.Development.CameraScripts
{
	public class CameraController: MonoBehaviour
	{
		public Transform MotionTarget;
		public Transform RotationTarget;
		public Vector3 Offset { get; set; }
		public float MotionLerp { get; set; }
		public float RotationLerp { get; set; }
		public bool LookAtTarget { get; set; }
		public Quaternion Rotation { get; set; }
		public bool NeedMove { get; set; }
		public bool Lerp { get; set; }

		private bool lerping;

		private Coroutine lerpCameraCoroutine;



		private void OnEnable()
		{
			this.Lerp = false;
			this.LookAtTarget = true;
			this.NeedMove = true;
			this.MotionLerp = 50f;
			this.RotationLerp = 35f;
			this.Offset = CameraRepository.Instance.Offset;
			this.Rotation = Quaternion.Euler(CameraRepository.Instance.Rotation);
		}
		
		
		private void LateUpdate()
		{
			if (!this.lerping && this.MotionTarget != null)
			{
				this.transform.position = Vector3.MoveTowards(this.transform.position,
					this.MotionTarget.position + this.Offset,
					this.MotionLerp * Time.deltaTime);
					
				this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation,
					this.Rotation,
					this.RotationLerp * Time.deltaTime);
			}
		}

		private IEnumerator LerpCameraProcess(Quaternion rot, Vector3 offs, float time, float endDelay = 0f)
		{
			this.lerping = true;
		
			var currT = time;

			var trPos = this.transform.position;

			var trRot = this.transform.rotation;
		
			while (currT > 0f)
			{
				if (this.MotionTarget == null)
				{
					this.lerping = false;
					this.Offset = offs;
					this.Rotation = rot;
				
					yield break;
				}
			
				var lerp = 1 - currT / time;
				
				this.transform.rotation = Quaternion.Lerp(trRot, rot, lerp);
				this.transform.position = Vector3.Lerp(trPos, this.MotionTarget.position + offs, lerp);

				currT -= Time.deltaTime;

				yield return new WaitForEndOfFrame();
			}
			
			this.transform.rotation = rot;
			this.transform.position = this.MotionTarget.position + offs;

			this.Offset = offs;
			this.Rotation = rot;
			
			yield return new WaitForSeconds(endDelay);
			
			this.lerping = false;
		}

		public void LerpCamera(Quaternion rot, Vector3 offs, float time, float endDelay = 0f)
		{
			if (this.lerpCameraCoroutine != null)
			{
				StopCoroutine(this.lerpCameraCoroutine);
			}
		
			this.lerpCameraCoroutine = StartCoroutine(LerpCameraProcess(rot, offs, time, endDelay));
		}
	}
}