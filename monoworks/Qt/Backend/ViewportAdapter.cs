// 
//  ViewportAdapter.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 MonoWorks Project
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.Qt.Backend
{
	public class ViewportAdapter : QGLWidget, IViewportAdapter
	{
		
		public ViewportAdapter(QWidget parent) : base(parent)
		{
			Viewport = new Viewport(this);
			SetMouseTracking(true);
			GrabKeyboard();
		}
		
		
		/// <summary>
		/// The viewport being adapted.
		/// </summary>
		public Viewport Viewport {get; private set;}
		
		
		#region OpenGL Methods

		/// <summary>
		/// The width of the viewport.
		/// </summary>
		public int WidthGL {
			get { return Width(); }
		}

		/// <summary>
		/// The height of the viewport.
		/// </summary>
		public int HeightGL {
			get { return Height(); }
		}
		
		/// <summary>
		/// Gets called when the adapter is resized.
		/// </summary>
		public void ResizeGL()
		{
			Viewport.Resize();
		}
		
		protected override void ResizeGL(int w, int h)
		{
			base.ResizeGL(w, h);
			
			Viewport.Resize();
		}
		
		
		public void RemotePaintGL()
		{
		}
		
		
		void IViewportAdapter.InitializeGL()
		{
			InitializeGL();
		}
		
		protected override void InitializeGL()
		{
			base.InitializeGL();
			Viewport.Initialize();
		}
		
		protected override void PaintGL()
		{
			base.PaintGL();
			Viewport.Render();
		}
		
		
		void IViewportAdapter.PaintGL()
		{
			Update();
		}
		
		#endregion
		
		
		#region Interaction
		
		/// <summary>
		/// Gets the interaction modifier from an input event.
		/// </summary>
		private InteractionModifier GetModifier(QInputEvent qevt)
		{
			InteractionModifier mod = InteractionModifier.None;
			var mods = qevt.Modifiers();
			if ((mods &= (uint)Qyoto.Qt.KeyboardModifier.ShiftModifier) == (uint)Qyoto.Qt.KeyboardModifier.ShiftModifier)
				mod |= InteractionModifier.Shift;
			if ((mods &= (uint)Qyoto.Qt.KeyboardModifier.ControlModifier) == (uint)Qyoto.Qt.KeyboardModifier.ControlModifier)
				mod |= InteractionModifier.Control;
			return mod;
		}
				
		/// <summary>
		/// Converts a QMouseEvent to a MouseButtonEvent.
		/// </summary>
		private MouseButtonEvent ConvertButtonEvent(QMouseEvent qevt, ClickMultiplicity mult)
		{
			var pos = new Coord(qevt.X(), HeightGL - qevt.Y());
			
			// determine which button was clicked
			int button;
			switch (qevt.Button())
			{
			case MouseButton.LeftButton:
				button = 1;
				break;
			case MouseButton.RightButton:
				button = 2;
				break;
			case MouseButton.MidButton:
				button = 3;
				break;
			default:
				button = 1;
				break;
			}
			return new MouseButtonEvent(Viewport.RootScene, pos, button, GetModifier(qevt), mult);
		}

		/// <summary>
		/// Converts a QMouseEvent to a MouseEvent.
		/// </summary>
		private MouseEvent ConvertEvent(QMouseEvent qevt)
		{
			var pos = new Coord(qevt.X(), HeightGL - qevt.Y());
			return new MouseEvent(Viewport.RootScene, pos);
		}
		
		/// <summary>
		/// Converts a QKeyEvent to a KeyEvent.
		/// </summary>
		private KeyEvent ConvertEvent(QKeyEvent qevt)
		{
			var keyVal = qevt.Key();
			var mod = GetModifier(qevt);
			if (keyVal >= 65 && keyVal <= 90 && // letter
				(mod & InteractionModifier.Shift) != InteractionModifier.Shift)
				keyVal += 32;
			if (keyVal == (int)Key.Key_Left)
				keyVal = (int)SpecialKey.Left;
			if (keyVal == (int)Key.Key_Right)
				keyVal = (int)SpecialKey.Right;
			if (keyVal == (int)Key.Key_Up)
				keyVal = (int)SpecialKey.Up;
			if (keyVal == (int)Key.Key_Down)
				keyVal = (int)SpecialKey.Down;
			if (keyVal == (int)Key.Key_Backspace)
				keyVal = (int)SpecialKey.Backspace;
			if (keyVal == (int)Key.Key_Enter)
				keyVal = (int)SpecialKey.Enter;
			if (keyVal == (int)Key.Key_Home)
				keyVal = (int)SpecialKey.Home;
			if (keyVal == (int)Key.Key_End)
				keyVal = (int)SpecialKey.End;
			if (keyVal == (int)Key.Key_Delete)
				keyVal = (int)SpecialKey.Delete;
			var evt = new KeyEvent(Viewport.RootScene, keyVal, mod);
			return evt;
		}

		protected override void MousePressEvent(QMouseEvent arg1)
		{
			base.MousePressEvent(arg1);
			
			Viewport.OnButtonPress(ConvertButtonEvent(arg1, ClickMultiplicity.Single));
			Update();
		}

		protected override void MouseReleaseEvent(QMouseEvent arg1)
		{
			base.MouseReleaseEvent(arg1);
			
			Viewport.OnButtonRelease(ConvertButtonEvent(arg1, ClickMultiplicity.Single));
			Update();
		}
		
		protected override void MouseMoveEvent(QMouseEvent arg1)
		{
			base.MouseMoveEvent(arg1);
			
			var evt = ConvertEvent(arg1);
			Viewport.OnMouseMotion(evt);
			Update();
		}		
		
		protected override void MouseDoubleClickEvent(QMouseEvent arg1)
		{
			base.MouseDoubleClickEvent(arg1);
			
			Viewport.OnButtonRelease(ConvertButtonEvent(arg1, ClickMultiplicity.Double));
			Update();
		}
		
		protected override void KeyPressEvent(QKeyEvent arg1)
		{
			base.KeyPressEvent(arg1);
			
			var evt = ConvertEvent(arg1);
			Console.WriteLine("key press {0}", evt);
			Viewport.OnKeyPress(evt);
			Update();
		}
		
		protected override void KeyReleaseEvent(QKeyEvent arg1)
		{
			base.KeyReleaseEvent(arg1);
			
			Viewport.OnKeyRelease(ConvertEvent(arg1));
			Update();
		}
		
		
		#endregion
		
		
		#region Misc
		
		public void ClearToolTip()
		{
		}
		
		
		public void Export(string fileName)
		{
			throw new System.NotImplementedException();
		}
		
		
		public void SetCursor(CursorType type)
		{
		}
		
		
		public bool FileDialog(FileDialogDef dialog)
		{
			throw new System.NotImplementedException();
		}
		
		#endregion
		
		
		
		
	}
}

