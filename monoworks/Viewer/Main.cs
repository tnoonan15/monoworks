// Main.cs - MonoWorks Project
//
// Copyright Andy Selvig 2008
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;

using Qyoto;

using MonoWorks.Model;
using mwb = MonoWorks.Base;
using MonoWorks.Gui;

namespace MonoWorks.Viewer
{	
	/// <summary>
	/// Entrypoint for the MonoWorks Viewer.
	/// </summary>
	class Viewer
	{	
		public static int Main(String[] args)
		{
						
				new QApplication(args);
				DocFrame frame = new DocFrame();
				frame.SetWindowTitle("MonoWorks Viewer");
				frame.Size = new QSize(800,600);
				frame.Show();
				return QApplication.Exec();
		    }
	}
	
}
