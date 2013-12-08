Mappy
=====

Mappy is a modern map editor for Total Annihilation, seeking to replace the current community favourite, Annihilator.

Here are some of the current advantages:
* Undo and redo support
* Ability to move placed features around
* No need to "compress" maps to remove duplicate tiles
* Scroll wheel support
* Less weird bugs (e.g. the bug where features are not properly removed)

![Screenshot](screenshot.png?raw=true)

Warning
-------

Beware that Mappy is currently **highly alpha**. For general purpose use, Annihilator is still the best choice. Mappy is still missing a number of basic features and may still crash in certain situations. However, Mappy is looking for interested users to try it out and report back any problems they encounter.

How to Use Mappy
----------------

Mappy is distributed as-is in a zip archive without an installer. Extract the archive to a folder of your choice and run Mappy.exe to launch the program.

When you first run Mappy, no map tilesets or features will be available. First, you should tell Mappy where they are. Go to Edit -> Preferences and add the directories that your tilesets and features live in to Mappy's search paths. The next time Mappy starts it will automatically load any tilesets and features it finds in those places.

The rest is up to you. Create a new map or load an existing one, drag and drop tiles and features from the sidebar, and edit your map's settings.

Settings
--------

Mappy stores its settings in %LocalAppData%/ArmouredFish/Mappy/settings.xml. If, for some reason, you want to delete these, here's how to clear them:

1. Press Windows Key + R.
2. In the Run dialog, type %LocalAppData% and press Enter.
3. In the explorer window that pops up, delete the folder called ArmouredFish.

Known Issues
------------

* Contour lines will not display on newly placed sections until the map is saved and reopened.
* Saving a map takes quite a long time and freezes Mappy until it has finished.

Future Improvements
-------------------

In no particular order, here's a list of features that Mappy might one day support:

* Annihilator-like tools to select and move areas
* Copying and pasting sections and features
* Rendering high quality minimaps
* Anything Annihilator already does that's cool

and possibly more.
