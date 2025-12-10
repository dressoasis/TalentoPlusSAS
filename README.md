# TalentoPlus S.A.S - Human Talent Management System

RESTful API developed in .NET 8 for managing employees, departments, and CV generation.

## 🚀 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- [PostgreSQL](https://www.postgresql.org/) (optional if using Docker)

## 🛠️ Configuration and Execution with Docker

The easiest way to run the project is using Docker Compose, which spins up the API, PostgreSQL database, and PgAdmin.

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/your-username/talentoplus.git
    cd talentoplus
    ```

2.  **Configure Environment Variables:**
    Ensure you have a `.env` file in the root directory (or use the default variables in `docker-compose.yml` for local development).

3.  **Run with Docker Compose:**
    ```bash
    docker-compose up -d --build
    ```

4.  **Access Services:**
    - **API (Swagger):** [http://localhost:5111/swagger](http://localhost:5111/swagger)
    - **PgAdmin:** [http://localhost:5050](http://localhost:5050)
      - Email: `admin@admin.com`
      - Password: `admin`

## 💻 Local Execution (.NET CLI)

If you prefer to run it directly on your machine:

1.  **Restore Packages:**
    ```bash
    dotnet restore
    ```

2.  **Run Migrations (if using local DB):**
    ```bash
    dotnet ef database update -p src/TalentoPlus.Infrastructure.Data -s src/TalentoPlus.Api
    ```

3.  **Run the API:**
    ```bash
    cd src/TalentoPlus.Api
    dotnet run
    ```
    The API will run at `http://localhost:5111` (or the configured port).

## 🧪 Tests

To run unit tests:

```bash
cd src/TalentoPlus.Tests.Unit
dotnet test
```

## 🏗️ Project Structure

- **TalentoPlus.Api**: Controllers and startup configuration.
- **TalentoPlus.Application**: Business logic, services, and DTOs.
- **TalentoPlus.Domain**: Entities and enums.
- **TalentoPlus.Infrastructure.Data**: DB Context and repositories.
- **TalentoPlus.Infrastructure.Integrations**: External services (PDF, Excel, Email, AI).
