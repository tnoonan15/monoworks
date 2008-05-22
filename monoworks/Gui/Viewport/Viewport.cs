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
	
	// Dictionary that maps the mouse event mask to an interaction action.
	using MouseMap = Dictionary<int, MouseAction>;
		
	
	// Dictionary that maps the mouse event mask to a cursor.
	using CursorMap = Dictionary<MouseAction, QCursor>;
	
	
	// Dictionary that maps the scroll event mask to an interaction action.
	using ScrollMap = Dictionary<long, ScrollAction>;
	
	
	/// <summary>
	/// Possible mouse interaction actions.
	/// </summary>
	public enum MouseAction :short {NONE, PAN, ZOOM, DOLLY, ROTATE, SPIN};
	
	
	/// <summary>
	/// Possible scroll actions.
	/// </summary>
	public enum ScrollAction :short {NONE, DOLLY, VERTICAL_PAN, HORIZONTAL_PAN};
	
	
	/// <summary>
	/// Custom signals for the viewport.
	/// </summary>
	public interface IViewportSignals
	{
		[Q_SIGNAL]
		void SelectionChanged();
	}
	
	
	/// <summary>
	/// The Viewport class represents a viewable area for rendering models,
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

			// initialize the render manager
			renderManager = new RenderManager();
			
			// initially not picking
			IsPicking = false;
			
			this.Format().SetDoubleBuffer(true);
			this.Format().SetSampleBuffers(true);
//			this.SetAttribute(Qt.WidgetAttribute.WA_NoSystemBackground);
		}
		
		
		/// <summary>
		/// Constructs the viewport and sets the document.
		/// </summary>
		/// <param name="document"> A <see cref="Document"/>. </param>
		public Viewport(Document document) : this()
		{
			this.document = document;
		}
		
		/// <value>
		/// Emits custom signals.
		/// </value>
		protected new IViewportSignals Emit
		{ 
			get { return (IViewportSignals)Q_EMIT; }
		}
		
#region Document
		
		/// <value>
		/// The document associated with this viewport.
		/// </value>
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
		
		/// <value>
		/// The cursor map.
		/// </value>
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
		
		double currentX;
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
		
		/// <value>
		/// Time of the last rendering.
		/// </value>
		protected DateTime lastExposed;
		
		
		protected RenderManager renderManager;
		/// <value>
		/// The viewport render manager.
		/// </value>
		public RenderManager RenderManager
		{
			get {return renderManager;}
		}
		
		
		protected bool isPicking;
		/// <summary>
		/// If true, the mouse interaction is used for picking objects instead of interactions.
		/// </summary>
		public bool IsPicking
		{
			get {return isPicking;}
			set {isPicking = value;}
		}
		
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
//			int mask = (int)evt.Button() | (int)evt.Modifiers();
			int mask = (int)evt.Button();
				
			if (IsPicking) // if the mouse press should be used to pick objects
			{	
				double[] modelViewMatrix = new double[16];
				double[] projectionMatrix = new double[16];
				double frontX, frontY, frontZ;
				double backX, backY, backZ;
				
				gl.glGetDoublev(gl.GL_MODELVIEW_MATRIX, modelViewMatrix);
				gl.glGetDoublev(gl.GL_PROJECTION_MATRIX, projectionMatrix);

				lastY = Height() - lastY;
				
				
				glu.gluUnProject(lastX, lastY, 0.0, modelViewMatrix, projectionMatrix, 
				                 new int[]{0, 0, Width(), Height()}, out frontX, out frontY, out frontZ);
				glu.gluUnProject(lastX, lastY, 1.0, modelViewMatrix, projectionMatrix, 
				                 new int[]{0, 0, Width(), Height()}, out backX, out backY, out backZ);
				
//				Console.WriteLine("pick from {0}, {1}, {2} to {3}, {4}, {5}", frontX, frontY, frontZ, backX, backY, backZ);
				
				// construct the hit vectors
				mwb.Vector v1 = new mwb.Vector(frontX, frontY, frontZ);
				mwb.Vector v2 = new mwb.Vector(backX, backY, backZ);
				
				// perform the hit test
				if (document.HitTest(v1, v2))
					this.Emit.SelectionChanged();

				Console.WriteLine("viewport hit");
				
				// if it was a right click, create a context menu
				if (evt.Button() == MouseButton.RightButton)
				{
					if (document.LastSelected == null) // nothing was selected
					{
						Console.WriteLine("nothing selected");
						ViewportMenu menu = new ViewportMenu(this);
						menu.Popup(MapToGlobal(evt.Pos()));
					}
					else // something was selected
					{
						Console.WriteLine("{0} selected", document.LastSelected.Name);
						EntityMenu menu = new EntityMenu(this, document.LastSelected);
						menu.Popup(MapToGlobal(evt.Pos()));
					}
				}
			}				
			else // the mouse press should be used for interaction
			{				
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
					
			UpdateGL();

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
//			ScrollAction action = scrollMap[evt.Buttons()];
			ScrollAction action = scrollMap[0];
			
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
			
				
		
#region Views
		
		/// <summary>
		/// Sets the camera to the standard view.
		/// </summary>
		[Q_SLOT("StandardView()")]
		public void StandardView()
		{
			camera.StandardView();
		}
		
		/// <summary>
		/// Sets the camera to the front view.
		/// </summary>
		[Q_SLOT("FrontView()")]
		public void FrontView()
		{
			camera.FrontView();
		}
		
		/// <summary>
		/// Sets the camera to the back view.
		/// </summary>
		[Q_SLOT("BackView()")]
		public void BackView()
		{
			camera.BackView();
		}
		
		/// <summary>
		/// Sets the camera to the top view.
		/// </summary>
		[Q_SLOT("TopView()")]
		public void TopView()
		{
			camera.TopView();
		}
		
		/// <summary>
		/// Sets the camera to the bottom view.
		/// </summary>
		[Q_SLOT("BottomView()")]
		public void BottomView()
		{
			camera.BottomView();
		}
		
		/// <summary>
		/// Sets the camera to the right view.
		/// </summary>
		[Q_SLOT("RightView()")]
		public void RightView()
		{
			camera.RightView();
		}
		
		/// <summary>
		/// Sets the camera to the left view.
		/// </summary>
		[Q_SLOT("LeftView()")]
		public void LeftView()
		{
			camera.LeftView();
		}
		
#endregion
		
		
		
#region Rendering
		
		/// <value>
		/// Rubberband object.
		/// </value>
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
			PaintGL();
		}

		
		
		/// <summary>
		/// Initializes the OpenGL rendering.
		/// </summary>
		protected override void InitializeGL()
		{
			RenderManager.SetupSolidMode();
			
			gl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);			// white Background
			gl.glClearDepth(1.0f);								// Depth Buffer Setup
			gl.glEnable(gl.GL_DEPTH_TEST);						// Enables Depth Testing
			gl.glDepthFunc(gl.GL_LEQUAL);						// The Type Of Depth Test To Do
			// Really Nice Perspective Calculations
			gl.glHint(gl.GL_PERSPECTIVE_CORRECTION_HINT, gl.GL_NICEST);
			
