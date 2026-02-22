dotnet ef migrations add InitialCreate --project src/services/users-module/Users.Infrastructure/Users.Infrastructure.csproj --startup-project src/api/Btree.Api/Btree.Api.csproj --context UsersDbContext

dotnet ef database update --project src/services/users-module/Users.Infrastructure/Users.Infrastructure.csproj --startup-project src/api/Btree.Api/Btree.Api.csproj --context UsersDbContext

dotnet ef database update --project src/services/users-module/Users.Infrastructure/Users.Infrastructure.csproj --startup-project src/api/Btree.Api/Btree.Api.csproj --context UsersDbContext