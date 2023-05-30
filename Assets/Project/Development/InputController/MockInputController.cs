using Project.Development.InputController.Interface;
using UnityEngine;

namespace Project.Development.InputController
{
	public class MockInputController: IInputController
	{
		public Vector3 MovingResultVector { get; set; }
		public Vector3 RotationResultVector { get; set; }




		public Vector3 GetMotion()
		{
			return this.MovingResultVector;
		}

		public Vector3 GetRotation()
		{
			return this.RotationResultVector;
		}
	}
}