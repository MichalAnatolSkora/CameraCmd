dotnet publish -r linux-arm -o ..\build -p:PublishSingleFile=true
scp ..\build\build.zip michal@192.168.1.21:~/