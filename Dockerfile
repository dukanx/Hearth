# --- Frontend build ---
FROM node:22-alpine AS web
# npm 11 — ista major verzija kojom je generisan package-lock.json
# (npm 10 iz base image-a strože validira lock i pada na wasm optional deps)
RUN npm install -g npm@11
WORKDIR /src
COPY hearth-web/package.json hearth-web/package-lock.json ./
RUN npm ci
COPY hearth-web/ ./
RUN npm run build

# --- Backend build ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Hearth.Domain/Hearth.Domain.csproj Hearth.Domain/
COPY Hearth.Application/Hearth.Application.csproj Hearth.Application/
COPY Hearth.Infrastructure/Hearth.Infrastructure.csproj Hearth.Infrastructure/
COPY Hearth.Api/Hearth.Api.csproj Hearth.Api/
RUN dotnet restore Hearth.Api/Hearth.Api.csproj
COPY Hearth.Domain/ Hearth.Domain/
COPY Hearth.Application/ Hearth.Application/
COPY Hearth.Infrastructure/ Hearth.Infrastructure/
COPY Hearth.Api/ Hearth.Api/
RUN dotnet publish Hearth.Api/Hearth.Api.csproj -c Release -o /app --no-restore

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
COPY --from=web /src/dist ./wwwroot
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
ENTRYPOINT ["dotnet", "Hearth.Api.dll"]
