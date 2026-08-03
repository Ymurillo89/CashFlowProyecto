# AGENT.md - Instrucciones del Proyecto CashFlow

Este archivo contiene las directrices críticas del proyecto **CashFlowProyecto**. Como agente de IA, debes leer, comprender y adherirte estrictamente a estas reglas en cada interacción.

---

## 🚨 CRITICAL DIRECTIVE: Leer la carpeta `docs/`
Antes de iniciar cualquier tarea, debes leer y seguir **todos** los archivos de documentación dentro de la carpeta `docs/`. Son tu fuente de verdad:
1. [ai-rules.md](file:///d:/PERFIL/Escritorio/letraViva/CashFlow/CashFlowProyecto/docs/ai-rules.md): Reglas estrictas de codificación, stack e idiomas.
2. [architecture.md](file:///d:/PERFIL/Escritorio/letraViva/CashFlow/CashFlowProyecto/docs/architecture.md): Patrones de diseño y arquitectura de la solución.
3. [api-spec.md](file:///d:/PERFIL/Escritorio/letraViva/CashFlow/CashFlowProyecto/docs/api-spec.md): Especificación y diseño de los endpoints HTTP.
4. [database.md](file:///d:/PERFIL/Escritorio/letraViva/CashFlow/CashFlowProyecto/docs/database.md): Modelado físico de base de datos en PostgreSQL.
5. [prompt-template.md](file:///d:/PERFIL/Escritorio/letraViva/CashFlow/CashFlowProyecto/docs/prompt-template.md): Guías de desarrollo para CRUDs y nuevas features.

---

## 📌 1. Regla de Oro: Idioma Obligatorio

El proyecto sigue una convención estricta respecto al idioma:

1. **Base de Datos (PostgreSQL):** Todo en **Español**.
   - Los nombres de tablas deben llevar obligatoriamente el prefijo `Flow_tbl` (ej. `Flow_tblPuntosVenta`, `Flow_tblConsignaciones`).
   - Los nombres de columnas y funciones también se escriben en Español (ej. `monto`, `fecha_consignacion`).
2. **Código Fuente (C# Backend, TypeScript/Angular Frontend):** Estrictamente en **Inglés**.
   - Clases, variables, métodos, interfaces, nombres de archivos, DTOs y comentarios técnicos deben escribirse en Inglés (ej. `GetSalesPoints`, `consignationService`).
   - **Nota sobre Dapper:** Utiliza alias SQL para mapear los campos en español a propiedades en inglés en los modelos de C#.
     *Ejemplo:* `SELECT id_punto AS Id, nombre AS Name FROM Flow_tblPuntosVenta`
3. **Interfaz de Usuario (UI):** Estrictamente en **Español**.
   - Todo lo que el usuario final ve (etiquetas, botones, alertas, confirmaciones, placeholders y mensajes de error de cara al usuario) debe estar en Español.

---

## 🛠️ 2. Stack Tecnológico

No introduzcas ni propongas nuevas tecnologías o dependencias sin autorización explícita. El stack consiste en:

### Backend
- **ASP.NET Core 8.0 (C#)**.
- **Dapper** como ORM ligero para el acceso a datos. **No usar Entity Framework** para consultas.
- **PostgreSQL** montado sobre Docker.
- Toda la lógica principal de base de datos se maneja a través de funciones SQL encapsuladas o consultas inline en los Repositorios.
- Arquitectura en capas: `Controller` ➔ `Service` ➔ `Repository (Dapper)` ➔ `Base de Datos`.

### Frontend
- **Angular** (Stand-alone Components).
- **PrimeNG** como biblioteca principal de componentes de UI. No usar ninguna otra biblioteca de componentes visuales.
- **Tailwind CSS** para maquetación y estilos adicionales.
- Control del estado reactivo mediante **Angular Signals** (`signal()`, `computed()`).
- Inyección de dependencias mediante la función `inject()` (evitar la inyección tradicional en constructores).

---

## 📁 3. Estructura de Archivos

Al crear o modificar una característica (Feature), debes respetar esta jerarquía:

```
Backend (AngularApp1.Server):
├── Controllers/[Entidad]Controller.cs
├── Services/[Entidad]Service.cs
├── Repositories/[Entidad]Repository.cs
└── Models/
    ├── Entities/[Entidad].cs
    ├── ViewModels/Get[Entidad].cs
    └── Dtos/Post[Entidad].cs

Frontend (angularapp1.client):
├── src/app/core/interfaces/[entidad].interface.ts
├── src/app/services/[entidad].service.ts
└── src/app/pages/[entidad]/
    ├── [entidad].component.ts
    ├── [entidad].component.html
    └── [entidad].component.scss
```

---

## 💾 4. Acceso a Datos y Seguridad

- **Transacciones:** Toda operación de escritura (INSERT, UPDATE, DELETE) que afecte a múltiples tablas debe estar protegida por una transacción de base de datos, con rollback automático en el bloque `catch`.
- **SQL Injection:** Usa siempre consultas parametrizadas en Dapper.
- **Eliminaciones:** Implementar borrado lógico (*Soft Delete*) siempre que sea viable en lugar de borrado físico directo.

---

## 📝 5. Documentación Obligatoria

Al hacer modificaciones estructurales o de endpoints, asegúrate de actualizar la documentación existente en la carpeta `docs/`:
- `docs/api-spec.md` (si agregas o alteras endpoints).
- `docs/database.md` (si creas o alteras tablas, columnas o funciones PostgreSQL).
- `docs/architecture.md` (si cambia la arquitectura).
- `docs/ai-rules.md` (reglas generales para desarrollo de IA).
