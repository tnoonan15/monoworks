// Viewport.cs - MonoWorks Project
//
// Copyright (C) 2008 Andy Selvig
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;

using Qyoto;

using gl = Tao.OpenGl.Gl;
using glu = Tao.OpenGl.Glu;

using mwb = MonoWorks.Base;
using MonoWorks.Model;


namespace MonoWorks.Gui
{
	
	/// <summary>
	/// Dictionary that maps the mouse event mask to an interaction action.
	/// </summary>
	using MouseMap = Dictionary<int, MouseAction>;
		
	/// <summary>
	/// Dictionary that maps the mouse event mask to a cursor.
	/// </summary>
	using CursorMap = Dictionary<MouseAction, QCursor>;
	
	
	/// <summary>
	/// Dictionary that maps the scroll event mask to an interaction action.
	/// </summary>
	using ScrollMap = Dictionary<int, ScrollAction>;
	
	
	/// <summary>
	/// Possible mouse interaction actions.
	/// </summary>
	public enum MouseAction :short {NONE, PAN, ZOOM, DOLLY, ROTATE, SPIN};
	
	
	/// <summary>
	/// Possible scroll actions.
	/// </summary>
	public enum ScrollAction :short {NONE, DOLLY, VERTICAL_PAN, HORIZONTAL_PAN};
	
	
	/// <summary>
	/// The Viewport class represents a viewable area for rendering models,
	/// as well as a toolbar that controls viewing options.
	/// </summary>
	public class Viewport : QGLWidget, IViewport
	{

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Viewport() : base()
		{
			// ensure the resource manager is initialized
			ResourceManager.Initialize();
			
			camera = new Camera(this);
			mouseAction = MouseAction.NONE;

			// assign default cursor positions
			lastX = 0.0;
			lastY = 0.0;
			currentX = 0.0;
			currentY = 0.0;
			
			lastExposed = DateTime.Now;

			// create the rubber band
			rubberBand = new RubberBand();
			
			// initialize the mouse map
			mouseMap = new MouseMap();
			mouseMap[(int)Qt.MouseButton.LeftButton] = MouseAction.ROTATE;
			mouseMap[(int)Qt.MouseButton.MidButton] = MouseAction.ZOOM;
			mouseMap[(int)Qt.MouseButton.RightButton] = MouseAction.PAN;
			mouseMap[(int)Qt.MouseButton.MidButton | (int)Qt.Modifier.CTRL] = MouseAction.DOLLY;
			
			// create the cursor map
			cursorMap = new CursorMap();
			cursorMap[MouseAction.PAN] = new QCursor(Qt.CursorShape.OpenHandCursor);
			cursorMap[MouseAction.ZOOM] = ResourceManager.GetCursor("zoom");
			cursorMap[MouseAction.DOLLY] = ResourceManager.GetCursor("dolly");
			cursorMap[MouseAction.ROTATE] =  new QCursor(Qt.CursorShape.ClosedHandCursor);
			cursorMap[MouseAction.SPIN] = ResourceManager.GetCursor("spin");
			
			
			// initialize the scroll map
			scrollMap = new ScrollMap();
			scrollMap[0] = ScrollAction.DOLLY;
			scrollMap[(int)Qt.Modifier.CTRL] = ScrollAction.VERTICAL_PAN;
			scrollMap[(int)Qt.Modifier.SHIFT] = ScrollAction.HORIZONTAL_PAN;

			
			this.SetAttribute(Qt.WidgetAttribute.WA_NoSystemBackground);
		}
		
		
		/// <summary>
		/// Constructs the viewport and sets the document.
		/// </summary>
		/// <param name="document"> A <see cref="Document"/>. </param>
		public Viewport(Document document) : this()
		{
			this.document = document;
		}
		
		
#region Document
		
		protected Document document;		
		/// <value>
		/// The document associated with this viewport.
		/// </value>
		public Document Document
		{
			get {return document;}
			set {document = value;}
		}
		
#endregion

		
#region Attributes
		
		/// <value>
		/// The camera.
		/// </value>
		protected Camera camera;
		/// <value>
		/// Accesses the viewport camera.
		/// </value>
		public Camera Camera
		{
			get {return camera;}
			set {camera = value;}
		}
		

