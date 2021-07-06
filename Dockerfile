FROM mcr.microsoft.com/dotnet/aspnet:3.1-focal AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:3.1-focal AS build
WORKDIR /src
COPY ["HouseManagementSystemAPI/HMSAPI.csproj", "HouseManagementSystemAPI/"]
RUN dotnet restore "HouseManagementSystemAPI/HMSAPI.csproj"
COPY . .
WORKDIR "/src/HouseManagementSystemAPI"
RUN dotnet build "HMSAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HMSAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HMSAPI.dll"]
