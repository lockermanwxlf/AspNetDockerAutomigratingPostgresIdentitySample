# PostgresIdentitySample

Annoyingly, when you use a local database in development, you have to start your database before you launch your dev server. You can simply run a docker container, which will run automatically upon startup, but it's simpler to use Docker Compose,
which will start both the local database and the server with just one click.

In a Compose project, the server is in it's own isolated network with the database (and compose automatically maps service names to IP addresses). This means it can access the database with `servicename`:`5432`, and thus the connection string must use these two values. A problem arises when you want to apply migrations to your database (using Update-Database in NuGet CLI). 

Since the CLI is ran in your host machine, you must forward a port from `forwardedport` to `5432` of the database container, and since Update-Database uses your connection string, YOU MUST NOW CHANGE TWO PARTS OF YOUR CONNECTION STRING (`servicename`->`localhost`, `5432`->`forwardedport` AND CHANGE IT BACK AFTERWARD!!!!!!!! very tedious.   

To fix this we will make our database automatically apply migrations. What this really means is that any migrations in our migrations folder that we haven't applied yet are applied on startup. What's important is that they are applied in the server container, which means we don't have to change our connection string.

# [docker-compose.yml](docker-compose.yml)

Here, two services are defined: `postgresidentitysample` (the server), and `identity` (the database).

By default, Docker puts the two containers for these services under their own isolated network. The postgres image exposes port 5432, so our server can use 5432 by default as it's in the same network. Our host machine is not in the same network, which is why we forward 15001 to 5432. We talk to the database with `localhost:15001`, but the server talks to it with `identity container ip address:5432`. Docker allows you to use the service name in leiu of the ip address (you should always use the service name and not the ip address), so it can also use `identity:5432`. We can see this in the [connection string](PostgresIdentitySample/appsettings.json).

# Database

In [PostgresIdentitySample/Identity](PostgresIdentitySample/Identity) and [Program.cs](PostgresIdentitySample/Program.cs) we create the identity DB per [this Microsoft article](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-8.0). Note the end of Program.cs where the auto-migration logic is located. You still have to create migrations with Add-Migration, which is from the [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/latest?_src=template#versions-body-tab) NuGet package.
