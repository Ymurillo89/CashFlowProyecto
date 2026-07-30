# Arquitectura del Sistema - CashFlow Control AI

## Visión General

Plataforma web para controlar, validar y centralizar consignaciones de efectivo mediante OCR.

```
┌─────────────────────────────────────────────────────────────┐
│                        CLIENTE                              │
│              Angular (SPA)                                  │
└──────────────────────┬──────────────────────────────────────┘
                       │ HTTP/HTTPS
┌──────────────────────▼──────────────────────────────────────┐
│                       SERVIDOR                              │
│              ASP.NET Core 8.0 (C#)                          │
└──────────────────────┬──────────────────────────────────────┘
                       │ Dapper + SQL
┌──────────────────────▼──────────────────────────────────────┐
│                   BASE DE DATOS                             │
│              PostgreSQL (Docker)                            │
└─────────────────────────────────────────────────────────────┘
```

## Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Frontend | Angular (Standalone) |
| Frontend UI | PrimeNG |
| Frontend CSS | Tailwind CSS |
| Backend | ASP.NET Core 8.0 |
| Lenguaje Backend | C# |
| Base de Datos | PostgreSQL (v15+) |
| ORM | Dapper |
| OCR | (Por definir: Cloud Vision / Gemini) |
| Reportes | Excel (EPPlus / ClosedXML) |

## Estructura de Proyectos

```
CashFlowProyecto/
├── AngularApp1.Server/         # Backend ASP.NET Core 8
│   ├── Controllers/            # Controladores REST
│   ├── Services/               # Lógica de negocio (Servicios)
│   ├── Repositories/           # Acceso a datos con Dapper
│   ├── Models/                 # DTOs y Entidades
│   ├── Program.cs              # Configuración e Inyección de dependencias
│   └── appsettings.json        
│
├── angularapp1.client/         # Frontend Angular
│   └── src/app/
│       ├── core/               # Interfaces y utilidades comunes
│       ├── services/           # Servicios HTTP
│       ├── pages/              # Páginas y componentes (Dashboard, Consignations)
│       └── app.routes.ts       # Enrutamiento principal
│
└── docker-compose.yml          # Contenedores de infraestructura (PostgreSQL)
```

## Patrones de Diseño

### Backend
- **Repository Pattern**: Acceso a datos encapsulado (Dapper).
- **Service Layer**: Lógica de negocio y llamadas OCR separadas de los controladores.
- **DTO Pattern**: ViewModels (respuestas) y SetModels (entradas).

### Frontend
- **Signals**: Estado reactivo con `signal()` y `computed()`.
- **Standalone Components**: Angular moderno sin NgModules.
- **Service Injection**: Uso de `inject()` en vez de constructores.
