// Viewport.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 

using System;
using System.Collections.Generic;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Rendering.Events;
using MonoWorks.Framework;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// The types of cursors that MonoWorks ensures will be available on all platforms. 
	/// </summary>
	public enum CursorType {Normal, Beam, Hand}
	
	/// <summary>
	/// The Viewport is the interface between the GUI and rendering pipeline.
	/// </summary>
	public class Viewport : IMouseHandler, IKeyHandler
	{

		public Viewport(IViewportAdapter adapter)
		{

			this.adapter = adapter;
			RootScene = new Scene(this);
		}

		protected IViewportAdapter adapter;
		
		
		public Scene RootScene
		{
			get;
			set;
		}

		#region Rendering

		/// <summary>
		/// Initialize rendering.
		/// </summary>
		public void Initialize()
		{
			RootScene.Initialize();
		}

		/// <summary>
		/// Callback for the viewport being resized.
		/// </summary>
		public void Resize()
		{
			RootScene.Resize(Width, Height);
			RootScene.Camera.Resize();
		}
		
		/// <summary>
		/// Render the viewport.
		/// </summary>
		public void Render()
		{
			adapter.MakeCurrent();
			
			RootScene.RenderManager.ClearScene();			
			RootScene.Render();
		}

		/// <summary>
		/// Height of the rendered area.
		/// </summary>
		public int Height
		{
			get { return adapter.HeightGL; }
		}

		/// <summary>
		/// Width of the rendered area.
		/// </summary>
		public int Width
		{
			get { return adapter.WidthGL; }
		}

		/// <summary>
		/// Passes the paint command to the viewport adapter.
		/// </summary>
		public void Paint()
		{
			adapter.PaintGL();
		}

		/// <summary>
		/// Passes the remote paint command to the viewport adapter.
		/// </summary>
		public void RemotePaint()
		{
			adapter.RemotePaintGL();
		}

		#endregion


		#region Mouse Interaction

		public void OnButtonPress(MouseButtonEvent evt)
		{
			RootScene.OnButtonPress(evt);
		}

		public void OnButtonRelease(MouseButtonEvent evt)
		{
			RootScene.OnButtonRelease(evt);
		}

		public void OnMouseMotion(MouseEvent evt)
		{
			RootScene.OnMouseMotion(evt);
		}


		public void OnMouseWheel(MouseWheelEvent evt)
		{
			RootScene.OnMouseWheel(evt);
		}

		/// <summary>
		/// Set the current cursor to the given type.
		/// </summary>
		public void SetCursor(CursorType type)
		{
			adapter.SetCursor(type);
		}

		public void OnKeyPress(KeyEvent evt)
		{
			RootScene.OnKeyPress(evt);
		}

		#endregion
		
		
		#region Exporting

		/// <summary>
		/// Prompts the user for a file name, then exports to that file.
		/// </summary>
		public void Export()
		{
			FileDialogDef dialogDef = new FileDialogDef() {Type = FileDialogType.SaveAs};
			dialogDef.Extensions.Add("png");
			adapter.FileDialog(dialogDef);
			if (dialogDef.Success)
				adapter.Export(dialogDef.FileName);
		}

		#endregion

	}
}
