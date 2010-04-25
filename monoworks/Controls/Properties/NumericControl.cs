// 
//  NumericControl.cs - MonoWorks Project
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

using MonoWorks.Base;
using MonoWorks.Controls;

namespace MonoWorks.Controls.Properties
{
	/// <summary>
	/// A property control for a number containing either a Spinner or Slider.
	/// </summary>
	public class NumericControl<T> : GenericPropertyControl<T>
	{
		public NumericControl(IMwxObject obj, MwxPropertyAttribute property) : base(obj, property)
		{
			if (property.NumericType == MwxNumericType.Slider)
				_control = new Slider();
			else if (property.NumericType == MwxNumericType.Spinner)
				_control = new Spinner();
			else
				throw new Exception("Don't know how to make a control for numeric type " + property.NumericType);
			AddChild(_control);
			
			_control.ValueChanged += delegate {
				Value = (T)Convert.ChangeType(_control.Value, typeof(T));
			};
		}
		
		
		private MonoWorks.Controls.NumericControl _control;
	}
}

