# Stage 1: Build Angular SPA
FROM node:22-alpine AS frontend-build
WORKDIR /app/frontend
# Copy package files first for layer caching
COPY frontend/package*.json ./
RUN npm ci
# Copy frontend source and backend structure so ng build can write to ../backend/.../wwwroot
COPY frontend/ ./
COPY backend/ ../backend/
RUN npm run build

# Stage 2: Publish .NET application (backend/wwwroot already populated by stage 1)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend-build
WORKDIR /app
COPY --from=frontend-build /app/backend/ ./backend/
RUN dotnet publish backend/Portfolio.Api/Portfolio.Api.csproj -c Release -o /out

# Stage 3: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=backend-build /out ./
EXPOSE 8080
CMD ["sh", "-c", "ASPNETCORE_HTTP_PORTS=${PORT:-8080} dotnet Portfolio.Api.dll"]
