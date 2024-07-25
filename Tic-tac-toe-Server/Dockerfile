FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TicTacToeGame.Client/TicTacToeGame.Client.csproj", "TicTacToeGame.Client/"]
COPY ["Tic-tac-toe-Server/Tic-tac-toe-Server.csproj", "Tic-tac-toe-Server/"]
RUN dotnet restore "Tic-tac-toe-Server/Tic-tac-toe-Server.csproj"
COPY . .
WORKDIR "/src/Tic-tac-toe-Server"
RUN dotnet build "Tic-tac-toe-Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN apt-get update -yq \
    && apt-get install -yq ca-certificates curl gnupg \
    && mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg \
    && echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_18.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list \
    && apt-get update -yq \
    && apt-get install nodejs -yq
RUN dotnet publish "Tic-tac-toe-Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tic-tac-toe-Server.dll"]
