This folder contains the "source of truth" documents for user guides.  Markdown formats stored here are translated to HTML (with in-line images)
and copied to each applications "UserGuide" directory.  The HTML contained in the applications user guide directory is deployed along with the code.

As of current we use PanDoc to translate the md files to image inlined HTML using (get our copy @ S:\PRM\PRM_IT\MillFrame\Software Component Installs)

pandoc SourceDocument --css=css_style -o TargetDocument --self-contained

pandoc uses extension of TargetDocument to determine type of output,  --self-contained means to inline the images in HTML

Running UpdateApplicationUserGuides.bat will execute pandoc for translation and copy output to respective folders.

To publish projects:
1)Visual Studio - compile all projects in solution
2)Run UpdateApplicationsUserGuides batch file, this will overwrite the placeholder HTML files included in solution
3)Publish projects as normal


At some point we may include PanDoc execution in solution Pre/Post builds, but don't want that dependancy at the moment
