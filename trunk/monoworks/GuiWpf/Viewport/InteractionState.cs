using System;
using System.Collections.Generic;

using System.Drawing;
using System.Windows.Forms;

using MonoWorks.Rendering;

namespace MonoWorks.GuiWpf
{
	public class InteractionState : InteractionStateBase
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks> Makes the default mapping.</remarks>
		public InteractionState()
		{
			modes = new Dictionary<MouseButtons, InteractionType>();

			modes[MouseButtons.Left] = InteractionType.Rotate;
			modes[MouseButtons.Middle] = InteractionType.Dolly;
			modes[MouseButtons.Right] = InteractionType.Pan;

			mouseType = InteractionType.None;
		}


		/// <summary>
		/// Maps the mouse button to the interaction mode.
		/// </summary>
		protected Dictionary<MouseButtons, InteractionType> modes;


		Point lastLoc = new Point();
		/// <summary>
		/// The last interaction location.
		/// </summary>
		public Point LastLoc
		{
			get {return lastLoc;}
		}

		protected Point anchorLoc;
		/// <summary>
		/// Location of the interaction anchor.
		/// </summary>
		public Point AnchorLoc
		{
			get { return anchorLoc; }
		}



		/// <summary>
		/// Call to register a button down event.
		/// </summary>
		/// <param name="evt"></param>
		public void OnMouseDown(MouseEventArgs evt)
		{
			if (modes.ContainsKey(evt.Button))
				mouseType = modes[evt.Button];
			lastLoc = evt.Location;
			anchorLoc = evt.Location;
		}

		/// <summary>
		/// Call to register a button up event.
		/// </summary>
		/// <param name="evt"></param>
		public void OnMouseUp(MouseEventArgs evt)
		{
			mouseType = InteractionType.None;
		}

		/// <summary>
		/// Call to register a motion event.
		/// </summary>
		/// <param name="evt"></param>
		public void OnMouseMove(MouseEventArgs evt)
		{
			lastLoc = evt.Location;
		}
	}
}
