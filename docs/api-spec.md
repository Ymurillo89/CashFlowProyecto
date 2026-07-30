# Especificación de API - CashFlow Control AI

Este archivo documenta los endpoints RESTful expuestos por el backend de ASP.NET Core (`AngularApp1.Server`).

## Formato Base

**URL Base:** `/api/[Controller]`

---

## 1. Módulo: Empresas (Company)

Controlador: `CompanyController`
Ruta base: `/api/Company`

### Obtener Todas las Empresas
- **Método:** `GET`
- **Ruta:** `/api/Company`
- **Descripción:** Obtiene la lista completa de empresas registradas.
- **Respuesta:** `200 OK`
  ```json
  [
    {
      "id": 1,
      "name": "Letra Viva S.A.S.",
      "nit": "901234567-8",
      "email": "contacto@letraviva.com",
      "phone": "+57 300 1234567",
      "address": "Calle 45 #12-34",
      "logoUrl": "https://...",
      "isActive": true,
      "createdAt": "2026-07-30T09:12:00Z"
    }
  ]
  ```

### Obtener Empresa por ID
- **Método:** `GET`
- **Ruta:** `/api/Company/{id}`
- **Descripción:** Obtiene los detalles de una empresa específica.
- **Respuesta:** `200 OK` o `404 Not Found`

### Crear Empresa
- **Método:** `POST`
- **Ruta:** `/api/Company`
- **Descripción:** Registra una nueva empresa.
- **Body:**
  ```json
  {
    "name": "Empresa Nueva",
    "nit": "999888777-6",
    "email": "admin@empresa.com",
    "phone": "555-1234",
    "address": "Av. Principal 100",
    "logoUrl": "",
    "isActive": true
  }
  ```
- **Respuesta:** `200 OK` (con el modelo `Result`) o `400 BadRequest`.

### Actualizar Empresa
- **Método:** `PUT`
- **Ruta:** `/api/Company/{id}`
- **Descripción:** Modifica los datos de una empresa existente.
- **Body:** DDTO idéntico a la creación.
- **Respuesta:** `200 OK` o `400 BadRequest`.

### Eliminar Empresa
- **Método:** `DELETE`
- **Ruta:** `/api/Company/{id}`
- **Descripción:** Elimina una empresa por su identificador único.
- **Respuesta:** `200 OK` o `400 BadRequest`.

---

## 2. Módulo: Puntos de Venta (Store)

Controlador: `StoreController`
Ruta base: `/api/Store`

### Obtener Todos los Puntos de Venta
- **Método:** `GET`
- **Ruta:** `/api/Store`
- **Descripción:** Obtiene todos los puntos de venta registrados.
- **Respuesta:** `200 OK`
  ```json
  [
    {
      "id": 1,
      "companyId": 1,
      "code": "PV-001",
      "name": "Sucursal Norte",
      "city": "Bogotá",
      "address": "Calle 100 #15-20",
      "managerName": "Juan Pérez",
      "managerPhone": "310-9876543",
      "isActive": true,
      "createdAt": "2026-07-30T10:00:00Z"
    }
  ]
  ```

### Obtener Punto de Venta por ID
- **Método:** `GET`
- **Ruta:** `/api/Store/{id}`
- **Respuesta:** `200 OK` o `404 Not Found`

### Crear Punto de Venta
- **Método:** `POST`
- **Ruta:** `/api/Store`
- **Body:**
  ```json
  {
    "companyId": 1,
    "code": "PV-002",
    "name": "Sucursal Sur",
    "city": "Cali",
    "address": "Carrera 5 #10-10",
    "managerName": "Ana Gómez",
    "managerPhone": "315-1112233",
    "isActive": true
  }
  ```
- **Respuesta:** `200 OK` o `400 BadRequest`.

### Actualizar Punto de Venta
- **Método:** `PUT`
- **Ruta:** `/api/Store/{id}`
- **Body:** DTO de Punto de Venta.
- **Respuesta:** `200 OK` o `400 BadRequest`.

### Eliminar Punto de Venta
- **Método:** `DELETE`
- **Ruta:** `/api/Store/{id}`
- **Respuesta:** `200 OK` o `400 BadRequest`.

---

## 3. Módulo: Bancos (Bank)

Controlador: `BankController`
Ruta base: `/api/Bank`

### Obtener Todos los Bancos
- **Método:** `GET`
- **Ruta:** `/api/Bank`
- **Descripción:** Obtiene los bancos registrados (Bancolombia, Davivienda, etc.).
- **Respuesta:** `200 OK`
  ```json
  [
    {
      "id": 1,
      "name": "Bancolombia",
      "code": "007"
    }
  ]
  ```

### Obtener Banco por ID
- **Método:** `GET`
- **Ruta:** `/api/Bank/{id}`
- **Respuesta:** `200 OK` o `404 Not Found`

### Crear Banco
- **Método:** `POST`
- **Ruta:** `/api/Bank`
- **Body:**
  ```json
  {
    "name": "Banco Nuevo",
    "code": "123"
  }
  ```
- **Respuesta:** `200 OK` o `400 BadRequest`.

