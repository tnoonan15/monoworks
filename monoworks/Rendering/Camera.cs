//   Camera.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;
using glu = Tao.OpenGl.Glu;
using MonoWorks.Base;


namespace MonoWorks.Rendering
{
	
	/// <summary>
	/// The camera class stores the viewing location, up-vector, and view center. 
	/// </summary>
	public class Camera
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Camera(IViewport theViewport)
		{
			viewport = theViewport;
			
			// set default positioning
			pos = new Vector(0.0, 0.0, 7.0);
			upVec = new Vector(0.0, 1.0, 0.0);
			center = new Vector();
			
			// set default field of view
			fov = new Angle();
			fov["deg"] = 45.0;
			
			// set default interaction factors
			dollyFactor = 0.15;
			panFactor = 0.05;
			
			// set default animation settings
			animateNumSteps = 4;
			animateDuration = 1000.0;
			animateTimer = new System.Timers.Timer(animateDuration / (double)animateNumSteps);
			animateTimer.Elapsed += OnAnimate;
			animateTimer.Enabled = false;
		}
		

#region Attributes
		
		protected IViewport viewport;
//		/// <value>
//		/// The viewport connected to this camera.
//		/// </value>
//		public Viewport Viewport
//		{
//			get{ return viewport;}
//			set {viewport = value;}
//		}
		
		
		protected Angle fov;
		/// <value>
		/// The vertical field of view of the camera.
		/// Automatically reconfigures the camera after assignment.
		/// </value>
		public Angle FoV
		{
			get {return fov;}
			set 
			{
				fov = value;
				Configure();
			}
		}
		
		
		/// <value>
		/// The position of the camera.
		/// </value>
		protected Vector pos;
		/// <value>
		/// Accesses the position of the camera.
		/// </value>
		public Vector Position
		{
			get	{return pos;}
			set	{pos = value;}
		}
		
		
		/// <value>
		/// The up-vector of the camera.
		/// </value>
		protected Vector upVec;
		/// <value>
		/// Accesses the up-vector of the camera.
		/// The vector normalized and forced orthatgonal to the 
		/// view direction when assigned.
		/// </value>
		public Vector UpVector
		{
			get {return upVec;}
			set 
			{
				upVec = value.Normalize();
				RecomputeUpVector();
			}
		}
		
		
		/// <value>
		/// The point the camera is looking at.
		/// </value>
		protected Vector center;
		/// <value>
		/// Accesses the point the camera is looking at.
		/// </value>
		public Vector Center
		{
			get	{return center;}
			set	{center = value;}
		}
		
		
#endregion
		

#region Configuration and Placement
		
		/// <summary>
		/// Configures the viewport based on its width and height.
		/// </summary>
		/// </param>
		public virtual void Configure()
		{						
			// get the width and height
			int width = viewport.Width();
			int height = viewport.Height();
			
			// prevent divide by zero
			if (height==0)	
				height=1;	
			
			gl.glViewport (0, 0, width, height);
			
			// initialize the projection matrix
			gl.glMatrixMode(gl.GL_PROJECTION);
			gl.glLoadIdentity();

			// Calculate The Aspect Ratio Of The Window
			glu.gluPerspective((float)fov["deg"], (float)width/(float)height, 0.1f, 20.0f);
		}
		
		
		/// <summary>
		/// Places the camera in it's position of the next drawing operation.
		/// </summary>
		public virtual void Place()
		{
			// reset the current Modelview matrix
			gl.glMatrixMode(gl.GL_MODELVIEW);				
			gl.glLoadIdentity();
			
			// place the camera
			glu.gluLookAt(pos[0], pos[1], pos[2], 
			              center[0], center[1], center[2],
			              upVec[0], upVec[1], upVec[2]);
			
//			float[] lightPos1 = new float[]{-lightDist, -lightDist, -lightDist,2f};
//			gl.glLightfv(gl.GL_LIGHT1, gl.GL_POSITION, lightPos1);
//			float[] lightPos2 = new float[]{lightDist, lightDist, -lightDist,2f};
//			gl.glLightfv(gl.GL_LIGHT2, gl.GL_POSITION, lightPos2);
//			float[] lightPos3 = new float[]{-lightDist, -lightDist, lightDist,2f};
//			gl.glLightfv(gl.GL_LIGHT3, gl.GL_POSITION, lightPos3);
		}
		
		
		/// <summary>
		/// Resets the camera to the default position.
		/// </summary>
		public virtual void Reset()
		{
//				AnimateTo(new Vector(0.0, 0.0, 7.0), new Vector(0.0, 0.0, 0.0), new Vector(0.0, 1.0, 0.0));
			pos = new Vector(0.0, 0.0, 7.0);
			center = new Vector(0.0, 0.0, 0.0);
			upVec = new Vector(0.0, 1.0, 0.0);
		}
		
		
		/// <summary>
		/// Places the camera for overla drawing.
		/// </summary>
		public virtual void PlaceOverlay()
		{
			// reset the current Modelview matrix
			gl.glMatrixMode(gl.GL_MODELVIEW);				
			gl.glLoadIdentity();
			
			// translate the camera so that something drawn in the
			// x-y plane maps directly to the screen
			gl.glTranslatef(-0.5f, -0.5f, -1.2f);
			gl.glScalef(1f/(float)viewport.Width(), 1f/(float)viewport.Height(), 1.0f);
			
		}
		
		
		/// <summary>
		/// Recomputes the up vector to ensure it is orthographic to the viewing direction.
		/// </summary>
		public virtual void RecomputeUpVector()
		{
			Vector direction = (center-pos).Normalize();
			Vector lateral = direction.Cross( upVec);
			upVec = lateral.Cross( direction);
		}
		
		
		/// <value>
		/// The scaling from the viewport to world coordinates.
		/// </value>
		public double ViewportToWorldScaling
		{
			get
			{
				return fov.Sin() * Math.Abs((center-pos).Magnitude)/ (double)viewport.Height();
			}
		}
			
		
#endregion
		
		
		
#region Dollying
		
