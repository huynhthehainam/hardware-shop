# Hardware shop
This is a simple .NET 6 project I create to manage my family material shop.
It's following Top-Down Architecture and I will upgrade to Clean Architecture in near future.
## Prerequisites
- Redis
- PostgreSQL >= 9.x
- .NET 6.0
- EF Core
- VSCode
## Development guide
If you're using VSCode you just need to run CreateDb.bat file to migrate and seed data to Db. Next just using debug tool (F5) to run this project

## Deployment guide
Because this repository already contains docker-compose.yaml file. So you only need to run command

- Window env

    `docker-compose up -d --build`
- Ubuntu env
- `sudo docker-compose up -d --build`
 
