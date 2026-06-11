# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and custom NuGet config (locks restore to nuget.org only)
COPY NuGet.config ./
COPY MealTimer.csproj ./

# Restore with explicit config to avoid any private feed references
RUN dotnet restore --configfile NuGet.config

# Copy remaining source and publish
COPY . .
RUN dotnet publish MealTimer.csproj -c Release -o /publish --no-restore

# ── Stage 2: Serve ────────────────────────────────────────────────────────────
FROM nginx:1.27-alpine AS final

# Copy the Blazor WASM static output
COPY --from=build /publish/wwwroot /usr/share/nginx/html

# Copy the nginx configuration (SPA routing + gzip + security headers)
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Railway injects PORT at runtime; default to 8080 for local testing
ENV PORT=8080
EXPOSE 8080

# Use shell form so $PORT is expanded at container start
CMD ["/bin/sh", "-c", "sed -i 's/__PORT__/'\"$PORT\"'/g' /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
