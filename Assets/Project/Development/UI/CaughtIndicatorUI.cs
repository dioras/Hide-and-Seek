using UnityEngine;
using UnityEngine.UI;

namespace Project.Development.UI
{
	public class CaughtIndicatorUI: MonoBehaviour
	{
		[field: SerializeField]
		public Image PlusImage { get; set; }
		[field: SerializeField]
		public Image AnimImage { get; set; }
		[field: SerializeField]
		public Image BackgroundImage { get; set; }
	}
}