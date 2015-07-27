using System;
using Newtonsoft.Json;

namespace ImageMultiplier
{
	/// <summary>
	/// iOS image set image.
	/// </summary>
	public class iOSImageSetImage
	{
		/// <summary>
		/// Gets or sets the device idiom.
		/// </summary>
		/// <value>The idiom.</value>
		public string idiom { get; set; }

		/// <summary>
		/// Gets or sets the image filename.
		/// </summary>
		/// <value>The filename.</value>
		public string filename { get; set; }

		/// <summary>
		/// Gets or sets the image scale.
		/// </summary>
		/// <value>The scale.</value>
		public string scale { get; set; }

		/// <summary>
		/// Gets or sets the subtype.
		/// </summary>
		/// <value>The subtype.</value>
		[JsonProperty (NullValueHandling = NullValueHandling.Ignore)]
		public string subtype { get; set; }
	}
}

