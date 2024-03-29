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
using System.Collections.Generic;

using gl = Tao.OpenGl.Gl;
using glu = Tao.OpenGl.Glu;
using MonoWorks.Base;
using MonoWorks.Rendering.Interaction;


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
	public class Camera : IAnimatable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Camera(Scene scene)
		{
			_scene = scene;
			
			// set default positioning
			_position = new Vector(0.0, 0.0, 9.0);
			_upVec = new Vector(0.0, 1.0, 0.0);
			_center = new Vector();
			
			// set default field of view
			fov = new Angle();
			fov["deg"] = 28.0;
			
			// set default interaction factors
			dollyFactor = 0.15;
			panFactor = 0.05;
			
			lastDirection = ViewDirection.Front;

			Frustum = new FrustumDef();

			DefaultAnimationOptions.Duration = 4.0;
			DefaultAnimationOptions.EaseType = EaseType.Quadratic;

			UseInertia = true;
			InertiaAnimationOptions = new AnimationOptions() {
				Duration = 6.0,
				EaseType = EaseType.Linear
			};
			InertiaType = InteractionType.None;
		}
		

#region Attributes
		
		protected Scene _scene;	
		
		/// <value>
		/// The height of the viewport.
		/// </value>
		protected double Height
		{
			get {return _scene.Height;}
		}
		
		/// <value>
		/// The width of the viewport.
		/// </value>
		protected double Width
		{
			get {return _scene.Width;}
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
		
		protected Vector _position;
		/// <value>
		/// Accesses the position of the camera.
		/// </value>
		public Vector Position
		{
			get	{return _position;}
			set	{_position = value;}
		}
				
		protected Vector _upVec;
		/// <value>
		/// Accesses the up-vector of the camera.
		/// The vector normalized and forced orthagonal to the 
		/// view direction when assigned.
		/// </value>
		public Vector UpVector
		{
			get {return _upVec;}
			set 
			{
				_upVec = value.Normalize();
				RecomputeUpVector();
			}
		}
		
		protected Vector _center;
		/// <value>
		/// Accesses the point the camera is looking at.
		/// </value>
		public Vector Center
		{
			get	{return _center;}
			set	{_center = value;}
		}
		
		/// <value>
		/// The vector pointing to the right in screen coords.
		/// </value>
		/// <remarks> This is generated when needed so you can do whatever you want with the reference.</remarks>
		public Vector RightVec
		{
			get {return _upVec.Cross( (_center-_position).Normalize() );}
		}
		
		/// <value>
		/// The direction of the camera.
		/// </value>
		public Vector Direction
		{
			get {return _position - _center;}
		}
		
		/// <value>
		/// The distance from the camera to the center of the viewing area.
		/// </value>
		public double Distance
		{
			get {return (Direction).Magnitude;}
		}
		
		/// <summary>
		/// Gets the distance between vec and the camera.
		/// </summary>
		public double GetDistance(Vector vec)
		{
			return Math.Abs((_position - vec).Magnitude);
		}
		
		
#endregion
		

#region Configuration and Placement
		
		/// <summary>
		/// Resets the viewport size of the OpenGL context to the scene's size.
		/// </summary>
		/// <remarks>This should only be called by the top level scene when it is resized.</remarks>
		public void Resize()
		{
			var width = _scene.Width;
			var height = _scene.Height;			
			gl.glViewport(0, 0, (int)width, (int)height);
		}
		
		/// <summary>
		/// Configures the viewport based on its width and height.
		/// </summary>
		public virtual void Configure()
		{
			// initialize the projection matrix
			gl.glMatrixMode(gl.GL_PROJECTION);
			gl.glLoadIdentity();
			
			// set the viewport
			var width = _scene.Width;
			var height = _scene.Height;
			if (height == 0)
				// prevent divide by zero
				height = 1;
			double ar = width / height;
			
			if (projection == Projection.Perspective)
			{								
				// store the frustum
				Frustum = new FrustumDef(fov, 0.0001*Distance, 1000*Distance, ar);
				// set the perspective projection matrix
				glu.gluPerspective((float)fov["deg"], (float)Frustum.AR, (float)Frustum.NearDist, (float)Frustum.FarDist);

			}
			else // parallel
			{
				// determine the size of the viewing box
				double h = (fov*0.5).Tan() * Distance;
				gl.glOrtho(-ar*h, ar*h, -h, h, 0.001*Distance, 100*Distance);
			}

			//  store the projection matrix for later setup
			gl.glGetDoublev(gl.GL_PROJECTION_MATRIX, projectionMatrix);

			// store the viewport size
			//gl.glGetIntegerv(gl.GL_VIEWPORT, viewportSize);
			viewportSize = new int[] {0, 0, (int)width, (int)height};
		}

		/// <summary>
		/// Gets raised whenever the projection changes.
		/// </summary>
		public EventHandler ProjectionChanged;

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
				if (ProjectionChanged != null)
					ProjectionChanged(this, new EventArgs());
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

		protected double[] projectionMatrix = new double[16];
		/// <summary>
		/// The projection matrix.
		/// </summary>
		public double[] ProjectionMatrix
		{
			get { return projectionMatrix; }
		}
		
		/// <summary>
		/// The model-view matrix.
		/// </summary>
		protected double[] modelMatrix = new double[16];

		protected int[] viewportSize = new int[4];
		/// <summary>
		/// The stored size of the viewport.
		/// </summary>
		public int[] ViewportSize
		{
			get { return viewportSize; }
		}

		/// <summary>
		/// The stored viewport width.
		/// </summary>
		public int ViewportWidth
		{
			get { return viewportSize[2]; }
		}

		/// <summary>
		/// The stored viewport height.
		/// </summary>
		public int ViewportHeight
		{
			get { return viewportSize[3]; }
		}
		
		/// <summary>
		/// Gets the context ready for rendering with this camera.
		/// </summary>
		public void Setup()
		{
			// setup the projection matrix
			gl.glMatrixMode(gl.GL_PROJECTION);
			gl.glLoadMatrixd(projectionMatrix);
			
			gl.glViewport((int)_scene.ViewportOffset.X, (int)_scene.ViewportOffset.Y, (int)_scene.Width, (int)_scene.Height);
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
			glu.gluLookAt(_position[0], _position[1], _position[2], 
			              _center[0], _center[1], _center[2],
			              _upVec[0], _upVec[1], _upVec[2]);


			//  store the model-view matrix
			gl.glGetDoublev(gl.GL_MODELVIEW_MATRIX, modelMatrix);
		}		
		
		/// <summary>
		/// Places the camera for overlay drawing.
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
				float ar = (float)_scene.Width / (float)_scene.Height; // viewport aspect ratio
				float dz = 0.5f / (float)((fov * 0.5).Tan()); // amount to translate in the z dimension to counteract perspective
				gl.glTranslatef(-0.5f * ar, -0.5f, -dz);
				gl.glScalef(1f / (float)_scene.Height, 1f / (float)_scene.Height, 1.0f);
			}
			else // parallel
			{
				double ar = (float)_scene.Width / (float)_scene.Height; // viewport aspect ratio
				double h = (fov*0.5).Tan() * Distance;
				gl.glTranslated(-ar * h, -h, -1);
				gl.glScaled(2 * h / Height, 2 * h / Height, 1.0);
			}

		}

		
		/// <summary>
		/// Recomputes the up vector to ensure it is orthoganal to the viewing direction.
		/// </summary>
		public virtual void RecomputeUpVector()
		{
			Vector direction = (_center-_position).Normalize();
			Vector lateral = direction.Cross( _upVec);
			_upVec = lateral.Cross( direction);
		}		


