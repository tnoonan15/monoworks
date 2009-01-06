// Controller.cs - MonoWorks Project
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

using MonoWorks.Framework;
using MonoWorks.Rendering;

namespace MonoWorks.Rendering.Controls
{
    /// <summary>
    /// Implements a Framework controller for a viewport.
    /// </summary>
    public class AbstractViewportController : AbstractController
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="viewport">The viewport that this controller controls.</param>
        public AbstractViewportController(IViewport viewport)
            : base()
        {
			this.viewport = viewport;
			UiManager = new UiManager(this);
        }

        protected IViewport viewport;

		/// <summary>
		/// The UiManager used by this controller.
		/// </summary>
		public UiManager UiManager { get; set; }
    }
}
