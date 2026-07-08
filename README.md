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

