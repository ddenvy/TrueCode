# 🏛️ TrueCode Микросервисная Архитектура

Полноценная микросервисная экосистема для управления пользователями и валютными операциями с поддержкой REST API, gRPC, JWT аутентификации и автоматическим обновлением курсов валют.


### 🚀 Сервисы:
- **👤 User Service** - управление пользователями, JWT аутентификация
- **💰 Finance Service** - курсы валют, избранные валюты пользователей
- **🌐 API Gateway** - единая точка входа, маршрутизация, JWT валидация
- **⏰ Currency Updater** - автоматическое обновление курсов с ЦБ РФ
- **🗄️ DB Migrator** - централизованное применение миграций БД

## 🚀 Быстрый старт

### Предварительные требования

- **.NET 8.0/9.0 SDK** - для компиляции и запуска приложений
- **PostgreSQL** - основная база данных для всех сервисов
- **Visual Studio 2022** или **VS Code** (опционально)
- **grpcurl** - для тестирования gRPC (опционально)

### Установка и запуск

1. **Клонирование репозитория**
   ```bash
   git clone <repository-url>
   cd TrueCode
   ```

2. **Восстановление зависимостей**
   ```bash
   dotnet restore
   ```

3. **Настройка PostgreSQL**
   
   Создайте базы данных для каждого сервиса:
   ```sql
   CREATE DATABASE truecode_user;
   CREATE DATABASE truecode_finance;
   ```

4. **Настройка строк подключения**
   
   Обновите `appsettings.json` в каждом сервисе:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=truecode_user;Username=postgres;Password=postgres"
     }
   }
   ```

5. **Применение миграций (автоматически или через DbMigrator)**
   
   **Вариант A: Автоматически при запуске каждого сервиса**
   ```bash
   # Миграции применяются автоматически при старте API
   ```
   
   **Вариант B: Через отдельный сервис миграций**
   ```bash
   dotnet run --project src/Services/DbMigrator/TrueCode.DbMigrator
   ```

6. **Запуск всех сервисов**

   **🌐 API Gateway (обязательно запустить первым):**
   ```bash
   dotnet run --project src/Gateway/TrueCode.Gateway
   ```
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5002`
   - Swagger UI: `http://localhost:5000/swagger`

   **👤 User Service:**
   ```bash
   dotnet run --project src/Services/User/TrueCode.User.API
   ```
   - HTTP: `http://localhost:5001`
   - gRPC: `http://localhost:5001`
   - Swagger UI: `http://localhost:5001/swagger`

   **💰 Finance Service:**
   ```bash
   dotnet run --project src/Services/Finance/TrueCode.Finance.API
   ```
   - HTTP: `http://localhost:5003`
   - Swagger UI: `http://localhost:5003/swagger`

   **⏰ Currency Updater (фоновый сервис):**
   ```bash
   dotnet run --project src/Services/CurrencyUpdater/TrueCode.CurrencyUpdater
   ```
   - Обновляет курсы валют каждые 4 часа
   - Первое обновление при запуске

## 🔍 Проверка работы системы

### 1. Проверка здоровья сервисов
```bash
# API Gateway
curl http://localhost:5000/health

# User Service  
curl http://localhost:5001/health

# Finance Service
curl http://localhost:5003/health
```

### 2. Проверка API Gateway маршрутизации
```bash
# Через Gateway -> User Service
curl http://localhost:5000/api/user/health

# Через Gateway -> Finance Service  
curl http://localhost:5000/api/finance/health
```

### 3. Полный workflow тестирования

**Шаг 1: Регистрация пользователя через Gateway**
```bash
curl -X POST "http://localhost:5000/api/user/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "testuser",
    "password": "password123"
  }'
```

**Шаг 2: Логин и получение JWT токена**
```bash
curl -X POST "http://localhost:5000/api/user/login" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "testuser", 
    "password": "password123"
  }'
```

**Шаг 3: Обновление избранных валют**
```bash
curl -X PUT "http://localhost:5000/api/user/favorite-currencies" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "favoriteCurrencies": ["USD", "EUR", "GBP"]
  }'
```

