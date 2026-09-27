# Construcción y Compilación con el SDK 
FROM ://microsoft.com AS build
WORKDIR /app

# Copiar los archivos del proyecto y restaurar dependencias
COPY ["src/Visiotech.Pokemon.Api/Visiotech.Pokemon.Api.csproj", "src/Visiotech.Pokemon.Api/"]
RUN dotnet restore "src/Visiotech.Pokemon.Api/Visiotech.Pokemon.Api.csproj"

# Copiar el resto del código fuente y compilar en modo Release
COPY . .
WORKDIR "/app/src/Visiotech.Pokemon.Api"
RUN dotnet publish "Visiotech.Pokemon.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime de la aplicación
FROM ://microsoft.com AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exponer el puerto de la API
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Visiotech.Pokemon.Api.dll"]