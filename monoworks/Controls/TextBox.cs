// 
//  TextBox.cs
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2009 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Collections.Generic;

using Cairo;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Rendering.Events;

namespace MonoWorks.Controls
{

	/// <summary>
	/// A control that allows the user to enter text.
	/// </summary>
	public class TextBox : Label, IStringParsable
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public TextBox() : base()
		{
			UserSize = new Coord(100, 16);
			Padding = 6;
		}
		

		
		#region Mouse Interaction
		
		protected override void OnEnter(MouseEvent evt)
		{
			base.OnEnter(evt);
		}

		protected override void OnLeave(MouseEvent evt)
		{
			base.OnLeave(evt);
		}
		
		#endregion
		
	}
}
