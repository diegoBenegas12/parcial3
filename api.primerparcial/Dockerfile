#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["api.primerparcial.csproj", "api.primerparcial/"]
RUN dotnet restore "api.primerparcial/api.primerparcial.csproj"
COPY . .
WORKDIR "/src/api.primerparcial"
RUN dotnet build "api.primerparcial.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api.primerparcial.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api.primerparcial.dll"]