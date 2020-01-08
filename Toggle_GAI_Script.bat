@echo off
IF EXIST "./GenerateAssemblyInfo.py.orig" (
	REM Milky scripts currently being used, switch back to normal.
	REN "./GenerateAssemblyInfo.py" "./GenerateAssemblyInfo.py.milk"
	REN "./GenerateAssemblyInfo.py.orig" "./GenerateAssemblyInfo.py"
	echo "Normal GenerateAssemblyInfo is now active"
) ELSE (
	REM Original scripts currently being used, switching to milky.
	REN "./GenerateAssemblyInfo.py" "./GenerateAssemblyInfo.py.orig"
	REN "./GenerateAssemblyInfo.py.milk" "./GenerateAssemblyInfo.py"
	echo "Milky GenerateAssemblyInfo is now active"
)
@pause