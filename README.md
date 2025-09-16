# Transaction Explorer

A comprehensive transaction management system with API backend, database integration, and responsive web frontend. This solution provides currency conversion capabilities with real-time exchange rates from the U.S. Treasury API, featuring a robust .NET Core backend and modern React frontend.

## 🏗️ Project Structure

```
transaction-explorer/
├── up.sh                   # Master startup script (starts all services)
├── down.sh                 # Master shutdown script (stops all services)
├── backend/                # .NET Core API backend
│   ├── WebApi/             # ASP.NET Core Web API
│   ├── Services/           # Business logic and external API clients
│   ├── Data/               # Entity Framework models and repositories
│   ├── Tests/              # Unit test project (MSTest)
│   ├── docker-compose.yml  # Backend Docker deployment
│   └── DEPLOYMENT.md       # Backend deployment guide
├── database/               # SQL Server database setup
│   ├── init-scripts/       # Database initialization scripts
│   ├── migrations/         # Database schema migrations
│   └── docker-compose.yml  # Database Docker setup
├── frontend/               # React frontend application
│   ├── src/                # React components and utilities
│   ├── build/              # Production build output
│   └── package.json        # Node.js dependencies
└── docs/                   # Project documentation
```

## ⚡ Quick Start

### Prerequisites
- **Docker Desktop**: Required for containerized services
- **Node.js** (v18+): For frontend development
- **.NET 9.0 SDK**: For backend development (optional if using Docker)

### Full System (Recommended)
```bash
./up.sh     # Start entire system (database + backend + frontend)
./down.sh   # Stop entire system
```

### Individual Services

#### Backend API
```bash
cd backend
./up.sh     # Start backend
./down.sh   # Stop backend
```
- Backend API: http://localhost:5070
- Swagger UI: http://localhost:5070/swagger

#### Database Only
```bash
cd database
./up.sh     # Start SQL Server database
./down.sh   # Stop database
```
- Database: localhost:1433
- Username: `sa`
- Password: `YourStrong!Passw0rd`

#### Frontend Only
```bash
cd frontend
npm install
npm run dev
```
- Frontend: http://localhost:3000

## 🔧 Dependencies

### Dockerization
This solution leverages **Docker Desktop** for consistent development and deployment environments:
- All services can run in Docker containers
- Docker Compose orchestrates multi-service deployments
- Database persistence through Docker volumes
- Cross-platform compatibility (Windows, macOS, Linux)

### Testing
The project includes comprehensive unit testing:
- **MSTest Framework**: Microsoft's testing framework for .NET
- **Moq**: Mocking framework for isolating dependencies
- **Entity Framework InMemory**: In-memory database for testing
- Run tests: `dotnet test` from the backend directory

### Core Technologies

