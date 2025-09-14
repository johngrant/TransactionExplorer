# Transaction Explorer

A comprehensive transaction management system with API backend and database integration.

## 🏗️ Project Structure

```
transaction-explorer/
├── up.sh                   # Master startup script (starts all services)
├── down.sh                 # Master shutdown script (stops all services)
├── backend/                # .NET Core API backend
│   ├── docker compose.yml  # Backend Docker deployment
│   ├── up.sh               # Backend startup script
│   ├── down.sh             # Backend shutdown script
│   ├── DEPLOYMENT.md       # Backend deployment guide
│   └── ...
├── database/               # SQL Server database setup
│   ├── docker compose.yml # Database Docker setup
│   ├── up.sh              # Database startup script
│   ├── down.sh            # Database shutdown script
│   └── ...
├── frontend/               # Frontend application (future)
└── docs/                   # Project documentation
```

## 🚀 Quick Start

### Full System (Recommended)
```bash
./up.sh     # Start entire system (database + backend)
./down.sh   # Stop entire system
```

### Individual Services

#### Backend API
```bash
cd backend
./up.sh     # Start backend
./down.sh   # Stop backend
```
Backend will be available at: http://localhost:5070

#### Database
```bash
cd database
./up.sh     # Start database
./down.sh   # Stop database
```
Database will be available at: localhost:1433

## 📚 Documentation

- **Backend Deployment**: [`backend/DEPLOYMENT.md`](backend/DEPLOYMENT.md)
- **Database Setup**: [`database/README.md`](database/README.md)

## 🔧 Development

Each component can be developed and deployed independently:

- **Full System**: Use root `./up.sh` and `./down.sh` for complete system management
- **Backend**: Located in `/backend` - .NET Core API with Docker support
- **Database**: Located in `/database` - SQL Server with initialization scripts
- **Frontend**: Located in `/frontend` - (Coming soon)

For detailed deployment instructions, see the respective DEPLOYMENT.md files in each directory.
