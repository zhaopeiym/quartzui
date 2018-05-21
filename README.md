## quartzui
- 基于Quartz.NET 3.0的web管理界面。
- docker方式开箱即用
- 内置SQLite持久化
- 支持 RESTful风格接口
- 业务代码零污染
- 语言无关
- 傻瓜式配置

## 使用
- 方式1（docker使用）
```
docker run -v /fileData/quartzuifile:/app/File  --restart=unless-stopped --privileged=true --name quartzui -dp 5088:80 bennyzhao/quartzui

一行命令开箱即用，赶快体验下docker的便捷吧！
1、其中/fileData/quartzuifile为映射的文件地址，如SQLite数据库和log日志
2、5088为映射到主机的端口
3、直接在浏览器 ip:5088 即可访问。（注意防火墙是否打开了5088端口，或者在主机测试 curl 127.0.0.1：5008）
```
- 方式2（可直接通过源码部署到windows或linux平台）   

## 欢迎贡献代码
- https://github.com/zhaopeiym/quartzui/blob/master/%E6%B3%A8%E6%84%8F.md

## 效果图
![qq 20180520160711](https://user-images.githubusercontent.com/5820324/40276903-e98a24c0-5c47-11e8-834c-67387b2d8935.png)
![qq 20180520160824](https://user-images.githubusercontent.com/5820324/40276909-0f8c9c02-5c48-11e8-8925-9d0ad5bd469d.png)





