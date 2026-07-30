# Prompt Templates - CashFlow Control AI

Plantillas reutilizables para pedirle al agente de IA tareas comunes en este proyecto.

---

## Crear un nuevo CRUD completo

```
Necesito crear un CRUD completo para la entidad [NOMBRE_ENTIDAD] en CashFlow.

Campos de la entidad:
- [campo1]: [tipo] - [descripción]
- [campo2]: [tipo] - [descripción]

Por favor crea:
1. Script SQL para la tabla y funciones necesarias (si aplica).
2. Modelos/ViewModels: Get[Entidad].cs, Post[Entidad].cs
3. Repositorio Dapper: [Entidad]Repository.cs
4. Servicio backend: [Entidad]Service.cs
5. Controlador: [Entidad]Controller.cs
6. Interface TypeScript: core/interfaces/[entidad].ts
7. Servicio Angular: services/[entidad].service.ts
8. Componente principal con lista (PrimeNG Table)
9. Formulario asociado
10. Agregar ruta en app.routes.ts

Seguir los patrones existentes del proyecto (ver docs/ai-rules.md).
```

---

## Agregar un endpoint nuevo

```
Necesito agregar un nuevo endpoint al [NOMBRE]Controller.

Tipo: GET / POST / PUT
Ruta: [nombre-del-action]
Parámetros: [lista de parámetros]
Respuesta: [qué retorna]

Crear:
1. El método en [NOMBRE]Controller.cs
2. El método en [NOMBRE]Service.cs y [NOMBRE]Repository.cs
3. El ViewModel si es necesario
4. El método en el servicio Angular [nombre].service.ts

Actualizar docs/api-spec.md con el nuevo endpoint.
```

---

## Actualizar documentación

```
Acabo de [agregar/modificar/eliminar] [descripción del cambio].

Por favor actualiza los archivos de documentación relevantes en docs/:
- docs/api-spec.md si cambiaron endpoints
- docs/database.md si cambiaron Tablas o funciones SQL
- docs/architecture.md si cambió la arquitectura
```
