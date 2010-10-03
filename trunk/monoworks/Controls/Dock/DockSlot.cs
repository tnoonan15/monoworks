using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using gl = Tao.OpenGl.Gl;

using MonoWorks.Rendering;
using MonoWorks.Base;

namespace MonoWorks.Controls.Dock
{
	/// <summary>
	/// An object representing a potential place for a dockable scene to go.
	/// </summary>
	public class DockSlot : Overlay
	{
		/// <summary>
		/// Create a slot for the given container and index.
		/// </summary>
		public DockSlot(DockContainer container, int index)
		{
			Origin = new Coord();
			Size = new Coord();
			Container = container;
			Index = index;
		}

		/// <summary>
		/// The origin of the rectangle outlining the slot.
		/// </summary>
		public Coord Origin { get; private set; }

		/// <summary>
		/// The size of the rectangle outlining the slot.
		/// </summary>
		public Coord Size { get; private set; }

		/// <summary>
		/// The container that has the slot.
		/// </summary>
		public DockContainer Container { get; set; }

		/// <summary>
		/// The index of the container that the slot represents.
		/// </summary>
		public int Index { get; private set; }

		protected override bool HitTest(Base.Coord pos)
		{
			return false;
		}

		public override void RenderOverlay(Scene scene)
		{
			base.RenderOverlay(scene);

			scene.Lighting.Disable();
			gl.glLineWidth(3f);
			gl.glBegin(gl.GL_LINE_LOOP);
			gl.glColor3f(1.0f, 0f, 0f);
			gl.glVertex2d(Origin.X, Origin.Y);
			gl.glVertex2d(Origin.X + Size.X, Origin.Y);
			gl.glVertex2d(Origin.X + Size.X, Origin.Y + Size.Y);
			gl.glVertex2d(Origin.X, Origin.Y + Size.Y);
			gl.glEnd();

			gl.glBegin(gl.GL_QUADS);
			gl.glColor4f(1.0f, 0f, 0f, 0.5f);
			gl.glVertex2d(Origin.X, Origin.Y);
			gl.glVertex2d(Origin.X + Size.X, Origin.Y);
			gl.glVertex2d(Origin.X + Size.X, Origin.Y + Size.Y);
			gl.glVertex2d(Origin.X, Origin.Y + Size.Y);
			gl.glEnd();
		}
	}
}
