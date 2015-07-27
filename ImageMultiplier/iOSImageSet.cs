using System;
using System.Collections.Generic;

namespace ImageMultiplier
{
	/// <summary>
	/// iOS Image set.
	/// </summary>
	public class iOSImageSet
	{
		/// <summary>
		/// Gets or sets the images.
		/// </summary>
		/// <value>The images.</value>
		public List<iOSImageSetImage> images { get; set; } = new List<iOSImageSetImage>();

		/// <summary>
		/// Gets or sets the info.
		/// </summary>
		/// <value>The info.</value>
		public Info info { get; set; } = new Info();


		/// <summary>
		/// iOS Imageset info.
		/// </summary>
		public class Info
		{
			/// <summary>
			/// Gets or sets the ImageSet version.
			/// </summary>
			/// <value>The version.</value>
			public int version { get; set; } = 1;

			/// <summary>
			/// Gets or sets the ImageSet author.
			/// </summary>
			/// <value>The author.</value>
			public string author { get; set; } = "xcode";
		}
	}
}