		protected MouseAction mouseAction;
		/// <value>
		/// The current mouse interaction action.
		/// </value>
		public MouseAction MouseAction
		{
			get {return mouseAction;}
		}
		
		protected CursorMap cursorMap;
		
		
		protected MouseMap mouseMap;
		/// <value>
		/// Maps mouse event masks to interaction actions.
		/// </value>
		public MouseMap MouseMap
		{
			get {return mouseMap;}
		}
		
		
		protected ScrollMap scrollMap;
		/// <value>
		/// Maps scroll event masks to interaction actions.
		/// </value>
		public ScrollMap ScrollMap
		{
			get {return scrollMap;}
		}
		
		
		protected double lastX;
		/// <value>
		/// The x value of the last mouse position.
		/// </value>
		public double LastX
		{
			get {return lastX;}
		}
		
		protected double lastY;
		/// <value>
		/// The y value of the last mouse position.
		/// </value>
		public double LastY
		{
			get {return lastY;}
		}
		
		
		protected double currentX;
		/// <value>
		/// The x value of the current mouse position.
		/// </value>
		public double CurrentX
		{
			get {return currentX;}
		}
		
		protected double currentY;
		/// <value>
		/// The y value of the current mouse position.
		/// </value>
		public double CurrentY
		{
			get {return currentY;}
		}
		
		
		protected DateTime lastExposed;
		
#endregion
		
		
#region Event Handlers
		
		/// <summary>
		/// Handles button press events.
		/// </summary>
		/// <param name="evt">The <see cref="QMouseEvent"/>. </param>
		protected override void MousePressEvent(QMouseEvent evt)
		{
			// get the position
			lastX = evt.X();
			lastY = evt.Y();
			
			// get the mask for this button event
			int mask = (int)evt.Button() | evt.Modifiers();
				
			// determine if the mask is present in the map
			if (mouseMap.ContainsKey(mask))
			{
				// store the current action
				mouseAction = mouseMap[mask];
				
				// set the override cursor
				if (cursorMap.ContainsKey(mouseAction))
				{
					QApplication.SetOverrideCursor(new QCursor(cursorMap[mouseAction]));
				}
				
				// special stuff for zooming
				if (mouseAction==MouseAction.ZOOM)
				{
					rubberBand.StartX = (int)lastX;
					rubberBand.StartY = (int)lastY;
					rubberBand.Enabled = true;
				}
			}

		}
			
		
		/// <summary>
		/// Handles mouse double-click events.
		/// </summary>
		/// <param name="evt">The <see cref="QMouseEvent"/>. </param>
		protected override void MouseDoubleClickEvent(QMouseEvent evt)
		{
			camera.Reset();
			UpdateGL();
		}
		
		/// <summary>
		/// Handles button release events.
		/// </summary>
		/// <param name="evt">The <see cref="QMouseEvent"/>. </param>	
		protected override void MouseReleaseEvent(QMouseEvent evt)
		{

			// special handling for zoom
			if (mouseAction==MouseAction.ZOOM)
			{
				camera.Zoom(rubberBand.StartX, rubberBand.StartY,
				              rubberBand.StopX, rubberBand.StopY);
			}
			
			mouseAction = MouseAction.NONE;
			rubberBand.Enabled = false;					
			
			// reset the override cursor
			QApplication.SetOverrideCursor(new QCursor(Qt.CursorShape.ArrowCursor));
			
			UpdateGL();
		}
		

