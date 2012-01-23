@Echo Off
xcopy "..\Source\AtomicMVVM\AtomicMVVM\bin\Release\AtomicMVVM.dll" .\lib\net40\ /Y
xcopy "..\Source\AtomicMVVM\AtomicMVVM (4.5)\bin\Release\AtomicMVVM.dll" .\lib\net45\ /Y
xcopy "..\Source\AtomicMVVM\AtomicMVVM (Silverlight 5)\Bin\Release\AtomicMVVM.dll" .\lib\sl5\ /Y
xcopy "..\Source\AtomicMVVM\AtomicMVVM (WinRT)\bin\Release\AtomicMVVM.dll" .\lib\NetCore45\ /Y
nuget pack AtomicMVVM.dll.nuspec
pause