		protected double dollyFactor;
		/// <value>
		/// The default factor by which dollies are performed.
		/// The DollyFactor is the fraction of the distance from the
		/// camera position to the center that the camera will travel.
		/// </value>
		public double DollyFactor
		{
			get {return dollyFactor;}
			set {dollyFactor = value;}
		}

		/// <summary>
		/// Dollies in by DollyFactor.
		/// </summary>
		public void DollyIn()
		{
			Dolly(-dollyFactor);
		}

		/// <summary>
		/// Dollies out by DollyFactor.
		/// </summary>
		public void DollyOut()
		{
			Dolly(dollyFactor);
		}
		
		/// <summary>
		/// Dollies by the given factor.
		/// The factor should be positive for dollying out
		/// and negative for dollying in.
		/// </summary>
		/// <param name="factor"> The dolly factor. </param>
		public void Dolly(double factor)
		{
			Vector travel = (pos - center) * factor;
			pos = travel + pos;
		}		
		
#endregion
		
		
#region Panning
		
		protected double panFactor;
		/// <value>
		/// The default factor by which pans are performed.
		/// The PanFactor is multiplied by the height to determine how far to pan.
		/// </value>
		public double PanFactor
		{
			get {return panFactor;}
			set {panFactor = value;}
		}
		
		/// <summary>
		/// Pans the camera by the given deltas.
		/// </summary>
		/// <param name="dx"> The delta in the x dimension. </param>
		/// <param name="dy"> The delta in the y dimension. </param>
		public void Pan(double dx, double dy)
		{
			// determine the scaling from view to world coordinates
			double scaling = ViewportToWorldScaling;
			
			// compute the view up (y) component of the tranformation
			Vector yPan = upVec * dy * scaling;
			
			// compute the lateral (x) compoment of the transformation
			Vector xPan = upVec.Cross( (center-pos).Normalize() ) * dx * scaling;
			
			// add the x and y components to the position and center
			pos += (xPan+yPan);
			center += (xPan+yPan);
		}	
		
		/// <summary>
		/// Pan to the left by PanFactor.
		/// </summary>
		public virtual void PanLeft()
		{
			Pan(-panFactor*(double)viewport.Height(), 0);
		}
		
		/// <summary>
		/// Pan to the right by PanFactor.
		/// </summary>
		public virtual void PanRight()
		{
			Pan(panFactor*(double)viewport.Height(), 0);
		}
		
		/// <summary>
		/// Pan up by PanFactor.
		/// </summary>
		public virtual void PanUp()
		{
			Pan(0, panFactor*(double)viewport.Height());
		}
		
		/// <summary>
		/// Pan down by PanFactor.
		/// </summary>
		public virtual void PanDown()
		{
			Pan(0, -panFactor*(double)viewport.Height());
		}
		
#endregion
		
		
#region Rotating
		
