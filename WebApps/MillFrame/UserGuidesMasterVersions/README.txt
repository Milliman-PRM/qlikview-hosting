This folder contains the "source of truth" documents for user guides.  Word(or maybe other) formats stored here are translated to HTML (with in-line images)
and copied to each applications "UserGuide" directory.  The HTML contained in the applications user guide directory is deployed along with the code.

As of current we use PanDoc to translate the docx files to image inlined HTML using

pandoc SourceDocument -o TargetDocument --self-contained

pandoc uses extension of TargetDocument to determine type of output,  --self-contained means to inline the images in HTML

Running UpdateApplicationUserGuides.bat will execute pandoc for translation and copy output to respective folders.