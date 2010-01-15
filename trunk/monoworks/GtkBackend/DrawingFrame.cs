// DrawingFrame.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Controls;
using MonoWorks.Modeling;
using MonoWorks.Modeling.SceneControls;
using MonoWorks.GtkBackend.Tree;
using MonoWorks.GtkBackend.AttributeControls;

namespace MonoWorks.GtkBackend
{
	
	/// <summary>
	/// The frame containing a drawing and it's tree.
	/// </summary>
	public class DrawingFrame : Gtk.HPaned
	{
		
		public DrawingFrame() : base()
		{
			// create the tree
			treeView = new TreeView();
			Add1(treeView);
			
			// create the box to hold the scene and attribute panel
			sceneBox = new Gtk.HBox();
			Add2(sceneBox);
			
			// create the scene			
			adapter = new ViewportAdapter();
			sceneBox.PackStart(adapter, true, true, 0);
			
			// create the attribute panel
			attributePanel = new AttributePanel();
			sceneBox.PackStart(attributePanel, false, true, 0);
			
			Controller = new DrawingController(Scene, attributePanel);
		}
		
		
		private Gtk.HBox sceneBox;
		
		protected Drawing drawing = null;
		/// <value>
		/// The drawing associated with this frame.
		/// </value>
		public Drawing Drawing
		{
			get {return drawing;}
			set
			{
				if (drawing != null)
					Scene.RenderList.RemoveActor(drawing);
				drawing = value;
				Scene.RenderList.AddActor(drawing);
				treeView.Drawing = drawing;
			
				// add the drawing interactor
				DrawingInteractor interactor = new DrawingInteractor(Scene, drawing);
				Scene.PrimaryInteractor = interactor;
				
				// connect the scene to the drawing
				drawing.EntityManager.SelectionChanged += Controller.OnSelectionChanged;
				drawing.EntityManager.RaiseSelectionChanged();
				drawing.EntityManager.RegisterSelectionListener(treeView);
				
				Scene.Camera.AnimateTo(ViewDirection.Standard);
			}
		}

		protected ViewportAdapter adapter;
		
		/// <value>
		/// The scene.
		/// </value>
		public Scene Scene {get; set;}
		
		public DrawingController Controller {get; private set;}
		
		protected TreeView treeView;
		/// <value>
		/// The tree view widget.
		/// </value>
		public TreeView Treeview
		{
			get {return treeView;}
			set {treeView = value;}
		}
		
		protected AttributePanel attributePanel;
		/// <value>
		/// The attribute panel.
		/// </value>
		public IAttributePanel AttributePanel
		{
			get {return attributePanel;}
		}
		
	}
}