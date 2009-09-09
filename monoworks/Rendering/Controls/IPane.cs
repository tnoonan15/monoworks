
using System;

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Interface for Panes.
	/// Panes are invisible renderables that holds a 2D control, like a pane of glass.
	/// It renders the control to a texture then places it in the scene, as well as pass 
	/// keyboard and mouse events to it.
	/// </summary>
	public interface IPane
	{
		
		/// <value>
		/// The control that gets rendered on the pane.
		/// </value>
		Control2D Control {get; set;}
		
		/// <value>
		/// Whether or not the pane is visible.
		/// </value>
		bool IsVisible {get; set;}
		
	}
}
