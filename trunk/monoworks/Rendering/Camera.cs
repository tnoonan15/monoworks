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
	/// The predefined view directions.
	/// </summary>
	public enum ViewDirection {Standard, Front, Back, Left, Right, Top, Bottom};
	
	/// <summary>
	/// The 3D perspective used to render the scene.
	/// </summary>
	public enum Projection {Perspective, Parallel};
	
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
			pos = new Vector(0.0, 0.0, 9.0);
			upVec = new Vector(0.0, 1.0, 0.0);
			center = new Vector();
			
			// set default field of view
			fov = new Angle();
			fov["deg"] = 30.0;
			
			// set default interaction factors
			dollyFactor = 0.15;
			panFactor = 0.05;
		}
		

#region Attributes
		
		protected IViewport viewport;	
		
		/// <value>
		/// The height of the viewport.
		/// </value>
		protected double Height
		{
			get {return (double)viewport.HeightGL;}
		}
		
		/// <value>
		/// The width of the viewport.
		/// </value>
		protected double Width
		{
			get {return (double)viewport.WidthGL;}
		}
		
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
		
		protected Vector pos;
		/// <value>
		/// Accesses the position of the camera.
		/// </value>
		public Vector Position
		{
			get	{return pos;}
			set	{pos = value;}
		}
				
		protected Vector upVec;
		/// <value>
		/// Accesses the up-vector of the camera.
		/// The vector normalized and forced orthagonal to the 
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
		
		protected Vector center;
		/// <value>
		/// Accesses the point the camera is looking at.
		/// </value>
		public Vector Center
		{
			get	{return center;}
			set	{center = value;}
		}
		
		/// <value>
		/// The vector pointing to the right in screen coords.
		/// </value>
		/// <remarks> This is generated when needed so you can do whatever you want with the reference.</remarks>
		public Vector RightVec
		{
			get {return upVec.Cross( (center-pos).Normalize() );}
		}
		
		/// <value>
		/// The direction of the camera.
		/// </value>
		public Vector Direction
		{
			get {return pos - center;}
		}
		
		/// <value>
		/// The distance from the camera to the center of the viewing area.
		/// </value>
		public double Distance
		{
			get {return (Direction).Magnitude;}
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
			int width = viewport.WidthGL;
			int height = viewport.HeightGL;
			
			// prevent divide by zero
			if (height==0)	
				height=1;	
			
			gl.glViewport(0, 0, width, height);
			
			// initialize the projection matrix
			gl.glMatrixMode(gl.GL_PROJECTION);
			gl.glLoadIdentity();

			if (projection == Projection.Perspective)
			{
				// set the perspective projection matrix
				glu.gluPerspective((float)fov["deg"], (float)width/(float)height, 0.1f, 20.0f);
			}
			else // parallel
			{
				// determine the size of the viewing box
				double h = (fov*0.5).Tan() * Distance;
				double ar = (double)width/(double)height;
				gl.glOrtho(-ar*h, ar*h, -h, h, 0.1, 20);
			}

			//  store the projection matrix
			gl.glGetDoublev(gl.GL_PROJECTION_MATRIX, projectionMatrix);

			// store the viewport size
			//gl.glGetIntegerv(gl.GL_VIEWPORT, viewportSize);
			viewportSize = new int[] {0, 0, width, height};
		}

		protected Projection projection = Projection.Perspective;
		/// <value>
		/// The projection.
		/// </value>
		public Projection Projection
		{
			get {return projection;}
			set
			{
				projection = value;
				Configure();
			}
		}
		
		/// <summary>
		/// Toggles the projection between parallel and perspective.
		/// </summary>
		public void ToggleProjection()
		{
			if (projection == Projection.Parallel)
				Projection = Projection.Perspective;
			else
				Projection = Projection.Parallel;
		}
		
		/// <summary>
		/// The projection matrix.
		/// </summary>
		protected double[] projectionMatrix = new double[16];
		
		/// <summary>
		/// The model-view matrix.
		/// </summary>
		protected double[] modelMatrix = new double[16];

		/// <summary>
		/// The stored size of the viewport.
		/// </summary>
		protected int[] viewportSize = new int[4];

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


			//  store the model-view matrix
			gl.glGetDoublev(gl.GL_MODELVIEW_MATRIX, modelMatrix);
		}		
		
		/// <summary>
		/// Places the camera for overla drawing.
		/// </summary>
		public virtual void PlaceOverlay()
		{
			// reset the current Modelview matrix
			gl.glMatrixMode(gl.GL_MODELVIEW);				
			gl.glLoadIdentity();
			
			if (projection == Projection.Perspective)
			{
				// translate the camera so that something drawn in the
				// x-y plane maps directly to the screen
				float ar = (float)viewport.WidthGL / (float)viewport.HeightGL; // viewport aspect ratio
				float dz = 0.5f / (float)((fov * 0.5).Tan()); // amount to translate in the z dimension to counteract perspective
				gl.glTranslatef(-0.5f * ar, -0.5f, -dz);
				gl.glScalef(1f / (float)viewport.HeightGL, 1f / (float)viewport.HeightGL, 1.0f);
			}
			else // parallel
			{
				double ar = (float)viewport.WidthGL / (float)viewport.HeightGL; // viewport aspect ratio
				double h = (fov*0.5).Tan() * Distance;
				gl.glTranslated(-ar * h, -h, -1);
				gl.glScaled(2 * h / Height, 2 * h / Height, 1.0);
			}

		}

		/// <summary>
		/// Resets the camera to the default position.
		/// </summary>
		public virtual void Reset()
		{
			pos = new Vector(0.0, 0.0, 7.0);
			center = new Vector(0.0, 0.0, 0.0);
			upVec = new Vector(0.0, 1.0, 0.0);
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
				return 2 * (fov / 2).Tan() * Math.Abs(Distance)/ Height;
			}
		}

		/// <summary>
		/// Converts the vector in world coordinates to screen coordinates.
		/// </summary>
		/// <param name="world"> A 3D vector in workd coordinates.</param>
		/// <returns> The corresponding 2D screen coordinates.</returns>
		public ScreenCoord WorldToScreen(Vector world)
		{
			double screenX, screenY, screenZ;
			glu.gluProject(world[0], world[1], world[2],
				modelMatrix, projectionMatrix, viewportSize,
				out screenX, out screenY, out screenZ);
			return new ScreenCoord((int)screenX, (int)screenY);
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
			Vector travel = Direction * factor;
			pos = travel + pos;
			if (projection == Projection.Parallel)
				Configure(); // dollying in parallel requires reconfiguration
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
			Pan(-panFactor*Height, 0);
		}
		
		/// <summary>
		/// Pan to the right by PanFactor.
		/// </summary>
		public virtual void PanRight()
		{
			Pan(panFactor*Height, 0);
		}
		
		/// <summary>
		/// Pan up by PanFactor.
		/// </summary>
		public virtual void PanUp()
		{
			Pan(0, panFactor*Height);
		}
		
		/// <summary>
		/// Pan down by PanFactor.
		/// </summary>
		public virtual void PanDown()
		{
			Pan(0, -panFactor*Height);
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
			Vector xRotate = RightVec * dx * scaling;
			
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
			double newCenterX = (startX+stopX - Width)/2.0; 
			double newCenterY = (startY+stopY - Height)/2.0; 
			Pan(-newCenterX, -newCenterY);
			
			// determine the relative height and width of the new viewport
			double newW = Math.Abs(startX-stopX) / Width;
			double newH = Math.Abs(startY-stopY) / Height; 
			double newRatio = Math.Max(newH, newW);		
			
			// move the camera closer to the center
			Vector distance = pos - center;
			pos = center + distance*newRatio;
		}
		
		
