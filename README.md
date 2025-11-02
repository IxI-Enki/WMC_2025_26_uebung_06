---
title: WMC_2025_26_uebung_08
date: '2025-11-02'
author:
  full_name: Jan Ritt
  github:
    username: IxI-Enki
    url: 'https://github.com/IxI-Enki'
---

<!--markdownlint-disable MD033-->

<div align="center">

![student-at-htl-dark](files/img/student-at-htl-dark.svg)

</div>

<!--markdownlint-enable MD033-->

## 🏗️ Projekt-Struktur

```text
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

Swagger UI: `http://localhost:5254/swagger`

---

<!--

## 💤 So hätte die Sleep Mode ASCII Art aussehen sollen

```ascii-art
   _____ _                   __  __           _
  / ____| |                 |  \/  |         | |
 | (___ | | ___  ___ _ __   | \  / | ___   __| | ___
  \___ \| |/ _ \/ _ \ '_ \  | |\/| |/ _ \ / _` |/ _ \
  ____) | |  __/  __/ |_) | | |  | | (_) | (_| |  __/
 |_____/|_|\___|\___| .__/  |_|  |_|\___/ \__,_|\___|
                    | |
                    |_|
```
-->
