##See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
#WORKDIR /src
#COPY ["BookLibraryProject/BookLibraryProject.csproj", "BookLibraryProject/"]
#COPY ["../Data/Data.csproj", "../Data/"]
#COPY ["../Models/Models.csproj", "../Models/"]
#COPY ["Interfaces/Interfaces.csproj", "Interfaces/"]
#COPY ["Helpers/Helpers.csproj", "Helpers/"]
#COPY ["DTOs/DTOs.csproj", "DTOs/"]
#COPY ["MiddleWare/MiddleWare.csproj", "MiddleWare/"]
#COPY ["Error/Errors.csproj", "Error/"]
#COPY ["Extensions/Extensions.csproj", "Extensions/"]
#COPY ["Services/Services.csproj", "Services/"]
#RUN dotnet restore "BookLibraryProject/BookLibraryProject.csproj"
#COPY . .
#WORKDIR "/src/BookLibraryProject"
#RUN dotnet build "BookLibraryProject.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "BookLibraryProject.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "BookLibraryProject.dll"]

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY . ./ 
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
EXPOSE 80
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS_=http://*:$PORT
ENTRYPOINT ["dotnet", "BookLibraryProject.dll"]