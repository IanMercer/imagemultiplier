# imagemultiplier

Image Multiplier is an AddIn for Xamarin Studio.

It adds a new file type '.multiplier' to the Add new menu under the category Misc.

In this file you specify a set of output image files that you want to create and then
a set of source SVG files you want to process. It multiplies each source SVG file by
each matched output specifier to create the large number of PNG files that are needed
for a typical, modern Android and iOS project in Xamarin.

Here is a sample '.multiplier' file:

    # This is a sample Image Multiplier specification file
    # In here you define each of the output types you want to create
    # And then provide search expressions for the SVG files you want to process
    # Each SVG file is processes against the matching type specifiers to create
    # multiple PNG files in all of the sizes you need for your Android and iOS projects
    
    { type: 'AndroidIcon', width: 36, color: '#f80c12', path: 'Android/Resources/drawable-ldpi/{0}' }
    { type: 'AndroidIcon', width: 48, color: '#f80c12', path: 'Android/Resources/drawable-mdpi/{0}' }
    { type: 'AndroidIcon', width: 72, color: '#f80c12', path: 'Android/Resources/drawable-hdpi/{0}' }
    { type: 'AndroidIcon', width: 96, color: '#f80c12', path: 'Android/Resources/drawable-xhdpi/{0}' }
    { type: 'AndroidIcon', width: 96, color: '#f80c12', path: 'Android/Resources/drawable/{0}' }
    
    { type: 'iosIcon', width:  32, path: 'iOS/Resources/{0}@1x' }
    { type: 'iosIcon', width:  29, path: 'iOS/Resources/{0}29@1x' }
    { type: 'iosIcon', width:  57, path: 'iOS/Resources/{0}57@1x' }
    { type: 'iosIcon', width:  58, path: 'iOS/Resources/{0}29@2x' }
    { type: 'iosIcon', width:  80, path: 'iOS/Resources/{0}40@2x' }
    { type: 'iosIcon', width: 114, path: 'iOS/Resources/{0}57@2x' }
    { type: 'iosIcon', width: 120, path: 'iOS/Resources/{0}60@2x' }
    
    { type: 'TabIcon', width: 52, color: '#f80c12', path: 'Android/Resources/drawable-ldpi{0}' }
    
    # You can process multiple directories, each with different type specifiers
    { process: './*.svg', as: ['AndroidIcon', 'iosIcon'] }

Note how the 'as' parameter in process instruction lists the 'type' values from earlier in the file
that should be created as outputs.

Note also that the 'path' parameter is a format string into which the source file name will be poured.

Each line in the file is a JSON object although the file itself is not a JSON object.

## Setup / Install

If you just want the add-in you can download the ImageMultipler_1.3.2.mpack (or later) file and then install it
manually into Xamarin Studio using the menu item 'Xamarin Studio' > 'Add-in Manager...' and then click 'Install from file ...'.


## Build

To build the Addin package, compile the solution and then open a terminal window and run the following commmand:

    "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool" setup pack ImageMultiplier/Properties/Manifest.addin.xml

This will create a file called "__Manifest_0.0.0.0.mpack" which you can then rename and copy to somewhere
people can install it.