### Actualizar Banco
- **Método:** `PUT`
- **Ruta:** `/api/Bank/{id}`
- **Body:** DTO de Banco.
- **Respuesta:** `200 OK` o `400 BadRequest`.

### Eliminar Banco
- **Método:** `DELETE`
- **Ruta:** `/api/Bank/{id}`
- **Respuesta:** `200 OK` o `400 BadRequest`.

---

## 4. Módulo: Usuarios (User)

Controlador: `UserController`
Ruta base: `/api/User`

### Obtener Todos los Usuarios
- **Método:** `GET`
- **Ruta:** `/api/User`
- **Descripción:** Obtiene la lista detallada de usuarios del sistema con resolución de nombres de empresa, sucursal y rol.
- **Respuesta:** `200 OK`
  ```json
  [
    {
      "id": 1,
      "companyId": 1,
      "companyName": "Letra Viva S.A.S.",
      "storeId": 1,
      "storeName": "Sucursal Norte",
      "roleId": 3,
      "roleName": "Cajero",
      "fullName": "Juan Pérez",
      "email": "juan@letraviva.com",
      "isActive": true,
      "createdAt": "2026-07-30T10:00:00Z"
    }
  ]
  ```

### Obtener Usuario por ID
- **Método:** `GET`
- **Ruta:** `/api/User/{id}`
- **Respuesta:** `200 OK` o `404 Not Found`

### Crear Usuario
- **Método:** `POST`
- **Ruta:** `/api/User`
- **Descripción:** Registra un nuevo usuario cifrando su contraseña.
- **Body:**
  ```json
  {
    "companyId": 1,
    "storeId": 1,
    "roleId": 3,
    "fullName": "Juan Pérez",
    "email": "juan@letraviva.com",
    "password": "PasswordSeguro123",
    "isActive": true
  }
  ```
- **Respuesta:** `200 OK` o `400 BadRequest`.

### Actualizar Usuario
- **Método:** `PUT`
- **Ruta:** `/api/User/{id}`
- **Descripción:** Modifica los datos del usuario. Si la contraseña se envía vacía, se conserva la existente.
- **Body:** Identico al DTO de creación (password es opcional).
- **Respuesta:** `200 OK` o `400 BadRequest`.

### Eliminar Usuario
- **Método:** `DELETE`
- **Ruta:** `/api/User/{id}`
- **Respuesta:** `200 OK` o `400 BadRequest`.

---

## 5. Módulo: Roles (Role)

Controlador: `RoleController`
Ruta base: `/api/Role`

### Obtener Todos los Roles
- **Método:** `GET`
- **Ruta:** `/api/Role`
- **Descripción:** Lista los roles cargados en el sistema (Administrador, Gerente Sucursal, Cajero).
- **Respuesta:** `200 OK`
  ```json
  [
    {
      "id": 1,
      "name": "Administrador"
    }
  ]
  ```

---

## 6. Módulo: Consignaciones (Consignation)

Controlador: `ConsignationController`
Ruta base: `/api/Consignation`

### Obtener Consignaciones Pendientes
- **Método:** `GET`
- **Ruta:** `/api/Consignation/pending`
- **Descripción:** Obtiene la lista de consignaciones pendientes de auditoría (Estado 1).
- **Respuesta:** `200 OK`
  ```json
  [
    {
      "id": 1,
      "companyId": 1,
      "companyName": "Letra Viva S.A.S.",
      "storeId": 1,
      "storeName": "Sucursal Norte",
      "bankId": 1,
      "bankName": "Bancolombia",
      "statusId": 1,
      "statusName": "Pendiente",
      "referenceNumber": "TX-12345",
      "declaredAmount": 150000,
      "detectedAmount": 150000,
      "createdAt": "2026-07-30T10:00:00Z",
      "fileUrl": "/uploads/uuid.png",
      "ocr": {
        "confidence": 98.2,
        "rawText": "..."
      }
    }
  ]
  ```

### Obtener Consignación por ID
- **Método:** `GET`
- **Ruta:** `/api/Consignation/{id}`
- **Respuesta:** `200 OK` o `404 Not Found`

### Crear Consignación (Subida de Recibo)
- **Método:** `POST`
- **Ruta:** `/api/Consignation`
- **Descripción:** Sube una imagen de recibo junto con los datos declarados. Ejecuta simulación OCR asíncrona.
- **Tipo de Contenido:** `multipart/form-data`
- **Campos (Form):**
  - `storeId` (long)
  - `bankId` (long)
  - `referenceNumber` (string)
  - `declaredAmount` (decimal)
  - `consignationDate` (string ISO)
  - `consignationTime` (string)
  - `notes` (string)
  - `file` (File - Blob)
- **Respuesta:** `200 OK`
  ```json
  {
    "id": 1
  }
  ```

### Auditar Consignación
- **Método:** `POST`
- **Ruta:** `/api/Consignation/audit/{id}`
- **Descripción:** Permite al auditor aprobar (2) o rechazar (3) una consignación con comentarios.
- **Body:**
  ```json
  {
    "statusId": 2,
    "comments": "Revisado y todo en orden"
  }
  ```
- **Respuesta:** `200 OK` o `404 Not Found`.

---

*Nota: Este documento debe actualizarse cada vez que se cree un nuevo controlador o endpoint.*
