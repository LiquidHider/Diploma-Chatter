dotnet build

coverlet .\bin\Debug\net6.0\Chatter.Domain.Tests.IntegrationTests.dll --target "dotnet" --targetargs "test --no-build"

cmd /k