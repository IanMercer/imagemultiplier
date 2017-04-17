using System;
using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using System.Collections.Generic;
using System.IO;
using System.CodeDom.Compiler;
using Svg;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace ImageMultiplier
{
    /// <summary>
    /// Image Processor converts directories full of SVG files into PNG files
    /// </summary>
    public class ImageProcessor
    {
        private readonly ProgressMonitor monitor;
        private readonly SingleFileCustomToolResult result;

        public ImageProcessor (ProgressMonitor monitor, SingleFileCustomToolResult result)
        {
            this.monitor = monitor;
            this.result = result;
        }

        public string GetFullOutputPath (string dir, OutputSpecifier outputter, string filename)
        {
            string formattedPath = string.Format(outputter.path, Path.GetFileNameWithoutExtension(filename)) + ".png";
            string fullPath = Path.Combine(dir, "..", formattedPath);
            return fullPath;
        }

        public void Process (string dir, IEnumerable<string> svgFiles, IEnumerable<OutputSpecifier> outputters, int lineNumber)
        {
            foreach (var svgFile in svgFiles)
            {
                monitor.Log.WriteLine (svgFile);

                if (!File.Exists(svgFile)) 
                {
                    result.Errors.Add(new CompilerError(svgFile, lineNumber, 1, "Err2", "File not found " + svgFile));
                    continue;
                }

                var inputInfo = new FileInfo(svgFile);

                foreach (var outputter in outputters)
                {
                    string formattedPath = GetFullOutputPath (dir, outputter, svgFile);
					ProcessOneFile (inputInfo, svgFile, formattedPath, outputter.width,outputter.height==0?outputter.width:outputter.height ,  lineNumber,outputter.color);
                };
            }
        }

		private void ProcessOneFile(FileInfo inputInfo, string svgFile, string outputPath, int width, int height, int lineNumber,string hexColor)
        {
            try
            {
                var outputInfo = new FileInfo(outputPath);

                if (!outputInfo.Exists)
                {
                    monitor.Log.WriteLine("   --> PNG is new : " + outputPath);
                }
                else if (outputInfo.LastWriteTimeUtc > inputInfo.LastWriteTimeUtc)
                {
                    monitor.Log.WriteLine("   --> PNG is up-to-date: " + outputPath);        // Assumes color, size are in the file name so no changes there
                    return;
                }
                else
                {
                    // Assumes color, size are in the file name so no changes there
                    monitor.Log.WriteLine("   --> PNG is older: " + outputPath + " " + (inputInfo.LastWriteTimeUtc-outputInfo.LastWriteTimeUtc).TotalMinutes + " minutes");
                }

                // Open a separate copy of the SVG file for each outputter as we will modify height and width
                var svgDocument = SvgDocument.Open(svgFile);
                if (svgDocument == null)
                {
                    result.Errors.Add(new CompilerError(svgFile, lineNumber, 1, "Err3", "Could not open SvgDocument " + svgFile));
                    return;
                }

                // Wipe out any size information
                svgDocument.Height = new SvgUnit(SvgUnitType.Pixel, height);
                svgDocument.Width =  new SvgUnit(SvgUnitType.Pixel, width);
				if(!string.IsNullOrEmpty(hexColor)){
					svgDocument.Color = new SvgColourServer(System.Drawing.ColorTranslator.FromHtml(hexColor));
				}

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

                using (var bb = new System.Drawing.Bitmap(width, height))
                {
                    svgDocument.Draw(bb);
                    bb.Save(outputPath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add (new CompilerError (outputPath, lineNumber, 1, "Err27", ex.Message));
            }
        }
    }
}