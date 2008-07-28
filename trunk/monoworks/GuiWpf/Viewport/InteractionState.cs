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
			modes = new Dictionary<MouseButtons, InteractionMode>();

			modes[MouseButtons.Left] = InteractionMode.Rotate;
			modes[MouseButtons.Middle] = InteractionMode.Dolly;
			modes[MouseButtons.Right] = InteractionMode.Pan;

			mode = InteractionMode.None;
		}


		/// <summary>
		/// Maps the mouse button to the interaction mode.
		/// </summary>
		protected Dictionary<MouseButtons, InteractionMode> modes;


		protected InteractionMode mode;
		/// <summary>
		/// The current interaction mode.
		/// </summary>
		public InteractionMode Mode
		{
			get { return mode; }
		}



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
				mode = modes[evt.Button];
			lastLoc = evt.Location;
			anchorLoc = evt.Location;
		}

		/// <summary>
		/// Call to register a button up event.
		/// </summary>
		/// <param name="evt"></param>
		public void OnMouseUp(MouseEventArgs evt)
		{
			mode = InteractionMode.None;
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
