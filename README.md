# 🛒 HybridShop – Distributed Microservices Platform (.NET 10)

**HybridShop** to zaawansowana, wysoce skalowalna platforma e-commerce zbudowana w architekturze mikroserwisowej z wykorzystaniem **.NET 10**. System wykorzystuje komunikację asynchroniczną (Event-Driven Architecture), szybką synchronizację wewnętrzną (gRPC z Polly Resilience), API Gateway (YARP) oraz elastyczne interfejsy danych (REST API, GraphQL, SignalR WebSockets).

---

## 🏗️ Architektura Systemu

### 🧩 Mikroserwisy
* **API Gateway (`YARP`)** – Centralny punkt wejścia (`http://localhost:5000`). Obsługuje routing, CORS oraz dystrybucję ruchu do mikroserwisów.
* **Auth Service** – Rejestracja, logowanie (JWT) i autoryzacja. Udostępnia gRPC Server dla innych serwisów oraz publikuje zdarzenia rejestracji.
* **Product Service** – Katalog produktów, kategorie i stany magazynowe w MongoDB. Udostępnia GraphQL (HotChocolate) oraz gRPC Server do pobierania stanów magazynowych.
* **Order Service** – Obsługa koszyka (Redis) i składania zamówień (PostgreSQL). Wykorzystuje wzorzec Outbox Pattern do gwarantowanego dostarczania zdarzeń.
* **Notification Service** – Asynchroniczna wysyłka maili (Mailpit/SMTP) oraz powiadomień w czasie rzeczywistym przez SignalR Hub.
* **Chat Service** – System wiadomości na żywo w czasie rzeczywistym oparty na SignalR Hub i MongoDB.

### 🌐 Infrastruktura i Bazy Danych
* **PostgreSQL** – Relacyjna baza danych (`Auth`, `Order`).
* **MongoDB** – Dokumentowa baza danych (`Product`, `Chat`).
* **Redis** – Distributed Cache dla koszyków zakupowych oraz optymalizacji odczytów.
* **RabbitMQ (MassTransit)** – Szyna zdarzeń (Event Bus) dla architektury Pub/Sub.
* **MinIO** – S3-compliant Object Storage dla zdjęć produktów.
* **Mailpit** – Lokalny serwer SMTP do testowania powiadomień e-mail.

---

## 🛠️ Stos Technologiczny i Integracje

| Serwis | REST API | GraphQL | SignalR | gRPC Server | gRPC Client | Event Bus (RabbitMQ) | Baza Danych / Cache |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: | :--- |
| **Auth** | Wykorzystuje | — | — | Wykorzystuje | — | Publisher (`UserRegisteredEvent`) | PostgreSQL |
| **Product** | Wykorzystuje | Wykorzystuje | — | Wykorzystuje | Wykorzystuje (`Auth`) | — | MongoDB, Redis, MinIO |
| **Order** | Wykorzystuje | — | — | — | Wykorzystuje (`Auth`, `Product`) | Publisher (`OrderCreatedEvent`) | PostgreSQL, Redis |
| **Notification** | — | — | Wykorzystuje | — | Wykorzystuje (`Auth`) | Consumer (`UserRegistered`, `OrderCreated`) | Mailpit (SMTP) |
| **Chat** | Wykorzystuje | — | Wykorzystuje | — | Wykorzystuje (`Auth`) | — | MongoDB, Redis |

---

## Architektura Zdarzeniowa (Event-Driven) & Resilience

* **MassTransit & RabbitMQ:** Serwisy komunikują się asynchronicznie za pomocą zdarzeń:
  * `UserRegisteredEvent` → wyzwalany przez **Auth**, obsługiwany przez **Notification** (wysyłka powitania).
  * `OrderCreatedEvent` → wyzwalany przez **Order**, obsługiwany przez **Notification** (mail z potwierdzeniem).
* **Polly Resilience Handlers:** Wywołania gRPC pomiędzy serwisami są zabezpieczone automatycznymi ponowieniami (*Retry*), wykładniczym czasem oczekiwania (*Exponential Backoff*) oraz bezpiecznikami (*Circuit Breaker*).
* **Outbox Pattern:** Serwis zamówień zapisuje zdarzenia w bazy danych i przesyła je w tle (`OutboxProcessor`), zapobiegając utracie wiadomości przy awarii sieci.

---

## 🗺️ Routing i Porty (API Gateway)

