// 
//  MwxDemo.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
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

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Controls;

namespace MonoWorks.Demo
{
	/// <summary>
	/// A dummy class that is used to demo all of the Mwx controls.
	/// </summary>
	public class MwxDemo : MwxDummyObject
	{
		public MwxDemo()
		{
		}

		[MwxProperty]
		public string Description { get; set; }

		[MwxProperty]
		public Orientation Orientation { get; set; }
	}
}


