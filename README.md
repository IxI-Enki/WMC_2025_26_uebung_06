# WMC_2025_26_uebung_08

Übung 8 für WMC 2025/26 - Clean Architecture mit CQRS

## 🏗️ Projekt-Struktur

```
┌─────────────────────────────────────────────┐
│   Clean Architecture + CQRS Pattern         │
├─────────────────────────────────────────────┤
│  API Layer         → Controllers, Swagger   │
│  Application Layer → Commands, Queries      │
│  Domain Layer      → Entities, Validation   │
│  Infrastructure    → DbContext, Repos       │
└─────────────────────────────────────────────┘
```

## ✨ Features

- Device Management (Smartphones, Tablets, Notebooks)
- Person Management mit Email-Validierung
- Usage Tracking mit Overlap Detection
- Optimistic Concurrency (RowVersion)
- Cascade Delete
- CSV Data Seeding
- Swagger UI für API-Tests

## 🚀 Start

```powershell
cd clean_architecture_08/Api
dotnet run
```

Swagger UI: http://localhost:5254/swagger

---
*Implemented with ❤️ using Clean Architecture principles*
