Changelog

0.4.0.2
	Thanks to github user infradmin4KSP for these fixes:
		Added localization lines to one string in the settings that was lacking it (Use alternate skin).
		Fixed mistype in parameter name in ru.cfg that resulted in another line missing localization in the settings,
		Enhanced few strings in Russian localization.

0.4.0.1
	Updated name by adding  "Enhanced" to the name in the settings page
	If available, the ClickThroughBlocker will be used, if not, then it will just use the standard window
	Updated README.md

0.4.0
	Adoption by Linuxgurugamer
	Added code to make available in flight.  If not set to be available in flight, will not affect performance (destroys itself)
	Added options to settings to enable/disable individual items in flight, exactly as done in the editor
	Fixed errors when trying to add mode to parts beginning with KerbalEVA or a flag
	Added ability to show files in window in-game in addition to or instead of an external editor
	In-game window also shows the FloatCurve graphs (not available in external editor)
	Added Bulkhead Profiles to PAW
	Updated settings page to reflect new options
	Updated english and russian config for new lines

0.3.0
- Added buttons "Open CFG node in editor" and "Open orig. CFG in editor" to PAW menu. Both buttons open corresponding file in system default editor for .cfg files.
- Added crew / maximum crew information to PAW menu.
- For KSP 1.11+ displayed wet part mass and cost takes into account kerbonauts mass and their inventories mass/cost.
- Engine information in PAW menu was completely overhauled. Now it also contains gimbal information, as well as other useful parameters, depending on engine type. Near/Far Future, KSPIE, RealFuels/RO are supported.
- New button "MM patches history" opens window with tree-like structure of all module manager patches, applied to part, in order of appliance. Information about patches is parsed from module manager cache.
- Mod menu options and buttons are now fully configurable in game difficulty settings ("Part Info in PAW" section).
- PartInfoInPAW adds itself to each and every part during game load. MM patch, adding partmodule to all parts, is no longer needed and is deprecated.
- Mod now only works in VAB/Hangar and is no more available in flight scene.
- Perfomance improvements and code cleanup.

0.2.3
-----
- Fixed "Copy Part CFG to Clipboard" not working for parts, having spaces in their cfg-file path (for example, Coatl Aerospace mod).
- Minor UI improvement: buttons "Copy Part ID to Clipboard" and "Copy Part CFG to Clipboard" now generate on-screen messages then pressed.
- Added part temperature and skin temperature (current/max) to PAW menu, but only for flight scene and if showInfoInFlight is true (see README).

0.2.2
-----
- Slightly changed info format in Parts selection window. Now full path to part CFG file is displayed. PAW menu still has only basic mod folder info to avoid clutter.
- Updated README.

0.2.1
-----
- Added mod folder parameter to displayed information. For some mods (like Bluedog_DB or USI family) 2 or 3 nested mod folder levels are shown (for example, "Bluedog_DB/Parts_Atlas" instead of just "Bluedog_DB").
- Localization implemented, added russian localization.

0.2.0
-----
- Part mass information now include dry and wet (if part contain any resources) mass.
- Added buttons: "copy part name to clipboard" and "copy part CFF node to clipboard". Could be useful for debug purposes.
- Code improvements.

0.1.1
-----
- Information in PAW menu is now properly updated, then part variant (B9PartSwitch) is changed.

0.1.0
-----
- Initial release.