using System;
using System.Linq;
using System.CodeDom.Compiler;
using MonoDevelop.Core;
using MonoDevelop.Ide.CustomTools;
using MonoDevelop.Projects;
using System.Text;
using Svg;
using System.Drawing.Imaging;
using System.IO;
using Svg.Transforms;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ImageMultiplier
{
    public class ImageMultiplierGenerator : ISingleFileCustomTool
    {
        public IAsyncOperation Generate(IProgressMonitor monitor, ProjectFile featureFile,
            SingleFileCustomToolResult result)
        {
            return new ThreadAsyncOperation(() => {

                // This is the project directory containing the .imfl file
                string dir = System.IO.Path.GetDirectoryName(featureFile.FilePath);
                monitor.Log.WriteLine("Creating images for " + featureFile.FilePath);

                var lines = System.IO.File.ReadLines(featureFile.FilePath);
                int lineNumber = 0;
                var outputSpecifiers = new List<OutputSpecifier>();
                foreach (var line in lines)
                {
                    lineNumber++;
                    if (String.IsNullOrWhiteSpace(line)) continue;
                    if (line.StartsWith("#")) continue;

                    //monitor.Log.WriteLine("Interpreting " + line);

                    try
                    {
                        // Slight hack ...
                        if (line.Contains("type:"))
                        {
                            var ts = JsonConvert.DeserializeObject<OutputSpecifier>(line);
                            if (ts != null)
                            {
                                string testPath = GetFullOutputPath(dir, ts, "test.svg");
                                var directory = Path.GetDirectoryName(testPath);
                                if (!Directory.Exists(directory))
                                {
                                    result.Errors.Add(new CompilerError(featureFile.FilePath, lineNumber, 1, "Err17", "Directory not found " + directory));
                                }
                                else
                                {
                                    outputSpecifiers.Add(ts);
                                    monitor.Log.WriteLine("Added output specifier " + line);
                                }
                            }
                            else
                            {
                                result.Errors.Add(new CompilerError(featureFile.FilePath, lineNumber, 1, "Err2", "Could not parse output specifier"));
                            }
                        }
                        else if (line.Contains("process:"))
                        {
                            var ps = JsonConvert.DeserializeObject<ProcessSpecifier>(line);
                            if (ps != null)
                            {
                                // Process output!
                                var subdir = Path.GetDirectoryName(ps.process);
                                var searchPattern = Path.GetFileName(ps.process);
                                var inputDirectory = Path.Combine(dir, subdir);

                                foreach (var file in Directory.GetFiles(inputDirectory, searchPattern))
                                {
                                    monitor.Log.WriteLine(file);

                                    var outputters = outputSpecifiers.Where(s => ps.@as.Contains(s.type));
                                    foreach (var outputter in outputters)
                                    {
                                        string formattedPath = GetFullOutputPath(dir, outputter, file);
                                        monitor.Log.WriteLine("    ---> " + formattedPath);
                                        ProcessOneFile(file, formattedPath, outputter.width, monitor, result, lineNumber);
                                    }
                                }
                            }
                            else
                            {
                                result.Errors.Add(new CompilerError(featureFile.FilePath, lineNumber, 1, "Err2", "Could not parse process specifier"));
                            }
                        }
                        else
                        {
                            result.Errors.Add(new CompilerError(featureFile.FilePath, lineNumber, 1, "Err2", "Could not parse this line"));
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add(new CompilerError(featureFile.FilePath, lineNumber, 1, "Err1", ex.ToString()));
                    }
                }

                result.GeneratedFilePath = "";

            }, result);
        }

        private string GetFullOutputPath (string dir, OutputSpecifier outputter, string filename)
        {
            string formattedPath = string.Format(outputter.path, Path.GetFileNameWithoutExtension(filename)) + ".png";
            string fullPath = Path.Combine(dir, "..", formattedPath);
            return fullPath;
        }

        private void ProcessOneFile(string inputPath, string path, int width,
            IProgressMonitor monitor, SingleFileCustomToolResult result, 
            int lineNumber)
        {
            try
            {
                if (!File.Exists(inputPath)) 
                {
                    result.Errors.Add(new CompilerError(inputPath, lineNumber, 1, "Err2", "File not found " + inputPath));
                    return;
                }

                var svgDocument = SvgDocument.Open(inputPath);
                if (svgDocument == null)
                {
                    result.Errors.Add(new CompilerError(inputPath, lineNumber, 1, "Err3", "Could not open svgDocument " + inputPath));
                    return;
                }

                // Wipe out any size information
                svgDocument.Height = new SvgUnit(SvgUnitType.Pixel, width);
                svgDocument.Width =  new SvgUnit(SvgUnitType.Pixel, width);

//                svgDocument.Height = new SvgUnit(SvgUnitType.Percentage, 100.0f);
//                svgDocument.Width = new SvgUnit(SvgUnitType.Percentage, 100.0f);
                //svgDocument.ViewBox = SvgViewBox.Empty;

                var bb = new System.Drawing.Bitmap(width, width);
                svgDocument.Draw(bb);
                bb.Save(path, ImageFormat.Png);
            }
           catch (Exception ex)
            {
                result.Errors.Add (new CompilerError (inputPath, lineNumber, 1, "Err27", ex.Message));
            }
        }
    }
}