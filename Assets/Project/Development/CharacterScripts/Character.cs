using System.Collections;
using Project.Development.CameraScripts;
using Project.Development.Events;
using Project.Development.InputController.Interface;
using Project.Development.SkinScripts;
using Project.Development.UI;
using UnityEngine;

namespace Project.Development.CharacterScripts
{
	public class Character: MonoBehaviour
	{
		public MovementController MovementController { get; set; }
		public CameraController CameraController { get; set; }
		public IInputController InputController { get; set; }
		public InvisibilityComponent InvisibilityComponent { get; set; }
		
		public bool InCage { get; set; }

		public Transform RightLeg => this.rightLeg;
		public Transform LeftLeg => this.leftLeg;

		[SerializeField] private Transform skinParent;
		[SerializeField] private Transform rightLeg;
		[SerializeField] private Transform leftLeg;

		[SerializeField] private new Collider interactionCollider;

		private Coroutine changeSkinCoroutine;
		


		private void Awake()
		{
			this.MovementController = GetComponent<MovementController>();
			this.InvisibilityComponent = GetComponent<InvisibilityComponent>();
			
			UpdateLegs(this.skinParent.GetChild(0).gameObject);
			
			EventManager.OnSkinSelected.AddListener(OnSkinSelected);
			EventManager.OnSkinChanged.AddListener(OnSkinChanged);
		}

		private void Update()
		{
			if (InputController != null)
			{
				var motion = InputController.GetMotion();
				var rotation = InputController.GetRotation();

				if (CameraController != null)
				{
					motion = CameraController.transform.TransformDirection(motion);
					rotation = CameraController.transform.TransformDirection(rotation);
				}

				motion.Normalize();
				rotation.Normalize();

				motion.y = 0f;
				rotation.y = 0f;

				MovementController.Motion = motion;
				MovementController.Rotation = rotation;
			}

			var meshY = this.skinParent.localPosition.y;

			meshY = Mathf.MoveTowards(meshY, this.MovementController.InWater && !InCage ? -0.5f : 0.1f, 4f * Time.deltaTime);
            
			this.skinParent.localPosition = new Vector3(this.skinParent.localPosition.x, meshY, this.skinParent.localPosition.z);

			var colliderRotation = this.interactionCollider.transform.localRotation;

			colliderRotation = Quaternion.RotateTowards(colliderRotation, 
			MovementController.InWater && !InCage ? Quaternion.Euler(90, 0, 0) : 
			Quaternion.Euler(0, 0, 0), 200f * Time.deltaTime);
            
			this.interactionCollider.transform.localRotation = colliderRotation;

			var colliderPosition = this.interactionCollider.transform.localPosition;

			colliderPosition = Vector3.MoveTowards(colliderPosition, 
				MovementController.InWater && !InCage ? new Vector3(0, 0.7f, -0.5f) :
				new Vector3(0, 0, 0), 4f * Time.deltaTime);
            
			this.interactionCollider.transform.localPosition = colliderPosition;
		}

		private void OnDestroy()
		{
			EventManager.OnSkinSelected.RemoveListener(OnSkinSelected);
			EventManager.OnSkinChanged.RemoveListener(OnSkinChanged);
		}


		public void SetSkin(Skin newSkin)
		{
			if (this.changeSkinCoroutine != null)
			{
				return;
			}

			this.changeSkinCoroutine = StartCoroutine(ChangeSkinProcess(newSkin));
		}


		private IEnumerator ChangeSkinProcess(Skin newSkin)
		{
			if (this.skinParent.childCount > 0)
			{
				Destroy(this.skinParent.GetChild(0).gameObject);
			}

			while (this.skinParent.childCount > 0)
			{
				yield return null;
			}
			
			var skin = Instantiate(newSkin.SkinPrefab, this.skinParent);

			EventManager.OnSkinChanged?.Invoke(this, skin);

			this.changeSkinCoroutine = null;
		}



		private void OnSkinSelected(SkinUI skinUi)
		{
			if (CompareTag("Player"))
			{
				SetSkin(skinUi.Skin);
			}
		}
		
		private void OnSkinChanged(Character character, GameObject skin)
		{
			if (character == this)
			{
				UpdateLegs(skin);
			}
		}

		private void UpdateLegs(GameObject skin)
		{
			var trs = skin.GetComponentsInChildren<Transform>();

			foreach (Transform tr in trs)
			{
				if (tr.name.Equals("calf_r") || tr.name.Equals("Right_Knee"))
				{
					this.rightLeg = tr;
				}
				
				if (tr.name.Equals("calf_l") || tr.name.Equals("Left_Knee"))
				{
					this.leftLeg = tr;
				}
			}
		}
	}
}