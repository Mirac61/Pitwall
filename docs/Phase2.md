# PitWall – Phase 2: Live im Browser

Ziel: Monaco 2024 läuft als Replay, die Positionstabelle aktualisiert sich live im Browser.
Nur Hinweise, kein fertiger Code. Die Stichworte in `Code-Schrift` sind die Dinge, nach denen du suchen kannst.

---

## 0. Vorher
- [x] Aktuellen Stand committen (Phase 1 fertig)
- [x] Template-Reste aus `PitWall.Api` löschen (WeatherForecast, `/debug`-Endpunkt)

---

## 1. Konfiguration
**Ziel:** Datenordner, Session und Geschwindigkeit nicht hart im Code.

- [x] In `appsettings.json` einen Abschnitt `PitWall` mit `DataDir`, `SessionKey`, `Speed`
- [x] Werte in `Program.cs` lesen: `builder.Configuration["PitWall:SessionKey"]`
- [x] Optional sauberer: eigene Klasse `PitWallOptions` + `builder.Services.Configure<PitWallOptions>(...)`

**Stolperstein:** Relative Pfade beziehen sich darauf, von wo du startest. Pfad über
`builder.Environment.ContentRootPath` zusammenbauen, dann ist es egal.

**Fertig, wenn:** Du die Werte im Code ausgeben kannst.

---

## 2. Dienste registrieren (Dependency Injection)
**Ziel:** ASP.NET weiß, wie `ISessionData` und die Uhr erzeugt werden.

- [ ] `ISessionData` → `FileSessionData` als Singleton (`AddSingleton<ISessionData>(...)`)
- [ ] `TimeProvider.System` als Singleton registrieren

**Frage an dich:** Warum Singleton und nicht `AddScoped`? (Tipp: Wie oft braucht man das Objekt?)

---

## 3. Aktuellen Stand teilen
**Ziel:** Ein Ort, an dem der neueste Tabellenstand liegt, damit auch später verbundene Browser ihn bekommen.

- [ ] Klasse in `PitWall.Api`, z. B. `LiveRace`, als Singleton
- [ ] Hält die letzte `List<Standing>`, eine Methode zum Setzen, eine zum Lesen
- [ ] Beim Setzen die ganze Liste austauschen, nicht einzelne Einträge ändern

**Warum austauschen:** Worker schreibt, Hub liest, gleichzeitig. Eine fertige Liste
auszutauschen ist sicher, eine Liste zu verändern, während jemand liest, nicht.

---

## 4. ReplayWorker
**Ziel:** Läuft im Hintergrund, spielt das Rennen ab, schickt jede Änderung raus.

- [ ] `Workers/ReplayWorker.cs`, erbt von `BackgroundService`
- [ ] Im Konstruktor anfordern: `ISessionData`, `TimeProvider`, `LiveRace`, `IHubContext<RaceHub>`, Optionen
- [ ] In `ExecuteAsync(CancellationToken ct)`:
  - [ ] Fahrer + Positionen laden
  - [ ] `RaceState` und `ReplaySource` erzeugen
  - [ ] `await foreach` über die Quelle → `Apply` → `GetStandings()` → in `LiveRace` speichern → per Hub senden
- [ ] Senden: `hubContext.Clients.All.SendAsync("standings", standings, ct)`
- [ ] Registrieren: `builder.Services.AddHostedService<ReplayWorker>()`

**Stolperstein:** Den `ct` aus `ExecuteAsync` überall durchreichen, sonst hängt `Ctrl+C`.

---

## 5. SignalR-Hub
**Ziel:** Endpunkt, mit dem sich Browser verbinden.

- [ ] `Hubs/RaceHub.cs`, erbt von `Hub`
- [ ] `OnConnectedAsync` überschreiben: aktuellen Stand aus `LiveRace` nur an den neuen Client schicken (`Clients.Caller`)
- [ ] `builder.Services.AddSignalR()`
- [ ] `app.MapHub<RaceHub>("/hub/race")`

**Warum `OnConnectedAsync`:** Wer mitten im Rennen die Seite öffnet, sieht sonst eine leere Tabelle bis zum nächsten Positionswechsel. In Monaco kann das lange dauern.

---

## 6. Frontend
**Ziel:** Eine HTML-Seite mit Tabelle, die sich aktualisiert.

- [ ] `app.UseDefaultFiles()` und `app.UseStaticFiles()` in `Program.cs`
- [ ] `wwwroot/index.html` anlegen
- [ ] SignalR-JS-Client per `<script>` von cdnjs einbinden (`@microsoft/signalr`)
- [ ] Verbindung: `HubConnectionBuilder` → `withUrl("/hub/race")` → `withAutomaticReconnect()` → `start()`
- [ ] `connection.on("standings", ...)` → Tabelle neu zeichnen
- [ ] Spalten: Position, Kürzel, Team, Farbstreifen mit `"#" + teamColour`

**Stolperstein:** SignalR sendet JSON in **camelCase**. In JS heißt es also
`standing.driver.nameAcronym`, nicht `NameAcronym`.

---

## 7. Ausprobieren
- [ ] `Speed` auf ca. 50–100 stellen
- [ ] `dotnet run --project PitWall.Api`, Browser öffnen
- [ ] Zweiten Tab öffnen: Hat er sofort den aktuellen Stand?
- [ ] Beobachten: Startaufstellung → Stillstand (52 min bis Start) → Rennen → rote Flagge

**Stolperstein:** Bei kleinem Faktor passiert am Anfang lange nichts. Das ist die Lücke
zwischen Startaufstellung und Rennstart, kein Bug.

---

## 8. Abschluss
- [ ] Tests laufen noch (`dotnet test`)
- [ ] Commit
- [ ] In `Plan.md` die Punkte von Phase 2 abhaken

**Phase 2 ist fertig, wenn:** Browser öffnen → Tabelle erscheint → Positionen ändern sich
von allein → ein zweiter Tab zeigt denselben Stand.

---

## Wenn du feststeckst
Frag konkret mit Fehlermeldung oder Screenshot. Dann gibt's einen Hinweis, keine Lösung,
außer du willst sie.
