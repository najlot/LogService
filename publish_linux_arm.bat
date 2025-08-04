dotnet publish -c Release -r linux-arm --self-contained src\LogService.Service\LogService.Service.csproj
dotnet publish -c Release -r linux-arm --self-contained src\LogService.Blazor\LogService.Blazor.csproj

rmdir deploy_linux_arm /s /q

robocopy ".\src\LogService.Blazor\bin\Release\net9.0\linux-arm\publish" ".\deploy_linux_arm\LogService.Blazor" /MIR
robocopy ".\src\LogService.Service\bin\Release\net9.0\linux-arm\publish" ".\deploy_linux_arm\LogService.Service" /MIR
