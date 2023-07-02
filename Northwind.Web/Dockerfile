#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Northwind.Web/Northwind.Web.csproj", "Northwind.Web/"]
COPY ["Northwind.Database/Northwind.Database.csproj", "Northwind.Database/"]
RUN dotnet restore "Northwind.Web/Northwind.Web.csproj"
COPY . .
WORKDIR "/src/Northwind.Web"
RUN dotnet build "Northwind.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Northwind.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Northwind.Web.dll"]