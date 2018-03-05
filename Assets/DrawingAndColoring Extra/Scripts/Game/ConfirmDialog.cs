using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class ConfirmDialog : MonoBehaviour
	{
		/// <summary>
		/// The animator of Confirm Dialog.
		/// </summary>
		public Animator animator;

		/// <summary>
		/// Wheter the Confirm Dialog is visible or not.
		/// </summary>
		[HideInInspector]
		public bool visible;

		/// <summary>
		/// The White Area animator.
		/// </summary>
		public Animator whiteAreaAnimator;

		void Start ()
		{
			if (animator == null) {
				animator = GetComponent<Animator> ();
			}

			if (whiteAreaAnimator == null) {
				whiteAreaAnimator = GameObject.Find ("whiteArea").GetComponent<Animator> ();
			}
		}

		/// <summary>
		/// Show the dialog.
		/// </summary>
		public void Show ()
		{
			visible = true;
			whiteAreaAnimator.SetTrigger ("Running");
			animator.SetBool ("Off", false);
			animator.SetTrigger ("On");
		}

		/// <summary>
		/// Hide the dialog.
		/// </summary>
		public void Hide ()
		{
			visible = false;
			whiteAreaAnimator.SetBool ("Running", false);
			animator.SetBool ("On", false);
			animator.SetTrigger ("Off");
		}
	}
}