		/// <summary>
		/// Handles mouse motion.
		/// </summary>
		/// <param name="evt">The <see cref="QMouseEvent"/>. </param>	
		protected override void MouseMoveEvent(QMouseEvent evt)
		{
			// get the new mouse coordinates
			currentX = evt.X();
			currentY = evt.Y();
			
			// decide what to do based on the mouse mode
			bool queueDraw = true;
			switch (mouseAction)
			{
			case MouseAction.DOLLY:
				OnDolly();
				break;
			case MouseAction.PAN:
				OnPan();
				break;
			case MouseAction.ROTATE:
				OnRotate();
				break;
			case MouseAction.ZOOM:
				OnZoom();
				break;
			case MouseAction.NONE:
				queueDraw = false;
				break;
			}
			lastX = currentX;
			lastY = currentY;
			if (queueDraw)
				UpdateGL();
		}
		
		
		/// <summary>
		/// Handles mouse scrolling.
		/// </summary>
		/// <param name="evt">The <see cref="WheelEvent"/>. </param>
		protected override void WheelEvent( QWheelEvent evt)
		{
			
			// determine what action to do based on the scroll map
			ScrollAction action = scrollMap[evt.Modifiers()];
			
			// decide what to do based on the action
			switch (action)
			{
			case ScrollAction.DOLLY:
				if (evt.Delta()<0)
					camera.DollyOut();
				else
					camera.DollyIn();
				break;
			case ScrollAction.VERTICAL_PAN:
				if (evt.Delta()<0)
					camera.PanDown();
				else
					camera.PanUp();
				break;
			case ScrollAction.HORIZONTAL_PAN:
				if (evt.Delta()<0)
					camera.PanRight();
				else
					camera.PanLeft();
				break;
			}
			UpdateGL();	
		}
		
		
#endregion
		
			
			
#region Interaction
				
		/// <summary>
		/// Callback for dolly motion.
		/// </summary>
		protected void OnDolly()
		{
			camera.Dolly((currentY-lastY) / (double)Height() * 5.0);
		}
		
		
		/// <summary>
		/// Callback for pan motion.
		/// </summary>
		protected void OnPan()
		{
			camera.Pan(currentX-lastX, currentY-lastY);
		}
		
		
		/// <summary>
		/// Callback for rotate motion.
		/// </summary>
		protected void OnRotate()
		{
			camera.Rotate(currentX-lastX, currentY-lastY);
		}
		
		
		/// <summary>
		/// Callback for zoom motion.
		/// </summary>
		protected void OnZoom()
		{
			rubberBand.StopX = currentX;
			rubberBand.StopY = currentY;
		}
					
#endregion
			
				
		
#region Rendering
		
		protected RubberBand rubberBand;
		/// <value>
		/// The rubber band used for zooming and seleting.
		/// </value>
		public RubberBand RubberBand
		{
			get {return rubberBand;}
			set {rubberBand = value;}
		}
		
		
		/// <summary>
		/// OpenGL resize event handler.
		/// </summary>
		protected override void ResizeGL(int width, int height)
		{							
			camera.Configure();
		}

		
//		QImage image;
		
		protected override void InitializeGL()
		{
			gl.glShadeModel(gl.GL_SMOOTH);
//			gl.glShadeModel(gl.GL_FLAT);
			gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);			// Black Background
			gl.glClearDepth(1.0f);								// Depth Buffer Setup
			gl.glEnable(gl.GL_DEPTH_TEST);						// Enables Depth Testing
			gl.glDepthFunc(gl.GL_LEQUAL);						// The Type Of Depth Test To Do
			// Really Nice Perspective Calculations
			gl.glHint(gl.GL_PERSPECTIVE_CORRECTION_HINT, gl.GL_NICEST);	

			int randomList = gl.glGenLists(1);
			Console.WriteLine("random list: {0}", randomList);
			
			// initialize Qt overlay
//			image = new QImage(Width(), Height(), QImage.Format.Format_ARGB32_Premultiplied);
//			image.Fill(Qt.QRgba(0, 0, 0, 127));
			
		}
				
		
		/// <summary>
		/// Repaints the OpenGL surface.
		/// </summary>
		protected override void PaintGL()
		{
			
			// Clear The Screen And The Depth Buffer
			gl.glClear(gl.GL_COLOR_BUFFER_BIT | gl.GL_DEPTH_BUFFER_BIT);
			
			document.Render(this);

			// render the rubber band
			rubberBand.Render(this);
			
			// keep track of frame rate
			double frameRate = 10000000.0 / (double)(DateTime.Now.Ticks - lastExposed.Ticks);
			lastExposed = DateTime.Now;
			gl.glColor3f(1f, 1f, 1f);
			this.RenderText(5, Height()-5, String.Format("{0:f2} fps", frameRate));
			
			
			// test out the Qt overlay
//			QPainter painter = new QPainter();
//			painter.Begin(this);
//			painter.SetRenderHint( QPainter.RenderHint.Antialiasing );
//			painter.End();
		}
		
#endregion
		
	}
}