FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GeneticShortestPointDemo/GeneticShortestPointDemo.csproj", "GeneticShortestPointDemo/"]
RUN dotnet restore "GeneticShortestPointDemo/GeneticShortestPointDemo.csproj"
COPY . .
WORKDIR "/src/GeneticShortestPointDemo"
RUN dotnet build "GeneticShortestPointDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GeneticShortestPointDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeneticShortestPointDemo.dll"]
