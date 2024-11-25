REM parameter 1 - zippath
REM parameter 2 - root to be zip'ed
REM example call - C:\Users\asoes\source\repos\zipargs.cmd "M:\Shared\DEV\repos\$(TargetName)_$([System.DateTime]::Now.ToString(yyyy-MM-dd_HHmmss)).zip" $(SolutionDir)

start /b "" "C:\Program Files\7-Zip\7z.exe" a -tzip %1 %2
