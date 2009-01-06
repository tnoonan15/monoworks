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

namespace MonoWorks.Framework
{
	/// <summary>
	/// Base class for Framework controllers.
	/// </summary>
    public abstract class AbstractController
    {
		/// <summary>
		/// Default constructor.
		/// </summary>
        public AbstractController()
		{
			// get the actions
			MethodInfo[] methods = GetType().GetMethods();
			foreach (MethodInfo method in methods)
			{
				object[] attributes = method.GetCustomAttributes(typeof(ActionAttribute), true);

				if (attributes.Length > 0)
				{
                    // TODO: Make this work if Action() is not the first attribute
					ActionAttribute action = attributes[0] as ActionAttribute;

					// assign the method name as the name if one wasn't assigned in the attribute
					if (action.Name.Length == 0)
						action.Name = method.Name;

					// store the action
					action.MethodInfo = method;
					actions[action.Name] = action;
				}
			}
        }


		/// <summary>
		/// The actions for this controller.
		/// </summary>
		protected Dictionary<string, ActionAttribute> actions = new Dictionary<string, ActionAttribute>();

		/// <summary>
		/// Returns true if the controller has the action of the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool HasMethod(string name)
		{
			return actions.ContainsKey(name);
		}

		/// <summary>
		/// Gets the action of the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ActionAttribute GetAction(string name)
		{
			return actions[name];
		}


    }
}
