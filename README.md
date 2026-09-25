# NunyFood

Plateforme permettant à la diaspora togolaise de commander des packs alimentaires livrés à leurs proches au Togo.
La spécification fonctionnelle complète est dans [`Readme.cs`](Readme.cs).

| Dossier | Rôle |
|---|---|
| `NunyFoodWebApi` | API ASP.NET Core (.NET 10) : contrôleurs, authentification, limitation des tentatives |
| `NunyFoodWebApi.Application` | Cas d'usage en CQRS (`Features/<Module>/Commands\|Queries`), validation, règles d'accès |
| `NunyFoodWebApi.Domain` | Entités et règles métier (cycle de vie des commandes) |
| `NunyFoodWebApi.Infrastructure` | EF Core + PostgreSQL, email, stockage des fichiers, paiement simulé |
| `NunyFoodWebApi.Tests` | Tests xUnit de la couche Application |
| `NunyFood` | Frontend React 19 + Vite + TypeScript |

## Prérequis

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js 24](https://nodejs.org/) et npm
- PostgreSQL (base `NunyFoodDb`, créée automatiquement au premier démarrage)

## Premier démarrage

### 1. Secrets (jamais dans le dépôt)

Les secrets sont stockés dans les *user-secrets* .NET, hors du dossier du projet :

```bash
# Connexion PostgreSQL
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=NunyFoodDb;Username=postgres;Password=<mot de passe>" --project NunyFoodWebApi

# Clé de signature des tokens JWT : une valeur aléatoire d'au moins 32 caractères
dotnet user-secrets set "Jwt:Secret" "<valeur aléatoire>" --project NunyFoodWebApi
```

Pour générer une valeur aléatoire sûre : `openssl rand -base64 48` (Git Bash) ou, en PowerShell :
`$b = New-Object byte[] 48; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b); [Convert]::ToBase64String($b)`.

L'API refuse de démarrer si `Jwt:Secret` est absent ou trop court.

### 2. Configuration de développement

```bash
cp NunyFoodWebApi/appsettings.Development.example.json NunyFoodWebApi/appsettings.Development.json
```

Avec `"LogCodesInsteadOfSending": true`, les codes de connexion et de réinitialisation ne sont pas envoyés par email :
ils apparaissent dans la console de l'API (`[DEV] Code OTP pour … : 123456`).

Pour envoyer de vrais emails, créez un [mot de passe d'application Gmail](https://myaccount.google.com/apppasswords)
pour le compte `Gmail:FromEmail`, puis :

```bash
dotnet user-secrets set "Gmail:AppPassword" "<16 caractères>" --project NunyFoodWebApi
```

et passez `LogCodesInsteadOfSending` à `false`.

### 3. Lancer l'application

```bash
# API : https://localhost:7000 (Swagger : https://localhost:7000/swagger)
dotnet run --project NunyFoodWebApi --launch-profile https

# Frontend : http://localhost:5173 (les appels /api sont relayés vers l'API)
cd NunyFood
npm install
npm run dev
```

Au premier démarrage, les migrations sont appliquées et deux comptes sont créés
(`NunyFoodWebApi.Infrastructure/Persistence/DatabaseSeeder.cs`) : un administrateur et un livreur de démonstration.
Changez leurs mots de passe hors développement.

## Tests et qualité

```bash
dotnet test NunyFoodWebApi.Tests      # backend (xUnit)

cd NunyFood
npm run lint                          # ESLint
npm test                              # frontend (Vitest + Testing Library)
```

La CI GitHub Actions (`.github/workflows/ci.yml`) exécute ces vérifications à chaque push et pull request.

## Points d'architecture

- **CQRS avec un médiateur maison** (`Application/Common/Messaging`) : un fichier par cas d'usage contenant
  la commande ou la requête, son validateur et son handler. Les handlers sont enregistrés automatiquement.
  Nouveau cas d'usage : créer le fichier, puis `await sender.Send(new MaCommande(...), ct)` dans le contrôleur.
- **Contrôle d'accès** : les rôles sont vérifiés par `[Authorize(Roles = ...)]` ; l'appartenance des données
  (un client ne voit que ses commandes, un livreur que ses livraisons) est vérifiée dans les handlers via `ICurrentUser`.
- **Cycle de vie des commandes** : `Domain/Rules/OrderStatusTransitions.cs`. Tout changement de statut passe par
  `ChangeStatus`, qui refuse les transitions impossibles.
- **Preuves de livraison** : stockage privé (`IFileStorage`, disque local en V1), servies uniquement par
  `GET /api/deliveries/{id}/photo|signature` à l'admin, au livreur affecté et au client de la commande.
- **Limitation des tentatives** : section `RateLimiting` de `appsettings.json` (par adresse IP).

## Production

- Secrets en variables d'environnement : `ConnectionStrings__DefaultConnection`, `Jwt__Secret`, `Gmail__AppPassword`.
- Derrière un reverse proxy, activer `UseForwardedHeaders` pour que la limitation des tentatives voie l'IP réelle des clients.
- Remplacer `LocalFileStorage` par un stockage cloud (implémenter `IFileStorage`) si l'API tourne sur plusieurs serveurs.
- Le paiement est simulé (`FakePaymentProvider`) : à remplacer par les vrais prestataires (PayPal, TMoney, Flooz),
  y compris la gestion des remboursements en cas d'annulation d'une commande payée.
