// AbstractController.cs - MonoWorks Project
//
//  Copyright (C) 2008 Andy Selvig
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
using System.Reflection;

using MonoWorks.Base;
using MonoWorks.Rendering;

namespace MonoWorks.Controls
{

	/// <summary>
	/// Key event delegate.
	/// </summary>
	public delegate void KeyHandler(int key);

	/// <summary>
	/// Base class for Framework controllers.
	/// </summary>
    public abstract class AbstractController<T> where T : Scene
    {
		/// <summary>
		/// Default constructor (sets the scene).
		/// </summary>
        public AbstractController(T scene)
        {
        	Scene = scene;
        	Mwx = new MwxSource();
        	Mwx.ParseCompleted += OnMwxParseCompleted;
			
			// get the actions
			MethodInfo[] methods = GetType().GetMethods();
			foreach (MethodInfo method in methods)
			{
				object[] attributes = method.GetCustomAttributes(typeof(ActionHandlerAttribute), true);

				if (attributes.Length > 0)
				{
                    // TODO: Make this work if Action() is not the first attribute
					var handler = attributes[0] as ActionHandlerAttribute;

					// assign the method name as the name if one wasn't assigned in the attribute
					if (handler.Name.Length == 0)
						handler.Name = method.Name;

					// store the action
					handler.MethodInfo = method;
					_handlers[handler.Name] = handler;
				}
			}
        }

		/// <summary>
		/// Gets called after the mwx source is done parsing something.
		/// </summary>
        private void OnMwxParseCompleted(object sender, System.EventArgs e)
        {
        	foreach (var action in Mwx.GetAll<UiAction>())
			{
        		if (HasHandler(action.Name)) 
				{
					var methodInfo = GetHandler(action.Name).MethodInfo;
        			action.Activated += delegate(object s, EventArgs args) {
						methodInfo.Invoke(this, new object[] { s, args });
					};
				}
			}
        }
		
		/// <summary>
		/// The scene this controller controls.
		/// </summary>
		public T Scene { get; private set; }

		/// <summary>
		/// The MWX source for the controller. 
		/// </summary>
		public MwxSource Mwx { get; private set; }
	
		/// <summary>
		/// The action handlers for this controller.
		/// </summary>
		protected Dictionary<string, ActionHandlerAttribute> _handlers = new Dictionary<string, ActionHandlerAttribute>();

		/// <summary>
		/// Returns true if the controller has an action handler of the given name.
		/// </summary>
		public bool HasHandler(string name)
		{
			return _handlers.ContainsKey(name);
		}

		/// <summary>
		/// Gets the action of the given name.
		/// </summary>
		public ActionHandlerAttribute GetHandler(string name)
		{
			return _handlers[name];
		}

		/// <summary>
		/// Base key press handler.
		/// </summary>
		/// <param name="key"></param>
		public virtual void OnKeyPress(int key)
		{

		}


		#region Internal Updates

		/// <summary>
		/// This is true when the controller is in the middle of an internal update.
		/// </summary>
		protected bool InternalUpdate { get; private set; }

		/// <summary>
		/// Call at the beginning of an internal update.
		/// </summary>
		protected void BeginInternalUpdate()
		{
			InternalUpdate = true;
		}

		/// <summary>
		/// Call at the end of an internal update.
		/// </summary>
		protected void EndInternalUpdate()
		{
			InternalUpdate = false;
		}

		#endregion


	}
}
