#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 9000

LABEL author="Rudi"

ENV ASPNETCORE_URLS=http://+:9000

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["CI.API/CI.API.csproj", "CI.API/"]
COPY ["Libs/CI.Registration/CI.Registration.csproj", "Libs/CI.Registration/"]
COPY ["Libs/CI.API.Handlers/CI.API.Handlers.csproj", "Libs/CI.API.Handlers/"]
COPY ["Libs/CI.Commons/CI.Commons.csproj", "Libs/CI.Commons/"]
COPY ["Libs/CI.Interface/CI.Interface.csproj", "Libs/CI.Interface/"]
COPY ["Libs/CI.Model/CI.Model.csproj", "Libs/CI.Model/"]
COPY ["Libs/CI.Interface.Model/CI.Interface.Model.csproj", "Libs/CI.Interface.Model/"]
COPY ["Libs/Data/CI.Data.Business/CI.Data.Business.csproj", "Libs/Data/CI.Data.Business/"]
COPY ["Libs/Data/CI.Data.Model/CI.Data.Entity.csproj", "Libs/Data/CI.Data.Model/"]
COPY ["Libs/CI.Commands/CI.Commands.csproj", "Libs/CI.Commands/"]
COPY ["Libs/CI.Attributes/CI.Attributes.csproj", "Libs/CI.Attributes/"]
COPY ["Libs/Data/EF/CI.Data.EF.MongoDB/CI.Data.EF.MongoDB.csproj", "Libs/Data/EF/CI.Data.EF.MongoDB/"]
COPY ["Libs/CI.Mappers/CI.Mappers.csproj", "Libs/CI.Mappers/"]
COPY ["Libs/CI.MapperRegistration/CI.MapperRegistration.csproj", "Libs/CI.MapperRegistration/"]
COPY ["Libs/CI.JWT/CI.JWT.csproj", "Libs/CI.JWT/"]
COPY ["Libs/Data/CI.Seeder/CI.Seeder.csproj", "Libs/Data/CI.Seeder/"]
COPY ["Libs/CI.Controllers/CI.Controllers.csproj", "Libs/CI.Controllers/"]
RUN dotnet restore "CI.API/CI.API.csproj"
COPY . .
WORKDIR "/src/CI.API"
RUN dotnet build "CI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CI.API.csproj" -c Release -o /app/publish

RUN rm /app/publish/*.pdb
RUN rm /app/publish/app_config/appsettings.Development.json

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "CI.API.dll", "--urls=http://0.0.0.0:9000"]

VOLUME /app/Log
VOLUME /app/app_config

CMD "CI.API"

ENTRYPOINT ["dotnet", "CI.API.dll"]