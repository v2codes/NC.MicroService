#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# 基础镜像
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# 基础镜像
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NC.MicroService.TeamService/NC.MicroService.TeamService.csproj", "NC.MicroService.TeamService/"]
COPY ["Omega/src/Servicecomb.Saga.Omega.Core/Servicecomb.Saga.Omega.Core.csproj", "Omega/src/Servicecomb.Saga.Omega.Core/"]
COPY ["Omega/src/Servicecomb.Saga.Omega.Abstractions/Servicecomb.Saga.Omega.Abstractions.csproj", "Omega/src/Servicecomb.Saga.Omega.Abstractions/"]
COPY ["Omega/src/Servicecomb.Saga.Omega.Protocol/Servicecomb.Saga.Omega.Protocol.csproj", "Omega/src/Servicecomb.Saga.Omega.Protocol/"]
COPY ["NC.MicroService.Infrastructure/NC.MicroService.Infrastructure.csproj", "NC.MicroService.Infrastructure/"]
COPY ["NC.MicroService.EntityFrameworkCore/NC.MicroService.EntityFrameworkCore.csproj", "NC.MicroService.EntityFrameworkCore/"]
COPY ["Omega/src/Servicecomb.Saga.Omega.AspNetCore/Servicecomb.Saga.Omega.AspNetCore.csproj", "Omega/src/Servicecomb.Saga.Omega.AspNetCore/"]
RUN dotnet restore "NC.MicroService.TeamService/NC.MicroService.TeamService.csproj"
COPY . .
WORKDIR "/src/NC.MicroService.TeamService"

# 构建项目
RUN dotnet build "NC.MicroService.TeamService.csproj" -c Release -o /app/build

# 发布项目
FROM build AS publish
RUN dotnet publish "NC.MicroService.TeamService.csproj" -c Release -o /app/publish

# 启动命令
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NC.MicroService.TeamService.dll"]