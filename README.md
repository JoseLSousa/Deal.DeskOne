# Deal.DeskOne

Plataforma de solicitações com API em .NET 8, autenticação via Keycloak e persistência em PostgreSQL.

## Stack

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core + Dapper
- PostgreSQL
- Keycloak
- Docker Compose

## Pré-requisitos

- .NET SDK 8
- Docker Desktop (ou Docker Engine)

## Subindo dependências (PostgreSQL + Keycloak)

```bash
docker compose up -d
```

O Keycloak inicia em `http://localhost:8080`.

## Import automático do Realm

O `docker-compose.yml` está configurado para importar automaticamente o realm na inicialização (`start-dev --import-realm`).

Coloque o arquivo exportado do realm em:

```
./containers/identity/import
```

## Executando a API

```bash
dotnet restore
dotnet run --project Deal.DeskOne.API
```

A API estará disponível em:

- HTTP: `http://localhost:5181`
- HTTPS: `https://localhost:7183`

Swagger:

- `https://localhost:7183/swagger`

## Configurações

As principais configurações estão em `Deal.DeskOne.API/appsettings.json`:

- `ConnectionStrings:Postgres`
- `Authentication:Authority`
- `Authentication:Audience`

## Migrations

Para aplicar migrations manualmente:

```bash
dotnet ef database update --project Deal.DeskOne.Infrastructure --startup-project Deal.DeskOne.API
```

## Estrutura do projeto

- `Deal.DeskOne.API` – API e endpoints
- `Deal.DeskOne.Application` – casos de uso e handlers
- `Deal.DeskOne.Domain` – regras de negócio e entidades
- `Deal.DeskOne.Infrastructure` – acesso a dados, EF Core, Dapper, integrações
