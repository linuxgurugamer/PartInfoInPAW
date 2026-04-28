# Part info in PAW menu

Mod for Kerbal Space Program intended for mod creators and advanced players.
Get useful information about part from PAW menu (including full module manager patches history), copy part ID or CFG to clipboard, open part CFG (original or in-game presentation) in editor with one click.

## What does this mod do?

1. It displays part name (basically, part ID) and path to part CFG file in the parts list (in VAB/Hangar):
![PartInfoInPAW GetInfo](https://i.imgur.com/x889rHz.png)
This feature is more or less identical to [PartInfo mod by linuxgurugamer](https://forum.kerbalspaceprogram.com/index.php?/topic/182040-*).

2. PartInfoInPAW also adds some information to PAW menu (right-click menu) for alls parts (works in VAB/SPH and in flight).
    This information include:
    * part name (basically, part ID);
    * part mod (GameData folder name);
    * dry/wet mass (takes into account crew mass for KSP 1.11+);
    * empty/full cost (takes into account crew inventories cost for KSP 1.11+);
    * entry cost;
    * current and maximum crew (only for crewable parts).

    ![Part info example](https://i.imgur.com/YEQNnRE.png)

    For engines, mod also display propellants information, thrust and ISP, minimum thrust (in %, and only if engine has non-zero minThrust) and engine gimbal information. If part has more than one ModuleEngines (for example, multi-mode engine like RAPIER), information for first two ModuleEngines will be displayed. RealFuels/RO, KSPIE and other engines are all supported.
    ![Engines info example](https://i.imgur.com/OvDfYXS.png)

3. Mod adds several buttons to PAW menu:
    * **Copy part name** - copy part name (ID) to clipboard;
    * **Copy orig. CFG file** - copy original part CFG file to clipboard 
    * **Open orig. CFG in editor** - opens it in system default editor for .cfg files;
    * **Open orig. CFG in window** - opens it in a window in-game;
    * **Copy part CFG node** - copy part CFG to clipboard 
    * **Open CFG node in editor** - save it to temporary file and open in system default editor for .cfg files. **Important**: "part CFG" here means actual in-game part representation, not text from original part CFG-file. That means, whole PART{} node with all MM-patches already applied to it, also all comments are stripped;
    * **Open CFG node in window** - save it to temporary file and open in a window in-game
    * **MM patches history** - opens window with history all module manager patches. See screenshot:
    ![MM patches history](https://i.imgur.com/o9CXClM.png)

4. All buttons and information options are configurable in game difficulty settings. Choose which options you want to see in PAW menu.
![Settings](https://i.imgur.com/ZJF09LX.png)

## No dependencies

As of version 0.3.0, PartInfoInPAW no longer relies on module manager patch for adding itself to every part. Instead, mod do so automatically during KSP startup.

## Supported KSP versions

KSP 1.8.0 or newer is supported.

## Licensing

The MIT License (MIT)

Copyright (c) 2022-2024 Alexander Rogov

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
