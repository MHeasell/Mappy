cd "%APPVEYOR_BUILD_FOLDER%" || goto :error

rem Use python 3, not python 2
SET PATH=C:\Python36-x64;%PATH%

nuget restore Mappy.sln || goto :error
msbuild Mappy.sln /p:Configuration=%Configuration% /m /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll" || goto :error

cd dist || goto :error
IF "%Configuration%"=="Release" (
    python MakeReleaseZip.py --release || goto :error
) ELSE (
    python MakeReleaseZip.py || goto :error
)
for %%i in (*.zip) do appveyor PushArtifact %%i

goto :EOF


:error
exit /b %errorlevel%
