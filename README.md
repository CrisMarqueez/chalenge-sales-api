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

## How to run the project

The project is composed of two applications that need to run in parallel:

- **Backend**: .NET 8 API (`root/src/backend`)
- **Frontend**: Angular 19 application (`root/src/frontend`)

Before starting either of them, it is **mandatory** to start the infrastructure (PostgreSQL) via Docker Compose, since the backend depends on the database to start.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/) and npm
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) with Docker Compose

### 1. Start the database (Docker Compose)

The backend's `docker-compose.yml` already defines the **PostgreSQL** service (as well as Mongo/Redis, in case you want to use them). To start only the Postgres database:

```bash
cd root/src/backend
docker compose up -d ambev.developerevaluation.database
```

To start all the auxiliary services (Postgres, Mongo and Redis):

```bash
docker compose up -d ambev.developerevaluation.database ambev.developerevaluation.nosql ambev.developerevaluation.cache
```

Default Postgres credentials (defined in the compose file):

- Host: `localhost`
- Port: `5432`
- Database: `developer_evaluation`
- User: `developer`
- Password: `ev@luAt10n`

> Make sure the container is running with `docker ps`. The backend **will not start correctly without Postgres**.

### 2. Run the backend (.NET API)

With the database already running, in a terminal:

```bash
cd root/src/backend
dotnet restore
dotnet run --project src/Ambev.DeveloperEvaluation.WebApi
```

The API will be available at:

- HTTP: `http://localhost:5119`
- Swagger: `http://localhost:5119/swagger`

### 3. Run the frontend (Angular)

In **another terminal**, inside the frontend folder:

```bash
cd root/src/frontend
npm install
npm start
```

The application will be available at `http://localhost:4200/`.

The frontend reads the API URL from the `.env` file (variable `VITE_API_BASE_URL`). If it does not exist yet, copy the example:

```bash
cp .env.example .env
```

By default it already points to `http://localhost:5119`, which is where the backend is exposed in development.

### Steps summary

1. `docker compose up -d ambev.developerevaluation.database` (from `root/src/backend`)
2. `dotnet run --project src/Ambev.DeveloperEvaluation.WebApi` (from `root/src/backend`)
3. `npm install && npm start` (from `root/src/frontend`)
