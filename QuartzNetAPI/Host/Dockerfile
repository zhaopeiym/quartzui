
#基础镜像（用来构建镜像）
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
RUN apk add -U tzdata
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN cp /usr/share/zoneinfo/Asia/Shanghai /usr/share/zoneinfo/Asia/Beijing

#编译（临时镜像，主要用来编译发布项目）
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS publish
WORKDIR /src
COPY . .
WORKDIR /src/Host
RUN dotnet publish -c Release -o /app

#构建镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ["dotnet", "Host.dll"]