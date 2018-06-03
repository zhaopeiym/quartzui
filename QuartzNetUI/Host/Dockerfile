#基础镜像（用来构建镜像）
FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80
ENV TZ=Asia/Shanghai
RUN cp /usr/share/zoneinfo/Asia/Shanghai /usr/share/zoneinfo/Asia/Beijing
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

#编译（临时镜像，主要用来编译发布项目）
FROM microsoft/aspnetcore-build:2.0 AS publish
WORKDIR /src
COPY . .
WORKDIR /src/Host
RUN dotnet publish -c Release -o /app

#构建镜像
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ["dotnet", "Host.dll"]