**Шаг 4: Получение курсов валют пользователя через Finance Service**
```bash
curl -X GET "http://localhost:5000/api/finance/user-rates" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## 🧪 Тестирование

### 🏃‍♂️ Запуск Unit-тестов

**Запуск всех тестов:**
```bash
dotnet test --verbosity minimal
```

**Запуск тестов User Service:**
```bash
dotnet test tests/TrueCode.User.Tests/TrueCode.User.Tests.csproj --verbosity minimal
```

**Запуск тестов Finance Service:**
```bash
dotnet test tests/TrueCode.Finance.Tests/TrueCode.Finance.Tests.csproj --verbosity minimal
```

**Подробный вывод с покрытием:**
```bash
dotnet test --verbosity detailed --collect:"XPlat Code Coverage"
```

**Ожидаемый результат:**
```
✅ TrueCode.User.Tests: 11 тестов прошли успешно
✅ TrueCode.Finance.Tests: 17 тестов прошли успешно
🎉 Всего: 28 тестов успешно
```

### 🌐 REST API тестирование через API Gateway

#### 1. Регистрация пользователя
```bash
curl -X POST "http://localhost:5000/api/user/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "testuser",
    "password": "password123"
  }'
```

#### 2. Вход в систему
```bash
curl -X POST "http://localhost:5000/api/user/login" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "testuser",
    "password": "password123"
  }'
```

#### 3. Получение профиля пользователя
```bash
curl -X GET "http://localhost:5000/api/user/profile" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### 4. Обновление избранных валют
```bash
curl -X PUT "http://localhost:5000/api/user/favorite-currencies" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "favoriteCurrencies": ["USD", "EUR", "GBP"]
  }'
```

#### 5. Получение курсов валют пользователя
```bash
curl -X GET "http://localhost:5000/api/finance/user-rates" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

#### 6. Получение курсов валют другого пользователя (если есть права)
```bash
curl -X GET "http://localhost:5000/api/finance/user-rates/USER_ID" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 🚀 gRPC тестирование (межсервисное взаимодействие)

#### Установка grpcurl (если не установлен)
```bash
# Windows (через Chocolatey)
choco install grpcurl

# macOS (через Homebrew)  
brew install grpcurl

# Linux
go install github.com/fullstorydev/grpcurl/cmd/grpcurl@latest
```

#### 1. Получение пользователя через gRPC (User Service)
```bash
grpcurl -plaintext -d '{"userId": "USER_GUID"}' localhost:5001 TrueCode.User.API.Grpc.UserService/GetUser
```

#### 2. Валидация токена через gRPC (User Service)
```bash
grpcurl -plaintext -d '{"token": "YOUR_JWT_TOKEN"}' localhost:5001 TrueCode.User.API.Grpc.UserService/ValidateToken
```

#### 3. Проверка доступных gRPC методов
```bash
# Список сервисов User Service
grpcurl -plaintext localhost:5001 list

# Описание методов UserService
grpcurl -plaintext localhost:5001 describe TrueCode.User.API.Grpc.UserService
```

### 🎨 Тестирование через Swagger UI

#### API Gateway Swagger (рекомендуется)
1. Откройте: `http://localhost:5000/swagger`
2. Все сервисы доступны через единую точку входа
3. JWT токены автоматически проксируются в микросервисы

#### User Service Swagger
1. Откройте: `http://localhost:5001/swagger` 
2. Прямое тестирование User API

#### Finance Service Swagger  
1. Откройте: `http://localhost:5003/swagger`
2. Прямое тестирование Finance API

**Процедура авторизации в Swagger:**
1. Выполните `/api/user/login` для получения JWT токена
2. Нажмите кнопку "Authorize" в Swagger UI
3. Введите токен в формате: `Bearer YOUR_JWT_TOKEN`
4. Все последующие запросы будут авторизованы

## 🏗️ Архитектура

### Микросервисная архитектура с Clean Architecture в каждом сервисе

**🏛️ Общие принципы:**
- **Database per Service** - каждый сервис имеет свою БД
- **API Gateway** - единая точка входа для всех клиентов
- **Service-to-Service Communication** - gRPC для внутренней связи
- **JWT Authentication** - сквозная аутентификация через все сервисы
- **Background Services** - для периодических задач

**📦 Структура каждого микросервиса (Clean Architecture):**
- **API Layer**: REST контроллеры, gRPC сервисы, middleware
- **Application Layer**: CQRS команды/запросы, handlers, DTOs, валидаторы
- **Domain Layer**: Доменные сущности, интерфейсы, бизнес-логика
- **Infrastructure Layer**: Репозитории, DbContext, миграции, внешние API
- **Shared Layer**: Общие сервисы (JWT, хеширование паролей, базовые сущности)

