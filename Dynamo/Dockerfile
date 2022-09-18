FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Dashboard/Dashboard.csproj", "Dynamo/"]
RUN dotnet restore "Dynamo/Dynamo.csproj"
COPY . .
WORKDIR "/src/Dynamo"
RUN dotnet build "Dynamo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dynamo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dynamo.dll"]