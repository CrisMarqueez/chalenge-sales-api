# Developer Evaluation Project

`READ CAREFULLY`

## Use Case
**You are a developer on the DeveloperStore team. Now we need to implement the API prototypes.**

As we work with `DDD`, to reference entities from other domains, we use the `External Identities` pattern with denormalization of entity descriptions.

Therefore, you will write an API (complete CRUD) that handles sales records. The API needs to be able to inform:

* Sale number
* Date when the sale was made
* Customer
* Total sale amount
* Branch where the sale was made
* Products
* Quantities
* Unit prices
* Discounts
* Total amount for each item
* Cancelled/Not Cancelled

It's not mandatory, but it would be a differential to build code for publishing events of:
* SaleCreated
* SaleModified
* SaleCancelled
* ItemCancelled

If you write the code, **it's not required** to actually publish to any Message Broker. You can log a message in the application log or however you find most convenient.

### Business Rules

* Purchases above 4 identical items have a 10% discount
* Purchases between 10 and 20 identical items have a 20% discount
* It's not possible to sell above 20 identical items
* Purchases below 4 items cannot have a discount

These business rules define quantity-based discounting tiers and limitations:

1. Discount Tiers:
   - 4+ items: 10% discount
   - 10-20 items: 20% discount

2. Restrictions:
   - Maximum limit: 20 items per product
   - No discounts allowed for quantities below 4 items

## Overview
This section provides a high-level overview of the project and the various skills and competencies it aims to assess for developer candidates. 

See [Overview](/.doc/overview.md)

## Tech Stack
This section lists the key technologies used in the project, including the backend, testing, frontend, and database components. 

See [Tech Stack](/.doc/tech-stack.md)

## Frameworks
This section outlines the frameworks and libraries that are leveraged in the project to enhance development productivity and maintainability. 

See [Frameworks](/.doc/frameworks.md)

<!-- 
## API Structure
This section includes links to the detailed documentation for the different API resources:
- [API General](./docs/general-api.md)
- [Products API](/.doc/products-api.md)
- [Carts API](/.doc/carts-api.md)
- [Users API](/.doc/users-api.md)
- [Auth API](/.doc/auth-api.md)
-->

## Project Structure
This section describes the overall structure and organization of the project files and directories. 

See [Project Structure](/.doc/project-structure.md)

## Como executar o projeto

O projeto é composto por dois aplicativos que precisam ser executados em paralelo:

- **Backend**: API em .NET 8 (`root/src/backend`)
- **Frontend**: aplicação Angular 19 (`root/src/frontend`)

Antes de iniciar qualquer um dos dois, é **obrigatório** subir a infraestrutura (PostgreSQL) via Docker Compose, pois o backend depende do banco para iniciar.

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) e npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) com Docker Compose

### 1. Subir o banco de dados (Docker Compose)

O `docker-compose.yml` do backend já define o serviço do **PostgreSQL** (e também Mongo/Redis, caso queira utilizá-los). Para iniciar apenas o banco Postgres:

```bash
cd root/src/backend
docker compose up -d ambev.developerevaluation.database
```

Para subir todos os serviços auxiliares (Postgres, Mongo e Redis):

```bash
docker compose up -d ambev.developerevaluation.database ambev.developerevaluation.nosql ambev.developerevaluation.cache
```

Credenciais padrão do Postgres (definidas no compose):

- Host: `localhost`
- Porta: `5432`
- Database: `developer_evaluation`
- Usuário: `developer`
- Senha: `ev@luAt10n`

> Verifique se o container está em execução com `docker ps`. O backend **não sobe corretamente sem o Postgres**.

### 2. Executar o backend (.NET API)

Com o banco já em execução, em um terminal:

```bash
cd root/src/backend
dotnet restore
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

A API ficará disponível em:

- HTTP: `http://localhost:5119`
- Swagger: `http://localhost:5119/swagger`

### 3. Executar o frontend (Angular)

Em **outro terminal**, na pasta do frontend:

```bash
cd root/src/frontend
npm install
npm start
```

A aplicação ficará disponível em `http://localhost:4200/`.

O frontend lê a URL da API a partir do arquivo `.env` (variável `VITE_API_BASE_URL`). Caso ainda não exista, copie o exemplo:

```bash
cp .env.example .env
```

Por padrão ele já aponta para `http://localhost:5119`, que é onde o backend é exposto em desenvolvimento.

### Resumo dos passos

1. `docker compose up -d ambev.developerevaluation.database` (a partir de `root/src/backend`)
2. `dotnet run --project src/Ambev.DeveloperEvaluation.WebApi` (a partir de `root/src/backend`)
3. `npm install && npm start` (a partir de `root/src/frontend`)