**🔄 Межсервисное взаимодействие:**
```
Client Request → API Gateway → User/Finance Service
Finance Service → gRPC → User Service (получение данных пользователя)
Currency Updater → HTTP → CBR.ru API → Finance Database
```

**📊 Технологический стек:**
- **.NET 8.0/9.0** - основная платформа
- **PostgreSQL** - база данных с JSONB поддержкой  
- **Entity Framework Core** - ORM с Code-First миграциями
- **YARP** - современный reverse proxy для API Gateway
- **gRPC** - высокопроизводительная межсервисная связь
- **MediatR** - CQRS implementation
- **FluentValidation** - валидация входных данных
- **BCrypt** - хеширование паролей
- **xUnit + Moq + FluentAssertions** - unit-тестирование

## 🔧 Конфигурация

### JWT настройки (одинаковые для всех сервисов)
```json
{
  "JwtSettings": {
    "SecretKey": "TrueCodeSuperSecretKeyForJWTTokenGeneration2024!",
    "Issuer": "TrueCode.User.API",
    "Audience": "TrueCode.Client",
    "ExpirationInMinutes": 60
  }
}
```

### Строки подключения к PostgreSQL

**User Service (`src/Services/User/TrueCode.User.API/appsettings.json`):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=truecode_user;Username=postgres;Password=postgres"
  }
}
```

**Finance Service (`src/Services/Finance/TrueCode.Finance.API/appsettings.json`):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=truecode_finance;Username=postgres;Password=postgres"
  },
  "Services": {
    "UserService": {
      "GrpcUrl": "http://localhost:5001"
    }
  }
}
```

**API Gateway (`src/Gateway/TrueCode.Gateway/appsettings.json`):**
```json
{
  "ReverseProxy": {
    "Routes": {
      "user-route": {
        "ClusterId": "user-cluster",
        "Match": { "Path": "/api/user/{**catch-all}" }
      },
      "finance-route": {
        "ClusterId": "finance-cluster",
        "Match": { "Path": "/api/finance/{**catch-all}" }
      }
    },
    "Clusters": {
      "user-cluster": {
        "Destinations": {
          "user-service": { "Address": "http://localhost:5001/" }
        }
      },
      "finance-cluster": {
        "Destinations": {
          "finance-service": { "Address": "http://localhost:5003/" }
        }
      }
    }
  }
}
```

**Currency Updater (`src/Services/CurrencyUpdater/TrueCode.CurrencyUpdater/appsettings.json`):**
```json
{
  "CurrencyUpdate": {
    "IntervalMinutes": 240,
    "EnableOnStartup": true
  },
  "Services": {
    "CbrUrl": "http://www.cbr.ru/scripts/XML_daily.asp"
  }
}
```

## 📝 Логирование

Система использует структурированное логирование .NET во всех сервисах:

**🌐 API Gateway:**
- Маршрутизация запросов
- Health check результаты
- Ошибки проксирования

**👤 User Service:**
- Регистрация и аутентификация пользователей
- gRPC вызовы
- Операции с БД

**💰 Finance Service:**
- Межсервисные gRPC вызовы к User Service
- Операции с валютами
- Ошибки получения курсов

**⏰ Currency Updater:**
- Периодические обновления курсов
- Ошибки парсинга XML от ЦБ РФ
- Статистика обновлений

**Уровни логирования:**
- `Information` - обычные операции
- `Warning` - потенциальные проблемы
- `Error` - ошибки выполнения
- `Critical` - критические сбои системы

## 🔒 Безопасность

**🔐 Аутентификация и авторизация:**
- **JWT токены** - stateless аутентификация во всех сервисах
- **BCrypt** - хеширование паролей с солью
- **API Gateway** - централизованная валидация токенов
- **HTTPS** - шифрование трафика (production ready)

**🛡️ Валидация данных:**
- **FluentValidation** - server-side валидация
- **Model binding** - автоматическая валидация DTOs
- **SQL Injection protection** - через Entity Framework параметризованные запросы
- **XSS protection** - автоматическое экранирование в ASP.NET Core

**🌐 Сетевая безопасность:**
- **CORS** - настроенная политика cross-origin запросов
- **Health checks** - мониторинг состояния сервисов
- **gRPC TLS** - шифрованная межсервисная связь (в production)

