# HybridShop - System Mikroserwisów (.NET 10)

HybridShop to nowoczesna, skalowalna aplikacja e-commerce zbudowana w architekturze mikroserwisów z wykorzystaniem platformy .NET 10. Całość systemu jest w pełni skonteneryzowana za pomocą Dockera, a komunikacja zewnętrzna i wewnętrzna jest zarządzana przez bramę API (API Gateway).

---

## Architektura Systemu

System składa się z następujących komponentów działających w izolowanej sieci Docker:

1. API Gateway (YARP) – Jedyny punkt wejścia do systemu (port 5000). Zarządza routingiem, wycina prefiksy ścieżek (PathRemovePrefix) i przekazuje ruch do odpowiednich serwisów.
2. Auth Service – Serwis odpowiedzialny za uwierzytelnianie, rejestrację i autoryzację użytkowników.
3. Product Service – Zarządzanie katalogiem produktów, stanami magazynowymi i kategoriami.
4. Order Service – Obsługa procesu zamówień i koszyka zakupowego.
5. Notification Service – Serwis powiadomień działający asynchronicznie.
6. Infrastructure:
   * PostgreSQL – Centralna relacyjna baza danych.
   * RabbitMQ – Szyna danych (Event Bus) do asynchronicznej komunikacji międzyserwisowej (pub/sub).

---

## Mapowanie Portów i Routing (YARP)

Wszystkie mikroserwisy wewnątrz kontenerów uruchamiają się domyślnie na porcie 8080 (zgodnie ze standardem .NET 10). Brama API automatycznie tłumaczy i oczyszcza ścieżki:

| Ścieżka zewnętrzna (Twój komputer) | Adres docelowy w sieci Docker | Endpoint w kodzie mikroserwisu |
| :--- | :--- | :--- |
| http://localhost:5000/api/auth/{..} | http://auth-service:8080/{..} | app.MapGet("/{..}") |
| http://localhost:5000/api/products/{..} | http://product-service:8080/{..} | app.MapGet("/{..}") |
| http://localhost:5000/api/orders/{..} | http://order-service:8080/{..} | app.MapGet("/{..}") |
| http://localhost:5000/api/notifications/{..} | http://notification-service:8080/{..} | app.MapGet("/{..}") |



## Migracje dla Service.Auth
folder: HybridShop.Services.Auth
dotnet ef migrations add InitialPostgresAuthDb -s HybridShop.Services.Auth.Api/HybridShop.Services.Auth.Api.csproj -p HybridShop.Services.Auth.Infrastructure/HybridShop.Services.Auth.Infrastructure.csproj

Infrastructure
dotnet ef database update --startup-project ../HybridShop.Services.Auth.Api/

## Migracje dla Service.Order
folder: HybridShop.Services.Order
dotnet ef migrations add InitialPostgresOrderDb -s HybridShop.Services.Order.Api/HybridShop.Services.Order.Api.csproj -p HybridShop.Services.Order.Infrastructure/HybridShop.Services.Order.Infrastructure.csproj

Infrastructure
dotnet ef database update --startup-project ../HybridShop.Services.Order.Api/


## Testowanie SignalR i powiadomień dla zamówień
F12 -> console

const token = "<token>";

fetch('https://unpkg.com/@microsoft/signalr@8.0.0/dist/browser/signalr.min.js')
  .then(response => response.text())
  .then(code => {
      new Function(code)();

      const connection = new signalR.HubConnectionBuilder()
          .withUrl("http://localhost:5000/hubs/notifications", {
              accessTokenFactory: () => token
          })
          .build();

      connection.on("ReceiveOrderNotification", (data) => {
          console.log("🔥 [SIGNALR] NOWE ZAMÓWIENIE!", data);
          alert(`Ktoś kupił Twój przedmiot!\nKupujący: ${data.buyerEmail}\nKwota: ${data.total} PLN`);
      });

      return connection.start();
  })
  .then(() => console.log("🚀 POŁĄCZONO Z SIGNALR! Czekam na zamówienia..."))
  .catch(err => console.error("❌ Błąd:", err));