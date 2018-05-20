# AutoUpdate

## 基于WPF框架，无具体功能的软件自动更新程序config，用户可以新建配置文件，并根据配置文件生成新版本

-------

### 软件运行：直接打开WpfApp2\bin\Debug\WpfApp2.exe文件

### 相关文件说明
- AutoUpdater.xml: 本地配置文件
- WpfApp2\web\AutoUpdater.xml: 模拟网站上的最新配置文件
- WpfApp2\webfile: 模拟最新版本需要下载的文件
- WpfApp2\download: 存放下载的文件
- WpfApp2\myConfig: 存放用户自己新建的配置文件
- MainWindow.xaml: 界面代码
- MainWindow.xaml.cs: 交互逻辑代码
- log.log: 日志文件，记录用户进行的操作和异常

### 功能说明
1) 显示当前版本：主要根据本地配置文件对应的版本号显示
2) 更新至最新版本：获取本地配置文件(AutoUpdater.xml)的日期和网站上配置文件(WpfApp2\web\AutoUpdater.xml)的日期，进行比较，前者小于后者则进行更新(即下载和更新相关文件)，并更新配置文件，否则无需更新
3) 新建配置文件：给出模版，用户填入版本号和要更新的文件信息，保存生成相应的配置文件
4) 生成版本：根据用户新建的配置文件中要更新的文件信息，生成相应的文件
5) 查看系统配置文件
6) 查看日志
