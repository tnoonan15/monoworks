using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoWorks.Rendering;
using MonoWorks.Rendering.Interaction;

namespace MonoWorks.Controls.Dock
{
	/// <summary>
	/// Interactor for the DockSpace.
	/// </summary>
	/// <remarks>Probably not useful for much else.</remarks>
	public class DockInteractor : GenericInteractor<DockSpace>
	{

		public DockInteractor(DockSpace scene) : base(scene)
		{
			scene.PreSceneUndocked += new DockEventHandler(OnSceneUndocked);

			_label = new Label();
			_pane = new OverlayPane(_label);
		}

		private Scene _dragScene;

		private Label _label;

		private OverlayPane _pane;

		void OnSceneUndocked(Scene scene)
		{
			_dragScene = scene;
			var container = _dragScene.Parent as SceneContainer;
			if (container == null)
				throw new Exception("Trying to begin dragging a scene that isn't in a container.");
			container.Remove(_dragScene);
			_label.Body = scene.Name;
		}

		public override void OnButtonPress(Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonPress(evt);
		}

		public override void OnButtonRelease(Rendering.Events.MouseButtonEvent evt)
		{
			base.OnButtonRelease(evt);

			// decide what to do with the dragging scene
			if (_dragScene != null)
			{
				evt.Handle(this);
				_dragScene = null;
			}
		}

		public override void OnMouseMotion(Rendering.Events.MouseEvent evt)
		{
			base.OnMouseMotion(evt);

			// move the pane to the cursor location
			if (_dragScene != null)
			{
				_pane.Origin = evt.Pos;
				evt.Handle(this);
			}
		}


		#region Rendering

		public override void RenderOverlay(Scene scene)
		{
 			base.RenderOverlay(scene);

			if (_dragScene != null)
			{
				_pane.RenderOverlay(scene);
			}
		}

		#endregion

	}
}
