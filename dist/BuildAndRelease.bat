MSBuild.exe ../Mappy.sln /p:Configuration=Release /t:Clean,Build && python MakeReleaseZip.py --release
@pause
