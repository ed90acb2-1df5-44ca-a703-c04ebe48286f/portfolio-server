# Sample Game Server
The main purpose of creating this repository is to showcase my knowledge in multiplayer game development.

### Solution Overview
- **Gameplay** - Domain layer, whole simulation takes place here. Works by the 'Black Box' principle, encapsulates game simulation and provides interface for interaction with it.
- **Server** - Application layer, packet routing and processing.
- **Startup** - Infrastructure and entry point.
- **Protocol** - Protobuf sources and generated classes, this project will be moved to separate repository and shared with the client in the future.
- **Entities** - Basic ECS implementation.

### Technologies Used
- LiteNetLib
- Protobuf
- Dapper
- FluentMigrator
- PostgreSQL
- Serilog
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Configuration