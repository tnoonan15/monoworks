using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace MonoWorks.GuiWpf.Framework
{
	/// <summary>
	/// Represents an icon that can have various sizes that are loaded from PNG's.
	/// </summary>
	public class Icon
	{
		/// <summary>
		/// Keep track of the number of icons made.
		/// </summary>
		private static int instanceCount = 0;

		public Icon()
		{
			instanceCount++;
			name = "icon" + instanceCount.ToString();
		}

		protected string name;
		/// <summary>
		/// The name of the icon.
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// The bitmap source for each size.
		/// </summary>
		protected Dictionary<int, BitmapSource> sources = new Dictionary<int, BitmapSource>();

		/// <summary>
		/// Adds a file to the decoders.
		/// </summary>
		/// <param name="filePath"></param>
		public void AddFile(string filePath)
		{
			FileStream stream = new FileStream(filePath, FileMode.Open);
			//using (FileStream stream = new FileStream(filePath, FileMode.Open))
			//{
				PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
				BitmapSource source = decoder.Frames[0];
				int size = source.PixelWidth;
				sources[size] = source;
			//}
		}

		/// <summary>
		/// Renders the icon at the given size.
		/// </summary>
		/// <param name="size"></param>
		/// <returns> An image at the given size.</returns>
		public Image Render(int size)
		{
			Image image = new Image();
			if (sources.ContainsKey(size)) // this exact size exists
			{
				image.Source = sources[size];
			}
			else // get the closest size
			{
				int closestSize = 0;
				foreach (int size_ in sources.Keys)
				{
					if (size_ > size)
						closestSize = size_;
				}
				if (closestSize == 0)
					throw new Exception(String.Format("Icon {0} does not have any sources greater than or equal to {1}.", name, size));
				image.Source = sources[closestSize];
			}
			image.Width = size;
			image.Height = size;
			return image;
		}

	}
}
