## WeEdit

## Projects
- API
- CronJob

### Migrations CLI
dotnet ef migrations add [AddedFileName] -o DataAccess/Migrations
dotnet ef migrations remove
dotnet ef migrations remove -f
dotnet ef database update

### Set Environment
Env:ASPNETCORE_ENVIRONMENT = "Development"

### Run
dotnet run --environment "Development"

### Deployment
- Deploy ECS
- Deploy Heroku
