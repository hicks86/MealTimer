# 🍽️ Meal Timer — Cooking Scheduler

[![.NET](https://img.shields.io/badge/.NET-9.0-blue?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor WASM](https://img.shields.io/badge/Blazor-WebAssembly-purple)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![MudBlazor](https://img.shields.io/badge/UI-MudBlazor-orange)](https://mudblazor.com/)
[![Deploy on Vercel](https://img.shields.io/badge/Deploy-Vercel-black?logo=vercel)](https://vercel.com/)

A **mobile-first** cooking scheduler that works out when to start each dish so everything hits the table at the same time — no more cold chips waiting for the fish.

---

## ✨ Features

- 🧮 **Smart scheduling** — calculates prep & cook start offsets so all dishes finish simultaneously
- ⏱️ **Live countdowns** — per-item timers with progress bars for each cooking phase
- 🔔 **Audio alerts** — Web Audio API beeps when it's time to start prepping or cooking
- ⏸️ **Pause / Resume** — pause the timer if you need a moment
- 📴 **Offline-capable** — pure static site, no backend required

---

## 📱 How It Works

1. **Name your meal** and tap **Add Item** to list every dish you're cooking
2. Each item accepts: cook time (required), prep time (optional), cooking device, and temperature
3. Tap **Start Cooking** — the app calculates the full schedule:
   - The dish with the longest total time (prep + cook) begins immediately
   - All other dishes are offset so they all finish at the same time
4. Watch per-item status badges cycle: **⏳ Waiting → ⚡ Prepare Now! → 🔥 Start Cooking! → ✓ Ready**

### Example — Fish and Chips (50-minute meal)

| Dish | Prep | Cook | Total | Prep start | Cook start | Done |
|---|---|---|---|---|---|---|
| Battered Fish | 10 min | 40 min | 50 min | 0:00 | 10:00 | 50:00 |
| Chips | — | 20 min | 20 min | — | 30:00 | 50:00 |

---

## 🚀 Local Development

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (or .NET 8+)

### Run locally

```bash
cd MealTimer
dotnet run
```

Open the URL shown in your terminal (usually `https://localhost:5001`).

For hot-reload during development:

```bash
dotnet watch
```

---

## 🌐 Deploying to Vercel

Vercel serves static files only, so you publish the Blazor WASM output first.

### 1. Publish

```bash
dotnet publish -c Release -o publish
```

The publishable static site will be at **`publish/wwwroot/`**.

### 2. Deploy

**Option A — Vercel CLI**

```bash
npm install -g vercel
cd publish/wwwroot
vercel --prod
```

**Option B — Vercel Dashboard (GitHub integration)**

1. Push this repository to GitHub
2. Import it at [vercel.com/new](https://vercel.com/new)
3. Configure the build settings:
   | Setting | Value |
   |---|---|
   | **Framework Preset** | Other |
   | **Build Command** | `dotnet publish -c Release -o publish` |
   | **Output Directory** | `publish/wwwroot` |
   | **Install Command** | *(see note below)* |

> **Note:** Vercel's build runners don't include the .NET SDK. You can add a `build.sh` script that installs it via the [dotnet-install script](https://dot.net/v1/dotnet-install.sh), or use a GitHub Action to build and push the `publish/wwwroot` folder to a separate branch that Vercel watches.

The `vercel.json` at the project root handles SPA fallback routing so deep-links (`/cook`) resolve correctly.

---

## 🏗️ Project Structure

```
MealTimer/
├── Components/
│   └── AddFoodItemDialog.razor    # Add food item modal
├── Layout/
│   └── MainLayout.razor           # App shell with MudBlazor theme
├── Models/
│   ├── FoodItem.cs                # Domain model
│   └── Meal.cs                    # Domain model
├── Pages/
│   ├── Home.razor                 # Meal setup page  (/)
│   └── Cook.razor                 # Live cooking screen  (/cook)
├── Services/
│   └── MealStateService.cs        # In-memory state + scheduling logic
├── wwwroot/
│   ├── css/app.css                # Global styles + animations
│   └── js/audioInterop.js         # Web Audio API beep via JS interop
├── vercel.json                    # Vercel SPA rewrite rule
└── Program.cs                     # Service registration
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | Blazor WebAssembly (.NET 9) |
| UI Components | MudBlazor |
| State | In-memory Scoped Service |
| Timer | `PeriodicTimer` + `InvokeAsync` |
| Audio | Web Audio API (JS Interop) |
| Hosting | Vercel (static) |
