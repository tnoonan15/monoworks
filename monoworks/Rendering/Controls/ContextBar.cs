// ContextBar.cs - MonoWorks Project
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

namespace MonoWorks.Rendering.Controls
{
	
	/// <summary>
	/// Exception for invalid contexts.
	/// </summary>
	public class InvalidContextException : Exception
	{
		public InvalidContextException(string context) : 
			base(context + " is not a valid context in this context bar.")
		{
		}
	}
	
	
	/// <summary>
	/// A container that holds many toolbars and can 
	/// switch between them based on centext keywords.
	/// </summary>	
	public class ContextBar : Control
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ContextBar() : base()
		{
		}
		
		
#region The Toolbars
		
		protected Dictionary<string, ToolBar> toolBars = new Dictionary<string, ToolBar>();
		
		/// <summary>
		/// Sets the given context with the associated toolbar.
		/// </summary>
		/// <remarks>The same as contextBar[context] = toolBar;</remarks>
		public void SetContext(string context, ToolBar toolBar)
		{
			if (HasContext(context))
				toolBars[context].Parent = null;
			toolBars[context] = toolBar;	
			toolBar.Parent = this;
		}		
		
		/// <summary>
		/// Returns true if the given context is present.
		/// </summary>
		public bool HasContext(string context)
		{
			return toolBars.ContainsKey(context);
		}
		
		/// <summary>
		/// Removes the given context.
		/// </summary>
		public void RemoveContext(string context)
		{
			if (!HasContext(context))
				throw new InvalidContextException(context);
			toolBars.Remove(context);	
		}
		
		/// <summary>
		/// Gets the toolbar for the given context.
		/// </summary>
		/// <remarks>The same as contextBar[context];</remarks>
		public ToolBar GetContext(string context)
		{
			if (!HasContext(context))
				throw new InvalidContextException(context);
			return toolBars[context];
		}
		
		/// <summary>
		/// Indexer for getting and setting contexts.
		/// </summary>
		public ToolBar this[string context]
		{
			get {return GetContext(context);}
			set {SetContext(context, value);}
		}
		
		private string currentContext = null;
		/// <value>
		/// The current context to display.
		/// </value>
		public string CurrentContext
		{
			get {return currentContext;}
			set
			{
				if (!HasContext(value))
					throw new InvalidContextException(value);
				currentContext = value;
				MakeDirty();
			}
		}
		
#endregion
		
		
#region Sizing
		
		
		public override Coord MinSize
		{
			get
			{
				if (currentContext != null)
					return toolBars[currentContext].MinSize;
				return base.MinSize;
			}
		}

		
#endregion
		
		
#region Rendering
		
		
		public override void ComputeGeometry()
		{
			base.ComputeGeometry();
			
			if (currentContext != null)
			{
				toolBars[currentContext].Position = position;
				toolBars[currentContext].MakeDirty();	
			}
		}

		
		public override void RenderOverlay(IViewport viewport)
		{
			base.RenderOverlay(viewport);
			
			if (currentContext != null)
			{
				toolBars[currentContext].RenderOverlay(viewport);	
			}
		}

		
		
#endregion
		
		
		
	}
}
