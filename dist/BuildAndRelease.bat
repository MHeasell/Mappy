"A:/Programs/Microsoft Visual Studio/2019/Community/MSBuild/Current/Bin/MSBuild.exe" ../Mappy.sln /p:Configuration=Release /t:Clean,Build && python MakeReleaseZip.py --release
@pause
