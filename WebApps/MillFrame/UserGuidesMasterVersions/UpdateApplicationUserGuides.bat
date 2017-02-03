@echo %PATH% > myTmpPath.tmp
@find /C /I "PanDoc" myTmpPath.tmp

@if %ERRORLEVEL% neq 0 (
   echo "I didn't find PanDoc on the path, it must be installed on your system and on the system PATH variable"
) else (
pandoc "PRM Client Publisher Console User Guide.docx" -o "..\MillimanClientPublisher\UserGuide\PRM Client Publisher Console User Guide.html" --self-contained
pandoc "PRM Client User Administration Console User Guide.docx" -o "..\MillimanClientUserAdmin\UserGuide\PRM Client User Administration Console User Guide.html" --self-contained
pandoc "PRM User Administration Console User Guide.docx" -o "..\MillimanUserAdmin\UserGuide\PRM User Administration Console User Guide.html" --self-contained
pandoc "PRM Web Portal User Guide.docx" -o "..\Milliman\Milliman\UserGuide\PRM Web Portal User Guide.html" --self-contained
pandoc "PRM Project Management Console User Guide.docx" -o "..\MillimanProjectManConsole\UserGuide\PRM Project Management Console User Guide.html" --self-contained
echo "Created HTML user guides sucessfully and wrote to respective directories...."
)

@del myTmpPath.tmp