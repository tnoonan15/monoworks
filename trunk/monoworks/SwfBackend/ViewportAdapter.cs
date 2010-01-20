// SwfViewport.cs - MonoWorks Project
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


using System.Windows.Forms;

using Tao.Platform.Windows;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;
using MonoWorks.Framework;


namespace MonoWorks.SwfBackend
{

	/// <summary>
	/// Viewport for WPF.
	/// </summary>
	public class ViewportAdapter : SimpleOpenGlControl, IViewportAdapter
	{
		public ViewportAdapter()
			: base()
		{
			InitializeContexts();

			this.DoubleBuffered = false;

			// create the tooltip
			_toolTip = new ToolTip();
			_toolTip.SetToolTip(this, "");

			Viewport = new Viewport(this);

			InitializeGL();
		}


		/// <summary>
		/// The viewport.
		/// </summary>
		public Viewport Viewport { get; set; }

		/// <summary>
		/// Rendering width.
		/// </summary>
		/// <returns></returns>
		public int WidthGL
		{
			get { return Size.Width; }
		}

		/// <summary>
		/// Rendering height.
		/// </summary>
		/// <returns></returns>
		public int HeightGL
		{
			get { return Size.Height; }
		}



		private readonly ToolTip _toolTip;

		/// <summary>
		/// The tooltip on the viewport.
		/// </summary>
		public string ToolTip
		{
			set
			{
				if (value.Length > 0)
					_toolTip.Show(value, this);
				else
					_toolTip.Hide(this);
			}
		}

		/// <summary>
		/// Clears the tooltip.
		/// </summary>
		public void ClearToolTip()
		{
			_toolTip.Hide(this);
		}

		/// <summary>
		/// Export the viewport to a bitmap in the given file name.
		/// </summary>
		/// <param name="fileName"></param>
		public void Export(string fileName)
		{
			//var bmp = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			//Imaging.BitmapData data =
			//    bmp.LockBits(this.ClientRectangle, Imaging.ImageLockMode.WriteOnly,
			//                 Imaging.PixelFormat.Format24bppRgb);
			//Tao.OpenGl.Gl.glReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, Tao.OpenGl.Gl.GL_BGR, Tao.OpenGl.Gl.GL_UNSIGNED_BYTE,
			//                           data.Scan0);
			//bmp.UnlockBits(data);
			//bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
			//bmp.Save(fileName);
		}

		/// <summary>
		/// Creates a file dialog.
		/// </summary>
		public bool FileDialog(FileDialogDef dialogDef)
		{
			//Microsoft.Win32.FileDialog dialog = null;

			//// create the dialog
			//if (dialogDef.Type == FileDialogType.Open)
			//    dialog = new Microsoft.Win32.OpenFileDialog();
			//else if (dialogDef.Type == FileDialogType.SaveAs)
			//    dialog = new Microsoft.Win32.SaveFileDialog();

			//// set the options from the def
			//if (dialogDef.FileName != null)
			//    dialog.FileName = "Document"; // Default file name
			//foreach (var ext in dialogDef.Extensions)
			//{
			//    dialog.Filter = String.Format("{0} (.{1})|*.{1}", dialogDef.GetDescription(ext), ext);
			//}
			//if (dialogDef.Extensions.Count > 0)
			//    dialog.DefaultExt = "." + dialogDef.Extensions[0]; // Default file extension

			//// Show the dialog
			//Nullable<bool> result = dialog.ShowDialog();
			//dialogDef.Success = result != null && (bool)result;
			//if (dialogDef.Success == true)
			//    dialogDef.FileName = dialog.FileName;
			//return dialogDef.Success;
			return false;
		}

		#region Mouse Interaction



		/// <summary>
		/// Convenience method that converts a mouse event point into a proper viewport coord.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		protected Coord MouseToViewport(System.Drawing.Point point)
		{
			return new Coord(point.X, HeightGL - point.Y);
		}

