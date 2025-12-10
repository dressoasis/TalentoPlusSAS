# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["TalentoPlusS.A.S.sln", "./"]
COPY ["src/TalentoPlus.Api/TalentoPlus.Api.csproj", "src/TalentoPlus.Api/"]
COPY ["src/TalentoPlus.Application/TalentoPlus.Application.csproj", "src/TalentoPlus.Application/"]
COPY ["src/TalentoPlus.Domain/TalentoPlus.Domain.csproj", "src/TalentoPlus.Domain/"]
COPY ["src/TalentoPlus.Infrastructure.Data/TalentoPlus.Infrastructure.Data.csproj", "src/TalentoPlus.Infrastructure.Data/"]
COPY ["src/TalentoPlus.Infrastructure.Identity/TalentoPlus.Infrastructure.Identity.csproj", "src/TalentoPlus.Infrastructure.Identity/"]
COPY ["src/TalentoPlus.Infrastructure.Integrations/TalentoPlus.Infrastructure.Integrations.csproj", "src/TalentoPlus.Infrastructure.Integrations/"]
COPY ["src/TalentoPlus.Web/TalentoPlus.Web.csproj", "src/TalentoPlus.Web/"]

# Restore dependencies
RUN dotnet restore "src/TalentoPlus.Api/TalentoPlus.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build and Publish
WORKDIR "/src/src/TalentoPlus.Api"
RUN dotnet build "TalentoPlus.Api.csproj" -c Release -o /app/build
RUN dotnet publish "TalentoPlus.Api.csproj" -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TalentoPlus.Api.dll"]
