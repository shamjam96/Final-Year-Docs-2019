This project will have to be built in Unity,  due to the large filesize created (>1GB).
How to build:
Open Unity project in Assets/Main.Unity
In Unity -> File -> Build Settings. If Universal Windows PLatform isn't selected, then select it and click Switch Platform.
Settings: Target Device is HoloLens, Archictecture is x86 and Build Type is D3D. 
Build Configuration is "Debug". Turn on Copy References, and Unity c# Projects.
Click Build and Select a folder. Reccomended is to create a new folder called App.

When built succesfully, the folder App should open automatically. The C# Project Visual Studio solution file "Cubes Test.sln" will be in this folder. 

The source code for this project is in Assets, however for debugging purpose you can edit the solution code in the App folder.