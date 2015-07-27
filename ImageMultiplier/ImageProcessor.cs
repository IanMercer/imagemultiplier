using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using Svg;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace ImageMultiplier
{
	/// <summary>
	/// Image Processor converts directories full of SVG files into PNG files
	/// </summary>
	public class ImageProcessor
	{
		private readonly IProgressMonitor monitor;
		private readonly SingleFileCustomToolResult result;

		public ImageProcessor (IProgressMonitor monitor, SingleFileCustomToolResult result)
		{
			this.monitor = monitor;
			this.result = result;
		}

		public string GetFullOutputPath (string dir, OutputSpecifier outputter, string filename)
		{
			string formattedPath = string.Format (outputter.path, Path.GetFileNameWithoutExtension (filename)) + ".png";
			string fullPath = Path.Combine (dir, "..", formattedPath);
			return fullPath;
		}

		public void Process (string dir, IEnumerable<string> svgFiles, IEnumerable<OutputSpecifier> outputters, int lineNumber)
		{
			foreach (var svgFile in svgFiles) {
				monitor.Log.WriteLine (svgFile);

				if (!File.Exists (svgFile)) {
					result.Errors.Add (new CompilerError (svgFile, lineNumber, 1, "Err2", "File not found " + svgFile));
					continue;
				}

				var inputInfo = new FileInfo (svgFile);

				foreach (var outputter in outputters) {
					string formattedPath = GetFullOutputPath (dir, outputter, svgFile);
					ProcessOneFile (inputInfo, svgFile, formattedPath, outputter, lineNumber);
				}
				;
			}
		}

		private void ProcessOneFile (FileInfo inputInfo, string svgFile, string outputPath, OutputSpecifier outputter, int lineNumber)
		{
			try {
				var outputInfo = new FileInfo (outputPath);

				//Create Directory if it doesn't exist
				outputInfo.Directory.Create ();

				if (!outputInfo.Exists) {
					monitor.Log.WriteLine ("   --> PNG is new : " + outputPath);
				} else if (outputInfo.LastWriteTimeUtc > inputInfo.LastWriteTimeUtc) {
					monitor.Log.WriteLine ("   --> PNG is up-to-date: " + outputPath);        // Assumes color, size are in the file name so no changes there
					return;
				} else {
					// Assumes color, size are in the file name so no changes there
					monitor.Log.WriteLine ("   --> PNG is older: " + outputPath + " " + (inputInfo.LastWriteTimeUtc - outputInfo.LastWriteTimeUtc).TotalMinutes + " minutes");
				}

				// Open a separate copy of the SVG file for each outputter as we will modify height and width
				var svgDocument = SvgDocument.Open (svgFile);
				if (svgDocument == null) {
					result.Errors.Add (new CompilerError (svgFile, lineNumber, 1, "Err3", "Could not open SvgDocument " + svgFile));
					return;
				}
					
				var OrigionalWidth = svgDocument.Bounds.Width;
				var OrigionalHeight = svgDocument.Bounds.Height;
				int? NewWidth = null;
				int? NewHeight = null;

				//Set Width and Height
				if (outputter.width != null && outputter.height != null) {
					NewWidth = outputter.width;
					NewHeight = outputter.height;
				} else if (outputter.width != null) {
					NewWidth = outputter.width;
					NewHeight = (int)Math.Round ((decimal)(OrigionalHeight * (outputter.width / OrigionalWidth)));	
				} else if (outputter.height != null) {
					NewWidth = outputter.height;
					NewHeight = (int)Math.Round ((decimal)(OrigionalWidth * (outputter.height / OrigionalHeight)));
				} else {
					NewWidth = (int)Math.Round (OrigionalWidth);
					NewHeight = (int)Math.Round (OrigionalHeight);
				}

				//Scale the Image
				if (outputter.scaling != null) {
					NewWidth = (int)Math.Round ((decimal)(NewWidth * outputter.scaling));
					NewHeight = (int)Math.Round ((decimal)(NewHeight * outputter.scaling));
				}

				monitor.Log.WriteLine ("Generating {0} Origional Size: {1}x{2} New Size: {3}x{4}", outputPath, OrigionalWidth, OrigionalHeight, NewWidth, NewHeight);

				if (NewWidth != null)
					svgDocument.Width = new SvgUnit (SvgUnitType.Pixel, (int)NewWidth);
				
				if (NewHeight != null)
					svgDocument.Height = new SvgUnit (SvgUnitType.Pixel, (int)NewHeight);

				// Change the default color
				// TODO: This is not yet working
//                if (svgDocument.HasNonEmptyCustomAttribute("style"))
//                {
//                    svgDocument.CustomAttributes["style"] = "fill:#bf1f1f;" + svgDocument.CustomAttributes["style"];
//                }
//                else
//                {
//                    svgDocument.CustomAttributes["style"] = "fill:#bf1f1f";
//                }
//                monitor.Log.WriteLine("Custom attributes style = " + svgDocument.CustomAttributes["style"]);

				using (var bb = new System.Drawing.Bitmap ((int)NewWidth, (int)NewHeight)) {
					svgDocument.Draw (bb);
					bb.Save (outputPath, ImageFormat.Png);
				}

				//If is imageset update Contents.json
				if (outputter.ios_idiom != null) {
					iOSImageSet imageSet;

					var image = new iOSImageSetImage () {
						filename = outputInfo.Name,
						idiom = outputter.ios_idiom,
						scale = outputter.ios_scale ?? "1x"
					};

					var JSONPath = Path.Combine (Path.GetDirectoryName (outputPath), "Contents.json");
					var contentsJSON = new FileInfo (JSONPath);
					if (contentsJSON.Exists) {
						imageSet = JsonConvert.DeserializeObject<iOSImageSet> (File.ReadAllText (contentsJSON.FullName));

						var currentImage = imageSet.images.FirstOrDefault (x => (x.idiom == image.idiom && x.scale == image.scale && x.subtype == image.subtype));
						if (currentImage == null) { 
							imageSet.images.Add (image);
						} else {
							currentImage = image;
						}
					} else {
						imageSet = new iOSImageSet ();
						imageSet.images.Add (image);

					}

					string json = JsonConvert.SerializeObject (imageSet, Formatting.Indented);
					File.WriteAllText (JSONPath, json);
				}
			} catch (Exception ex) {
				result.Errors.Add (new CompilerError (outputPath, lineNumber, 1, "Err27", ex.Message));
			}
		}
	}
}