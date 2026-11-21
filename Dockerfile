FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["doe_sangue_api.sln", "./"]
COPY ["DoeSangue.API/DoeSangue.API.csproj", "DoeSangue.API/"]
COPY ["DoeSangue.Application/DoeSangue.Application.csproj", "DoeSangue.Application/"]
COPY ["DoeSangue.Domain/DoeSangue.Domain.csproj", "DoeSangue.Domain/"]
COPY ["DoeSangue.Infrastructure/DoeSangue.Infrastructure.csproj", "DoeSangue.Infrastructure/"]
RUN dotnet restore "doe_sangue_api.sln"

COPY . .
RUN dotnet publish "DoeSangue.API/DoeSangue.API.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "DoeSangue.API.dll"]
