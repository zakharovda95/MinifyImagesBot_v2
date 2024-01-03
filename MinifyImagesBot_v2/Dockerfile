FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MinifyImagesBot_v2/MinifyImagesBot_v2.csproj", "MinifyImagesBot_v2/"]
RUN dotnet restore "MinifyImagesBot_v2/MinifyImagesBot_v2.csproj"
COPY . .
WORKDIR "/src/MinifyImagesBot_v2"
RUN dotnet build "MinifyImagesBot_v2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MinifyImagesBot_v2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MinifyImagesBot_v2.dll"]
