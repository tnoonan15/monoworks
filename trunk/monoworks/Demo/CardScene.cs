// 
//  CardScene.cs - MonoWorks Project
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
using System.Reflection;

using MonoWorks.Base;
using MonoWorks.Rendering;
using MonoWorks.Controls;
using MonoWorks.Controls.Cards;

namespace MonoWorks.Demo
{
	/// <summary>
	/// A demo scene containing a card book.
	/// </summary>
	public class CardScene : Scene
	{

		public CardScene(Viewport viewport) : base(viewport)
		{
			Name = "Cards";
			Mwx = new MwxSource(ResourceHelper.GetStream("cards.mwx"));
			
			var book = Mwx.Get<CardBook>("Book");
			RenderList.AddActor(book);
			book.ComputeGeometry();
			
			var interactor = new GenericCardInteractor<DemoCard>(this) { CardBook = book };
			PrimaryInteractor = interactor;

			var sceneInfo = new SceneInfoOverlay(this);
			RenderList.AddOverlay(sceneInfo);
		}
		
		/// <summary>
		/// The mwx source containing the cards.
		/// </summary>
		public MwxSource Mwx { get; private set; }
	}
}
