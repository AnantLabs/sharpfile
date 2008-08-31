SharpFile
Version 0.7.6.572

A small, efficient orthodox file manager written in C#.

Definitions:
Pane: Contains all of the tabs. For Norton Commander mode (the only supported mode, so far), there are two panes next to each other.
Plugin pane: A pane with dynamic plugins usually pertaining to the currently selected tab (i.e. preview the currently selected file, command line for the currently selected path, etc).
Status bar: Standard status bar at the bottom of the form.
Tab: Contains a drive selection, path, filter, and the ListView.
ListView: Lists the current directories and files with appropriate attributes. Can also contain meta-data for the current path such as a link to the parent or root directory.

Key shortcuts:
Space: Selects a file/directory; calculates the total size of selected files/directories
Tab: Rotate through the panes
Ctrl-Tab: Rotate through the tabs in the active pane
Ctrl-W: Close the active tab
Ctrl-E: Focus the path textbox for the active tab
Ctrl-F: Focus the filter textbox for the active tab
Ctrl-T: Create a new tab in the active pane

The source code for this program is available under the GPL; a copy of the license should be distributed with the executable. 
The executable and source can be downloaded from http://www.longueur.org/software/.

There is also a Google Code project set up which will have more in-depth information, 
read-only svn repository, etc at http://sharpfile.googlecode.com

There are no warranties, implicit or otherwise, included with this software. 
Run at your own risk. And read the source to send me suggestions or improvements.

Email: longueur[dot]software[at]gmail[dot]com