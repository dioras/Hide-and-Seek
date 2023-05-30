using UnityEngine;

namespace Project.Development.InputController.Interface
{
	public interface IInputController
	{
		Vector3 GetMotion();
		Vector3 GetRotation();
	}
}