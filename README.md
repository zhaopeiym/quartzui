[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
[![996.icu](https://img.shields.io/badge/link-996.icu-red.svg)](https://996.icu)
[![GitHub license](https://img.shields.io/github/license/alienwow/SnowLeopard.svg)](https://github.com/zhaopeiym/quartzui/blob/master/LICENSE)

## 注意：请不要跑在IIS上，因为IIS会自动回收。建议使用docker运行，或者直接用命令dotnet Host.dll。

## 说明文档 
https://github.com/zhaopeiym/quartzui/wiki  

## 演示地址
https://scheduler.haojima.net  
默认口令：admin  

## quartzui
- 基于.NET5.0和Quartz.NET3.2.4的任务调度Web界面管理。
- docker方式开箱即用
- 内置SQLite持久化
- 支持 RESTful风格接口
- 业务代码零污染
- 语言无关
- 傻瓜式配置
- 异常请求邮件通知

## 使用
- 方式1（docker使用）
```
docker run -v /fileData/quartzuifile:/app/File  --restart=unless-stopped --privileged=true --name quartzui -dp 5088:80 bennyzhao/quartzui

一行命令开箱即用，赶快体验下docker的便捷吧！
1、其中/fileData/quartzuifile为映射的文件地址，如SQLite数据库和log日志
2、5088为映射到主机的端口
3、直接在浏览器 ip:5088 即可访问。（注意防火墙是否打开了5088端口，或者在主机测试 curl 127.0.0.1:5088）
```
- 方式2（docker部署树莓派）
```
docker run -v /fileData/quartzuifile:/app/File  --restart=unless-stopped --privileged=true --name quartzui -dp 5088:80 bennyzhao/quartzui:RaspberryPi
```
- 方式3（可直接通过源码部署到windows或linux平台） 

## 更换数据源  
默认使用的是SQLite-Microsoft  
如果需要使用其他数据源请自行在[appsettings.json](https://github.com/zhaopeiym/quartzui/blob/dev/QuartzNetAPI/Host/appsettings.json)进行正确配置。如：  
```
"dbProviderName":"OracleODPManaged",
"connectionString": "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=xe)));User Id=system;Password=oracle;";

"dbProviderName":"SqlServer",
"connectionString": "Server=localhost;Database=quartznet;User Id={SqlServerUser};Password={SqlServerPassword};";

"dbProviderName":"SQLServerMOT",
"connectionString": "Server=localhost,1444;Database=quartznet;User Id={SqlServerUser};Password={SqlServerPassword};"

"dbProviderName":"MySql", // MySql 测试通过
"connectionString": "Server = localhost; Database = quartznet; Uid = quartznet; Pwd = quartznet";

"dbProviderName":"Npgsql", // Npgsql 测试通过
"connectionString": "Server=127.0.0.1;Port=5432;Userid=quartznet;Password=quartznet;Pooling=true;MinPoolSize=1;MaxPoolSize=20;Timeout=15;SslMode=Disable;Database=quartznet";

"dbProviderName":"SQLite",
"connectionString": "Data Source=test.db;Version=3;";

"dbProviderName":"SQLite-Microsoft", // SQLite-Microsoft 测试通过
"connectionString": "Data Source=test.db;";

"dbProviderName":"Firebird",
"connectionString": "User=SYSDBA;Password=masterkey;Database=/firebird/data/quartz.fdb;DataSource=localhost;Port=3050;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
```

## 效果图
![1](https://user-images.githubusercontent.com/5820324/105847993-574c5000-6019-11eb-8779-802fdd172a96.png)
![2](https://user-images.githubusercontent.com/5820324/56856559-1c267400-6990-11e9-8433-4705e0c4a984.png)
![3](https://user-images.githubusercontent.com/5820324/56856560-1cbf0a80-6990-11e9-835c-268efac70d50.png)
![4](https://user-images.githubusercontent.com/5820324/56856561-1cbf0a80-6990-11e9-8af6-a93ad0e09740.png)
![5](https://user-images.githubusercontent.com/5820324/56856562-1d57a100-6990-11e9-8433-5d6e1d78880a.png)





