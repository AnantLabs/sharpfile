### Summary ###
This is intended to be a C# implementation of an orthodox file manager. As of right now, it is about alpha-quality, but it is slowly becoming more usable.

![http://sharpfile.googlecode.com/svn/trunk/Images/SharpFile_screenshot_01.jpg](http://sharpfile.googlecode.com/svn/trunk/Images/SharpFile_screenshot_01.jpg)

### Features ###
  * Drag and drop for copy/move
  * Right-click context menu
  * File rename with filename selection and tooltips for incorrect file characters
  * Automatically refreshing file view
  * Calculate directory size inline
  * Tabs
  * Filter files on-the-fly
  * File icons (including TortoiseSVN overlay icons)
  * Most operations are multi-threaded for a smoother experience
  * Massive amounts of configurability
    * Columns (name, order, property to show, whether the data gets altered in any way)
    * Whether or not to show a root/parent directory
    * How to sort directories (all first, like Windows, or the same as files, like UNIX or OS X)
    * How the drive dropdown displays information
    * How the preview panel displays information
  * Plugin support for panes
    * Preview panel which shows the text or thumbnails for files
    * Command-line
  * Dynamic tool menu to use custom inputs for commands
  * Compressed file support

Features in the works: plug-in architecture for different file retrieval (for example, ftp/ssh) or different ways to show the file information (for example, audio files might have a mini player inside the listview), show detailed information about copy/move operations with ability to cancel.

For more information about orthodox file managers, please see the wiki article http://en.wikipedia.org/wiki/Orthodox_file_manager.

SharpFile requires .NET 2.0 Framework only. For more information and download information on the .NET framework and what is necessary for your system, please visit: http://www.SmallestDotNet.com.

### 12/06/2008 ###
A new version has been uploaded. It was released mainly because there was a relatively serious bug when copying folders from one pane to the other. Another bug with middle-clicking to close tabs has been fixed and the functionality has improved somewhat. The CurrentChangelog has been updated with more details. Recently I have been experimenting with localization to allow SharpFile to be used with other languages. After that is more support for file operations like copying/moving/deleting for a better user experience.

I have also been working on a prototype of a [Windows 7-like dock/app launcher/taskbar](http://lifehacker.com/5075076/new-windows-7-taskbar-peek-feature-look-awesome). There is a project for that on [GoogleCode](http://sharpbar.googlecode.com), but no releases just yet. Any feedback is appreciated for either project, though!

### 09/24/2008 ###
Work on SharpFile is going to stop for a few months because my wife and I just had our first child together about three weeks ago. Once he is on more of a schedule and I have some uninterrupted time to get some work done there are lots of ideas and enhancements I have for the project.

### 08/23/2008 ###
The new version of SharpFile is 0.7.6.572. It includes lots of changes including a brand new docking/tabbing solution (DockPanel Suite), which will allow much greater flexibility in the future. I decided to release an interim download before 0.8 because there have been so many changes that were done in the code, including dynamic support for plugins, lots of new keyboard functionality related to the new docking solution and usability enhancements. The CurrentChangelog should be up to date for more information. Please report any problems in the issue tracker.

### 07/21/2008 ###
Lots of work has gone into 0.8 so far. A new docking docking solution has been implemented with Weifen Lou's excellent [DockPanel Suite](http://sourceforge.net/projects/dockpanelsuite/) that will provide a much nicer overall look. It will also enable a more "free-form" docking ability to drag and drop browsers in any configuration you might want. Also completed is a new command line plugin that allows the user to execute files from a command line (it can also process the output and display it, like a regular command line). Hopefully, the rest of the proposed features will be finished soon and then 0.8 will be released.

If you have Visual Studio 2005+ and svn installed you can always download the current source and attempt to build it to see the new features! I try to keep the source as functional as possible, but be aware that there may be some regressions when testing nightly builds!

UDPATE: Oops, I almost forgot to mention that if you find bugs, please report them in the issue tracker so I can take a look.

### 06/12/2008 ###
Version 0.7.061208 uploaded to fix some bugs and add in some small new features including configurable hotkeys. Download links and CurrentChangelog are updated.

### 05/20/2008 ###
Update 0.7.051208 to 0.7.052008 to fix issues with viewing the contents of compressed files. Download links and CurrentChangelog are updated.

### 05/12/2008 ###
Update 0.7 to 0.7.051208 to fix a tiny issue where the Time column was showing the date by default. Download links and CurrentChangelog are updated.

### 05/11/2008 ###
New release available! Check out the CurrentChangelog in the wiki section and download version 0.7!