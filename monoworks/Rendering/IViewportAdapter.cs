// Viewport.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;

using MonoWorks.Framework;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// General delegate for update events with no arguments.
	/// </summary>
	public delegate void UpdateHandler();
	
	/// <summary>
	/// The ViewportAdapter interface defines an interface for MonoWorks viewports adapters. 
	/// This lets the rendering system interact with viewports in a GUI-independant manner.
	/// </summary>
	public interface IViewportAdapter
	{
		/// <summary>
		/// The viewport.
		/// </summary>
		Viewport Viewport { get; }

		/// <summary>
		/// Returns the viewport width.
		/// </summary>
		int WidthGL { get; }
		
		/// <summary>
		/// Returns the viewport height.
		/// </summary>
		int HeightGL { get; }
		
		/// <summary>
		/// Makes the viewport's context the current one.
		/// </summary>
		void MakeCurrent();
		
		/// <summary>
		/// Initializes the rendering.
		/// </summary>
		void InitializeGL();

		/// <summary>
		/// Called when the viewport is resized.
		/// </summary>
		void ResizeGL();
		
		/// <summary>
		/// Queues the rendering for one frame.
		/// </summary>
		void PaintGL();
		
		/// <summary>
		/// Queues the rendering for one frame from another thread.
		/// </summary>
		void RemotePaintGL();

		/// <summary>
		/// The tooltip on the viewport.
		/// </summary>
		string ToolTip { set; }

		/// <summary>
		/// Clears the tooltip on the viewport.
		/// </summary>
		void ClearToolTip();

		/// <summary>
		/// Takes a snapshot of the viewport and exports it to the given file.
		/// </summary>
		void Export(string fileName);

		/// <summary>
		/// Creates a file dialog with the given dialog definition.
		/// </summary>
		/// <param name="dialog">Contains information about the dialog.</param>
		/// <returns>True if the action was applied by the user.</returns>
		bool FileDialog(FileDialogDef dialog);
	}
}
