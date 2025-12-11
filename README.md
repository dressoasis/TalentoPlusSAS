# 🚀 TalentoPlus S.A.S - Sistema de Gestión de Recursos Humanos

Sistema completo de gestión de recursos humanos desarrollado con .NET 8 y PostgreSQL, que incluye gestión de empleados, importación de datos desde Excel, generación de reportes PDF, y consultas con IA.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Tecnologías](#-tecnologías)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Configuración](#️-configuración)
- [Uso con Docker](#-uso-con-docker)
- [Endpoints de la API](#-endpoints-de-la-api)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Contribuir](#-contribuir)

## ✨ Características

### Gestión de Empleados
- ✅ CRUD completo de empleados
- ✅ Importación masiva desde archivos Excel (.xlsx)
- ✅ Generación de hojas de vida en PDF
- ✅ Gestión de departamentos, posiciones y niveles educativos
- ✅ Control de estado de empleados (Activo, Vacaciones, Inactivo)

### Autenticación y Seguridad
- ✅ Autenticación JWT (JSON Web Tokens)
- ✅ Roles de usuario (Admin, User)
- ✅ Registro de usuarios vinculado a empleados
- ✅ Protección de endpoints por roles

### Dashboard y Reportes
- ✅ Estadísticas en tiempo real
- ✅ Distribución por departamento, posición y nivel educativo
- ✅ Análisis de salarios (promedio, máximo, mínimo)
- ✅ Chat con IA para consultas en lenguaje natural (Google Gemini)

### Notificaciones
- ✅ Envío de correos de bienvenida
- ✅ Configuración SMTP personalizable

## 🛠 Tecnologías

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Base de datos
- **Identity** - Autenticación y autorización
- **JWT Bearer** - Tokens de autenticación

### Librerías y Servicios
- **ClosedXML** - Procesamiento de archivos Excel
- **QuestPDF** - Generación de documentos PDF
- **Google Gemini AI** - Procesamiento de lenguaje natural
- **MailKit/SMTP** - Envío de correos electrónicos

### DevOps
- **Docker & Docker Compose** - Contenedorización
- **Swagger/OpenAPI** - Documentación de API

## 📦 Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (opcional, para despliegue)
- [PostgreSQL](https://www.postgresql.org/download/) (si no usas Docker)

## 🚀 Instalación

### Opción 1: Desarrollo Local

1. **Clonar el repositorio**
```bash
git clone https://github.com/tu-usuario/TalentoPlusS.A.S.git
cd TalentoPlusS.A.S
```

2. **Configurar variables de entorno**
```bash
cp .env.example .env
# Editar .env con tus configuraciones
```

3. **Restaurar dependencias**
```bash
dotnet restore
```

4. **Aplicar migraciones**
```bash
cd src/TalentoPlus.Api
dotnet ef database update
```

5. **Ejecutar la aplicación**
```bash
dotnet run
```

La API estará disponible en `http://localhost:5111`

### Opción 2: Docker (Recomendado)

1. **Clonar el repositorio**
```bash
git clone https://github.com/tu-usuario/TalentoPlusS.A.S.git
cd TalentoPlusS.A.S
```

2. **Configurar variables de entorno**
```bash
cp .env.example .env
# Editar .env con tus configuraciones
```

3. **Construir e iniciar contenedores**
```bash
docker compose build
docker compose up -d
```

4. **Verificar que los servicios estén corriendo**
```bash
docker compose ps
```

La API estará disponible en:
- **Swagger UI**: http://localhost:5111/swagger
- **API**: http://localhost:5111

## ⚙️ Configuración

### Variables de Entorno (.env)

```bash
# Base de Datos
CONNECTIONSTRINGS__DEFAULT=Host=localhost;Port=5432;Database=TalentoPlusDB;Username=postgres;Password=tu_password

# JWT
JWT__Key=tu_clave_secreta_muy_larga_y_segura
JWT__Issuer=TalentoPlus
JWT__Audience=TalentoPlusAPI

# SMTP (Correo)
SMTP__Host=smtp.gmail.com
SMTP__Port=587
SMTP__User=tu_email@gmail.com
SMTP__Password=tu_app_password
SMTP__From=tu_email@gmail.com

# Google Gemini AI
GEMINI__ApiKey=tu_api_key_de_gemini
```

### Usuario Administrador por Defecto

Al iniciar la aplicación por primera vez, se crea automáticamente un usuario administrador:

- **Email**: admin@talentoplus.com
- **Password**: Admin123*
- **Rol**: Admin

## 🐳 Uso con Docker

### Comandos Principales

```bash
# Iniciar servicios
docker compose up -d

# Ver logs
docker compose logs -f app

# Detener servicios
docker compose down

# Reconstruir imagen
docker compose build app
docker compose up -d

# Ver estado
docker compose ps
```

### Acceso Remoto

Para acceder desde otros dispositivos en la misma red:

1. Obtén tu IP local:
```bash
hostname -I | awk '{print $1}'
```

2. Accede desde otro dispositivo:
```
http://TU_IP:5111/swagger
```

### Servicios Docker

| Servicio | Puerto | Descripción |
|----------|--------|-------------|
| **API** | 5111 | API de TalentoPlus |
| **PostgreSQL** | 5433 | Base de datos |

## 📡 Endpoints de la API

### Autenticación

```http
POST /api/Auth/login
POST /api/Auth/register (requiere Admin)
```

### Empleado Autenticado

```http
GET  /api/Me              # Ver mi información
GET  /api/Me/cv           # Descargar mi CV en PDF
```

### Dashboard (Admin)

```http
GET  /api/Dashboard/stats      # Estadísticas generales
POST /api/Dashboard/ai-query   # Consulta con IA
```

### Importación (Admin)

```http
POST /api/Import/employees     # Importar desde Excel
```

### Documentación Completa

Accede a Swagger UI para ver todos los endpoints y probarlos:
```
http://localhost:5111/swagger
```

## 📁 Estructura del Proyecto

```
TalentoPlusS.A.S/
├── src/
│   ├── TalentoPlus.Api/              # API Web
│   │   ├── Controllers/              # Controladores
│   │   ├── Middlewares/              # Middleware personalizado
│   │   └── Program.cs                # Punto de entrada
│   │
│   ├── TalentoPlus.Application/      # Lógica de aplicación
│   │   ├── DTOs/                     # Data Transfer Objects
│   │   ├── Services/                 # Servicios de negocio
│   │   └── Interfaces/               # Contratos
│   │
│   ├── TalentoPlus.Domain/           # Entidades y lógica de dominio
│   │   ├── Entities/                 # Entidades del dominio
│   │   └── Enums/                    # Enumeraciones
│   │
│   ├── TalentoPlus.Infrastructure.Data/      # Acceso a datos
│   │   ├── Context/                  # DbContext
│   │   ├── Migrations/               # Migraciones EF
│   │   └── Repositories/             # Repositorios
│   │
│   ├── TalentoPlus.Infrastructure.Identity/  # Autenticación
│   │   ├── Identity/                 # Configuración Identity
│   │   ├── Services/                 # Servicios de auth
│   │   └── Seed/                     # Datos iniciales
│   │
│   └── TalentoPlus.Infrastructure.Integrations/  # Integraciones
│       ├── Excel/                    # Importación Excel
│       ├── Pdf/                      # Generación PDF
│       ├── Email/                    # Envío de correos
│       └── AI/                       # Integración con IA
│
├── docker-compose.yml                # Configuración Docker
├── Dockerfile                        # Imagen Docker
├── .env                              # Variables de entorno
└── README.md                         # Este archivo
```

## 🧪 Pruebas

### Probar con Swagger

1. Abre http://localhost:5111/swagger
2. Haz login con el usuario admin
3. Copia el token de la respuesta
4. Click en "Authorize" y pega el token
5. Prueba los endpoints protegidos

### Probar con cURL

```bash
# Login
curl -X POST http://localhost:5111/api/Auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@talentoplus.com","password":"Admin123*"}'

# Obtener estadísticas (reemplaza TOKEN)
curl http://localhost:5111/api/Dashboard/stats \
  -H "Authorization: Bearer TOKEN"
```

## 🔒 Seguridad

- ✅ Autenticación JWT con tokens de corta duración
- ✅ Contraseñas hasheadas con Identity
- ✅ Validación de datos con Data Annotations
- ✅ CORS configurado
- ✅ HTTPS redirection
- ✅ Roles y autorización basada en claims

## 📝 Licencia

Este proyecto es privado y pertenece a TalentoPlus S.A.S.

## 👥 Contribuir

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📞 Soporte

Para soporte y preguntas, contacta a: soporte@talentoplus.com

---

**Desarrollado con ❤️ por el equipo de TalentoPlus S.A.S**
