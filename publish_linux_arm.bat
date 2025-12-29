dotnet publish -c Release -r linux-arm --self-contained src\LogService\LogService.csproj

rmdir deploy_linux_arm /s /q

robocopy ".\src\LogService\bin\Release\net10.0\linux-arm\publish" ".\deploy_linux_arm\LogService" /MIR
