@Echo Off
xcopy ..\Source\AtomicMVVM\AtomicMVVM\bin\Release\AtomicMVVM.dll .\lib\net40\ /Y
nuget pack AtomicMVVM.dll.nuspec