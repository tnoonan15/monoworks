// 
//  StudioScene.cs - MonoWorks Project
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

using MonoWorks.Rendering;
using MonoWorks.Modeling;
using MonoWorks.Controls;
using MonoWorks.Controls.Dock;

namespace MonoWorks.Studio
{
	/// <summary>
	/// The top level scene for Studio.
	/// </summary>
	public class StudioScene : DockSpace
	{
		public StudioScene(Viewport viewport) : base(viewport)
		{
			_drawingBook = new DockBook(viewport);
			Root = _drawingBook;
			
			new StudioController(this);
		}

		private readonly DockBook _drawingBook;
		
		
		/// <summary>
		/// Adds a drawing to the main document book.
		/// </summary>
		public DrawingScene AddDrawing(Drawing drawing)
		{
			var scene = new DrawingScene(Viewport);
			scene.Drawing = drawing;
			_drawingBook.Add(scene);
			scene.MakeCurrent();
			return scene;
		}
		
	}
}

