//   FillGradient.cs - MonoWorks Project
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
using System.Collections.Generic;
using System.Xml;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Base;

namespace MonoWorks.Rendering
{
	/// <summary>
	/// The direction that a gradient points across a rectangle.
	/// </summary>
	public enum GradientDirection { E2W, N2S, NE2SW, SE2NW };

	/// <summary>
	/// A simple gradient used to fill objects.
	/// </summary>
	public class FillGradient : IFill
	{
		/// <summary>
		/// Default constructor (North-South) direction.
		/// </summary>
		public FillGradient(Color startColor, Color stopColor)
		{
			this.startColor = startColor;
			this.stopColor = stopColor;
		}

		
		public object Clone()
		{
			FillGradient fg = new FillGradient(startColor, stopColor);
			fg.Direction = direction;
			fg.IsInverted = isInverted;
			return fg;
		}


		public override string ToString()
		{
			return String.Format("Fill Gradient start={0}, stop={1}", startColor, stopColor);
		}
		
		protected Color startColor;
		/// <summary>
		/// The color at the beginning of the gradient.
		/// </summary>
		public Color StartColor
		{
			get { return startColor; }
			set { startColor = value; }
		}

		protected Color stopColor;
		/// <summary>
		/// The color at the end of the gradient.
		/// </summary>
		public Color StopColor
		{
			get { return stopColor; }
			set { stopColor = value; }
		}

		protected GradientDirection direction = GradientDirection.SE2NW;
		/// <summary>
		/// The direction that the gradient uses to fill a rectangle.
		/// </summary>
		public GradientDirection Direction
		{
			get { return direction; }
			set { direction = value; }
		}


		protected bool isInverted = false;
		/// <summary>
		/// Whether the gradient direction is inverted
		/// (the start color is actually at the stop position).
		/// </summary>
		public bool IsInverted
		{
			get { return isInverted; }
			set { isInverted = value; }
		}



		public void DrawRectangle(Coord pos, Coord size)
		{
			// flip colors, if necessary
			Color start = startColor;
			Color stop = stopColor;
			if (IsInverted)
			{
				start = stopColor;
				stop = startColor;
			}

			gl.glBegin(gl.GL_QUADS);
			Color mid;
			switch (Direction)
			{
			case GradientDirection.N2S:
				stop.Setup();
				gl.glVertex2d(pos.X, pos.Y);
				gl.glVertex2d(pos.X + size.X, pos.Y);
				start.Setup();
				gl.glVertex2d(pos.X + size.X, pos.Y + size.Y);
				gl.glVertex2d(pos.X, pos.Y + size.Y);
				break;
			case GradientDirection.E2W:
				stop.Setup();
				gl.glVertex2d(pos.X, pos.Y);
				gl.glVertex2d(pos.X, pos.Y + size.Y);
				start.Setup();
				gl.glVertex2d(pos.X + size.X, pos.Y + size.Y);
				gl.glVertex2d(pos.X + size.X, pos.Y);
				break;
			case GradientDirection.NE2SW:
				mid = Color.Interp(start, stop);
				start.Setup();
				gl.glVertex2d(pos.X + size.X, pos.Y + size.Y);
				mid.Setup();
				gl.glVertex2d(pos.X + size.X, pos.Y);
				stop.Setup();
				gl.glVertex2d(pos.X, pos.Y);
				mid.Setup();
				gl.glVertex2d(pos.X, pos.Y + size.Y);
				break;
			case GradientDirection.SE2NW:
				mid = Color.Interp(start, stop);
				start.Setup();
				gl.glVertex2d(pos.X + size.X, pos.Y);
				mid.Setup();
				gl.glVertex2d(pos.X, pos.Y);
				stop.Setup();
				gl.glVertex2d(pos.X, pos.Y + size.Y);
				mid.Setup();
				gl.glVertex2d(pos.X + size.X, pos.Y + size.Y);
				break;
			}
			gl.glEnd();
		}



		/// <summary>
		/// Draws a gradient triangle in the given corner.
		/// </summary>
		public void DrawCorner(Coord pos, Coord size, Corner corner)
		{
			gl.glBegin(gl.GL_TRIANGLES);
			switch (corner)
			{
			case Corner.NE:
				stopColor.Setup();
				gl.glVertex2d(pos.X, pos.Y + size.Y);
				gl.glVertex2d(pos.X + size.X, pos.Y);
				startColor.Setup();
				gl.glVertex2d(pos.X + size.X, pos.Y + size.Y);
				break;
			default:
				throw new NotImplementedException();
			}
			gl.glEnd();
		}
		
		
#region XML Loading
		
		/// <summary>
		/// Creates a FillGradient from an XML element.
		/// </summary>
		/// <param name="reader"> </param>
		/// <returns> </returns>
		/// <remarks>The element should have a start and stop attributes 
		/// that resolve to global colors, and an optional direction.</remarks>
		public static FillGradient FromXml(XmlReader reader)
		{
			// get the colors
			string startName = reader.GetRequiredString("start");
			string stopName = reader.GetRequiredString("stop");
			FillGradient grad = new FillGradient(ColorManager.Global[startName], ColorManager.Global[stopName]);
			
			// get the direction (optional)
			string dirString = reader.GetAttribute("direction");
			if (dirString != null)
				grad.Direction =(GradientDirection)Enum.Parse(typeof(GradientDirection), dirString);	
			
			// get isInverted (optional)
			string invString = reader.GetAttribute("inverted");
			if (invString != null)
				grad.IsInverted = Boolean.Parse(invString);	
			
			return grad;
		}
		
		
#endregion

	}
}
