# AI Rules - CashFlow Control AI

Reglas y contexto que el agente de IA debe seguir siempre al trabajar en este proyecto.

---

## Contexto del Proyecto

Este es un sistema de gestión para controlar, validar y centralizar las consignaciones de efectivo de múltiples puntos físicos. El dominio incluye:
- Registro de consignaciones por punto de venta.
- Carga y validación automática de comprobantes de pago mediante IA (OCR).
- Consolidación y dashboard de control.
- Exportación de información estructurada para sistemas ERP.

---

## Stack Obligatorio

**Regla de Idioma Obligatoria:** 
- **Base de Datos:** Estrictamente en **Español** (nombres de tablas con prefijo `Flow_tbl` y columnas).
- **Código Fuente (Backend C# y Frontend Angular/TS):** Estrictamente en **Inglés** (nombres de clases, variables, métodos, interfaces, nombres de archivos, comentarios técnicos).
- **Textos de la Interfaz de Usuario (UI):** Estrictamente en **Español** (todas las etiquetas, textos, botones, alertas y mensajes que visualiza el usuario final en las pantallas).
- *Nota de Dapper:* Usar alias en las consultas SQL del backend para mapear las columnas en español de la base de datos hacia las propiedades en inglés de los modelos de C# (Ej: `SELECT Id, Nombre AS Name FROM Flow_tblEmpresas`).

No introducir nuevas tecnologías sin consultar. El stack está definido:

**Backend**
- ASP.NET Core 8.0 (C#)
- Dapper para acceso a datos (NO Entity Framework para queries)
- PostgreSQL (mediante Docker) como motor de base de datos
- Stored Procedures / Funciones PL/pgSQL para toda la lógica SQL principal (o consultas SQL encapsuladas en repositorios)
- Patrón: Controller → Service → DapperRepository → SQL/SP

**Frontend**
- Angular (versión actual del proyecto) con Standalone Components
- PrimeNG para UI (NO introducir otras librerías de UI)
- Tailwind CSS para estilos
- Signals de Angular (`signal()`, `computed()`) para estado reactivo
- `inject()` en lugar de constructor injection

---

## Convenciones de Código

### Backend (C#)

**Naming**
- Controllers: `[Entidad]Controller.cs`
- Services/Repositories: `[Entidad]Service.cs` / `[Entidad]Repository.cs`
- ViewModels: `Get[Entidad].cs`
- SetModels: `Post[Entidad].cs` / `PostEdit[Entidad].cs`

**Estructura de Controller**
```csharp
[Route("api/[controller]")]
[ApiController]
public class [Entidad]Controller : ControllerBase
{
    private readonly [Entidad]Service _service;

    public [Entidad]Controller([Entidad]Service service)
    {
        _service = service;
    }

    [HttpGet("[action]")]
    public async Task<ActionResult<IEnumerable<Get[Entidad]>>> Get[Entidad]()
    {
        try { 
            var response = await _service.Get[Entidad]Async();
            return Ok(response); 
        }
        catch (Exception ex) { return BadRequest("Mensaje descriptivo."); }
    }
}
```

**Respuesta estándar de mutación**
```csharp
return new Result { Message = "...", Success = true/false };
```

### Frontend (Angular/TypeScript)

**Naming**
- Servicios: `[entidad].service.ts`
- Interfaces: `I[Get/Post][Entidad]` en `core/interfaces/[entidad].ts`
- Componentes: `[entidad]/[entidad].component.ts`

**Estructura de Servicio Angular**
```typescript
@Injectable({ providedIn: 'root' })
export class [Entidad]Service {
  http = inject(HttpClient);
  // Reemplazar con URL de entorno correspondiente
  private apiUrl = 'api/[Entidad]'; 

  get[Entidad]() {
    return this.http.get<I[Get][Entidad][]>(`${this.apiUrl}/Get[Entidad]`);
  }

  post[Entidad](dataSend: any) {
    return this.http.post<IResult>(`${this.apiUrl}/Post[Entidad]`, dataSend);
  }
}
```

**Estructura de Componente**
```typescript
@Component({ selector: 'app-[entidad]', standalone: true, imports: [...] })
export class [Entidad]Component implements OnInit {
  loading = signal<boolean>(false);
  data = signal<I[Get][Entidad][]>([]);

  service = inject([Entidad]Service);
  // messageService = inject(MessageService); // PrimeNG

  ngOnInit() { this.loadData(); }
  
  loadData() {
      // lógica
  }
}
```

**Estado reactivo**: Usar `signal()` para estado local, `computed()` para valores derivados.

---

## Reglas de Acceso a Datos

1. **Nomenclatura de Tablas**: Todas las tablas en la base de datos deben estar en Español y llevar obligatoriamente el prefijo `Flow_tbl` (Ejemplo: `Flow_tblPuntosVenta`, `Flow_tblConsignaciones`).
2. **Dapper y PostgreSQL**: Utilizar consultas parametrizadas o Funciones de PostgreSQL para evitar SQL Injection.
3. **Siempre usar transacciones** en operaciones de escritura (INSERT, UPDATE, DELETE) que involucren múltiples tablas.
4. **Rollback en catch** - Siempre hacer rollback si falla la transacción.

---

## Reglas de API

1. **Rutas**: `[HttpGet/Post/Put("[action]")]` - usar el nombre del método como ruta.
2. **Eliminaciones**: Preferiblemente Soft Delete usando UPDATE, o endpoints dedicados con `POST`/`DELETE`.
3. **Respuesta de error**: Siempre retornar `BadRequest("Mensaje descriptivo")`.
4. **Respuesta de éxito**: `Ok(response)` donde response es el ViewModel o objeto de Resultado.

---

## Reglas de UI

1. **Notificaciones**: Usar `MessageService` de PrimeNG (Toast).
2. **Confirmaciones**: Usar `ConfirmationService` de PrimeNG (ConfirmDialog).
3. **Formularios**: Usar `ReactiveFormsModule` o `FormsModule` según complejidad.
4. **Loading states**: Siempre mostrar estado de carga con `signal<boolean>(false)`.
5. **Tema**: PrimeNG Aura, sin dark mode (a menos que se requiera después).

---

## Estructura de Archivos al Crear Features

Al agregar una nueva feature, crear:

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
    └── [entidad].component.scss (o css)
```

---

## Documentación

Al agregar o modificar features, actualizar los archivos en `docs/`:
- `docs/api-spec.md` - Si se agregan/modifican endpoints
- `docs/database.md` - Si se agregan/modifican tablas o funciones SQL
- `docs/architecture.md` - Si cambia la arquitectura general

### Regla obligatoria: documentar Base de Datos / SQL

**Siempre que se cree o modifique lógica SQL compleja (SPs/Functions)**, actualizar `docs/database.md`.