		/// <summary>
		/// Rotate the camera by the given deltas.
		/// </summary>
		/// <param name="dx"> The delta in the x dimension. </param>
		/// <param name="dy"> The delta in the y dimension. </param>
		public void Rotate(double dx, double dy)
		{
			// determine the scaling from view to world coordinates
			double scaling = ViewportToWorldScaling * 6;
			
			// compute the lateral (x) compoment of the transformation
			Vector xRotate = upVec.Cross( (center-pos).Normalize() ) * dx * scaling;
			
			// compute the view up (y) component of the tranformation
			Vector yRotate = upVec * dy * scaling;
			
			// compute the current distance from the center
			double currentDistance = (pos-center).Magnitude;
			
			// add the x and y components to the position and center
			pos += (xRotate+yRotate);
			
			// maintain the distance to the center
			Vector distanceVec = pos-center;
			double newDistance = distanceVec.Magnitude;
			pos = distanceVec * currentDistance / newDistance + center;
			
			// ensure the up vector is still orthagonal to the viewing direction
			RecomputeUpVector();
		}		
		
#endregion
		
		
#region Zooming
	
		/// <summary>
		/// Zooms to screen rectangle defined by the given start and stop points.
		/// </summary>
		public virtual void Zoom(double startX, double startY, double stopX, double stopY)
		{						
			// pan the camera so it's centered on the new viewing area
			double newCenterX = (startX+stopX - (double)viewport.Width())/2.0; 
			double newCenterY = (startY+stopY - (double)viewport.Height())/2.0; 
			Pan(-newCenterX, -newCenterY);
			
			// determine the relative height and width of the new viewport
			double newW = Math.Abs(startX-stopX) / (double)viewport.Width();
			double newH = Math.Abs(startY-stopY) / (double)viewport.Height(); 
			double newRatio = Math.Max(newH, newW);		
			
			// move the camera closer to the center
			Vector distance = pos - center;
			pos = center + distance*newRatio;
		}
		
		
#endregion


		
#region Views
		
		/// <summary>
		/// Sets the camera to the standard view.
		/// </summary>
		public void StandardView()
		{
			
		}
		
		/// <summary>
		/// Sets the camera to the front view.
		/// </summary>
		public void FrontView()
		{
			
		}
		
		/// <summary>
		/// Sets the camera to the back view.
		/// </summary>
		public void BackView()
		{
			
		}
		
		/// <summary>
		/// Sets the camera to the top view.
		/// </summary>
		public void TopView()
		{
			
		}
		
		/// <summary>
		/// Sets the camera to the bottom view.
		/// </summary>
		public void BottomView()
		{
			
		}
		
		/// <summary>
		/// Sets the camera to the right view.
		/// </summary>
		public void RightView()
		{
			
		}
		
		/// <summary>
		/// Sets the camera to the left view.
		/// </summary>
		public void LeftView()
		{
			
		}
		
#endregion
		
		
		
#region Animation
	
		protected double animateDuration;
		/// <value>
		/// Duration (in milliseconds) of animation events.
		/// </value>
		public double AnimateDuration
		{
			get {return animateDuration;}
			set {animateDuration = value;}
		}

		protected int animateNumSteps, animateCount;
		protected Vector animateDiffPos, animateDiffCenter, animateDiffUpVec;
		System.Timers.Timer animateTimer;
		
		/// <summary>
		/// Animates the camera to the given position.
		/// </summary>
		/// <param name="pos"> The final position. </param>
		/// <param name="center"> The final center. </param>
		/// <param name="upVec"> the final up-vector.  </param>
		public virtual void AnimateTo(Vector pos, Vector center, Vector upVec)
		{
			animateDiffPos = (pos - pos) / (double)animateNumSteps;
			animateDiffCenter = (center - center) / (double)animateNumSteps;
			animateDiffUpVec = (upVec - upVec) / (double)animateNumSteps;
		
			animateCount = 0;
			
			animateTimer.Enabled = true;
		}
			
		/// <summary>
		/// Animation callback.
		/// </summary>
		protected virtual void OnAnimate(object o, System.Timers.ElapsedEventArgs args)
		{
			
			if (animateCount<animateNumSteps)
			{
				Console.WriteLine("animating");
				pos += animateDiffPos;
				center += animateDiffCenter;
				upVec += animateDiffUpVec;
//				viewport.QueueDraw();
				animateCount++;
			}
			else
				animateTimer.Enabled = false;
		}
		
		
#endregion
		
	}
}
