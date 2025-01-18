# AutodeskOnlineHelp2Offline

Source code for auxiliary utility and description how download online help from Autodesk Help portal to local help file (in Qt assistant)

About using that utility read article [How to convert Autodesk online-help to offline format](https://georggrebenyuk.github.io/blog/article_18012025_DOC_1.html)

My compilled files look at [ad_help – Google Drive](https://drive.google.com/drive/folders/1sGKqsnjDdIaWVv900J2g848IfqNAR792). How open QCH file read [it doc](./QT_VIEW.md).

# How use utility

Command-line app (.NET 6.0).

```batch
--workmode         Required. The mode of working. If == GetLinks: set TocTreePath and if need (LinksTextFile,
                     BaseLink);If == ValidateLinks: set TocTreePath and PagesDir;if == CreateHelp: set TocTreePath,
                     PagesDir and if need (ServerPaths);

--toctreepath      Required. The path to toctree.json

--helpcaption      The name of target help, default help

--serverpaths      The part of path to files (used in raw HTML. If more use comma without tabs

--pagesdir         The path directory with downloaded pages

--linkstextfile    Text file name for links (from toctree.json), default _Links.txt

--baselink         Address to help server without las slash, default https://help.autodesk.com

--help             Display this help screen.

--version          Display version information.
```

There are 3 work-modes `--workmode`:

### Mode GetLinks:

Reading toctree.json and getting all links to HTML pages at server.

```batch
AD_HelpGen --mode GetLinks --toctreepath "E:\Temp\AD_C3D_Docs\api\toctree.json"
```

### Mode ValidateLinks

```batch
ADHelpGen --mode ValidateLinks --toctreepath E:\Temp\AD_C3D_Docs\api\toctree.json --pagesdir E:\Temp\AD_C3D_Docs\api\output
```

### Mode CreateHelp

```batch
ADHelpGen --mode CreateHelp --toctreepath E:\Temp\AD_C3D_Docs\api\toctree.json --pagesdir E:\Temp\AD_C3D_Docs\api\output --serverpaths = "https://help.autodesk.com/cloudhelp/2025/ENU/Civil3D-API/files/html/,https://help.autodesk.com/cloudhelp/2025/ENU/"
```

# Dependencies

- `CommandLineParser` NuGet-package for simplify process of CMD-arguments;