Portal Gateway przyjmuje ruch z zewnątrz na porcie `5000` i przekierowuje go wewnętrznie (port `8080` / `8081`):

| Ścieżka zewnętrzna | Usługa docelowa | Opis / Protokół |
| :--- | :--- | :--- |
| `http://localhost:5000/swagger` | `api-gateway` | Dokumentacja OpenAPI / Swagger UI |
| `http://localhost:5000/api/auth/{..}` | `auth-service:8080` | REST API (Rejestracja, JWT) |
| `http://localhost:5000/api/product/{..}` | `product-service:8080` | REST API (Katalog produktów) |
| `http://localhost:5000/graphql/` | `product-service:8080` | GraphQL (HotChocolate IDE) |
| `http://localhost:5000/api/order/{..}` | `order-service:8080` | REST API (Koszyk i Zamówienia) |
| `http://localhost:5000/hubs/notifications` | `notification-service:8080` | SignalR Hub (Powiadomienia) |
| `http://localhost:5000/api/chat/{..}` | `chat-service:8080` | REST API (Historia wiadomości) |
| `http://localhost:5000/hubs/chat` | `chat-service:8080` | SignalR Hub (Czat na żywo) |

---

## Migracje EF Core (PostgreSQL)

Wykonaj migracje z poziomu katalogu konkretnego serwisu:

### Auth Service
> **Katalog:** `src/Services.Auth/`

```bash
# Tworzenie migracji
dotnet ef migrations add InitialPostgresAuthDb -s HybridShop.Services.Auth.Api/HybridShop.Services.Auth.Api.csproj -p HybridShop.Services.Auth.Infrastructure/HybridShop.Services.Auth.Infrastructure.csproj

# Aktualizacja bazy danych
cd HybridShop.Services.Auth.Infrastructure
dotnet ef database update --startup-project ../HybridShop.Services.Auth.Api/
```

### Order Service
> **Katalog:** `src/Services.Order/`

```bash
# Tworzenie migracji
dotnet ef migrations add InitialPostgresOrderDb -s HybridShop.Services.Order.Api/HybridShop.Services.Order.Api.csproj -p HybridShop.Services.Order.Infrastructure/HybridShop.Services.Order.Infrastructure.csproj

# Aktualizacja bazy danych
cd HybridShop.Services.Order.Infrastructure
dotnet ef database update --startup-project ../HybridShop.Services.Order.Api/
```


## Uruchomienie (Docker Compose)

1. Upewnij się, że posiadasz plik `.env` utworzony na podstawie `.env.template`.
2. Uruchom całe środowisko wraz z bazami danych i infrastrukturą:

```bash
docker compose up -d --build
```

## 🧪 Testowanie SignalR i WebSockets (Pliki HTML)

W katalogu głównym projektu znajdują się dwa dedykowane pliki HTML do szybkiego testowania komunikacji w czasie rzeczywistym z poziomu przeglądarki:

### 1. `testSignalR.html` – Powiadomienia na Żywo
Służy do weryfikacji połączenia z `Notification Service` poprzez SignalR Hub.
* **Adres połączenia:** `http://localhost:5000/hubs/notifications`
* **Jak użyć:** 
  1. Otwórz plik `testSignalR.html` w przeglądarce.
  2. Wklej poprawny token JWT (uzyskany po zalogowaniu w Auth Service).
  3. Połącz się z Hubem – po zarejestrowaniu użytkownika lub złożeniu zamówienia powiadomienia w czasie rzeczywistym pojawią się w oknie logów.

### 2. `testChat.html` – Czat i Komunikacja WebSocket
Służy do testowania wysyłania i odbierania wiadomości w czasie rzeczywistym w `Chat Service`.
* **Adres połączenia:** `http://localhost:5000/hubs/chat`
* **Jak użyć:**
  1. Otwórz plik `testChat.html` w dwóch osobnych kartach lub okienkach przeglądarki.
  2. Podaj tokeny JWT dwóch różnych użytkowników oraz wspólne `ConversationId`.
  3. Przeprowadź rozmowę na żywo – wiadomości będą przesyłane natychmiastowo przez WebSockets i zapisywane w bazie MongoDB.

## 🖥️ Dostępne Dashboardy i Interfejsy
Swagger UI (OpenAPI): http://localhost:5000/swagger

GraphQL (Banana Cake Pop): http://localhost:5000/graphql/

RabbitMQ Management: http://localhost:15672

MinIO Console: http://localhost:9001

Mailpit (Podgląd E-mail): http://localhost:8025
