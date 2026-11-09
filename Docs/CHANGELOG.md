# HostsMan Change Log 📋

## v1.2 (2026) *(current)* 🆕
#### 🌟 Improvements:
 - Added a light/dark visual theme switch (top-right corner of the search panel), whose caption shows the active theme. The light theme uses the default Windows system colors.
 - The selected theme is saved in the user settings and restored at startup.
 - The dark theme also darkens the window title bar (DWM immersive dark mode) of all the windows, and it is applied/removed at run-time without restarting.
 - The dark theme includes dark tool strip, context menu, list (owner-drawn column headers, selection and grid lines, dark scroll bars), text boxes, custom-painted combo boxes and buttons.
 - The window title shows the program version.
 - Added "Comment" to the search types, to filter the mappings by their comment.
 - Reviewed the texts and tooltips of the user interface to make them clearer (e.g. "Hostname", "IP address", "Comment" columns, "Open in web browser" menu item).
 - The text of the mappings list is 1 pixel bigger.
 - The buttons of the mapping window are now "OK" and "Cancel", with icons and a smaller font.
 - Added IPv6 support: IPv6 mappings are now listed, and the mapping window accepts IPv4 and IPv6 addresses in a single field, with live validation (invalid values are highlighted and cannot be confirmed).
 - The same hostname can be mapped at the same time to an IPv4 and an IPv6 address.
 - Enter and Escape keys confirm and cancel the mapping window.
 - The program asks to save the pending changes when closing the window, and only asks for confirmation on "Rescan" when there are unsaved changes.
 - The search filter is kept when adding, editing or rescanning mappings, and the selection is preserved.
 - The Enable/Disable buttons now reflect the state of all the selected mappings.
 - Saving is much faster on big Hosts files (block lists with thousands of entries).
 - Saving now replaces the Hosts file atomically (preserving its attributes and permissions) and suggests running as Administrator when access is denied.
 - The About window shows the real program version.
 - Added a unit tests project (MSTest).

#### 🛠️ Fixes:
 - Editing the IP address or the hostname of a mapping had no effect, or showed a wrong "This HostName already exists in list" error.
 - Lines that map several hostnames (e.g. "0.0.0.0 a.com b.com") lost all but the first hostname after saving (the rest were turned into a comment).
 - IPv6 mappings were silently removed from the Hosts file after saving.
 - A commented line containing only an IP address (e.g. "# 10.0.0.1") made the program crash when loading the Hosts file.
 - Malformed addresses (e.g. "1abc.2.3.4") were loaded as mappings, and the mapping window accepted invalid octets (e.g. "300").
 - Two identical entries could not be edited or removed independently.
 - A failed save disabled the Save button, so the pending changes looked saved but were lost; temporary files were left behind.
 - If the Hosts file could not be loaded, saving could overwrite it with an empty list; editing is now disabled in that case.
 - The search by hostname did not work in some cultures (e.g. Turkish) because of culture-sensitive lower-casing.
 - Text column sorting kept using the numeric/date comparer after a numeric/date sort.
 - Mixed line endings (LF/CRLF) when creating a new Hosts file.
 - Several unhandled exceptions and resource leaks (icon handles) in the user interface.
 - Library (HostsTools): space-separated entries were not found by IsMapped/Disable/Remove/GetMappingFromHostname/GetMappingsFromIP; GetMappingFromHostname/GetMappingsFromIP crashed on entries without comment and returned the comment with its leading "#"; GetMappingsFromIP ignored disabled entries; comments with line breaks could inject lines into the Hosts file; FileOpen() always failed with its default "notepad.exe" value.

## v1.1 (2024) 🔄
#### 🌟 Improvements:
 - Source-code upgraded to .NET Framework 4.8

## v1.0 (2015) 🔄
Initial Release.