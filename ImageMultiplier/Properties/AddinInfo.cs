using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly:Addin (
    "ImageMultiplier", 
    Namespace = "ImageMultiplier",
    Version = "1.2"
)]

[assembly:AddinName ("ImageMultiplier")]
[assembly:AddinCategory ("IDE Extensions")]
[assembly:AddinDescription ("Image Multiplier creates icons for Android and iOS from SVG files")]
[assembly:AddinAuthor ("Ian Mercer")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
