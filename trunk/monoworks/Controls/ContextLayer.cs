// ContextLayer.cs - MonoWorks Project
//
//  Copyright (C) 2009 Andy Selvig
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

using MonoWorks.Base;
using MonoWorks.Rendering.Events;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{
	
	/// <summary>
	/// Exception for invalid contexts.
	/// </summary>
	public class InvalidContextException : Exception
	{
		public InvalidContextException(string context) : 
			base(context + " is not a valid context in this context laye.")
		{
		}
	}

	
	/// <summary>
	/// A control layer that contains anchors on all edges of the viewport.
	/// It also contains a collection of toolbars that can be dynamically added
	/// and removed to the anchors by use of string keywords called contexts.
	/// </summary>	
	public class ContextLayer : Overlay
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ContextLayer() : base()
		{
			// create the anchors
			foreach (AnchorLocation anchorLoc in Enum.GetValues(typeof(AnchorLocation)))
			{
				anchors[anchorLoc] = new AnchorPane(anchorLoc);
			}

			// create the stacks
			foreach (ContextLocation loc in Enum.GetValues(typeof(ContextLocation)))
			{
				Stack stack = new Stack();
				stack.Orientation = ContextOrientation(loc);
				stack.Padding = 0;
				stacks[loc] = stack;
				anchors[(AnchorLocation)loc].Control = stack;
			}
		}
		
		
#region The Toolbars
		
				
		protected Dictionary<string, ToolBar> toolBars = new Dictionary<string, ToolBar>();
		
		/// <summary>
		/// Sets the given context with the associated toolbar.
		/// </summary>
		/// <remarks>The same as contextBar[context] = toolBar;</remarks>
		public void AddToolbar(string context, ToolBar toolBar)
		{
			if (HasToolbar(context))
				toolBars[context].Parent = null;
			toolBars[context] = toolBar;	
//			toolBar.Parent = this;
		}		
		
		/// <summary>
		/// Returns true if the given context is present.
		/// </summary>
		public bool HasToolbar(string context)
		{
			return toolBars.ContainsKey(context);
		}
		
		/// <summary>
		/// Removes the given context.
		/// </summary>
		public void RemoveToolbar(string context)
		{
			if (!HasToolbar(context))
				throw new InvalidContextException(context);
			toolBars.Remove(context);	
		}
		
		/// <summary>
		/// Gets the toolbar for the given context.
		/// </summary>
		/// <remarks>The same as contextBar[context];</remarks>
		public ToolBar GetToolbar(string context)
		{
			if (!HasToolbar(context))
				throw new InvalidContextException(context);
			return toolBars[context];
		}
		
		/// <summary>
		/// Indexer for getting and setting contexts.
		/// </summary>
		public ToolBar this[string context]
		{
			get { return GetToolbar(context); }
			set { AddToolbar(context, value); }
		}
		
		
#endregion


#region The Anchors and Stacks

		/// <summary>
		/// Gets the orientation for the context in the given position.
		/// </summary>
		/// <param name="loc"></param>
		/// <returns></returns>
		public static Orientation ContextOrientation(ContextLocation loc)
		{
			if (loc == ContextLocation.N || loc == ContextLocation.S)
				return Orientation.Horizontal;
			else
				return Orientation.Vertical;
		}

		Dictionary<AnchorLocation, AnchorPane> anchors = new Dictionary<AnchorLocation, AnchorPane>();

		Dictionary<ContextLocation, Stack> stacks = new Dictionary<ContextLocation, Stack>();


		/// <summary>
		/// Anchors a control at the given location.
		/// </summary>
		public void AnchorControl(Control2D control, AnchorLocation location)
		{
			if (anchors[location].Control != null)
				throw new Exception("There's already something at this anchor.");

			anchors[location].Control = control;
		}
		
#endregion


#region The Contexts

		/// <summary>
		/// Adds the given context to the location.
		/// </summary>
		/// <param name="loc"></param>
		/// <param name="context"></param>
		public void AddContext(ContextLocation loc, string context)
		{
			ToolBar toolbar = GetToolbar(context);
			toolbar.Orientation = ContextOrientation(loc);
			toolbar.StyleClassName = "toolbar-" + loc.ToString().ToLower();
			toolbar.ToolStyle = "tool-" + loc.ToString().ToLower();
			stacks[loc].Add(toolbar);
			anchors[(AnchorLocation)loc].MakeDirty();
		}

		/// <summary>
		/// Clears all contexts from the location.
		/// </summary>
		/// <param name="loc"></param>
		public void ClearContexts(ContextLocation loc)
		{
			stacks[loc].Clear();
		}

		/// <summary>
		/// Clears all contexts from all locations.
		/// </summary>
		public void ClearAllContexts()
		{
			foreach (ContextLocation loc in Enum.GetValues(typeof(ContextLocation)))
			{
				stacks[loc].Clear();
			}
		}


#endregion



#region Rendering

		
		public override void OnViewportResized(Viewport viewport)
		{
			base.OnViewportResized(viewport);
			
			foreach (AnchorPane anchor in anchors.Values)
				anchor.OnViewportResized(viewport);
		}

		public override void RenderOverlay (Viewport viewport)
		{
			base.RenderOverlay (viewport);
			
			foreach (var anchor in anchors.Values)
			{
				anchor.RenderOverlay(viewport);
			}
		}

		
		
#endregion
		
		
#region Interaction		
		
		protected override bool HitTest (Coord pos)
		{
			return false;
		}

		public override void OnButtonPress (MouseButtonEvent evt)
		{
			base.OnButtonPress (evt);
			
			foreach (var anchor in anchors.Values)
				anchor.OnButtonPress(evt);
		}

		public override void OnButtonRelease (MouseButtonEvent evt)
		{
			base.OnButtonRelease (evt);
			
			foreach (var anchor in anchors.Values)
				anchor.OnButtonRelease(evt);
		}

		public override void OnMouseMotion (MouseEvent evt)
		{
			base.OnMouseMotion (evt);
			
			foreach (var anchor in anchors.Values)
				anchor.OnMouseMotion(evt);
		}

		
#endregion
		
		
	}
}
