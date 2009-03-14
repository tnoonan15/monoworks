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

using System;
using System.Collections.Generic;
using swf = System.Windows.Forms;
using sd = System.Drawing;

using Tao.Platform.Windows;


using MonoWorks.Base;
using MonoWorks.Rendering;

using MonoWorks.Rendering.Events;
using MonoWorks.Rendering.Interaction;
using MonoWorks.Framework;

namespace MonoWorks.GuiWpf
{

	/// <summary>
	/// Viewport for WPF.
	/// </summary>
	public class SwfViewportAdapter : SimpleOpenGlControl, IViewportAdapter
	{
		public SwfViewportAdapter()
			: base()
		{
			InitializeContexts();

			this.DoubleBuffered = false;

			// create the tooltip
			toolTip = new swf.ToolTip();
			toolTip.SetToolTip(this, "");

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
			get { return this.Size.Width; }
		}

		/// <summary>
		/// Rendering height.
		/// </summary>
		/// <returns></returns>
		public int HeightGL
		{
			get { return Size.Height; }
		}



		private swf.ToolTip toolTip;

		/// <summary>
		/// The tooltip on the viewport.
		/// </summary>
		public string ToolTip
		{
			set
			{
				toolTip.Show(value, this);
			}
		}

		/// <summary>
		/// Export the viewport to a bitmap in the given file name.
		/// </summary>
		/// <param name="fileName"></param>
		public void Export(string fileName)
		{
			sd.Bitmap bmp = new sd.Bitmap(this.ClientSize.Width, this.ClientSize.Height);
			sd.Imaging.BitmapData data =
			bmp.LockBits(this.ClientRectangle, sd.Imaging.ImageLockMode.WriteOnly,
			sd.Imaging.PixelFormat.Format24bppRgb);
			Tao.OpenGl.Gl.glReadPixels(0, 0, this.ClientSize.Width, this.ClientSize.Height, Tao.OpenGl.Gl.GL_BGR, Tao.OpenGl.Gl.GL_UNSIGNED_BYTE,
			data.Scan0);
			bmp.UnlockBits(data);
			bmp.RotateFlip(sd.RotateFlipType.RotateNoneFlipY);
			bmp.Save(fileName);
		}




#region Mouse Interaction



		/// <summary>
		/// Convenience method that converts a mouse event point into a proper viewport coord.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		protected Coord MouseToViewport(System.Drawing.Point point)
		{
			return new Coord((double)point.X, (double)(HeightGL - point.Y));
			//return new Coord((double)point.X, (double)point.Y);
		}


		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseDown(args);

			MouseButtonEvent evt = new MouseButtonEvent(MouseToViewport(args.Location),
									SwfExtensions.ButtonNumber(args.Button),
									SwfExtensions.GetModifier(ModifierKeys));
			Viewport.OnButtonPress(evt);

			PaintGL();
		}

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs args)
		{
			base.OnMouseUp(args);

            MouseButtonEvent evt = new MouseButtonEvent(MouseToViewport(args.Location),
									SwfExtensions.ButtonNumber(args.Button),
									SwfExtensions.GetModifier(ModifierKeys));
			Viewport.OnButtonRelease(evt);

			PaintGL();
		}

		protected override void OnMouseMove(swf.MouseEventArgs args)
		{
			base.OnMouseMove(args);

			MouseEvent evt = new MouseEvent(MouseToViewport(args.Location),
									SwfExtensions.GetModifier(ModifierKeys));
			Viewport.OnMouseMotion(evt);

			PaintGL();
		}


		protected override void OnMouseWheel(swf.MouseEventArgs args)
		{
			base.OnMouseWheel(args);

			WheelDirection direction = WheelDirection.Up;
			if (args.Delta < 0)
				direction = WheelDirection.Down;
			MouseWheelEvent evt = new MouseWheelEvent(direction, InteractionModifier.None);
			Viewport.OnMouseWheel(evt);

			PaintGL();
		}

		protected override void OnMouseDoubleClick(swf.MouseEventArgs args)
		{
			base.OnMouseDoubleClick(args);

			MouseButtonEvent evt = new MouseButtonEvent(MouseToViewport(args.Location),
									SwfExtensions.ButtonNumber(args.Button),
									InteractionModifier.None, ClickMultiplicity.Double);
			Viewport.OnButtonPress(evt);

			PaintGL();
		}


#endregion


#region Keyboard Interaction

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
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

			KeyEvent evt = new KeyEvent(val, (InteractionModifier)mod);
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

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)
		{
			//base.OnPaintBackground(e);
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
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

		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// Viewport
			// 
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Name = "Viewport";
			this.Size = new System.Drawing.Size(0, 0);
			this.ResumeLayout(false);

		}

	}
}
