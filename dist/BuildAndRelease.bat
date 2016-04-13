"C:/Program Files (x86)/MSBuild/14.0/bin/amd64/MSBuild.exe" ../Mappy.sln /p:Configuration=Release /t:Clean,Build && python MakeReleaseZip.py --release
@pause
