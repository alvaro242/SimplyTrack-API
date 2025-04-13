# SimplyTrack API

**SimplyTrack API** is a backend application developed using .NET 8 Web API. The project is designed to serve as the core for the SimplyTrack system, enabling efficient data processing and interaction via RESTful endpoints. It uses MariaDB as its database and follows secure practices by leveraging environment variables for sensitive configuration details.

## Features
- Basic structure for RESTful API endpoints.
- Integration with MariaDB using Entity Framework Core with the Pomelo provider.
- Environment variable support for secure management of database credentials.
- Dockerized deployment for seamless scalability.

## Setup

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/)
- MariaDB server
- Docker (for containerization, optional)

### Configuration
1. Clone the repository:
   ```bash
   git clone https://github.com/alvaro242/SimplyTrack-API.git
   cd SimplyTrack-API

2. Create a .env file: In the root of the project, create a .env file with the following content:
   ```bash
    DB_SERVER=localhost
    DB_DATABASE=SimplyTrack
    DB_USER=YourDatabaseUser
    DB_PASSWORD=YourDatabasePassword

3. Add .env to .gitignore: To prevent sensitive information from being committed:
    ```bash
      .env

### Database Migration
  1. Install Entity Framework Core tools:
       ```bash
       dotnet tool install --global dotnet-ef
    
  2. Create an initial migration:
       ```bash
        dotnet ef migrations add InitialCreate
  3. Apply Migrations:
    ```bash
        dotnet ef database update

### Database Migration
  1. Restore dependencies:
      ```bash
        dotnet restore
  2. Run the application:
      ```bash
      dotnet run
  3. Access the API: Visit the Swagger documentation at:
     ```bash
      http://localhost:5000/swagger
### Database Migration
  1. Build the Docker image:
      ```bash
        docker build -t simplytrack-api .
  2. Run the Docker container:
      ```bash
        docker run -d -p 8060:80 --env-file .env simplytrack-api
  3. Access the API: Visit the Swagger documentation at:
       ```bash
      http://localhost:8060/swagger

### Technologies Used
- .NET 8: For building the backend.

- MariaDB: Database management.

- Entity Framework Core: Object-relational mapping (ORM).

- Pomelo.EntityFrameworkCore.MySql: MariaDB provider for EF Core.

- Docker: For containerized deployment.

       