//			gl.glEnable(gl.GL_BLEND);
//			gl.glBlendFunc(gl.GL_SRC_ALPHA, gl.GL_DST_ALPHA);
			gl.glEnable(gl.GL_LINE_SMOOTH);
			
			// enable color tracking
			gl.glEnable(gl.GL_COLOR_MATERIAL);
			// set material properties which will be assigned by glColor
			gl.glColorMaterial(gl.GL_FRONT_AND_BACK, gl.GL_AMBIENT_AND_DIFFUSE);

			// enable polygon offset so wireframes are displayed correctly
			gl.glPolygonOffset(1f, 1f);
			
			// enable the lighting
//			gl.glEnable(gl.GL_LIGHTING);
			gl.glEnable(gl.GL_LIGHT0);
//			gl.glEnable(gl.GL_LIGHT1);
//			gl.glEnable(gl.GL_LIGHT2);
//			gl.glEnable(gl.GL_LIGHT3);
			
			// initialize Qt overlay
//			image = new QImage(Width(), Height(), QImage.Format.Format_ARGB32_Premultiplied);
//			image.Fill(Qt.QRgba(0, 0, 0, 127));
			
		}
				
		/// <summary>
		/// Initialize drawing.
		/// Calls InitializeGL().
		/// </summary>
		public void Initialize()
		{
			InitializeGL();
			UpdateGL();
		}
				
		
		/// <summary>
		/// Repaints the OpenGL surface.
		/// </summary>
		protected override void PaintGL()
		{
			base.PaintGL();
			
			// Clear The Screen And The Depth Buffer
			gl.glClear(gl.GL_COLOR_BUFFER_BIT | gl.GL_DEPTH_BUFFER_BIT);
			gl.glClearColor(1.0f, 1.0f, 1.0f, 0.0f);			// white Background
			
			// place the lights
			float lightDist = 10f;
			float[] lightAmbient = new float[]{0.2f, 0.2f, 0.2f};
			float[] lightDiffuse = new float[]{0.5f, 0.5f, 0.5f};
			float[] lightSpecular = new float[]{0.1f, 0.1f, 0.1f};
			float[] lightPos0 = new float[]{lightDist, lightDist, -lightDist,2f};
			gl.glLightfv(gl.GL_LIGHT0, gl.GL_POSITION, lightPos0);
			gl.glLightfv(gl.GL_LIGHT0, gl.GL_AMBIENT, lightAmbient);
			gl.glLightfv(gl.GL_LIGHT0, gl.GL_DIFFUSE, lightDiffuse);
			gl.glLightfv(gl.GL_LIGHT0, gl.GL_SPECULAR, lightSpecular);
			
			document.Render(this);

			// render the rubber band
			rubberBand.Render(this);
			
			// keep track of frame rate
			double frameRate = 10000000.0 / (double)(DateTime.Now.Ticks - lastExposed.Ticks);
			lastExposed = DateTime.Now;
			gl.glColor3f(0f, 0f, 0f);
			this.RenderText(5, Height()-5, String.Format("{0:f2} fps", frameRate));
			
			gl.glFlush();
			
			SwapBuffers();
			
			// test out the Qt overlay
//			QPainter painter = new QPainter();
//			painter.Begin(this);
//			painter.SetRenderHint( QPainter.RenderHint.Antialiasing );
//			painter.End();
		}
		
				
		/// <summary>
		/// Paint to the current context.
		/// Calls UpdateGL().
		/// </summary>
		[Q_SLOT]
		public void Paint()
		{
			UpdateGL();
		}
		


		
		
#endregion
		
	}
}