[![LICENSE](https://img.shields.io/badge/license-Anti%20996-blue.svg)](https://github.com/996icu/996.ICU/blob/master/LICENSE)
[![996.icu](https://img.shields.io/badge/link-996.icu-red.svg)](https://996.icu)
[![GitHub license](https://img.shields.io/github/license/alienwow/SnowLeopard.svg)](https://github.com/zhaopeiym/quartzui/blob/master/LICENSE)

## 说明文档 
https://github.com/zhaopeiym/quartzui/wiki  

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
3、直接在浏览器 ip:5088 即可访问。（注意防火墙是否打开了5088端口，或者在主机测试 curl 127.0.0.1:5008）
```
- 方式2（可直接通过源码部署到windows或linux平台）   

## 效果图
![1](https://user-images.githubusercontent.com/5820324/40886833-b779990e-6771-11e8-88e2-8bd52ebec39f.png)
![2](https://user-images.githubusercontent.com/5820324/40886838-c0597d14-6771-11e8-8b77-ffd1d24b5abd.png)
![3](https://user-images.githubusercontent.com/5820324/40886841-c9c03938-6771-11e8-941e-e82063a7cf49.png)
![4](https://user-images.githubusercontent.com/5820324/40886843-d4b209de-6771-11e8-8b22-b9224a43a06e.png)