#endregion


#region Transformations

		/// <value>
		/// The scaling from the viewport to world coordinates.
		/// </value>
		public double SceneToWorldScaling
		{
			get	{return 2 * (fov / 2).Tan() * Math.Abs(Distance)/ Height;}
		}

		/// <summary>
		/// Converts the vector in world coordinates to screen coordinates.
		/// </summary>
		/// <param name="world"> A 3D vector in workd coordinates.</param>
		/// <returns> The corresponding 2D screen coordinates.</returns>
		public Coord WorldToScreen(Vector world)
		{
			return WorldToScreen(world.X, world.Y, world.Z);
		}


		public Coord WorldToScreen(double x, double y, double z)
		{
			double screenX, screenY, screenZ;
			glu.gluProject(x, y, z,
				modelMatrix, projectionMatrix, viewportSize,
				out screenX, out screenY, out screenZ);
			if (Double.IsNaN(screenX) || Double.IsNaN(screenY))
				return new Coord(0, 0);
			return new Coord((int)screenX, (int)screenY);
		}


		/// <summary>
		/// Unprojects the screen coordinate into a world space hit line.
		/// </summary>
		public HitLine ScreenToWorld(Coord screen)
		{
			HitLine hitLine = new HitLine();
			hitLine.Front = ScreenToWorld(screen, false);
			hitLine.Back = ScreenToWorld(screen, true);
			hitLine.Camera = this;
			hitLine.Screen = new Coord(screen.X, screen.Y);
			//Console.WriteLine("hit screen {0}, {1}", screen, hitLine);
			return hitLine;
		}

		/// <summary>
		/// Unprojects the screen coordinate into world space.
		/// </summary>
		/// <param name="screen"></param>
		/// <param name="farPlane"> Whether to project on to the far plane.</param>
		/// <returns></returns>
		public Vector ScreenToWorld(Coord screen, bool farPlane)
		{
			double x, y, z;
			double screenZ = 0;
			if (farPlane)
				screenZ = 1;
			glu.gluUnProject(screen.X, screen.Y, screenZ, 
				modelMatrix, projectionMatrix, viewportSize, 
				out x, out y, out z);
			return new Vector(x, y, z);
		}
		
