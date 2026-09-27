var builder = WebApplication.CreateBuilder(args);

// Schritt 1–5: Konfiguration lesen, Dienste registrieren
// (AddSingleton, AddHostedService, AddSignalR …)

var app = builder.Build();

// Schritt 5–6: Endpunkte und statische Dateien
// (UseDefaultFiles, UseStaticFiles, MapHub …)

app.Run();
