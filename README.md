# Visiotech Pokémon Assessment

A Web API built with .NET 8 and Entity Framework Core using SQLite. This solution implements a Pokedex catalog and an automated turn-by-turn battle simulation engine.

## Architectural Decisions

### Separation of Concerns (Composition vs Inheritance)
To maintain structural independence, `UserPokemon` (captured team assets) does not inherit from the base `Pokemon` catalog entity. Instead, it utilizes composition through a `PokemonBaseId` foreign key. This ensures the global Pokedex definitions remain immutable while isolating player-specific customizations, levels, and learned moves inside distinct storage layers.

### Storage Persistence
Data persistence is handled by an SQLite relational engine (`pokedex.db`). This guarantees constraints safety across intermediate join tables linking movements to specific entities. The database initialization and structural seed data are fully managed automatically during application startup via `context.Database.EnsureCreated()`.

### Simulation Framework
The core damage metrics calculation inside `BattleEngine` is implemented as a stateless static utility class. It evaluates type interactions and randomized parameters defensively without allocations on the heap, ensuring thread-safe processing for concurrent requests.

---

## Endpoint Layout

### Pokedex Management (CRUD)
- `GET /api/pokedex/pokemon` - List all base species.
- `GET /api/pokedex/pokemon/{id}` - Fetch specific base stats.
- `POST /api/pokedex/pokemon` - Register a base template.
- `PUT /api/pokedex/pokemon/{id}` - Modify base attributes.
- `DELETE /api/pokedex/pokemon/{id}` - Remove a base template.

### Dictionary & Teams
- `GET /api/pokedex/movements` - View the moves dictionary.
- `POST /api/pokedex/movements` - Add a new combat move.
- `GET /api/pokedex/my-pokemon` - View customized user team assets.
- `POST /api/pokedex/my-pokemon` - Instantiate a captured pokemon.
- `PUT /api/pokedex/my-pokemon/{id}/teach-move/{moveId}` - Learn a movement (Validates max 4 slots).
- `GET /api/pokedex/pokemon/{id}/possible-moves` - List compatible movements based on element alignment.
- `GET /api/pokedex/movements/{movementId}/shared-by` - Query which entities share a specific move.

### Battle Simulator
- `POST /api/battle/simulate?playerPokemonId=X&rivalPokemonId=Y` - Executes a turn-by-turn simulation until HP reaches zero. Turn initiative order is resolved evaluating `BaseSpeed` metrics.

---

## Build and Run Instructions

### Option 1: Local Execution via .NET CLI (Recommended / Tested)
Run the following command in the root folder to boot the application natively using the lightweight local SQLite engine:
```bash
dotnet restore
dotnet build
dotnet run --project src/Visiotech.Pokemon.Api
```
The API application will host locally on `http://localhost:5014`.

### Option 2: Running Test Automation Suite
To execute the suite of unit and integration tests (6 tests passed), run:
```bash
dotnet test
```

### Option 3: Deployment using Docker Compose
```bash
docker compose up --build -d
```
The application will map and expose the endpoint on `http://localhost:5014`.