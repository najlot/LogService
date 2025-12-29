dotnet publish -c Release -r linux-x64 --self-contained src\LogService\LogService.csproj

rmdir deploy_linux_x64 /s /q

robocopy ".\src\LogService\bin\Release\net10.0\linux-x64\publish" ".\deploy_linux_x64\LogService" /MIR
