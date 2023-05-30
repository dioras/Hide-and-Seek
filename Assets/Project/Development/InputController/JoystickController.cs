using Project.Development.InputController.Interface;
using UnityEngine;

namespace Project.Development.InputController
{
	public class JoystickController: MonoBehaviour, IInputController
	{
		public Joystick Joystick => this.joystick;

		[SerializeField] private Joystick joystick;
		
		
		
		public Vector3 GetMotion()
		{
			return this.Joystick.Direction;
		}

		public Vector3 GetRotation()
		{
			return this.Joystick.Direction;
		}
	}
}