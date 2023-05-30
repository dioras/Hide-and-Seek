using Project.Development.InputController.Interface;
using UnityEngine;

namespace Project.Development.InputController
{
	public class KeyboardController: IInputController
	{
		public Vector3 GetMotion()
		{
			return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		}

		public Vector3 GetRotation()
		{
			return new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		}
	}
}