		/// <summary>
		/// This is true when the last button down event was handled.
		/// </summary>
		private bool _lastClickHandled;

		protected override void OnMouseDown(MouseEventArgs args)
		{
			base.OnMouseDown(args);

			var evt = new MouseButtonEvent(Viewport.RootScene, 
			                               MouseToViewport(args.Location),
			                               Extensions.ButtonNumber(args.Button),
			                               Extensions.GetModifier(ModifierKeys));
			Viewport.OnButtonPress(evt);

			_lastClickHandled = evt.Handled;

			PaintGL();
		}

		protected override void OnMouseUp(MouseEventArgs args)
		{
			base.OnMouseUp(args);

			var evt = new MouseButtonEvent(Viewport.RootScene, 
			                               MouseToViewport(args.Location),
			                               Extensions.ButtonNumber(args.Button),
			                               Extensions.GetModifier(ModifierKeys));
			Viewport.OnButtonRelease(evt);

			PaintGL();
		}

		protected override void OnMouseMove(MouseEventArgs args)
		{
			base.OnMouseMove(args);

			var evt = new MouseEvent(Viewport.RootScene, MouseToViewport(args.Location),
			                         Extensions.GetModifier(ModifierKeys));
			Viewport.OnMouseMotion(evt);

			PaintGL();
		}


		protected override void OnMouseWheel(MouseEventArgs args)
		{
			base.OnMouseWheel(args);

			var direction = WheelDirection.Up;
			if (args.Delta < 0)
				direction = WheelDirection.Down;
			var evt = new MouseWheelEvent(Viewport.RootScene, direction, InteractionModifier.None);
			Viewport.OnMouseWheel(evt);

			PaintGL();
		}

		protected override void OnMouseDoubleClick(MouseEventArgs args)
		{
			base.OnMouseDoubleClick(args);

			if (_lastClickHandled)
				return;

			var evt = new MouseButtonEvent(Viewport.RootScene, MouseToViewport(args.Location),
			                               Extensions.ButtonNumber(args.Button),
			                               InteractionModifier.None, ClickMultiplicity.Double);
			Viewport.OnButtonPress(evt);

			PaintGL();
		}

		public void SetCursor(CursorType cursorType)
		{
			switch(cursorType)
			{
				case CursorType.Normal:
					Cursor = Cursors.Arrow;
					break;
				case CursorType.Beam:
					Cursor = Cursors.IBeam;
					break;
				case CursorType.Hand:
					Cursor = Cursors.Hand;
					break;
			}
		}

		#endregion


		#region Keyboard Interaction

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			
			// get the modifier
			int mod = 0;
			if (e.Shift)
				mod += (int)InteractionModifier.Shift;
			if (e.Control)
				mod += (int)InteractionModifier.Control;
			if (e.Alt)
				mod += (int)InteractionModifier.Alt;
			if (mod == 0)
				mod = (int)InteractionModifier.None;

			int val = e.KeyValue;

			var evt = new KeyEvent(val, (InteractionModifier)mod);
			//Console.WriteLine("viewport key press {0} ({1}, {2})", evt.Value, evt.SpecialKey, evt.Modifier);

			Viewport.OnKeyPress(evt);

			e.Handled = evt.Handled;

			PaintGL();
		}


		#endregion


		#region GL Stuff

		/// <summary>
		/// Initialize the OpenGL context.
		/// </summary>
		public void InitializeGL()
		{
			MakeCurrent();

			Viewport.Initialize();

			PaintGL();
		}

		/// <summary>
		/// Called when the widget is resized.
		/// </summary>
		public void ResizeGL()
		{
			MakeCurrent();

			Viewport.Resize();

			PaintGL();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			
			if (e.ClipRectangle.Width == 0)
			{
				return;
			}

			Viewport.Render();

			base.OnPaint(e);
		}

		public void PaintGL()
		{
			Draw();
		}

		public void RemotePaintGL()
		{
			Draw();
		}


		#endregion
	}
}


