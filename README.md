# PartyCLI: VPN Server Listing

A .NET 10 console application for fetching and lsiting VPN server lists from NordVPN API.

## Quick Start

### Prerequisites
- .NET 10 SDK

### First Run
```bash
cd Party.Cli
dotnet run
```

The application will:
- Create the database automatically (party.db)
- Display help information

## Project Structure

```
Party.Core/              Domain layer (entities, enums, interfaces)
Party.Application/       Business logic and services(external APIs)
Party.Infrastructure/    Data access and external APIs
Party.Cli/               Console application entry point
```

## File Locations

### Database
Location: `Party.Cli/party.db`
- Created automatically on first run
- SQLite database storing server information

### Logs
Location: `Party.Cli/logs/`
- File pattern: `partycli-YYYYMMDD.log`
- Daily rotation
- 7 days retention

### Configuration
Location: `Party.Cli/appsettings.json`

Contains:
- Database connection string
- NordVPN API base URL and timeout
- Serilog logging configuration

## Available Commands

### List all servers from API
```bash
dotnet run server_list
```

### List servers by country
```bash
dotnet run server_list --france
dotnet run server_list --albania
dotnet run server_list --denmark
dotnet run server_list --poland
dotnet run server_list --us
dotnet run server_list --uk
```

Available countries: albania, argentina, france, denmark, poland, us, uk
We can add more based on need by just modifing CountryId enums in Party.Core.

### List servers by protocol
```bash
dotnet run server_list --tcp
dotnet run server_list --udp
```

### List servers from local database
```bash
dotnet run server_list --local
```

### Display help
```bash
dotnet run
dotnet run --help
dotnet run -h
```

## Command Syntax

Parameters can be used with or without dashes:
```bash
dotnet run server_list france
dotnet run server_list -france
dotnet run server_list --france
```

All three formats work identically.

## Database Migrations

Migrations run automatically when needed. No manual intervention required.

To create a new migration manually:
```bash
cd Party.Infrastructure
dotnet ef migrations add MigrationName --startup-project ..\Party.Cli
```

## Technology Stack

- .NET 10
- Entity Framework Core 10 with SQLite
- Serilog for logging
- Newtonsoft.Json for API deserialization