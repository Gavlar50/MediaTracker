Track media usage in your site content, report on where media is referenced and identify potentially unused files. 
Installation adds a MediaTracker.Properties key to your web.config. Edit this item and list all property aliases to be tracked, comma separated. e.g.

add key="MediaTracker.Properties" value="bodyText,footerText"

To enable verbose logging, add the following key to your web.config:

add key="MediaTracker.VerboseLogging" value="true"

New content is tracked on publish. To begin tracking existing content in an existing site after install, republish the parent node and its children. For large sites I recommend you do this in smaller batches. 

Run reports from the Tracker tab in Media pane. To report usages for a specific media item, open the item in the media tree and note it's id from the properties tab. Enter this id in the media report.

Available reports are:
- Show usages for selected media = shows properties and pages where media id is used
- Show usages for all tracked media = as above but for all tracked media
- Show unused media = shows media that is potentially unused, based on tracked properties
- Show media by file size = shows all media in site be descending file size, whether tracked or untracked