FROM microsoft/aspnetcore
COPY . /app
WORKDIR /app
EXPOSE 80
ENV TZ=Asia/Shanghai
RUN cp /usr/share/zoneinfo/Asia/Shanghai /usr/share/zoneinfo/Asia/Beijing
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
CMD ["dotnet", "Host.dll"]