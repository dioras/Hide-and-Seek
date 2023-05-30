using System.Collections.Generic;
using Project.Development.CharacterScripts;
using Project.Development.Events;
using UnityEngine;

namespace Project.Development.ModeScripts
{
	public class BonusLevelGameMode: GameMode
	{
		[SerializeField] 
		private AnimatorOverrideController characterAC;




		public override void Join(Character character, Dictionary<string, object> settings)
		{
			base.Join(character, settings);
			
			var animationController = character.GetComponent<CharacterAnimationController>();
			
			animationController.SetAnimatorController(this.characterAC);
		}

		public override void StartMatch()
		{
			EventManager.OnStartMatch?.Invoke(this);
			
			SetPlayersMoveState(true);

			IsStarted = true;
			
			EventManager.OnMatchStarted?.Invoke(this);
		}

		public override void FinishMatch()
		{
			base.FinishMatch();
			
			EventManager.OnMatchFinished?.Invoke(this, Result.Win);
		}
	}
}