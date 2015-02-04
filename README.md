# imagemultiplier

Image Multiplier is an AddIn for Xamarin Studio.

It adds a new file type '.multiplier' in which you can specify a set of output image files that
you want to create and a set of source SVG files you want to process. 

Image Multiplier processes each source SVG file against each matched set of output specifiers
to create PNG files. The output specifier defines the size of the image and the file name and
directory where it should be created.

From a simple text file and a set of SVG files, Image Multiplier can create all of the image
sizes you need for any iOS, Android, Windows Phone, Mac, or Web projects in your solution.

You can specify different sets of output image specifiers for each component of your application,
e.g. one set for icons, another for toolbar icons, another for App Store icons and another for
your web site.

To regenerate all your image files simply edit the .multiplier file and save it.

There is no easier way to create all of the PNG files you need for your Xamarin cross-platform
solution.

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

You can install Image Multiplier from the `Addin Manager` menu in Xamarin Studio. Go to `Gallery` and look
under `IDE extensions`.

You can also download the `ImageMultipler_1.3.5.mpack` (or later) file and then install it
manually into Xamarin Studio using the menu item 'Xamarin Studio' > 'Add-in Manager...' and then click 'Install from file ...'.

## Build

To build the Addin package, compile the solution and then open a terminal window and run the following commmand:

    "/Applications/Xamarin Studio.app/Contents/MacOS/mdtool" setup pack ImageMultiplier/bin/debug/ImageMultiplier.dll

This will create the .mpack file which you can install manually by following the instructions above.

