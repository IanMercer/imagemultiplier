using System;
using System.Collections.Generic;

namespace ImageMultiplier
{
    /// <summary>
    /// Specifies how to output a series of images of a given type
    /// </summary>
    public class ProcessSpecifier
    {
        /// <summary>
        /// A path specifier (including wildcards for the input files)
        /// </summary>
        /// <value>The process.</value>
        public string process { get; set; }

        /// <summary>
        /// The types of the output specifiers that should be created
        /// </summary>
        /// <value>As.</value>
        public List<string> @as { get; set; }
    }
}