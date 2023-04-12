using ACDCTestSystemPart1.Models;
using Common_API;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACDCTestSystemPart1.ViewModels
{
    public class ConfigWindowViewModel : BindableBase, INavigationAware, IConfirmNavigationRequest
    {
        private readonly IRegionManager regionManager;
        public ConfigWindowViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            CommonAPI common = new CommonAPI();
            if (common.GetSerialPort() != null && common.GetSerialPort()?.Length != 0)
            {
                SerialPorts.AddRange(common.GetSerialPort());
            }
            LoadFile();
        }


        #region property
        //private string honeyWellPort;
        //public string HoneyWellPort
        //{
        //    get
        //    {
        //        return honeyWellPort;
        //    }
        //    set
        //    {
        //        SetProperty(ref honeyWellPort, value);
        //    }
        //}

        //private string fIDPort;
        //public string FIDPort
        //{
        //    get
        //    {
        //        return fIDPort;
        //    }
        //    set
        //    {
        //        SetProperty(ref fIDPort, value);
        //    }
        //}

        //private string iPAddr;
        //public string IPAddr
        //{
        //    get
        //    {
        //        return iPAddr;
        //    }
        //    set
        //    {
        //        SetProperty(ref iPAddr, value);
        //    }
        //}

        //private string pLCAddr;
        //public string PLCAddr
        //{
        //    get
        //    {
        //        return pLCAddr;
        //    }
        //    set
        //    {
        //        SetProperty(ref pLCAddr, value);
        //    }
        //}

        private string configfilename;
        public string ConfigFileName
        {
            get
            {
                return configfilename;
            }
            set
            {
                SetProperty(ref configfilename, value);
            }
        }

        //private int ngcount;
        //public int NGCount
        //{
        //    get
        //    {
        //        return ngcount;
        //    }
        //    set
        //    {
        //        SetProperty(ref ngcount, value);
        //    }
        //}

        //private string databasestring;
        //public string DataBaseString
        //{
        //    get
        //    {
        //        return databasestring;
        //    }
        //    set
        //    {
        //        SetProperty(ref databasestring, value);
        //    }
        //}

        private int electrifyTime;
        public int ElectrifyTime
        {
            get
            {
                return electrifyTime;
            }
            set
            {
                SetProperty(ref electrifyTime, value);
            }
        }

        private int startuptime;
        public int StartupTime
        {
            get
            {
                return startuptime;
            }
            set
            {
                SetProperty(ref startuptime, value);
            }
        }

        private double voltagemax;
        public double VoltageMax
        {
            get
            {
                return voltagemax;
            }
            set
            {
                SetProperty(ref voltagemax, value);
            }
        }

        private double voltagemin;
        public double VoltageMin
        {
            get
            {
                return voltagemin;
            }
            set
            {
                SetProperty(ref voltagemin, value);
            }
        }

        private string configFilePath;
        public string ConfigFilePath
        {
            get
            {
                return configFilePath;
            }
            set
            {
                SetProperty(ref configFilePath, value);
            }
        }

        private ConnectionConfig connecteConfig;
        public ConnectionConfig ConnecteConfig
        {
            get
            {
                return connecteConfig;
            }
            set
            {
                SetProperty(ref connecteConfig, value);
            }
        }

        private TestConfig testConfig;
        public TestConfig TestConfig
        {
            get
            {
                return testConfig;
            }
            set
            {
                SetProperty(ref testConfig, value);
            }
        }

        private ObservableCollection<string> serialPorts;
        public ObservableCollection<string> SerialPorts
        {
            get
            {
                return serialPorts;
            }
            set
            {
                SetProperty(ref serialPorts, value);
            }
        }

        public string DefualtConfigName
        {
            get; set;
        }
        #endregion

        #region command
        private DelegateCommand saveCommand;
        public DelegateCommand SaveCommand =>
            saveCommand ?? (saveCommand = new DelegateCommand(ExecuteSaveCommand));
        void ExecuteSaveCommand()
        {
            if (string.IsNullOrEmpty(TestConfig.FileName))
            {
                System.Windows.MessageBox.Show("请输入需要保存的文件名", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                return;
            }
            
            try
            {
                JObject jobj = new JObject();
                string path = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}TESTCONFIG.json";
                using (StreamReader stm = new StreamReader(path))
                {
                    using (JsonTextReader jsontext = new JsonTextReader(stm))
                    {
                        jobj = (JObject)JToken.ReadFrom(jsontext);
                    }
                }
                ConnectionConfig config = ConnecteConfig ?? new ConnectionConfig();

                if (jobj.ContainsKey("ConnectionConfig"))
                {
                    jobj["ConnectionConfig"] = JObject.Parse(JsonConvert.SerializeObject(config));
                }
                else
                {
                    jobj.Add("ConnectionConfig", JObject.Parse(JsonConvert.SerializeObject(config)));
                }
                using (StreamWriter wirte = new StreamWriter(path, false))
                {
                    wirte.Write(jobj);
                    wirte.Flush();
                }


                //以下是测试配置
                DirectoryInfo dir = new DirectoryInfo("D:\\ACDC自动测试线配置");
                if (!dir.Exists)
                {
                    dir.Create();
                }
                JObject jobj1 = new JObject();
                string path1 = $"D:\\ACDC自动测试线配置\\{TestConfig.FileName}.json";

                if (File.Exists(path1))
                {
                    if (MessageBox.Show("文件已经存在是否覆盖？","提示",MessageBoxButton.OKCancel,MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        using (StreamReader stm = new StreamReader(path1))
                        {
                            using (JsonTextReader jsontext = new JsonTextReader(stm))
                            {
                                jobj1 = (JObject)JToken.ReadFrom(jsontext);
                            }
                        }
                        if (jobj1.ContainsKey(TestConfig.FileName))
                        {
                            jobj1[TestConfig.FileName] = JObject.Parse(JsonConvert.SerializeObject(TestConfig));
                        }
                        else
                        {
                            jobj1.Add(TestConfig.FileName, JObject.Parse(JsonConvert.SerializeObject(TestConfig)));
                        }
                        using (StreamWriter wirte = new StreamWriter(path1, false))
                        {
                            wirte.Write(jobj1);
                            wirte.Flush();
                        }
                    }
                }
                else
                {
                    jobj1.Add(TestConfig.FileName, JObject.Parse(JsonConvert.SerializeObject(TestConfig)));
                    using (StreamWriter wirte = new StreamWriter(path1, false))
                    {
                        wirte.Write(jobj1);
                        wirte.Flush();
                    }
                }
                Share.ShareModel.connectionConfig = config;
                Properties.Settings.Default.TestConfigName = TestConfig.FileName;
                Properties.Settings.Default.Save();
                MessageBox.Show("保存成功");

            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message, "保存失败");
            }




        }


        private DelegateCommand opencommand;
        public DelegateCommand OpenCommand =>
            opencommand ?? (opencommand = new DelegateCommand(ExecuteOpenCommand));

        void ExecuteOpenCommand()
        {
            try
            {
                JObject jobj = new JObject();
                JObject jobj1 = new JObject();
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Title = "加载文件";
                openFile.DefaultExt = "json";
                openFile.Multiselect = false;
                openFile.ShowDialog();
                if (!string.IsNullOrEmpty(openFile.FileName))
                {
                    if (!openFile.SafeFileName.Split('.')[1].Equals("json"))
                    {
                        throw new Exception("不支持此类文件！");
                    }
                    using (StreamReader stm = new StreamReader($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}TESTCONFIG.json"))
                    {
                        using (JsonTextReader jsontext = new JsonTextReader(stm))
                        {
                            jobj = (JObject)JToken.ReadFrom(jsontext);
                        }
                    }

                    using (StreamReader stm = new StreamReader(openFile.FileName))
                    {
                        using (JsonTextReader jsontext = new JsonTextReader(stm))
                        {
                            jobj1 = (JObject)JToken.ReadFrom(jsontext);
                        }
                    }

                    if (!jobj1.ContainsKey(openFile.SafeFileName.Split('.')[0]))
                    {
                        throw new Exception("不支持此文件，请使用正确的配置文件配置参数");
                    }
                    ConnectionConfig connectAddress = jobj["ConnectionConfig"]?.ToObject<ConnectionConfig>();
                    TestConfig tConfig = jobj1[openFile.SafeFileName.Split('.')[0]]?.ToObject<TestConfig>();
                    ConnecteConfig = connectAddress;
                    TestConfig = tConfig;
                    Share.ShareModel.connectionConfig = connectAddress;
                    System.Windows.MessageBox.Show("加载完成");
                }

            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message, "加载失败");
            }
        }


        private DelegateCommand exitCommand;

        public DelegateCommand ExitCommand =>
            exitCommand ?? (exitCommand = new DelegateCommand(ExecuteExitCommand));

        void ExecuteExitCommand()
        {
            NavigationParameters keys = new NavigationParameters();
            keys.Add("key", "Exit");
            this.regionManager.RequestNavigate("ContentRegion", "LoginPage", arg => { }, keys);
        }
        #endregion


        private void LoadFile()
        {//加载处需要考虑TestConfig.Name如何获取问题 -----------未解决
            try
            {
                JObject jobj = new JObject();
                JObject jobj1 = new JObject();
                using (StreamReader stm = new StreamReader($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}{"TESTCONFIG".Trim()}.json"))
                {
                    using (JsonTextReader jsontext = new JsonTextReader(stm))
                    {
                        jobj = (JObject)JToken.ReadFrom(jsontext);
                    }
                }
                if (!jobj.ContainsKey("ConnectionConfig"))
                {
                    throw new Exception("不支持此文件，请使用正确的配置文件配置参数");
                }
                ConnectionConfig connectAddress = jobj["ConnectionConfig"]?.ToObject<ConnectionConfig>();
                ConnecteConfig = connectAddress;


                string path = $"D:\\ACDC自动测试线配置\\{Properties.Settings.Default.TestConfigName}.json";
                if (File.Exists(path))
                {
                    using (StreamReader stm = new StreamReader(path))
                    {
                        using (JsonTextReader jsontext = new JsonTextReader(stm))
                        {
                            jobj1 = (JObject)JToken.ReadFrom(jsontext);
                        }
                    }
                    TestConfig tConfig = jobj1[Properties.Settings.Default.TestConfigName]?.ToObject<TestConfig>();
                    TestConfig = tConfig;
                }
                else
                {
                    using (StreamReader stm = new StreamReader($"D:\\ACDC自动测试线配置\\DefaultTestConfig.json"))
                    {
                        using (JsonTextReader jsontext = new JsonTextReader(stm))
                        {
                            jobj1 = (JObject)JToken.ReadFrom(jsontext);
                        }
                    }
                    TestConfig tConfig = jobj1["Default"]?.ToObject<TestConfig>();
                    TestConfig = tConfig;
                }
                
                //FIDPort = connectAddress?.FIDPort;
                //HoneyWellPort = connectAddress?.HoneyWellPort;
                //PLCAddr = connectAddress?.PLCAddr;
                //IPAddr = connectAddress?.IPAddr;
                //NGCount = (int)(connectAddress?.NGCount);
                //ConfigFileName = tConfig?.FileName;
                //ElectrifyTime = (int)(tConfig?.ElectrifyTime);
                //StartupTime = (int)(tConfig?.StartupTime);
                //VoltageMax = (double)(tConfig?.VoltageMax);
                //VoltageMin = (double)(tConfig?.VoltageMin);
                //DataBaseString = connectAddress?.DataBaseString;


                Share.ShareModel.connectionConfig = connectAddress;

            }
            catch (Exception err)
            {
                MessageBox.Show($"{err.Message}；加载配置失败，请手动加载通讯配置");
            }
        }
        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }
    }
}