#### Backend (.NET 9.0)
- **[.NET Core](https://dotnet.microsoft.com/)**: Modern, cross-platform framework
- **[ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)**: Web API framework
- **[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)**: ORM for database operations
- **[SQL Server](https://www.microsoft.com/en-us/sql-server)**: Relational database backend
- **[Polly](https://github.com/App-vNext/Polly)**: Resilience library for handling transient exchange rate API failures
- **[RestSharp](https://restsharp.dev/)**: HTTP client for flexible exchange rate API calls
- **[Swashbuckle/Swagger](https://swagger.io/)**: API documentation and testing interface

#### Frontend (React 18)
- **[React](https://reactjs.org/)**: Modern JavaScript library for building user interfaces
- **[TypeScript](https://www.typescriptlang.org/)**: Type-safe JavaScript development
- **[Vite](https://vitejs.dev/)**: Fast build tool and development server
- **[Tailwind CSS](https://tailwindcss.com/)**: Utility-first CSS framework
- **[Radix UI](https://www.radix-ui.com/)**: Accessible component primitives

### External API Dependency
The application depends on the **U.S. Treasury Exchange Rate API** for real-time currency conversion rates:
- **API Documentation**: [U.S. Treasury Fiscal Data API](https://fiscaldata.treasury.gov/datasets/treasury-reporting-rates-exchange/)
- **Endpoint**: Treasury Reporting Rates of Exchange
- **Resilience**: Polly handles transient failures with retry policies
- **Caching**: Exchange rates are cached to reduce API calls

## ✨ Features

### Responsive Design
The frontend is fully responsive and optimized for:
- 📱 **Mobile phones**: Touch-friendly interface with optimized layouts
- 📱 **Tablets**: Adaptive design for medium screen sizes
- 💻 **Desktop**: Full-featured interface with enhanced productivity
- 🌓 **Light & Dark Mode**: Toggle between light and dark themes for optimal viewing comfort

### API Documentation
- **Swagger UI**: Interactive API documentation and testing interface
- **Access**: Once the Web API is started, visit http://localhost:5070/swagger
- **Features**: Test endpoints, view request/response schemas, download OpenAPI spec

### Logging & Monitoring
- **Controller Logging**: Comprehensive logging implemented within API controllers
- **Structured Logging**: Uses .NET's built-in logging framework
- **Request/Response Tracking**: Monitor API usage and performance
- **Error Handling**: Graceful error responses with appropriate HTTP status codes

## 🧪 Running Tests

Execute the unit test suite:

```bash
# Run all tests
cd backend
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test project
dotnet test Tests/Tests.csproj
```

Test coverage includes:
- Controller endpoint testing
- Repository data access testing
- Exchange rate service testing
- API client retry logic testing

## 📋 Requirements Implementation

This solution addresses the requirements specified in the **WEX TAG and Gateways Product Brief**:

### ✅ Core Requirements Met
- **Transaction Management**: Complete CRUD operations for purchase transactions
- **Currency Conversion**: Real-time exchange rate integration with U.S. Treasury API
- **Data Persistence**: SQL Server database with proper schema and relationships
- **API Design**: RESTful API following industry best practices
- **Error Handling**: Comprehensive error handling with appropriate HTTP status codes
- **Input Validation**: Robust validation for all API endpoints
- **Documentation**: Complete API documentation with Swagger/OpenAPI

### ✅ Technical Requirements
- **Modern Framework**: Built with .NET 9.0 and React 18
- **Containerization**: Docker support for all components
- **Testing**: Unit test coverage for critical components
- **Resilience**: Polly integration for handling external API failures
- **Responsive UI**: Mobile-first design approach
- **Security**: Input validation and SQL injection protection

### ✅ Additional Features
- **Performance**: Optimized database queries and caching strategies
- **Monitoring**: Comprehensive logging and error tracking
- **Deployment**: Streamlined deployment with Docker Compose
- **Development Experience**: Hot reload, auto-restart, and debugging support

## 📚 Documentation

- **Backend Deployment**: [`backend/DEPLOYMENT.md`](backend/DEPLOYMENT.md)
- **Database Setup**: [`database/README.md`](database/README.md)
- **API Documentation**: [U.S. Treasury Fiscal Data](docs/API%20Documentation%20_%20U.S.%20Treasury%20Fiscal%20Data.md)
- **Product Requirements**: [WEX TAG and Gateways Product Brief](docs/WEX%20TAG%20and%20Gateways%20Product%20Brief.md)

## 🔧 Development

### Local Development Setup
1. **Clone the repository**
2. **Start Docker Desktop**
3. **Run the backend system**: `./up.sh` (starts database + backend API)
4. **Start the frontend** (not containerized yet):
   ```bash
   cd frontend
   ./start.sh
   ```
5. **Access the application**:
   - Frontend: http://localhost:3000
   - API: http://localhost:5070
   - Swagger: http://localhost:5070/swagger
   - Database: localhost:1433

### Development Workflow
- **Backend Changes**: Hot reload enabled with `dotnet watch`
- **Frontend Changes**: Vite provides instant HMR (Hot Module Replacement)
- **Database Changes**: Apply migrations with Entity Framework tools
- **Testing**: Run `dotnet test` after making changes

### Individual Component Development
Each component can be developed and deployed independently:
- **Full System**: Use root `./up.sh` and `./down.sh` for complete system management
- **Backend**: Located in `/backend` - .NET Core API with Docker support
- **Database**: Located in `/database` - SQL Server with initialization scripts
- **Frontend**: Located in `/frontend` - React application with Vite

For detailed deployment instructions, see the respective documentation files in each directory.
