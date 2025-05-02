# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos csproj e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia o restante do projeto
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa final (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Exponha a porta usada pela aplicação
EXPOSE 8080

ENTRYPOINT ["dotnet", "FuriaApi.dll"]