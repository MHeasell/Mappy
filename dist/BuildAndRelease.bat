"C:/Program Files (x86)/Microsoft Visual Studio/2017/Community/MSBuild/15.0/Bin/MSBuild.exe" ../Mappy.sln /p:Configuration=Release /t:Clean,Build && python MakeReleaseZip.py --release
@pause
