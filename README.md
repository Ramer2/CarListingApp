To launch your project, you need to add appsettings.json file with these contents:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "CarListingDb": "<your connection string>"
  },
  "Jwt":{
    "Issuer": <who provides tokens>,
    "Audience": <who is provided with tokens>,
    "Key": <hashing key>,
    "ValidInMinutes": <token's lifespan>
  }
}
```

Data source can be changed via changing the connection string in the appsettings + changing the .AddDbContext options type in Program.cs accroding to your database engine.

Step by step guide:
1. Change the connection string in the appsettings to your connection string
2. Go to Program.cs in CarListingApp.API
3. In line 31 (builder.Services.AddDbContext...) change options.UseSqlLite to the corresponding method
4. If needed, correct the NuGet packages installed in the project
