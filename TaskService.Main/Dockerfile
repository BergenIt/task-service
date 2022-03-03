FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TaskService.Main/TaskService.csproj", "TaskService.Main/"]
COPY ["proto-client-library/ProtobufClient.csproj", "proto-client-library/"]
COPY ["TaskService.Core/TaskService.Core.csproj", "TaskService.Core/"]
COPY ["database-extension/DatabaseExtension.csproj", "database-extension/"]
RUN dotnet restore "TaskService.Main/TaskService.csproj"
COPY . .
WORKDIR "/src/TaskService.Main"
RUN dotnet build "TaskService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskService.dll"]
