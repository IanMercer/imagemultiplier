using System;

namespace ImageMultiplier
{
	/// <summary>
	/// Specifies an output size and path format for a converted file
	/// </summary>
	public class OutputSpecifier
	{
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public string type { get; set; }

		/// <summary>
		/// Gets or sets the width.
		/// </summary>
		/// <value>The width.</value>
		public int? width { get; set; }

		/// <summary>
		/// Gets or sets the height.
		/// </summary>
		/// <value>The height.</value>
		public int? height { get; set; }

		/// <summary>
		/// Gets or sets the image scaling factor.
		/// </summary>
		/// <value>The scale.</value>
		public float? scaling { get; set; }

		/// <summary>
		/// Not used yet, but will allow recoloring of SVG icons
		/// </summary>
		/// <value>The color.</value>
		public string color { get; set; }

		/// <summary>
		/// The path format which is string.formatted with the file name stem
		/// </summary>
		/// <value>The path.</value>
		public string path { get; set; }

		/// <summary>
		/// Gets or sets the ios idiom.
		/// </summary>
		/// <value>The ios idiom.</value>
		public string ios_idiom { get; set; }

		/// <summary>
		/// Gets or sets the ios scale.
		/// </summary>
		/// <value>The ios scale.</value>
		public string ios_scale { get; set; }

		/// <summary>
		/// Gets or sets the ios subtype.
		/// </summary>
		/// <value>The ios subtype.</value>
		public string ios_subtype { get; set; }
	}
}