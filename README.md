Mappy
=====

[![Build status](https://ci.appveyor.com/api/projects/status/2o90in7blk4qd3g0/branch/master?svg=true)](https://ci.appveyor.com/project/MHeasell/mappy/branch/master)

Mappy is a modern map editor for Total Annihilation, seeking to replace the current community favourite, Annihilator. Annihilator is the most well-known and widely used map editor, but in many cases Mappy is better. Here's some of the benefits you'll enjoy:

* Full undo and redo support.
* Ability to drag features after placing them.
* Accurate, Cavedog quality minimaps.
* No need to "compress" maps to remove duplicate tiles. Mappy does this automatically.
* Scroll wheel support.
* No known bugs that creep their way into your published maps.
  For example, when you delete a feature in Annihilator, it still leaves a reference in your map.
  This is a problem if you're trying to get rid of a third-party feature pack.

![Screenshot](screenshot.png?raw=true)

How to Use Mappy
----------------

Download and run MappySetup, then follow the on-screen instructions to install Mappy. Once Mappy is installed, run it from the start menu.

Alternatively, download the zip distribution and extract it to a folder of your choice. Then double-click on Mappy.exe to launch the program.

When you first run Mappy, no map tilesets or features will be available. First, you should tell Mappy where they are. Go to `Edit -> Preferences` and add the folders that your tilesets and features live in to Mappy's search paths. The next time Mappy starts, it will automatically load any tilesets and features it finds in those folders.

The rest is up to you. Create a new map or load an existing one, drag and drop tiles and features from the sidebar, and edit your map's settings. Mappy works similarly to Annihilator, so you should feel right at home.

Settings
--------

Mappy stores its settings in `%LocalAppData%/ArmouredFish/Mappy/settings.xml`. If, for some reason, you want to delete these, here's how to clear them:

1. Press `Windows Key + R`.
2. In the Run dialog, type `%LocalAppData%` and press Enter.
3. In the explorer window that pops up, delete the folder called `ArmouredFish`.

Future Improvements
-------------------

In no particular order, here's a list of features that Mappy might one day support:

* Void editing
* Bandbox select/move for features
* Map resizing
* Anything Annihilator already does that's cool

and possibly more.
