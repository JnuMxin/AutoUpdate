using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.Xml;
using System.Collections;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 获取当前目录
        public string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        private static System.Diagnostics.Process p;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        public void Hide_all_pannel()
        {
            text_panel.Visibility = Visibility.Collapsed;
            myConfig_panel.Visibility = Visibility.Collapsed;
            newConfig_panel.Visibility = Visibility.Collapsed;
            table_panel.Visibility = Visibility.Collapsed;
            update_panel.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try{
                string filname = System.IO.Path.Combine(currentPath, @"..\..\AutoUpdater.xml");
                StreamReader reader = new StreamReader(filname);
                string version = reader.ReadLine();
                version = version.Substring(15, 3);
                reader.Close();
                MessageBox.Show("当前版本为"+version, "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
            
        }

        private void SysConfig(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide_all_pannel();
                
                string sysconfig = System.IO.Path.Combine(currentPath, @"..\..\AutoUpdater.xml");
                StreamReader reader = new StreamReader(sysconfig);
                string content = reader.ReadToEnd();
                
                textblock.Text = content;
                reader.Close();
                text_panel.Visibility = Visibility;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            try
            {
                string prePath = System.IO.Path.Combine(currentPath, @"..\..");
                string lastPath = System.IO.Path.Combine(currentPath, @"..\..\", "webconfig");

                string thePreUpdateDate = this.GetTheLastUpdateTime(prePath);
                string theLastUpdateDate = this.GetTheLastUpdateTime(lastPath);

                if (thePreUpdateDate != "")
                {
                    if(Convert.ToDateTime(thePreUpdateDate) >= Convert.ToDateTime(theLastUpdateDate))
                    {
                        // 客户端更新日期大于服务器端，不用更新
                        MessageBox.Show("当前已是最新版本！", "提示", MessageBoxButton.OK);
                        return;
                    }
                }
                // 更新(即下载或覆盖新增文件)
                this.GetUpdateFile(System.IO.Path.Combine(currentPath, @"..\..\webfile"));

                // 写入日志
                this.WriteLog("更新版本");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }

        }

        // 获取配置文件日期
        private string GetTheLastUpdateTime(string Dir)
        {
            try
            {
                string LastUpdateTime = "";
                string AutoUpdaterFileName = Dir + @"\AutoUpdater.xml";
                if (!File.Exists(AutoUpdaterFileName))
                {
                    return "配置文件不存在";
                }
                
                StreamReader reader = new StreamReader(AutoUpdaterFileName);
                string content = reader.ReadToEnd();
                string expr = @"Date(.*)/";
                MatchCollection mc = Regex.Matches(content, expr);
                foreach (Match m in mc)
                {
                    LastUpdateTime = m.ToString().Split('"')[1];
                }
                reader.Close();
                return LastUpdateTime;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return null;
            }

        }

        // 获取更新文件
        private void GetUpdateFile(string Dir)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(Dir);
                FileInfo[] files = directory.GetFiles();
                this.progressBarUpdate.Minimum = 0;
                this.progressBarUpdate.Maximum = files.Length;
                this.progressBarUpdate.Value = 0;

                string downloadPath = System.IO.Path.Combine(currentPath, @"..\..\download\");
                this.update_panel.Visibility = Visibility;
                foreach(FileInfo nextFile in files)
                {
                    File.Copy(nextFile.FullName, downloadPath + nextFile.Name, true);
                    this.tb.Text = "更新" + nextFile.Name;
                    this.progressBarUpdate.Value += 1;
                    this.percent.Text = "更新进度..." + (this.progressBarUpdate.Value / this.progressBarUpdate.Maximum * 100) + "%";
                }
                this.percent.Text = "更新完成";
                if(MessageBox.Show("更新至最新版本", "提示", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    this.update_panel.Visibility = Visibility.Collapsed;
                }
                //复制替换本地配置文件
                File.Copy(System.IO.Path.Combine(currentPath, @"..\..\webconfig\AutoUpdater.xml"), System.IO.Path.Combine(currentPath, @"..\..\AutoUpdater.xml"), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void WriteLog(string text)
        {
            try
            {
                string path = System.IO.Path.Combine(currentPath, @"..\..\", "log.log");

                FileStream logfile;
                StreamWriter writer;
                if (!File.Exists(path))
                {
                    logfile = new FileStream(path, FileMode.Create, FileAccess.Write);
                }
                else
                {
                    logfile = new FileStream(path, FileMode.Append, FileAccess.Write);
                }
                writer = new StreamWriter(logfile);
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                writer.WriteLine(text);
                writer.Close();
                logfile.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void NewConfig(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide_all_pannel();
                newConfig_panel.Visibility = Visibility;
                // 初始化
                string configText = "<?xml version=\"1.0\"?>\n"+
                                    "<AutoUpdater>\n" +
                                    "<URLAddres URL=\"\\webfile\" />\n" +
                                    "<UpdateInfo>\n" +
                                    "<UpdateTime Date = \""+ DateTime.Now.ToString("yyyy-MM-dd") + "\"/>\n" +
                                     "</UpdateInfo>\n" +
                                    "<UpdateFileList>\n" +
                                    "<UpdateFile FileName = \"filename1\" />\n" +
                                    "<UpdateFile FileName = \"filename2\" />\n" +
                                    "<UpdateFile FileName = \"filename3\" />\n" +
                                    "</UpdateFileList>\n" +
                                    "<RestartApp>\n" +
                                    "<ReStart Allow = \"Yes\" />\n" +
                                    "<AppName Name = \"AutoUpdate.exe\" />\n" +
                                    "</RestartApp>\n" +
                                    "</AutoUpdater>";
                this.newConfigedit.Text = configText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void Save_config(object sender, RoutedEventArgs e)
        {
            try
            {
                //绑定到我的配置文件里
                string configDir = System.IO.Path.Combine(currentPath, @"..\..\myConfig\");
                if(!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                // 获取版本号
                string configtext = this.newConfigedit.Text;
                string version = configtext.Substring(15, 3);
                string filename = configDir + version + ".xml";

                if (File.Exists(filename))
                {
                    MessageBox.Show("当前版本配置文件已存在，请修改版本号或修改该版本配置文件", "提示", MessageBoxButton.OK);
                    return;
                }
                // 保存我的配置文件
                FileStream configFile;
                configFile = new FileStream(filename, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(configFile);
                writer.Write(configtext);
                writer.Close();
                configFile.Close();

                // 写入日志
                this.WriteLog("新建配置文件" + version);

                newConfig_panel.Visibility = Visibility.Collapsed;
                MessageBox.Show("保存成功", "提示");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void Cancle(object sender, RoutedEventArgs e)
        {
            newConfig_panel.Visibility = Visibility.Collapsed;
        }

        private void Save_myconfig(object sender, RoutedEventArgs e)
        {
            try
            {
                // 获取版本号
                string configtext = this.myConfigtxt.Text;
                string version = configtext.Substring(15, 3);

                string filename = System.IO.Path.Combine(currentPath, @"..\..\myConfig\" + version + ".xml");

                // 保存我的配置文件
                FileStream configFile;
                configFile = new FileStream(filename, FileMode.Truncate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(configFile);
                writer.Write(configtext);
                writer.Close();
                configFile.Close();

                // 写入日志
                this.WriteLog("修改配置文件" + version);

                MessageBox.Show("保存成功", "提示");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void Cancle_myconfig(object sender, RoutedEventArgs e)
        {
            myConfig_panel.Visibility = Visibility.Collapsed;
        }

        // private void All_version(object sender, RoutedEventArgs e)
        // {
        //     this.Hide_all_pannel();
        //     table_panel.Visibility = Visibility;
        // }

        private void ReadMyconfig(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();    //打开文件窗口
                ofd.Filter = "所有文件|*.xml";     //要显示/查看的文件格式  
                if (ofd.ShowDialog() == false)  //如果没有打开  
                {
                    return;
                }
                string fileName = ofd.FileName;     //打开的文件名 
                
                // 显示配置文件
                StreamReader reader = new StreamReader(fileName);
                string content = reader.ReadToEnd();
                this.myConfigtxt.Text = content;

                reader.Close();

                myConfig_panel.Visibility = Visibility;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void Log(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Hide_all_pannel();
                text_panel.Visibility = Visibility;

                string logPath = System.IO.Path.Combine(currentPath, @"..\..\log.log");

                StreamReader logReader = new StreamReader(logPath);
                string logContent = logReader.ReadToEnd();
                logReader.Close();
                textblock.Text = logContent;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void GenerateVersion(object sender, RoutedEventArgs e)
        {
            try
            {
                // 版本号
                string version = this.myConfigtxt.Text.Substring(15, 3);
                string filename = System.IO.Path.Combine(currentPath, @"..\..\myConfig\" + version + ".xml");

                ArrayList files = new ArrayList();

                StreamReader reader = new StreamReader(filename);
                string content = reader.ReadToEnd();
                string expr = @"FileName(.*)/";
                MatchCollection mc = Regex.Matches(content, expr);
                foreach(Match m in mc)
                {
                    files.Add(m.ToString().Split('"')[1]);
                }
                reader.Close();

                // 根据配置文件生成更新文件
                string configDir = System.IO.Path.Combine(currentPath, @"..\..\" + version);
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }
                foreach (string f in files)
                {
                    File.Create(configDir + "\\" + f);
                }
                // 写入日志
                this.WriteLog("根据配置文件生成版本" + version);
                // 提示
                MessageBox.Show("版本" + version + "生成成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void Delete_myconfig(object sender, RoutedEventArgs e)
        {
            try
            {
                // 版本号
                string version = this.myConfigtxt.Text.Substring(15, 3);
                string filename = System.IO.Path.Combine(currentPath, @"..\..\myConfig\" + version + ".xml");

                if(MessageBox.Show("确定要删除该配置文件吗","警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    File.Delete(filename);
                    MessageBox.Show("删除成功");
                    myConfig_panel.Visibility = Visibility.Collapsed;
                    //写入日志
                    this.WriteLog("删除配置文件" + version);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                this.WriteLog("异常"+ex.Message);
                return;
            }
        }

        private void CompleteUpdate(object sender, RoutedEventArgs e)
        {
            if (p == null)
            {
                string bat = System.IO.Path.Combine(currentPath, @"..\..\..\..\update.bat");

                p = new System.Diagnostics.Process();

                p.StartInfo.FileName = bat;
                p.Start();
                Application.Current.Shutdown();
            }
            else
            {
                if (p.HasExited) //是否正在运行
                {
                    p.Start();
                }
            }
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        }
    }
}
