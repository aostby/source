param ([string]$rootpath, [string]$zippath)
# Postbuild call from visual studio like 
# "c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy unrestricted -file C:\Users\asoes\source\repos\zipargs.ps1 -rootpath $(SolutionDir) -zippath "F:\temp\$(TargetName)_$([System.DateTime]::Now.ToString(yyyy-MM-dd_hh-mm-ss))"  
 
Write-Host ($rootpath +" is the path to read from (rootpath).");
Write-Host ($zippath +" is the archive to create (zippath).");

#write-host "Creating "+$zippath
#md -Force $zippath | Out-Null

write-host "Compressing $rootpath to $zippath" 
#Compress-Archive -Path $rootpath -DestinationPath $zippath

$BakFiles = Get-ChildItem -Recurse -Path $rootpath -Include *.cs

$BakFiles | Compress-Archive -DestinationPath $zippath
#$BakFiles | Remove-Item -Force




#"c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy unrestricted -file C:\Users\asoes\source\repos\zipargs.ps1 -rootpath $(SolutionDir) -zippath "F:\temp\Archives"


#7z a -tzip "M:\Shared\DEV\repos\$(TargetName)_$([System.DateTime]::Now.ToString(yyyy-MM-dd_hhmmss)).zip" $(SolutionDir)
#"c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy unrestricted -file C:\Users\asoes\source\repos\zipargs.ps1 -rootpath $(SolutionDir) -zippath "M:\Shared\DEV\repos\$(TargetName)_$([System.DateTime]::Now.ToString(yyyy-MM-dd_hhmmss))"


#"c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy unrestricted -file C:\Users\asoes\source\repos\zipargs.ps1 -rootpath $(SolutionDir) -zippath "M:\Shared\DEV\repos\$(TargetName)_$([System.DateTime]::Now.ToString(yyyy-MM-dd_hhmmss))"