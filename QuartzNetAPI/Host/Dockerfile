
#基础镜像（用来构建镜像）
FROM bennyzhao/aspnet:3.0-alpine AS base
WORKDIR /app
EXPOSE 80

#编译（临时镜像，主要用来编译发布项目）
FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS publish
WORKDIR /src
COPY . .
WORKDIR /src/Host
RUN dotnet publish -c Release -o /app

#构建镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ["dotnet", "Host.dll"]