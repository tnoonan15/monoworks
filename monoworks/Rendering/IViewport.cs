// IViewport.cs - MonoWorks Project
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

namespace MonoWorks.Rendering
{
	/// <summary>
	/// How the viewport is being used.
	/// </summary>
	public enum ViewportUsage {CAD, Plotting, Custom};
	
	/// <summary>
	/// The viewing mode of the viewport.
	/// </summary>
	public enum ViewMode { TwoD, ThreeD };

	/// <summary>
	/// General delegate for update events with no arguments.
	/// </summary>
	public delegate void UpdateHandler();
	
	/// <summary>
	/// The IViewport interface defines an interface for MonoWorks viewports. 
	/// This lets the model interact with viewports in a GUI-independant manner.
	/// </summary>
	public interface IViewport
	{
		/// <summary>
		/// Returns the viewport width.
		/// </summary>
		int WidthGL { get; }
		
		/// <summary>
		/// Returns the viewport height.
		/// </summary>
		int HeightGL { get; }
		
		/// <value>
		/// Access the viewport camera.
		/// </value>
		Camera Camera { get; }

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
		/// Performs the rendering for one frame.
		/// </summary>
		void PaintGL();

		/// <value>
		/// Access the viewport's render manager.
		/// </value>
		RenderManager RenderManager { get; }
		
		/// <value>
		/// The current interaction state of the viewport.
		/// </value>
		Interaction.InteractionState InteractionState {get; set;}
		
		/// <value>
		/// Interactor that gets to handle events before the renderables.
		/// </value>
		Interaction.AbstractInteractor PrimaryInteractor {get; set;}
		
		/// <value>
		/// The renderable interactor.
		/// </value>
		Interaction.RenderableInteractor RenderableInteractor { get; }

		/// <value>
		/// The overlay interactor.
		/// </value>
		Interaction.OverlayInteractor OverlayInteractor { get; }
		
		/// <value>
		/// The lighting effects for the viewport.
		/// </value>
		Lighting Lighting {	get; }

		/// <summary>
		/// The rendering list for the viewport.
		/// </summary>
		RenderList RenderList { get; }
		
		/// <summary>
		/// Lets the renderables know that the view direction has been changed.
		/// </summary>
		void OnDirectionChanged();

		/// <summary>
		/// Renders text to the viewport.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		void RenderText(TextDef text);

		/// <summary>
		/// Renders lots of text to the viewport.
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		void RenderText(TextDef[] text);
	}
}
