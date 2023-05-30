using System.Collections;
using Project.Development.Events;
using Project.Development.ModeScripts;
using UnityEngine;

namespace Project.Development.CharacterScripts
{
	public class CharacterAnimationController: MonoBehaviour
	{
		private static readonly int Water = Animator.StringToHash("Water");
		private static readonly int Move = Animator.StringToHash("Move");
		private static readonly int Dance = Animator.StringToHash("Dance");
		private static readonly int InCage = Animator.StringToHash("InCage");

		public Animator Animator
		{
			get
			{
				if (this.animator == null)
				{
					this.animator = GetComponentInChildren<Animator>();
				}

				return this.animator;
			}
			set => this.animator = value;
		}

		private Animator animator;

		private static readonly int RunSpeedMultiplier = Animator.StringToHash("RunSpeedMultiplier");

		private bool dance;
		private Character character;
		private AnimatorOverrideController currentAOC;
		private float currRunAnimSpeedMultiplier;




		private void Awake()
		{
			this.character = GetComponent<Character>();

			this.currRunAnimSpeedMultiplier = -1f;
			
			EventManager.OnMatchFinished.AddListener(OnMatchFinished);
			EventManager.OnSkinChanged.AddListener(OnSkinChanged);
		}

		private void Update()
		{
			if (Animator == null)
			{
				return;
			}

			if (!this.dance)
			{
				Animator.SetBool(InCage, this.character.InCage);
			
				Animator.SetBool(Move, this.character.MovementController.IsMoving);

				Animator.SetBool(Water, this.character.MovementController.InWater);
			}
		}

		private void OnDestroy()
		{
			EventManager.OnMatchFinished.RemoveListener(OnMatchFinished);
			EventManager.OnSkinChanged.RemoveListener(OnSkinChanged);
		}



		public void SetAnimatorController(AnimatorOverrideController aoc)
		{
			if (Animator != null)
			{
				Animator.runtimeAnimatorController = aoc;
			}

			this.currentAOC = aoc;
		}

		public void SetRunAnimSpeedMultiplier(float speed)
		{
			if (Animator != null)
			{
				Animator.SetFloat(RunSpeedMultiplier, speed);
			}

			this.currRunAnimSpeedMultiplier = speed;
		}

		public void SetDance(bool dance)
		{
			this.dance = dance;
		
			Animator.SetTrigger(Dance);
		}
		
		private void OnMatchFinished(GameMode gameMode, Result result)
		{
			if (CompareTag("Player") && result == Result.Win)
			{
				SetDance(true);
			}
		}
		
		private void OnSkinChanged(Character character, GameObject skin)
		{
			if (this.character == character)
			{
				var animator = skin.GetComponent<Animator>();

				Animator = animator;

				if (this.currentAOC != null)
				{
					SetAnimatorController(this.currentAOC);
				}

				if (this.currRunAnimSpeedMultiplier >= 0f)
				{
					SetRunAnimSpeedMultiplier(this.currRunAnimSpeedMultiplier);
				}
			}
		}
	}
}