$basePath = "c:\Users\Admin\Desktop\asp.net\SupplyChain"
New-Item -ItemType Directory -Force -Path $basePath

dotnet new sln -n SupplyChainSystem -o $basePath
dotnet new classlib -n SupplyChainSystem.Core -o "$basePath\SupplyChainSystem.Core"
dotnet new classlib -n SupplyChainSystem.Application -o "$basePath\SupplyChainSystem.Application"
dotnet new classlib -n SupplyChainSystem.Infrastructure -o "$basePath\SupplyChainSystem.Infrastructure"
dotnet new webapi -n SupplyChainSystem.API -o "$basePath\SupplyChainSystem.API"

Set-Location $basePath
dotnet sln add SupplyChainSystem.Core
dotnet sln add SupplyChainSystem.Application
dotnet sln add SupplyChainSystem.Infrastructure
dotnet sln add SupplyChainSystem.API

Set-Location "$basePath\SupplyChainSystem.Application"
dotnet add reference ../SupplyChainSystem.Core

Set-Location "$basePath\SupplyChainSystem.Infrastructure"
dotnet add reference ../SupplyChainSystem.Core
dotnet add reference ../SupplyChainSystem.Application
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package BCrypt.Net-Next

Set-Location "$basePath\SupplyChainSystem.API"
dotnet add reference ../SupplyChainSystem.Core
dotnet add reference ../SupplyChainSystem.Application
dotnet add reference ../SupplyChainSystem.Infrastructure
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore.Design
