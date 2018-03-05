using UnityEngine;
using System.Collections;

///Developed by Indie Studio
///https://www.assetstore.unity3d.com/en/#!/publisher/9268
///www.indiestd.com
///info@indiestd.com

namespace IndieStudio.DrawingAndColoring.Logic
{
	[DisallowMultipleComponent]
	public class WhiteArea : MonoBehaviour
	{
			/// <summary>
			/// White area animator.
			/// </summary>
			private static Animator WhiteAreaAnimator;

			// Use this for initialization
			void Awake ()
			{
					///Setting up the references
					if (WhiteAreaAnimator == null) {
							WhiteAreaAnimator = GetComponent<Animator> ();
					}
			}

			/// <summary>
			/// When the GameObject becomes visible
			/// </summary>
			void OnEnable ()
			{
					///Hide the White Area
					Hide ();
			}

			///Show the White Area
			public static void Show ()
			{
					if (WhiteAreaAnimator == null) {
							return;
					}
					WhiteAreaAnimator.SetTrigger ("Running");
			}
			///Hide the White Area
			public static void Hide ()
			{
				if(WhiteAreaAnimator!=null)
					WhiteAreaAnimator.SetBool ("Running", false);
			}
	}
}