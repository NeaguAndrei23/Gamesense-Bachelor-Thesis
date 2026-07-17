# GameSense

**An ML-powered playtesting analytics tool that helps indie game designers objectively evaluate level difficulty.**

![Unity](https://img.shields.io/badge/Unity-2D%20Prototype-black?logo=unity)
![C#](https://img.shields.io/badge/C%23-.NET-239120?logo=csharp)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API%20%26%20MVC-512BD4?logo=dotnet)
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-ORM-lightgrey)
![SQLite](https://img.shields.io/badge/SQLite-Database-003B57?logo=sqlite)
![ML.NET](https://img.shields.io/badge/ML.NET-Machine%20Learning-orange)

---

## Overview

Small independent game studios rarely have a dedicated playtesting or data analysis team. Difficulty balancing usually falls back on the designer's own instinct — which is inherently biased, since the designer already knows the level intimately.

**GameSense** closes that gap. It's a full-stack application that automatically collects real playtesting data from a Unity game, aggregates it, and uses a trained machine learning model to predict a level's actual difficulty — Easy, Medium, Hard, or Unfair — from real player behavior. That prediction is then compared against what the designer originally intended, surfacing exactly where perception and reality diverge, and which specific areas of a level are causing the most trouble.

This project was developed as a bachelor's thesis at the **Bucharest University of Economic Studies**, Faculty of Economic Cybernetics, Statistics and Informatics.

---

## Key Features

- **Automatic data collection** — Unity records deaths, session duration, attempts, and completion status during real playtesting, with zero manual tagging.
- **ML-based difficulty prediction** — an SDCA Maximum Entropy classifier (ML.NET), trained on a level's aggregated playtesting statistics, predicts its difficulty objectively.
- **Designer/player mismatch detection** — compares the designer's intended difficulty against the model's prediction, flagging discrepancies directly on the dashboard.
- **Zone-level hotspot analysis** — shows deaths broken down by declared zone, including zones with *zero* deaths, so a designer can distinguish "safe" from "unreached." Each zone can carry a designer-written description, visible on hover.
- **Live, end-to-end pipeline** — from a single death in Unity to a rendered prediction on the dashboard, with no manual steps in between.

---

## Architecture

Three tiers, communicating over HTTP:

```
┌─────────────────┐      HTTP       ┌───────────────────┐      HTTP      ┌──────────────────┐
│  Unity Prototype │ ───────────────▶│  GameSense.API     │◀───────────────│  GameSense.Web     │
│  (C#, 2D)         │                │  ASP.NET Core       │                │  ASP.NET Core MVC   │
│                   │                │  EF Core + SQLite   │                │  Designer dashboard │
│                   │                │  ML.NET pipeline     │                │                    │
└─────────────────┘                 └───────────────────┘                └──────────────────┘
```

Three independent data flows feed the system, converging on a single level's detail view:

| Flow | Origin | Destination |
|---|---|---|
| Gameplay sessions | Unity, on level completion/quit | `Sessions`, `DeathEvents` |
| Zone registration | Unity, on scene load | `LevelZones` |
| Designer configuration | Dashboard's Configure page | `LevelConfigs` |

---

## Machine Learning

The prediction model is a **multiclass SDCA Maximum Entropy classifier**, built with ML.NET, trained on four aggregated features per level:

- Average deaths
- Average session duration
- Completion rate
- Average attempts

SDCA (Stochastic Dual Coordinate Ascent) was chosen specifically for its fast convergence on **small datasets** — a deliberate fit for the target audience, since an indie studio typically has only a handful of levels to learn from, unlike the large-data regimes where gradient descent variants shine. The Maximum Entropy component keeps the model from being overconfident when a level's difficulty is genuinely ambiguous.

The trained model is cached in memory for the API's lifetime, so predictions are near-instant after the first request.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Game client | Unity (C#), 2D physics, custom playtesting instrumentation |
| API | ASP.NET Core Web API |
| Dashboard | ASP.NET Core MVC, Razor, Bootstrap, Chart.js |
| Data access | Entity Framework Core |
| Database | SQLite |
| Machine learning | ML.NET (SDCA Maximum Entropy) |

---

## Project Structure

```
GameSense-Bachelor-Thesis/
├── GameSense Prototype/     # Unity project — SessionManager, HazardZone, PlayerControls
├── GameSense.API/           # REST API — controllers, EF Core models, ML.NET pipeline
└── GameSense.Web/           # MVC dashboard — Levels Overview, Details, Configure
```

---

## Getting Started

**Prerequisites**
- .NET SDK
- Unity (2D template)
- A REST client (e.g. Postman) for seeding the database

**Run the API**
```bash
cd GameSense.API
dotnet restore
dotnet run
```

**Seed sample data**
```
POST https://localhost:7146/api/seed
```

**Run the dashboard**
```bash
cd GameSense.Web
dotnet run
```

**Run the Unity prototype**
Open `GameSense Prototype/` in Unity and press Play. Playtesting sessions will be sent to the API automatically as you play.

---

## Database Schema

Four tables: `Sessions`, `DeathEvents`, `LevelConfigs`, `LevelZones` — with `Sessions → DeathEvents` and `LevelConfigs → LevelZones` as enforced one-to-many relationships. `Sessions.LevelId` intentionally has no foreign key against `LevelConfigs`, allowing playtesting to begin before a level is formally configured.

---

## Academic Context

Developed as a bachelor's thesis — *"Application for Player Behaviour Analysis and Game Difficulty Balancing"* — under the supervision of Prof. Liviu-Adrian Cotfas, PhD., at the Bucharest University of Economic Studies.
