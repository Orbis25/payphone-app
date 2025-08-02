# Payphone API

Una API RESTful para gestión de billeteras digitales y transacciones financieras, construida con .NET 8 y siguiendo principios de Clean Architecture.

## 📋 Tabla de Contenidos

- [Funcionalidad de la API](#funcionalidad-de-la-api)
- [Arquitectura](#arquitectura)
- [Cómo Ejecutar la API](#cómo-ejecutar-la-api)
- [Documentación de la API](#documentación-de-la-api)
- [Endpoints Disponibles](#endpoints-disponibles)
- [Configuración](#configuración)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Preguntas](#preguntas)

## 🚀 Funcionalidad de la API

La API de Payphone proporciona las siguientes funcionalidades principales:

### Gestión de Usuarios
- **Autenticación**: Sistema de login con JWT tokens
- **Gestión de perfiles**: Información básica de usuarios (nombre completo, email)

### Gestión de Billeteras
- **Creación de billeteras**: Cada usuario puede crear billeteras digitales
- **Consulta de saldo**: Verificar el balance actual de las billeteras
- **Actualización de información**: Modificar datos de la billetera (propietario, documento)

### Transacciones Financieras
- **Depósitos**: Agregar fondos a la billetera (transacciones de crédito)
- **Retiros**: Extraer fondos de la billetera (transacciones de débito)
- **Historial de transacciones**: Consultar el historial completo de movimientos

### Características Técnicas
- **Autenticación JWT**: Seguridad basada en tokens
- **Paginación**: Consultas paginadas para grandes volúmenes de datos
- **Validación**: Validación robusta de datos de entrada
- **Soft Delete**: Eliminación lógica de registros
- **CORS**: Configuración para acceso desde diferentes orígenes

## 🏗️ Arquitectura

El proyecto sigue los principios de **Clean Architecture** con una estructura modular:

```
src/
├── Payphone.API/           # Capa de presentación (Controllers, Program.cs)
├── Payphone.Application/   # Capa de aplicación (Services, DTOs, Validators)
├── Payphone.Domain/        # Capa de dominio (Models, Enums, Core)
└── Payphone.Infrastructure/# Capa de infraestructura (EF Core, Repositories)
```

### Capas de la Arquitectura

#### 1. **Payphone.API** (Capa de Presentación)
- **Controllers**: Manejo de requests HTTP
- **Program.cs**: Configuración de la aplicación
- **appsettings.json**: Configuración de la aplicación
- **Swagger**: Documentación automática de la API

#### 2. **Payphone.Application** (Capa de Aplicación)
- **Services**: Lógica de negocio
- **DTOs**: Objetos de transferencia de datos
- **Validators**: Validación de datos de entrada
- **Mappers**: Mapeo entre entidades y DTOs

#### 3. **Payphone.Domain** (Capa de Dominio)
- **Models**: Entidades del dominio
  - `User`: Usuario del sistema
  - `Wallet`: Billetera digital
  - `WalletTransaction`: Transacciones financieras
- **Enums**: Enumeraciones del dominio
  - `TransactionType`: Tipos de transacción (Credit/Debit)
- **Core**: Modelos base y interfaces

#### 4. **Payphone.Infrastructure** (Capa de Infraestructura)
- **EF**: Entity Framework Core
  - **Persistence**: Contexto de base de datos
  - **Repositories**: Implementación de repositorios
  - **Migrations**: Migraciones de base de datos
  - **Configurations**: Configuración de entidades
- **Extensions**: Extensiones para configuración de servicios

### Patrones Utilizados

- **Repository Pattern**: Abstracción del acceso a datos
- **Service Pattern**: Lógica de negocio encapsulada
- **DTO Pattern**: Separación entre modelos de dominio y transferencia
- **Dependency Injection**: Inyección de dependencias

## 🚀 Cómo Ejecutar la API

### Prerrequisitos

- **.NET 8.0 SDK** o superior
- **Docker** y **Docker Compose** (opcional)
- **PostgreSQL** 15 o superior

### Opción 1: Ejecución Local

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd payphone-app
   ```

2. **Configurar la base de datos**
   - Instalar PostgreSQL
   - Crear una base de datos llamada `payphone`
   - Actualizar la cadena de conexión en `src/Payphone.API/appsettings.json`

3. **Restaurar dependencias**
   ```bash
   cd src
   dotnet restore
   ```

4. Las migraciones corren solas
  

5. **Ejecutar la aplicación**
   ```bash
   dotnet run
   ```

La API estará disponible en: `https://localhost:5001`

### Opción 2: Ejecución con Docker

1. **Clonar el repositorio**
   ```bash
   git clone <repository-url>
   cd payphone-app
   ```

2. **Ejecutar con Docker Compose**
   ```bash
   docker-compose up -d
   ```

La API estará disponible en: `http://localhost:5001`

### Verificar la Instalación

- **Health Check**: `GET http://localhost:5001/health`
- **Swagger UI**: `http://localhost:5001/swagger`

## 📚 Documentación de la API

### Swagger UI

La documentación interactiva está disponible en:
```
http://localhost:5001/swagger
```

### Autenticación

La API utiliza **JWT Bearer Token** para autenticación:

1. **Obtener token**: `POST /api/users/auth/login`
2. **Usar token**: Incluir en header: `Authorization: Bearer <token>`

### Códigos de Respuesta

- `200 OK`: Operación exitosa
- `201 Created`: Recurso creado exitosamente
- `204 No Content`: Operación exitosa sin contenido
- `400 Bad Request`: Error en datos de entrada
- `401 Unauthorized`: No autenticado
- `404 Not Found`: Recurso no encontrado
- `500 Internal Server Error`: Error interno del servidor

## 🔗 Endpoints Disponibles

### Autenticación

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| POST | `/api/users/auth/login` | Iniciar sesión | No requerida |

### Billeteras

| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| GET | `/api/wallets` | Obtener billeteras (paginated) | JWT Bearer |
| GET | `/api/wallets/{id}` | Obtener billetera por ID | JWT Bearer |
| POST | `/api/wallets` | Crear nueva billetera | JWT Bearer |
| PUT | `/api/wallets/{id}` | Actualizar billetera | JWT Bearer |
| DELETE | `/api/wallets/{id}` | Eliminar billetera (soft delete) | JWT Bearer |

### Ejemplos de Uso

#### 1. Login de Usuario
```bash
curl -X POST "http://localhost:5001/api/users/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "payphone@payphone.com",
    "password": "123@payphone"
  }'
```

#### 2. Crear Billetera
```bash
curl -X POST "http://localhost:5001/api/wallets" \
  -H "Authorization: Bearer <your-jwt-token>" \
  -H "Content-Type: application/json" \
  -d '{
    "ownerDocumentId": "12345678",
    "ownerName": "Juan Pérez"
  }'
```

#### 3. Consultar Billeteras
```bash
curl -X GET "http://localhost:5001/api/wallets?page=1&pageSize=10" \
  -H "Authorization: Bearer <your-jwt-token>"
```

## ⚙️ Configuración

### Variables de Entorno

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;Database=payphone;Username=postgres;Password=postgres"
  },
  "JwtOption": {
    "DefaultUser": "payphone@payphone.com",
    "DefaultPassword": "123@payphone",
    "Audience": "localhost.com",
    "Issuer": "localhost.com",
    "Key": "your-jwt-secret-key"
  },
  "CorsConfigOption": {
    "OriginsAllowed": "*",
    "MethodsAllowed": "*"
  }
}
```

### Configuración de Base de Datos

- **Motor**: PostgreSQL 15
- **Migraciones**: Entity Framework Core
- **ORM**: Entity Framework Core 8.0

### Configuración de Seguridad

- **JWT**: Autenticación basada en tokens
- **CORS**: Configurado para desarrollo
- **HTTPS**: Redirección automática en producción

## 🛠️ Tecnologías Utilizadas

### Backend
- **.NET 8**: Framework principal
- **ASP.NET Core**: Framework web
- **Entity Framework Core**: ORM para base de datos
- **PostgreSQL**: Base de datos relacional
- **JWT**: Autenticación y autorización
- **Swagger**: Documentación de API

### Arquitectura y Patrones
- **Clean Architecture**: Separación de responsabilidades
- **Repository Pattern**: Abstracción de acceso a datos
- **Dependency Injection**: Inyección de dependencias
- **DTO Pattern**: Transferencia de datos

### Herramientas de Desarrollo
- **Docker**: Containerización
- **Docker Compose**: Orquestación de servicios
- **Git**: Control de versiones

## 🔧 Desarrollo

### Estructura de Proyectos

```
Payphone.sln
├── Payphone.API/           # API Web
├── Payphone.Application/   # Lógica de aplicación
├── Payphone.Domain/        # Modelos de dominio
└── Payphone.Infrastructure/# Infraestructura
```



##  Notas Adicionales

- La API incluye un usuario por defecto para pruebas
- Las transacciones se registran con timestamps automáticos
- Se implementa soft delete para preservar integridad de datos
- La documentación se genera automáticamente con Swagger
- El proyecto está configurado para desarrollo y producción


## Preguntas