#endregion


#region Frustum

		/// <summary>
		/// The definition of the view frustum geometry.
		/// </summary>
		public FrustumDef Frustum { get; private set; }

		/// <summary>
		/// Hit lines that lay on the frustum edges.
		/// </summary>
		public HitLine[] FrustumEdges
		{
			get
			{
				HitLine[] edges = new HitLine[4];

				edges[0] = ScreenToWorld(new Coord(0, 0));
				edges[1] = ScreenToWorld(new Coord(0, Height));
				edges[2] = ScreenToWorld(new Coord(Width, 0));
				edges[3] = ScreenToWorld(new Coord(Width, Height));

				return edges;
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
			Dolly(dollyFactor);
		}

		/// <summary>
		/// Dollies out by DollyFactor.
		/// </summary>
		public void DollyOut()
		{
			Dolly(-dollyFactor);
		}
		
		/// <summary>
		/// Dollies by the given factor.
		/// The factor should be positive for dollying out
		/// and negative for dollying in.
		/// </summary>
		/// <param name="factor"> The dolly factor. </param>
		public void Dolly(double factor)
		{
			Vector travel = Direction * -factor;
			_position = travel + _position;
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
		/// Pans by diff.
		/// </summary>
		/// <param name="diff"></param>
		public void Pan(Coord diff)
		{
			Pan(diff.X, diff.Y);
		}
		
		/// <summary>
		/// Pans the camera by the given deltas.
		/// </summary>
		/// <param name="dx"> The delta in the x dimension. </param>
		/// <param name="dy"> The delta in the y dimension. </param>
		public void Pan(double dx, double dy)
		{
			if (Math.Abs(dx) < 0.01 && Math.Abs(dy) < 0.01) // don't perform infinitesmal pans
				return;
			InertiaType = InteractionType.Pan;
			_inertiaVelocity.X = dx;
			_inertiaVelocity.Y = dy;

			// determine the scaling from view to world coordinates
			double scaling = SceneToWorldScaling;
			
			// compute the view up (y) component of the tranformation
			Vector yPan = _upVec * -dy * scaling;
			
			// compute the lateral (x) compoment of the transformation
			Vector xPan = _upVec.Cross( (_center-_position).Normalize() ) * dx * scaling;
			
			// add the x and y components to the position and center
			_position += (xPan+yPan);
			_center += (xPan+yPan);
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

		/// <summary>
		/// Signifies that the user stopped panning the camera.
		/// </summary>
		public void EndPan()
		{
			if (UseInertia && InertiaType == InteractionType.Pan)
				_scene.Animator.RegisterAnimation(this, InertiaAnimationOptions.Duration);
		}
		
		#endregion
		
		
		#region Rotating

		/// <summary>
		/// Rotate by diff.
		/// </summary>
		/// <param name="diff"></param>
		public void Rotate(Coord diff)
		{
			Rotate(diff.X, diff.Y);
		}

		/// <summary>
		/// Rotate the camera by the given deltas.
		/// </summary>
		/// <param name="dx"> The delta in the x dimension. </param>
		/// <param name="dy"> The delta in the y dimension. </param>
		public void Rotate(double dx, double dy)
		{
			if (Math.Abs(dx) < 0.01 && Math.Abs(dy) < 0.01) // don't perform infinitesmal rotations
				return;
			InertiaType = InteractionType.Rotate;
			_inertiaVelocity.X = dx;
			_inertiaVelocity.Y = dy;

			// determine the scaling from view to world coordinates
			double scaling = SceneToWorldScaling * 6;
			
			// compute the lateral (x) compoment of the transformation
			Vector xRotate = RightVec * dx * scaling;
			
			// compute the view up (y) component of the tranformation
			Vector yRotate = _upVec * -dy * scaling;
			
			// compute the current distance from the center
			double currentDistance = (_position-_center).Magnitude;
			
			// add the x and y components to the position and center
			_position += (xRotate+yRotate);
			
			// maintain the distance to the center
			Vector distanceVec = _position-_center;
			double newDistance = distanceVec.Magnitude;
			_position = distanceVec * currentDistance / newDistance + _center;
			
			// ensure the up vector is still orthagonal to the viewing direction
			RecomputeUpVector();
		}		

		/// <summary>
		/// Signifies that the user stopped rotating the camera.
		/// </summary>
		public void EndRotate()
		{
			if (UseInertia && InertiaType == InteractionType.Rotate)
				_scene.Animator.RegisterAnimation(this, InertiaAnimationOptions.Duration);
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
			Vector distance = _position - _center;
			_position = _center + distance*newRatio;
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
//			_scene.RenderList.ResetBounds();
			Bounds bounds = _scene.RenderList.Bounds;
			
			// determine the distance needed to view all renderables
			double dist = 0;
			if (_scene.RenderList.ActorCount > 0 && bounds.IsSet)
			{
				dist = bounds.MaxWidth / (fov * 0.5).Tan();				
				centerOut = bounds.Center;
			}
			else
			{
				dist = 1 / (fov * 0.5).Tan();
				centerOut = new Vector();
			}
			
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
				travel = new Vector(1, -1, 0.5);
				travel.Normalize();
				dist = 0.8 * dist;
				upVecOut = new Vector(0, 0, 1);
				break;
			}
			posOut = (travel * dist) + centerOut;
		}
		
		/// <summary>
		/// Sets the camera view direction.
		/// </summary>
		public void SetViewDirection(ViewDirection direction)
		{
			lastDirection = direction;
			GetDirectionVectors(direction, out _center, out _position, out _upVec);
			RecomputeUpVector();
//			_scene.Resize();
			_scene.OnDirectionChanged();
		}

		/// <summary>
		/// Gets the vector needed to look at a plane.
		/// </summary>
		public void GetPlaneVectors(IPlane plane, out Vector centerOut, out Vector posOut, out Vector upVecOut)
		{
			Bounds bounds = _scene.RenderList.Bounds;
		
			// determine the distance needed to view all renderables
			double dist = 0;
			if (_scene.RenderList.ActorCount > 0 && bounds.IsSet)
				dist = bounds.MaxWidth / (fov * 0.5).Tan();
			else
				dist = 2 / (fov * 0.5).Tan();
		
			centerOut = plane.Origin;
			posOut = centerOut + plane.Normal.Normalize() * dist * 0.8;
			if (plane.Normal[1] == 0 && plane.Normal[2] == 0) // the normal is in the Y-Z axis
				upVecOut = new Vector(0.0, 1.0, 0.0);
			else // the plane is not in the Y-Z axis
				upVecOut = new Vector(1.0, 0.0, 0.0);		
		}


		/// <summary>
		/// Forces the camera to look squarely at a plane.
		/// </summary>
		/// <param name="plane"></param>
		public void LookAt(IPlane plane)
		{
			GetPlaneVectors(plane, out _center, out _position, out _upVec);
			RecomputeUpVector();
//			_scene.Resize();
			_scene.OnDirectionChanged();
		}

		#endregion
			
		
		#region Animation

		Vector animStartCenter, animStartDir, animStartUpVec;

		Vector animStopCenter, animStopDir, animStopUpVec;

		double animStartDist, animStopDist;

		/// <summary>
		/// The default options for all camera animations (except inertia).
		/// </summary>
		public AnimationOptions DefaultAnimationOptions;

		/// <summary>
		/// Store the options for the current animation.
		/// </summary>
		private AnimationOptions _currentAnimationOptions;

		/// <summary>
		/// Animates to the given view direction.
		/// </summary>
		/// <param name="direction"></param>
		public void AnimateTo(ViewDirection direction)
		{
			Vector center, position, upVec;
			GetDirectionVectors(direction, out center, out position, out upVec);
			AnimateTo(center, position, upVec);
			lastDirection = direction;
		}


		/// <summary>
		/// Animates to look at the given plane.
		/// </summary>
		public void AnimateTo(IPlane plane)
		{
			Vector center, position, upVec;
			GetPlaneVectors(plane, out center, out position, out upVec);
			AnimateTo(center, position, upVec);
		}

		/// <summary>
		/// Performs an animation with default duration and ease type.
		/// </summary>
		public void AnimateTo(Vector center, Vector position, Vector upVec)
		{
			AnimateTo(center, position, upVec, DefaultAnimationOptions);
		}

		/// <summary>
		/// Animates to the given set of vectors. 
		/// </summary>
		/// <remarks>All other AnimateTo() methods should call this one.</remarks>
		public void AnimateTo(Vector center, Vector position, Vector upVec, AnimationOptions opts)
		{
			EndInertiaAnimation();
			Configure();
			animStartCenter = this._center;
			animStartDir = Direction.Normalize();
			animStartUpVec = this._upVec;
			animStartDist = Distance;

			animStopCenter = center;
			animStopUpVec = upVec;
			animStopDir = position - center;
			animStopDist = animStopDir.Magnitude;
			animStopDir = animStopDir.Normalize();

			_currentAnimationOptions = opts;
			_scene.Animator.RegisterAnimation(this, opts.Duration);
		}

		/// <summary>
		/// Callback for the animation step.
		/// </summary>
		public void Animate(double progress)
		{
			if (UseInertia && InertiaType == InteractionType.None) // must be a regular animation
			{
				var f = Ease.InOutFactor(progress, _currentAnimationOptions.EaseType);
				_center = (animStopCenter - animStartCenter) * f + animStartCenter;
				double dist = (animStopDist - animStartDist) * f + animStartDist;
				Vector dir = (animStopDir - animStartDir) * f + animStartDir;
				_position = _center + dir * dist;
				_upVec = (animStopUpVec - animStartUpVec) * f + animStartUpVec;
				RecomputeUpVector();
			}
			else // an inertia animation
			{
				InertiaAnimate(progress);
			}
		}

		/// <summary>
		/// Gets raised when an animation ends.
		/// </summary>
		public event EventHandler AnimationEnded;

		/// <summary>
		/// Gets called when an animation ends.
		/// </summary>
		/// <remarks>This gets cleared every time an animation ends, 
		/// so any handlers only see one event.</remarks>
		public void EndAnimation()
		{
			EndInertiaAnimation();
			if (AnimationEnded != null)
			{
				AnimationEnded(this, new EventArgs());
				AnimationEnded = null;
			}
		}	
				
		#endregion


		#region Inertia

		/// <summary>
		/// The velocity of the last interaction that has inertia.
		/// </summary>
		private Coord _inertiaVelocity = new Coord();

		/// <summary>
		/// Whether or not to use inertia during rotation actions.
		/// </summary>
		public bool UseInertia { get; set; }

		/// <summary>
		/// The options for all camera inertia animations.
		/// </summary>
		public AnimationOptions InertiaAnimationOptions;

		/// <summary>
		/// The type of interaction that is currently performing an inertia animation.
		/// </summary>
		public InteractionType InertiaType { get; private set; }

		/// <summary>
		/// Performs a step of the current inertia animation.
		/// </summary>
		protected void InertiaAnimate(double progress)
		{
			var velocity = _inertiaVelocity * (1 - progress);
			if (InertiaType == InteractionType.Rotate)
			{
				Rotate(velocity);
			}
			else if (InertiaType == InteractionType.Pan)
			{
				Pan(velocity);
			}
			else
				throw new Exception("Don't know how to inertia animate type " + InertiaType);
		}

		/// <summary>
		/// Used internally to force the inertia animation to stop.
		/// </summary>
		/// <remarks>Presumably, this is to make way for another animation.</remarks>
		protected void EndInertiaAnimation()
		{
			InertiaType = InteractionType.None;
		}

		#endregion
	}


	/// <summary>
	/// A data structure containing the geometry of a view frustum.
	/// </summary>
	public struct FrustumDef
	{
		/// <summary>
		/// Constructs the frustm def.
		/// </summary>
		public FrustumDef(Angle fov, double near, double far, double ar) : this()
		{
			NearDist = near;
			FarDist = far;
			AR = ar;
			NearHeight = 2.0 * (fov / 2.0).Tan() * NearDist;
			FarHeight = 2.0 * (fov / 2.0).Tan() * FarDist;
		}

		/// <summary>
		/// The aspect ratio.
		/// </summary>
		public double AR { get; private set; }
		
		/// <summary>
		/// The distance from the camera to the near plane.
		/// </summary>
		public double NearDist { get; private set; }

		/// <summary>
		/// Height of the near plane.
		/// </summary>
		public double NearHeight { get; private set; }

		/// <summary>
		/// Width of the near plane.
		/// </summary>
		public double NearWidth
		{
			get	{return AR * NearHeight;}
		}

		/// <summary>
		/// The distance from the camera to the far plane.
		/// </summary>
		public double FarDist { get; private set; }

		/// <summary>
		/// Height of the far plane.
		/// </summary>
		public double FarHeight { get; private set; }

		/// <summary>
		/// Width of the far plane.
		/// </summary>
		public double FarWidth
		{
			get { return AR * FarHeight; }
		}
	}

}