#endregion


		
#region View Direction
		
		
		protected ViewDirection lastDirection;
		/// <value>
		/// The last view direction set.
		/// </value>
		public ViewDirection LastDirection
		{
			get {return lastDirection;}
		}
		
		/// <summary>
		/// Gets the view vectors for the given view position.
		/// </summary>
		/// <param name="direction"> A <see cref="ViewDirection"/>./ </param>
		/// <param name="centerOut"> The center. </param>
		/// <param name="posOut"> The position. </param>
		/// <param name="upVecOut"> The up vector. </param>
		public void GetDirectionVectors(ViewDirection direction, out Vector centerOut, out Vector posOut, out Vector upVecOut)
		{
			Bounds bounds = viewport.Bounds;
			
			// determine the distance needed to view all renderables
			double dist = bounds.MaxWidth / (fov * 0.5).Tan();
			
			centerOut = bounds.Center;
			Vector travel;
			switch (direction)
			{
			case ViewDirection.Front:
				travel = new Vector(0, -1, 0);
				upVecOut = new Vector(0, 0, 1);
				break;
			case ViewDirection.Back:
				travel = new Vector(0, 1, 0);
				upVecOut = new Vector(0, 0, 1);
				break;
			case ViewDirection.Left:
				travel = new Vector(-1, 0, 0);
				upVecOut = new Vector(0, 0, 1);
				break;
			case ViewDirection.Right:
				travel = new Vector(1, 0, 0);
				upVecOut = new Vector(0, 0, 1);
				break;
			case ViewDirection.Top:
				travel = new Vector(0, 0, 1);
				upVecOut = new Vector(0, 1, 0);
				break;
			case ViewDirection.Bottom:
				travel = new Vector(0, 0, -1);
				upVecOut = new Vector(0, 1, 0);
				break;
			default:
				travel = new Vector(1, 1, 0.5);
				travel.Normalize();
				dist = 0.7 * dist;
				upVecOut = new Vector(0, 0, 1);
				break;
			}
			posOut = (travel*dist) + centerOut;
		}
		
		/// <summary>
		/// Sets the camera view direction.
		/// </summary>
		public void SetViewDirection(ViewDirection direction)
		{
			lastDirection = direction;
			GetDirectionVectors(direction, out center, out pos, out upVec);
			RecomputeUpVector();
			viewport.OnDirectionChanged();
		}
		
#endregion
		
		
		
#region Animation
	
		
		
		
#endregion
		
	}
}
