using ACDCTestSystemPart1.Models;
using ACDCTestSystemPart1.PublishEvent;
using ACDCTestSystemPart1.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ACDCTestSystemPart1.ViewModels
{
    public class TestWindowViewModel : BindableBase, INavigationAware, IConfirmNavigationRequest
    {
        private readonly IEventAggregator eventAggregator;
        public TestWindowViewModel(IEventAggregator _eventAggregator)
        {
            eventAggregator = _eventAggregator;
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                SingleChannel = "1";
                MultiChannel = "0";
                CurrentMode = "单路模式";
            }));
            Result_Voltage = WaitTest;
            Result_Voltage2 = WaitTest;
            Result_Function1 = WaitTest;
            Result_Function2 = WaitTest;
            RunStatus = "未运行";
            InitialClass();
        }

        #region dependproperty
        private string singlechannel;
        public string SingleChannel
        {
            get
            {
                return singlechannel;
            }
            set
            {
                SetProperty(ref singlechannel, value);
            }
        }

        private string multichannel;
        public string MultiChannel
        {
            get
            {
                return multichannel;
            }
            set
            {
                SetProperty(ref multichannel, value);
            }
        }

        private string result_Voltage;
        public string Result_Voltage
        {
            get
            {
                return result_Voltage;
            }
            set
            {
                SetProperty(ref result_Voltage, value);
            }
        }

        private string result_Voltage2;
        public string Result_Voltage2
        {
            get
            {
                return result_Voltage2;
            }
            set
            {
                SetProperty(ref result_Voltage2, value);
            }
        }

        private string result_Function1;
        public string Result_Function1
        {
            get
            {
                return result_Function1;
            }
            set
            {
                SetProperty(ref result_Function1, value);
            }
        }

        private string result_Function2;
        public string Result_Function2
        {
            get
            {
                return result_Function2;
            }
            set
            {
                SetProperty(ref result_Function2, value);
            }
        }

        private string runstatus;
        public string RunStatus
        {
            get
            {
                return runstatus;
            }
            set
            {
                SetProperty(ref runstatus, value);
            }
        }

        private string operationLog;
        public string OperationLog
        {
            get
            {
                return operationLog;
            }
            set
            {
                SetProperty(ref operationLog, value);
            }
        }

        private string showMessageToTest;
        public string ShowMessageToTest
        {
            get
            {
                return showMessageToTest;
            }
            set
            {
                SetProperty(ref showMessageToTest, value);
            }
        }

        private string messageToTest;
        public string MessageToTest
        {
            get
            {
                return messageToTest;
            }
            set
            {
                SetProperty(ref messageToTest, value);
            }
        }

        private string currentModel;
        public string CurrentModel
        {
            get
            {
                return currentModel;
            }
            set
            {
                SetProperty(ref currentModel, value);
            }
        }

        private string workNum;
        public string WorkNum
        {
            get
            {
                return workNum;
            }
            set
            {
                SetProperty(ref workNum, value);
            }
        }

        private int voltage_Num;
        public int Voltage_Num
        {
            get
            {
                return voltage_Num;
            }
            set
            {
                SetProperty(ref voltage_Num, value);
            }
        }

        private int voltPassNum;
        public int VoltPassNum
        {
            get
            {
                return voltPassNum;
            }
            set
            {
                SetProperty(ref voltPassNum, value);
            }
        }

        private int function_Num;
        public int Function_Num
        {
            get
            {
                return function_Num;
            }
            set
            {
                SetProperty(ref function_Num, value);
            }
        }

        private int funcPassNum;
        public int FuncPassNum
        {
            get
            {
                return funcPassNum;
            }
            set
            {
                SetProperty(ref funcPassNum, value);
            }
        }

        private string currentMode;
        public string CurrentMode
        {
            get
            {
                return currentMode;
            }
            set
            {
                SetProperty(ref currentMode, value);
            }
        }

        private string loadMboardid;
        public string LoadMBoardID
        {
            get
            {
                return loadMboardid;
            }
            set
            {
                SetProperty(ref loadMboardid, value);
            }
        }

        private string loadMinfo;
        public string LoadMInfo
        {
            get
            {
                return loadMinfo;
            }
            set
            {
                SetProperty(ref loadMinfo, value);
            }
        }

        private string loadmstatus;
        public string LoadMStatus
        {
            get
            {
                return loadmstatus;
            }
            set
            {
                SetProperty(ref loadmstatus, value);
            }
        }


        private string elecTestboardid;
        public string ElecTestBoardID
        {
            get
            {
                return elecTestboardid;
            }
            set
            {
                SetProperty(ref elecTestboardid, value);
            }
        }

        private string electestinfo;
        public string ElecTestInfo
        {
            get
            {
                return electestinfo;
            }
            set
            {
                SetProperty(ref electestinfo, value);
            }
        }

        private string electeststatus;
        public string ElecTestStatus
        {
            get
            {
                return electeststatus;
            }
            set
            {
                SetProperty(ref electeststatus, value);
            }
        }


        #endregion

        #region commonProperty
        private const string WaitTest = "22222222";
        private int ReConnect;
        private string message;//原始测试结果
        private volatile bool IsAppRunning, InATE1, InATE2, InATE3, RecevieATE1, RecevieATE2, IsReadID1, IsReadID2, IsReadID3, IsReadID4,IsReadID5, ClearProduct;
        private volatile bool BeginSend, testModelError, TestResultSendComfirm, IsOperatePLCTaskAlive, IsOperate8735TaskAlive;
        private volatile bool[] OperationBit = new bool[16];
        private volatile bool[] OPPLC = new bool[48];
        private Socket serverSocket,PLC_client;
        private SerialPort serialPort2;
        private SqlConnection connect = null;
        private SqlCommand command;
        private FIDCard ReadID0, ReadID1, ReadID2, ReadID3, ReadID6, ReadID7;
        private List<byte> buffer = new List<byte>(1024);
        AppendRunningRecord appendRecord = new AppendRunningRecord();
        AppendTestRecord appendTestRecord = new AppendTestRecord();
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private List<string> BlackList;
        List<string> ResultList = new List<string>();
        private byte[] data = new byte[1024];
        private byte[] PLCData = new byte[1024];
        private BoardData Board1, Board2, BoardTest1, BoardTest2, BoardTest3, BoardTest4;
        private TestConfig TestConfig = new TestConfig();
        #endregion

        #region command
        private DelegateCommand singlechannelcommand;
        public DelegateCommand SingleChannelCommand =>
            singlechannelcommand ?? (singlechannelcommand = new DelegateCommand(ExecuteSingleChannelCommand));

        void ExecuteSingleChannelCommand()
        {
            if (MessageBox.Show("是否切换到单路模式？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SingleChannel = "1";
                MultiChannel = "0";
                CurrentMode = "单路模式";
            }
        }

        private DelegateCommand multichannelcommand;
        public DelegateCommand MultiChannelCommand =>
            multichannelcommand ?? (multichannelcommand = new DelegateCommand(ExecuteMultiChannelCommand));

        void ExecuteMultiChannelCommand()
        {
            if (MessageBox.Show("是否切换到双路模式？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                SingleChannel = "0";
                MultiChannel = "1";
                CurrentMode = "双路模式";
            }
        }

        private DelegateCommand startcommand;
        public DelegateCommand StartCommand =>
            startcommand ?? (startcommand = new DelegateCommand(ExecuteStartCommand));

        void ExecuteStartCommand()
        {
            if (IsAppRunning)
            {
                MessageBox.Show("程序已运行！");
                return;
            }
            ReConnect = 10;
            IsAppRunning = true;
            Share.ShareModel.AppRunning = true;
            LoadConfig();
            LoadProductData();
            ClearCache();
        }


        private DelegateCommand stopcommand;
        public DelegateCommand StopCommand =>
            stopcommand ?? (stopcommand = new DelegateCommand(ExecuteStopCommand));

        void ExecuteStopCommand()
        {
            if (Share.ShareModel.IsEndApplication)
            {
                MessageBox.Show("程序已停止！");
                return;
            }
            Share.ShareModel.IsEndApplication = true;
            Share.ShareModel.AppRunning = false;
            IsAppRunning = false;
            SaveProductData();
        }

        private DelegateCommand confirmNGCommand;
        public DelegateCommand ConfirmNGCommand =>
            confirmNGCommand ?? (confirmNGCommand = new DelegateCommand(ExecuteConfirmNGCommand));

        void ExecuteConfirmNGCommand()
        {
            //R271--->上位机治具ng取走确认标志OPPLC[33]
            OPPLC[33] = true;
            LoadMStatus = "Normal";
        }
        #endregion



        #region Function
        /// <summary>
        /// 初始化需要使用到的类
        /// </summary>
        private void InitialClass()
        {
            serialPort2 = new SerialPort();
            ReadID0 = new FIDCard(1, Share.ShareModel.connectionConfig.FIDPort);
            ReadID1 = new FIDCard(2, Share.ShareModel.connectionConfig.FIDPort);
            ReadID2 = new FIDCard(3, Share.ShareModel.connectionConfig.FIDPort);
            ReadID3 = new FIDCard(4, Share.ShareModel.connectionConfig.FIDPort);
            ReadID6 = new FIDCard(7, Share.ShareModel.connectionConfig.FIDPort);
            ReadID7 = new FIDCard(8, Share.ShareModel.connectionConfig.FIDPort);
            Board1 = new BoardData();
            Board2 = new BoardData();
            BoardTest1 = new BoardData();
            BoardTest2 = new BoardData();
            BoardTest3 = new BoardData();
            BoardTest4 = new BoardData();
        }
        /// <summary>
        /// 加载生产数据
        /// </summary>
        private void LoadProductData()
        {
            try
            {
                JObject jobj = new JObject();
                using (StreamReader stm = new StreamReader($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}ProductData.json"))
                {
                    using (JsonTextReader jsontext = new JsonTextReader(stm))
                    {
                        jobj = (JObject)JToken.ReadFrom(jsontext);
                    }
                }
                if (!jobj.ContainsKey("ProductData"))
                {
                    throw new Exception("不支持此文件，请使用正确的配置文件配置参数");
                }
                Share.ShareModel.productData = jobj["ProductData"]?.ToObject<ProductData>();
                CurrentModel = Share.ShareModel.productData.CurrentModel;
                WorkNum = Share.ShareModel.productData.WorkNum;
                Voltage_Num = Share.ShareModel.productData.Voltage_Num;
                VoltPassNum = Share.ShareModel.productData.VoltPassNum;
                Function_Num = Share.ShareModel.productData.Function_Num;
                FuncPassNum = Share.ShareModel.productData.FuncPassNum;
                BlackList = Share.ShareModel.productData.BlackList ?? new List<string>(); 
            }
            catch (Exception err)
            {
                MessageBox.Show($"{err.Message}；加载生产数据失败，请检查生产数据文件是否存在");
            }
        }
        /// <summary>
        /// 保存生产数据
        /// </summary>
        private void SaveProductData()
        {
            try
            {
                JObject jobj = new JObject();
                string path = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}ProductData.json";
                Share.ShareModel.productData.WorkNum = WorkNum;
                Share.ShareModel.productData.CurrentModel = CurrentModel;
                Share.ShareModel.productData.Voltage_Num = Voltage_Num;
                Share.ShareModel.productData.VoltPassNum = VoltPassNum;
                Share.ShareModel.productData.Function_Num = Function_Num;
                Share.ShareModel.productData.FuncPassNum = FuncPassNum;
                Share.ShareModel.productData.BlackList = BlackList;

                if (jobj.ContainsKey("ProductData"))
                {
                    jobj["ProductData"] = JObject.Parse(JsonConvert.SerializeObject(Share.ShareModel.productData));
                }
                else
                {
                    jobj.Add("ProductData", JObject.Parse(JsonConvert.SerializeObject(Share.ShareModel.productData)));
                }
                using (StreamWriter wirte = new StreamWriter(path, false))
                {
                    wirte.Write(jobj);
                    wirte.Flush();
                }

            }
            catch (Exception err)
            {
                System.Windows.MessageBox.Show(err.Message, "生产数据保存失败");
            }
        }
        /// <summary>
        /// 清理缓存
        /// </summary>
        private void ClearCache()
        {
            Task.Run(async () =>
            {
                //var cancellationTokenSource = new CancellationTokenSource();
                //var cancellationToken = cancellationTokenSource.Token;
                //IsCancelTask = false;
                while (IsAppRunning)
                {
                    //if (IsCancelTask) cancellationTokenSource.Cancel();
                    //cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(TimeSpan.FromMinutes(6));
                    appendTestRecord.ClearString();
                    TestRecord("自动清理缓存信息");
                    appendRecord.ClearString();
                    RunningRecord("自动清理缓存信息");
                }
            });
        }

        /// <summary>
        /// 加载本地配置
        /// </summary>
        public void LoadConfig()
        {
            try
            {
                JObject jobj = new JObject();
                using (StreamReader stm = new StreamReader($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}{"TESTCONFIG".Trim()}.json"))
                {
                    using (JsonTextReader jsontext = new JsonTextReader(stm))
                    {
                        jobj = (JObject)JToken.ReadFrom(jsontext);
                    }
                }
                if (!jobj.ContainsKey("ConnectionConfig"))
                {
                    throw new Exception("配置文件损坏，请到管理员设置检查配置是否正确！");
                }
                ConnectionConfig connectAddress = jobj["ConnectionConfig"]?.ToObject<ConnectionConfig>();
                Share.ShareModel.connectionConfig = connectAddress;

            }
            catch (Exception err)
            {
                MessageBox.Show($"{err.Message}；加载配置失败，请手动加载通讯配置");
            }
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        private void SqlConnect()
        {
            string connectStr = $"Data Source={Share.ShareModel.connectionConfig.DataBaseString};Initial Catalog=MySQL;User ID = sa ;Password = qaz123.;MultipleActiveResultSets=true";
            try
            {
                connect = new SqlConnection(connectStr);
                connect.Open();
                RunningRecord("数据库连接成功！");

            }
            catch (Exception err)
            {
                MessageBox.Show("数据库连接失败" + err.Message);
            }
        }

        /// <summary>
        /// 创建TCP连接
        /// </summary>
        /// 
        private void ConnectToClient()
        {
            Task.Run(() =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(Share.ShareModel.connectionConfig.IPAddr))
                    {
                        string ip = Share.ShareModel.connectionConfig.IPAddr;
                        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPAddress ipAddress = IPAddress.Parse(ip);//"169.254.145.213"
                        EndPoint ipEndPoint = new IPEndPoint(ipAddress, 7878);
                        serverSocket.Bind(ipEndPoint);
                        serverSocket.Listen(2);
                        while (IsAppRunning)
                        {
                            allDone.Reset();
                            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), serverSocket);
                            allDone.WaitOne();
                        }
                    }
                    else
                    {
                        MessageBox.Show("本机服务器IP未设置，请到管理员设置里设置！");
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            });
        }
        private void AcceptCallBack(IAsyncResult result)
        {
            allDone.Set();
            Socket serverSocket = result.AsyncState as Socket;
            if (serverSocket != null && IsAppRunning)
            {
                Socket clientSocket = serverSocket.EndAccept(result);
                if (clientSocket != null)
                {
                    clientSocket.BeginReceive(data, 0, 1024, SocketFlags.None, new AsyncCallback(ReceiveCallBack), clientSocket);
                }
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), serverSocket);
            }
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            Socket clientSocket = null;
            try
            {
                clientSocket = result.AsyncState as Socket;
                int length = clientSocket.EndReceive(result);
                string clientMessage = Encoding.UTF8.GetString(data, 0, length).Trim();
                string[] eachpath = clientMessage.Split('_');
                if (clientMessage.Equals("QUERY_STATE_ATE1") || clientMessage.Equals("QUERY_STATE_ATE2"))
                {
                    if (InATE1 && !string.IsNullOrEmpty(BoardTest1.IDstring) && !string.IsNullOrEmpty(BoardTest2.IDstring) && WorkNum != null && !RecevieATE1)
                    {
                        if (MultiChannel == "1")
                        {
                            MessageToTest = $"STATE_ATE1_OK_1;{BoardTest1.IDstring};{BoardTest2.IDstring};{CurrentModel};8;{WorkNum}";

                            Send(clientSocket, MessageToTest);
                        }
                        else
                        {
                            MessageToTest = $"STATE_ATE1_OK_1;{BoardTest1.IDstring};{BoardTest2.IDstring};{CurrentModel};16;{WorkNum}";
                            Send(clientSocket, MessageToTest);
                        }

                        TestRecord("正在发送测试信号给耐压测试机，等待测试");
                    }

                    if (InATE2 && !string.IsNullOrEmpty(BoardTest3.IDstring) && !string.IsNullOrEmpty(BoardTest4.IDstring) && WorkNum != null && !RecevieATE2)
                    {
                        if (MultiChannel == "1")
                        {
                            MessageToTest = $"STATE_ATE2_OK_1;{BoardTest3.IDstring};{BoardTest4.IDstring};{CurrentModel};8;{WorkNum}";
                            Send(clientSocket, MessageToTest);
                        }
                        else
                        {
                            MessageToTest = $"STATE_ATE2_OK_1;{BoardTest3.IDstring};{BoardTest4.IDstring};{CurrentModel};16;{WorkNum}";
                            Send(clientSocket, MessageToTest);
                        }
                        TestRecord("正在发送测试信号给性能测试机，等待测试");
                    }

                    if (InATE3 && !string.IsNullOrEmpty(BoardTest4.IDstring) && ClearProduct && WorkNum != null && !RecevieATE2)
                    {
                        if (MultiChannel == "1")
                        {
                            MessageToTest = $"STATE_ATE2_OK_1;{BoardTest4.IDstring};{CurrentModel};4;{WorkNum}";
                            Send(clientSocket, MessageToTest);
                        }
                        else
                        {
                            MessageToTest = $"STATE_ATE2_OK_1;{BoardTest4.IDstring};{CurrentModel};8;{WorkNum}";
                            Send(clientSocket, MessageToTest);
                        }
                        TestRecord("正在发送测试信号给性能测试机，等待测试");
                        ClearProduct = false;
                    }

                }
                else if (clientMessage == "START_TEST_ATE1")
                {
                    RecevieATE1 = true;
                    InATE1 = false;
                    //正在测试
                    Send(clientSocket, "START_ATE1_OK");
                    TestRecord("耐压测试中");
                    Result_Voltage = "22222222";
                    Result_Voltage2 = "22222222";
                }
                else if (clientMessage == "START_TEST_ATE2")
                {
                    RecevieATE2 = true;
                    InATE2 = false;
                    InATE3 = false;
                    //正在测试
                    Send(clientSocket, "START_ATE2_OK");
                    TestRecord("性能测试中");
                    Result_Function1 = "22222222";
                    Result_Function2 = "22222222";
                }

                else if (eachpath[0] == "END" && eachpath[2] == "ATE1" && eachpath[4].Length == 8)
                {
                    try
                    {
                        TestRecord("耐压测试完成测试结果：" + eachpath[4]);
                        RecevieATE1 = false;
                        message = eachpath[4].Trim();
                        Result_Voltage2 = message.Substring(0, 4);
                        Result_Voltage = message.Substring(4, 4);
                        Send(clientSocket, "END_ATE1_OK");
                        if (Result_Voltage != "2222" && BoardTest1.IDstring.Length == 10)//Res1 == "null" && 
                        {
                            string tosql = "update TestSystem set Result1 = " + "'" + Result_Voltage + "'" + "where BoardID = " + "'" + BoardTest1.IDstring + "'";//更新测试一的值
                            command = new SqlCommand(tosql, connect);
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("耐压测试一结果录入错误！");
                            }
                            VoltTestStatistics(Result_Voltage);
                            BoardTest1.IDstring = "";
                            RunningRecord("耐压测试一完成");
                            Voltage_Num += Result_Voltage.Length;
                            //Send(Bcc("%01#WCSR04201"));
                            //IsReadID2 = false;
                        }
                        else
                        {
                            RunningRecord("未获取到耐压测试一结果");
                            Send(clientSocket, "END_ATE1_NG");
                        }


                        if (Result_Voltage2 != "2222" && BoardTest2.IDstring.Length == 10)//Res1 == "null" && 
                        {
                            string tosql = "update TestSystem set Result1 = " + "'" + Result_Voltage2 + "'" + "where BoardID = " + "'" + BoardTest2.IDstring + "'";//更新测试一的值
                            command = new SqlCommand(tosql, connect);
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("耐压测试二结果录入错误！");
                            }
                            VoltTestStatistics(Result_Voltage2);
                            BoardTest2.IDstring = "";
                            RunningRecord("耐压测试二完成");
                            Voltage_Num += Result_Voltage2.Length;
                            //Send(Bcc("%01#WCSR04201"));
                            OPPLC[16] = true;
                            //Thread.Sleep(50);
                            IsReadID2 = false;
                        }
                        else
                        {
                            RunningRecord("未获取到耐压测试二结果");
                            Send(clientSocket, "END_ATE1_NG");
                        }
                    }
                    catch (Exception)
                    {
                        Send(clientSocket, "END_ATE1_NG");
                        TestRecord("耐压测试结果有误重新获取");
                        //Send(Bcc("%01#WCSR04251"));
                        OPPLC[21] = true;
                    }
                }



                else if (eachpath[0] == "END" && eachpath[2] == "ATE1" && eachpath[4].Length == 16)
                {
                    try
                    {
                        TestRecord("耐压测试完成测试结果：" + eachpath[4]);
                        RecevieATE1 = false;
                        message = eachpath[4].Trim();
                        Result_Voltage2 = message.Substring(0, 8);
                        Result_Voltage = message.Substring(8, 8);
                        Send(clientSocket, "END_ATE1_OK");
                        if (Result_Voltage != "22222222" && BoardTest1.IDstring.Length == 10)//Res1 == "null" && 
                        {
                            string tosql = "update TestSystem set Result1 = " + "'" + Result_Voltage + "'" + "where BoardID = " + "'" + BoardTest1.IDstring + "'";//更新测试一的值
                            command = new SqlCommand(tosql, connect);
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("耐压测试结果录入错误！");
                            }
                            VoltTestStatistics(Result_Voltage);
                            BoardTest1.IDstring = "";
                            RunningRecord("耐压测试完成");
                            Voltage_Num += Result_Voltage.Length;
                            //Send(Bcc("%01#WCSR04201"));
                            //Thread.Sleep(50);
                            //IsReadID2 = false;
                        }
                        else
                        {
                            RunningRecord("未获取到耐压测试一结果");
                            Send(clientSocket, "END_ATE1_NG");
                        }


                        if (Result_Voltage2 != "22222222" && BoardTest2.IDstring.Length == 10)//Res1 == "null" && 
                        {
                            string tosql = "update TestSystem set Result1 = " + "'" + Result_Voltage2 + "'" + "where BoardID = " + "'" + BoardTest2.IDstring + "'";//更新测试一的值
                            command = new SqlCommand(tosql, connect);
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("耐压测试结果录入错误！");
                            }
                            VoltTestStatistics(Result_Voltage2);
                            BoardTest2.IDstring = "";
                            RunningRecord("耐压测试二完成");
                            Voltage_Num += Result_Voltage2.Length;
                            //Send(Bcc("%01#WCSR04201"));
                            OPPLC[16] = true;
                            //Thread.Sleep(50);
                            IsReadID2 = false;
                        }
                        else
                        {
                            RunningRecord("未获取到耐压测试二结果");
                            Send(clientSocket, "END_ATE1_NG");
                        }

                    }
                    catch (Exception)
                    {
                        Send(clientSocket, "END_ATE1_NG");
                        TestRecord("耐压测试结果有误重新获取");
                        //Send(Bcc("%01#WCSR04251"));
                        OPPLC[21] = true;
                    }
                }




                else if (eachpath[0] == "END" && eachpath[2] == "ATE1" && (eachpath[4].Length != 8 || eachpath[4].Length != 16))
                {
                    Send(clientSocket, "END_ATE1_NG");
                    TestRecord("耐压测试完成测试结果有误：" + eachpath[4]);
                }
                else if (eachpath[0] == "END" && eachpath[2] == "ATE2" && eachpath[4].Length == 4)
                {
                    try
                    {
                        TestRecord("性能测试完成测试结果：" + eachpath[4]);
                        RecevieATE2 = false;
                        message = eachpath[4].Trim();
                        Result_Function2 = message;
                        Send(clientSocket, "END_ATE2_OK");
                        if (Result_Function2 != "22222222" && BoardTest4.IDstring != "" && BoardTest4.IDstring.Length == 10)// Res2 == "null" && 
                        {
                            string tosql = "update TestSystem set Result2 = " + "'" + Result_Function2 + "'" + "where BoardID = " + "'" + BoardTest4.IDstring + "'";
                            command = new SqlCommand(tosql, connect);//更新测试二的值
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("性能（1）测试结果录入错误！");
                            }
                            BoardTest4.IDstring = "";
                            FuncTestStatistics(Result_Function2);
                            RunningRecord("性能测试完成");
                            Function_Num += 4;
                            //Send(Bcc("%01#WCSR04211"));
                            OPPLC[17] = true;
                            IsReadID4 = false;
                        }
                        else
                        {
                            RunningRecord("未获取到性能（1）测试结果");
                            Send(clientSocket, "END_ATE2_NG");
                        }
                    }
                    catch (Exception)
                    {
                        RunningRecord("性能测试（1）结果有误重新获取");
                        Send(clientSocket, "END_ATE2_NG");
                        //Send(Bcc("%01#WCSR04251"));
                        OPPLC[21] = true;
                    }

                }
                else if (eachpath[0] == "END" && eachpath[2] == "ATE2" && eachpath[4].Length == 8)
                {
                    try
                    {
                        TestRecord("性能测试完成测试结果：" + eachpath[4]);
                        RecevieATE2 = false;
                        message = eachpath[4].Trim();
                        Result_Function2 = message;
                        Send(clientSocket, "END_ATE2_OK");
                        if (MultiChannel == "0")
                        {
                            if (Result_Function2 != "22222222" && BoardTest4.IDstring != "" && BoardTest4.IDstring.Length == 10)// Res2 == "null" && 
                            {
                                string tosql = "update TestSystem set Result2 = " + "'" + Result_Function2 + "'" + "where BoardID = " + "'" + BoardTest4.IDstring + "'";
                                command = new SqlCommand(tosql, connect);//更新测试二的值
                                if (command.ExecuteNonQuery() == -1)
                                {
                                    MessageBox.Show("性能（1）测试结果录入错误！");
                                }
                                FuncTestStatistics(Result_Function2);
                                BoardTest4.IDstring = "";
                                RunningRecord("性能测试完成");
                                Function_Num += 8;
                                //Send(Bcc("%01#WCSR04211"));
                                OPPLC[17] = true;
                                IsReadID4 = false;
                            }
                            else
                            {
                                MessageBox.Show("未获取到性能（1）测试结果");
                                Send(clientSocket, "END_ATE2_NG");
                            }
                        }
                        else
                        {
                            Result_Function2 = message.Substring(0, 4);
                            Result_Function1 = message.Substring(4, 4);
                            if (Result_Function2 != "2222" && BoardTest4.IDstring != "" && BoardTest4.IDstring.Length == 10)//Res2 == "null" && 
                            {
                                string tosql = "update TestSystem set Result2 = " + "'" + Result_Function2 + "'" + "where BoardID = " + "'" + BoardTest4.IDstring + "'";
                                command = new SqlCommand(tosql, connect);//更新性能测试的值
                                if (command.ExecuteNonQuery() == -1)
                                {
                                    MessageBox.Show("性能（1）测试结果录入错误！");
                                }
                                FuncTestStatistics(Result_Function2);
                                RunningRecord("性能测试(1)完成");
                                Function_Num += 4;
                                BoardTest4.IDstring = "";
                            }
                            else
                            {
                                RunningRecord("未获取到性能（1）测试结果");
                            }


                            if (Result_Function1 != "2222" && BoardTest3.IDstring != "" && BoardTest3.IDstring.Length == 10)//Res2_2 == "null" && 
                            {
                                string tosql = "update TestSystem set Result2 = " + "'" + Result_Function1 + "'" + "where BoardID = " + "'" + BoardTest3.IDstring + "'";
                                command = new SqlCommand(tosql, connect);//更新性能测试的值
                                if (command.ExecuteNonQuery() == -1)
                                {
                                    MessageBox.Show("性能测试（2）结果录入错误！");
                                }
                                FuncTestStatistics(Result_Function1);
                                RunningRecord("性能测试(2)完成");
                                Function_Num += 4;
                                BoardTest3.IDstring = "";
                                Send(clientSocket, "END_ATE2_OK");
                                //Send(Bcc("%01#WCSR04211"));
                                OPPLC[17] = true;
                                IsReadID3 = false;
                                IsReadID4 = false;
                            }
                            else
                            {
                                RunningRecord("未获取到性能（2）测试结果");
                                Send(clientSocket, "END_ATE2_NG");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        RunningRecord("性能测试（1）结果有误重新获取");
                        Send(clientSocket, "END_ATE2_NG");
                        //Send(Bcc("%01#WCSR04251"));
                        OPPLC[21] = true;
                    }

                }
                else if (eachpath[0] == "END" && eachpath[2] == "ATE2" && eachpath[4].Length == 16)
                {
                    try
                    {
                        TestRecord("性能测试完成测试结果：" + eachpath[4]);
                        RecevieATE2 = false;
                        message = eachpath[4].Trim();
                        Result_Function2 = message.Substring(0, 8);
                        Result_Function1 = message.Substring(8, 8);
                        Send(clientSocket, "END_ATE2_OK");
                        if (Result_Function2 != "22222222" && BoardTest4.IDstring != "" && BoardTest4.IDstring.Length == 10)//Res2 == "null" && 
                        {
                            string tosql = "update TestSystem set Result2 = " + "'" + Result_Function2 + "'" + "where BoardID = " + "'" + BoardTest4.IDstring + "'";
                            command = new SqlCommand(tosql, connect);//更新性能测试的值
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("性能（1）测试结果录入错误！");
                            }
                            FuncTestStatistics(Result_Function2);
                            RunningRecord("性能测试(1)完成");
                            Function_Num += 8;
                            BoardTest4.IDstring = "";
                        }
                        else
                        {
                            RunningRecord("未获取到性能（1）测试结果");
                        }


                        if (Result_Function1 != "22222222" && BoardTest3.IDstring != "" && BoardTest3.IDstring.Length == 10)//Res2_2 == "null" && 
                        {
                            string tosql = "update TestSystem set Result2 = " + "'" + Result_Function1 + "'" + "where BoardID = " + "'" + BoardTest3.IDstring + "'";
                            command = new SqlCommand(tosql, connect);//更新性能测试的值
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("性能测试（2）结果录入错误！");
                            }
                            FuncTestStatistics(Result_Function1);
                            RunningRecord("性能测试(2)完成");
                            Function_Num += 8;
                            BoardTest3.IDstring = "";
                            //Send(Bcc("%01#WCSR04211"));
                            OPPLC[17] = true;
                            IsReadID3 = false;
                            IsReadID4 = false;
                        }
                        else
                        {
                            RunningRecord("未获取到性能（2）测试结果");
                            Send(clientSocket, "END_ATE2_NG");
                        }
                    }
                    catch (Exception err)
                    {
                        RunningRecord("性能测试结果有误重新获取" + err.ToString());
                        Send(clientSocket, "END_ATE2_NG");
                        //Send(Bcc("%01#WCSR04251"));
                        OPPLC[21] = true;
                    }

                }
                if (length == 0)
                {
                    clientSocket.Close();
                    return;
                }
                clientSocket.BeginReceive(data, 0, 1024, SocketFlags.None, new AsyncCallback(ReceiveCallBack), clientSocket);
            }
            catch (Exception e)
            {
                MessageBox.Show("错误" + e);
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }
            }

        }

        private void Send(Socket handler, string sendmessage)
        {
            byte[] messagedata = Encoding.ASCII.GetBytes(sendmessage);
            handler.BeginSend(messagedata, 0, messagedata.Length, 0, new AsyncCallback(SendCallBack), handler);
        }

        private void SendCallBack(IAsyncResult result)
        {
            try
            {
                Socket handler = (Socket)result.AsyncState;
                int len = handler.EndSend(result);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
        }



        /// <summary>
        /// 启动PLC连接
        /// </summary>
        private void ConnectToPLCThread()
        {
            Task.Run(() =>
            {
                try
                {
                    if (IsAppRunning)
                    {
                        PLC_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        PLC_client.BeginConnect(new IPEndPoint(IPAddress.Parse(Share.ShareModel.connectionConfig.PLCAddr), 9094), new AsyncCallback(ConnectToServer), PLC_client);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            });
        }


        private void ConnectToServer(IAsyncResult result)
        {
            Socket client_ = (Socket)result.AsyncState;
            if (!PLC_client.Connected)
            {
                ReConnect--;
                RunningRecord("PLC未启动，请启动PLC，正在尝试重连中...");
                if (ReConnect != 0)
                {
                    ConnectToPLCThread();
                }
                else
                {
                    RunningRecord("多次重连PLC未响应，连接已中止，请重启应用");
                }
            }
            else
            {
                try
                {
                    Thread.Sleep(1000);
                    RunningRecord("PLC端通讯成功");
                    ReConnect = 10;
                    CheckPLC();
                    CheckThreadAlive();
                    client_.EndConnect(result);
                    client_.BeginReceive(PLCData, 0, PLCData.Length, SocketFlags.None, new AsyncCallback(PLCReceiveCallBack), client_);
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
            }

        }

        private void PLCReceiveCallBack(IAsyncResult result)
        {
            if ((Socket)result.AsyncState != null)
            {
                Socket clientSocket = (Socket)result.AsyncState;
                try
                {
                    int ReadMessage = clientSocket.EndReceive(result);
                    if (ReadMessage == 0)
                    {
                        return;
                    }
                    string PLCMessage = Encoding.UTF8.GetString(PLCData, 0, ReadMessage);
                    if (PLCMessage.Length == 13 && PLCMessage.Contains("%01$RD"))
                    {
                        PLCMessage = PLCMessage.Substring(6, 4);
                        int temp;
                        if (int.TryParse(PLCMessage, System.Globalization.NumberStyles.AllowHexSpecifier, null, out temp))
                        {
                            if (Convert.ToInt32(PLCMessage, 16) <= 65535)
                            {
                                PLCMessage = PLCMessage.Substring(2, 2) + PLCMessage.Substring(0, 2);
                                PLCMessage = (Convert.ToString(Convert.ToInt32(PLCMessage, 16), 2)).PadLeft(16, '0');
                                char[] PLCchar = PLCMessage.ToCharArray();
                                if (PLCchar.Length == 16)
                                {
                                    for (int count = 0; count < PLCchar.Length; count++)
                                    {
                                        OperationBit[count] = PLCchar[count] == '1' ? true : false;
                                    }
                                }
                            }
                        }
                    }
                    else if (PLCMessage.Length == 10 && PLCMessage.Contains("%01$RC"))
                    {
                        if (PLCMessage.Contains("%01$RC1"))
                        {
                            TestResultSendComfirm = true;//%01#RCSR5067
                        }
                    }
                    //Array.Clear(PLCData, 0, PLCData.Length);
                    if (PLC_client.Connected)
                    {
                        clientSocket.BeginReceive(PLCData, 0, PLCData.Length, SocketFlags.None, new AsyncCallback(PLCReceiveCallBack), clientSocket);
                    }
                }
                catch (Exception)
                {
                    RunningRecord("连接中断，PLC掉线，正在尝试重连");
                    if (IsAppRunning)
                    {
                        ConnectToPLCThread();
                    }
                }
            }
        }

        private void RunResponsePLC()
        {
            Task.Factory.StartNew(async () =>
            {
                while (IsAppRunning)
                {
                    await Task.Delay(100);
                    await ResponsePLC();
                }
            },TaskCreationOptions.LongRunning);
        }

        private async Task ResponsePLC()
        {
            try
            {
                if (OperationBit[14] && !IsReadID5)
                {
                    //上电位读卡
                    IsReadID5 = true;
                    ReadID7.OpenFID();
                    Board2.IDstring = await ReadID7.GetCardNum();
                    ReadID7.Close();
                    if (!string.IsNullOrEmpty(Board2.IDstring))
                    {
                        await GetIDAddress(ReadID7.ID, Board2.IDstring);
                    }
                    else
                    {
                        IsReadID5 = false;
                        RunningRecord($"上电位读卡失败请检查ID卡并重试,错误原因：{ReadID7.outErrMessage}");
                    }
                }
                if (OperationBit[13] && !IsReadID1)
                {
                    //上料位读卡器
                    IsReadID1 = true;
                    ReadID0.OpenFID();
                    Board1.IDstring = await ReadID0.GetCardNum();
                    ReadID0.Close();
                    if (!string.IsNullOrEmpty(Board1.IDstring))
                    {
                        await GetIDAddress(ReadID0.ID, Board1.IDstring);
                    }
                    else
                    {
                        IsReadID1 = false;
                        RunningRecord($"上料位读卡失败请检查ID卡并重试,错误原因：{ReadID0.outErrMessage}");
                    }
                }
                if (OperationBit[12] && !IsReadID2)
                {
                    //耐压测试读卡器
                    IsReadID2 = true;
                    ReadID1.OpenFID();
                    BoardTest1.IDstring = await ReadID1.GetCardNum();
                    ReadID1.Close();

                    await Task.Delay(100);

                    ReadID6.OpenFID();
                    BoardTest2.IDstring = await ReadID6.GetCardNum();
                    ReadID6.Close();


                    if (!string.IsNullOrEmpty(BoardTest1.IDstring) && !string.IsNullOrEmpty(BoardTest2.IDstring))
                    {
                        await GetIDAddress(ReadID1.ID, BoardTest1.IDstring);
                        await Task.Delay(100);
                        await GetIDAddress(ReadID6.ID, BoardTest2.IDstring);
                    }
                    else
                    {
                        IsReadID2 = false;
                        if (string.IsNullOrEmpty(BoardTest1.IDstring))
                        {
                            RunningRecord($"耐压测试一读卡失败请检查ID卡并重试,错误原因：{ReadID1.outErrMessage}");
                        }
                        if (string.IsNullOrEmpty(BoardTest2.IDstring))
                        {
                            RunningRecord($"耐压测试二读卡失败请检查ID卡并重试,错误原因：{ReadID6.outErrMessage}");
                        }
                    }
                }
                if (OperationBit[11] && !IsReadID3)
                {
                    //性能测试读卡器一
                    IsReadID3 = true;
                    ReadID2.OpenFID();
                    BoardTest3.IDstring = await ReadID2.GetCardNum();
                    ReadID2.Close();
                    if (!string.IsNullOrEmpty(BoardTest3.IDstring))
                    {
                        await GetIDAddress(ReadID2.ID, BoardTest3.IDstring);
                    }
                    else
                    {
                        IsReadID3 = false;
                        RunningRecord($"性能测试一位置读卡失败请检查ID卡并重试,错误原因：{ReadID2.outErrMessage}");
                    }
                }
                if (OperationBit[10] && !IsReadID4)
                {
                    //性能测试读卡器二
                    IsReadID4 = true;
                    ReadID3.OpenFID();
                    BoardTest4.IDstring = await ReadID3.GetCardNum();
                    ReadID3.Close();
                    if (!string.IsNullOrEmpty(BoardTest4.IDstring))
                    {
                        await GetIDAddress(ReadID3.ID, BoardTest4.IDstring);
                    }
                    else
                    {
                        IsReadID4 = false;
                        RunningRecord($"性能测试二位置读卡失败请检查ID卡并重试,错误原因：{ReadID3.outErrMessage}");
                    }
                }
                if (OperationBit[7])
                {
                    //清料
                    ClearProduct = true;
                    //Send(Bcc("%01#WCSR04080"));
                    OPPLC[8] = true;
                }
                if (OperationBit[5])
                {
                    //Send(Bcc("%01#WCSR040A0"));
                    OPPLC[10] = true;
                    InATE1 = true;
                }
                if (OperationBit[4])
                {
                    //Send(Bcc("%01#WCSR040B0"));
                    OPPLC[11] = true;
                    InATE2 = true;
                    InATE3 = true;
                }
                if (OperationBit[3])
                {
                    IsReadID1 = false;
                    IsReadID2 = false;
                    IsReadID3 = false;
                    IsReadID4 = false;
                    IsReadID5 = false;
                    //Send(Bcc("%01#WCSR040C0"));
                    OPPLC[12] = true;
                }
                if (OperationBit[1] && MultiChannel == "1" && !testModelError)
                {
                    testModelError = true;
                    MessageBox.Show("当前模式为双路测试模式,下料机模式为单路测试模式,请保持一致!", "警告！", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
                else if (!OperationBit[1] && MultiChannel == "0" && !testModelError)
                {
                    testModelError = true;
                    MessageBox.Show("当前模式为单路测试模式,下料机模式为双路测试模式,请保持一致!", "警告！", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                }
                else if (OperationBit[1] && MultiChannel == "0" && testModelError)
                {
                    testModelError = false;
                }
                else if (!OperationBit[1] && MultiChannel == "1" && testModelError)
                {
                    testModelError = false;
                }
            }
            catch (Exception err)
            {
                IsReadID1 = false;
                IsReadID2 = false;
                IsReadID3 = false;
                IsReadID4 = false;
                IsReadID5 = false;
                RunningRecord(err.Message);
            }
        }

        private void Send(string message1)
        {
            lock (PLC_client)
            {
                byte[] messagedata = Encoding.ASCII.GetBytes(message1);
                PLC_client.BeginSend(messagedata, 0, messagedata.Length, SocketFlags.None, new AsyncCallback(PLCSendCallBack), PLC_client);
            }
        }

        private void PLCSendCallBack(IAsyncResult result)
        {
            try
            {
                Socket handler = (Socket)result.AsyncState;
                int len = handler.EndSend(result);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void CheckPLC()
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (!BeginSend)
                    {
                        BeginSend = true;
                        IsOperatePLCTaskAlive = true;
                        eventAggregator.GetEvent<CurrentUserEvent>().Publish("CheckTaskStart");
                        string command1 = Bcc("%01#RDD0000000000");
                        while (PLC_client.Connected)
                        {
                            Send(command1);
                            await Task.Delay(100);
                            for (int i = 0; i < OPPLC.Length; i++)
                            {
                                if (OPPLC[i])
                                {
                                    OPPLC[i] = false;
                                    if (i < 16)
                                    {
                                        Send(Bcc($"%01#WCSR040{i:X1}0"));
                                    }
                                    else if (i >= 16 && i < 32)
                                    {
                                        Send(Bcc($"%01#WCSR042{i - 16:X1}1"));
                                    }
                                    else if (i >= 32)
                                    {
                                        Send(Bcc($"%01#WCSR027{i - 32:X1}1"));
                                    }
                                    await Task.Delay(100);
                                }
                            }
                        }
                        BeginSend = false;
                        IsOperatePLCTaskAlive = false;
                    }
                }
                catch (Exception err)
                {
                    RunningRecord(err.Message);
                    IsOperatePLCTaskAlive = false;
                    BeginSend = false;
                }
            },TaskCreationOptions.LongRunning);
        }


        private async Task GetIDAddress(int AddressOfIDCard, string ID_Card_num)
        {

            try
            {
                if (ID_Card_num.Length == 10)
                {
                    if (AddressOfIDCard == 1 && IsReadID1)
                    {
                        RunningRecord($"载具{ID_Card_num} 到上料位");
                        LoadMBoardID = ID_Card_num;
                        //TestResultSendComfirm = false;
                        //await Task.Run(async () =>
                        //{
                        //    while (!TestResultSendComfirm && IsAppRunning)
                        //    {
                        //        Send(Bcc("%01#WCSR04261"));
                        //        await Task.Delay(500);
                        //        Send(Bcc("%01#RCSR0426"));
                        //        await Task.Delay(500);
                        //    }
                        //});
                        OPPLC[22] = true;//读卡完成信号
                        //Send(Bcc("%01#WCSR04020"));
                        OPPLC[2] = true;
                        if (BlackList.Contains(ID_Card_num))
                        {
                            MessageBox.Show("载板已经锁定，请取走维修后，扫码解除！");
                            LoadMInfo = $"载具{ID_Card_num}已锁定";
                            LoadMStatus = "Warning";
                            IsReadID1 = false;
                            return;
                        }

                        if (CurrentModel == null)
                        {
                            MessageBox.Show("未选择测试文件！");
                            IsReadID1 = false;
                            return;
                        }
                        string tosql = "Select count(*) from TestSystem where BoardID = " + "'" + ID_Card_num + "'";
                        command = new SqlCommand(tosql, connect);
                        if ((int)command.ExecuteScalar() == 0)
                        {
                            tosql = "insert into TestSystem(BoardID,ProductCode,Result1,Result2)Values(" + "'" + ID_Card_num + "'" + ",'" + CurrentModel + "'" + ",'null','null')";
                            command = new SqlCommand(tosql, connect);
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("载板信息录入错误！");
                                IsReadID1 = false;
                                return;
                            }
                        }
                        else
                        {
                            tosql = "update TestSystem set ProductCode = " + "'" + CurrentModel + "'" + "where BoardID = " + "'" + ID_Card_num + "'";
                            command = new SqlCommand(tosql, connect);//更新测试文件信息
                            if (command.ExecuteNonQuery() == -1)
                            {
                                MessageBox.Show("更新测试文件失败！");
                                IsReadID1 = false;
                                return;
                            }
                        }
                        await Task.Delay(50);




                        tosql = "Select Result1,Result2,Result3,Result4,Result5,Maintain,FPY from TestRecord where BoardID = " + "'" + ID_Card_num + "'";
                        command = new SqlCommand(tosql, connect);
                        if (command.ExecuteReader().Read())
                        {
                            ResultList.Clear();
                            int index = 128;
                            bool[] ng = new bool[8];
                            string ngstring = "";
                            SqlDataAdapter adapter = new SqlDataAdapter(tosql, connect);
                            DataSet ds = new DataSet();
                            adapter.Fill(ds, "TestRecord");
                            DataTable table = ds.Tables["TestRecord"];
                            int numb = table.Rows.Count;
                            if (numb > 0)
                            {
                                foreach (DataRow each in table.Rows)
                                {
                                    if ((string.IsNullOrEmpty(each[int.Parse(Share.ShareModel.connectionConfig.NGCount) - 1].ToString()) || each[int.Parse(Share.ShareModel.connectionConfig.NGCount) - 1].ToString().Equals("NULL")) && !each[5].Equals("Y") && !each[6].Equals("Y"))
                                    {
                                        //若查询正常
                                        //R270--->上位机治具ok正常作业OPPLC[32]
                                        LoadMInfo = "载具正常";
                                        LoadMStatus = "Normal";
                                        OPPLC[32] = true;
                                    }
                                    else
                                    {
                                        if (each[5].Equals("Y"))
                                        {
                                            LoadMInfo = $"{ID_Card_num}载具铜柱使用超限";
                                            LoadMStatus = "Warning";
                                            adapter.Dispose();
                                            ds.Dispose();
                                            table.Dispose();
                                            IsReadID1 = false;
                                            MessageBox.Show($"{ID_Card_num}载具铜柱使用超限,请立即取走载板维修！");
                                            return;
                                        }
                                        if (each[6].Equals("Y"))
                                        {
                                            LoadMInfo = $"{ID_Card_num}载具直通率报警";
                                            LoadMStatus = "Warning";
                                            adapter.Dispose();
                                            ds.Dispose();
                                            table.Dispose();
                                            IsReadID1 = false;
                                            MessageBox.Show($"{ID_Card_num}直通率报警，请立即排查！");
                                            return;
                                        }
                                        if (!string.IsNullOrEmpty(each[int.Parse(Share.ShareModel.connectionConfig.NGCount) - 1].ToString()))
                                        {
                                            //有同位置不良
                                            //若查询有同位置多次不良
                                            BlackList.Add(ID_Card_num);
                                            //R271--->上位机治具ng取走确认标志OPPLC[33]
                                            for (int add = 0; add < int.Parse(Share.ShareModel.connectionConfig.NGCount); add++)
                                            {
                                                ResultList.Add(each[add].ToString());
                                            }
                                            for (int i = 0; i < 8; i++)
                                            {
                                                for (int j = 0; j < int.Parse(Share.ShareModel.connectionConfig.NGCount); j++)
                                                {
                                                    if ((Convert.ToInt32(ResultList[j], 2) ^ index) - Convert.ToInt32(ResultList[j], 2) != index)
                                                    {
                                                        ng[i] = true;
                                                        break;
                                                    }
                                                }
                                                index = index >> 1;
                                            }
                                            for (int i = 0; i < ng.Length; i++)
                                            {
                                                if (!ng[i])
                                                {
                                                    ngstring = $"{ngstring},{i + 1}";
                                                }
                                            }
                                            LoadMInfo = $"第{ngstring}号位置连续不良";
                                            LoadMStatus = "Warning";
                                            adapter.Dispose();
                                            ds.Dispose();
                                            table.Dispose();
                                            IsReadID1 = false;
                                            return;
                                        }
                                    }
                                }
                            }
                            adapter.Dispose();
                            ds.Dispose();
                            table.Dispose();
                        }
                        else
                        {
                            //若查询正常
                            //R270--->上位机治具ok正常作业OPPLC[32]
                            LoadMInfo = "载具正常";
                            LoadMStatus = "Normal";
                            OPPLC[32] = true;
                        }
                        IsReadID1 = false;
                    }
                    else if (AddressOfIDCard == 8 && IsReadID5)
                    {
                        //上电位
                        RunningRecord($"载具{ID_Card_num} 到上电位");
                        ElecTestBoardID = ID_Card_num;
                        OPPLC[21] = true;//上电位读卡成功信号
                        OPPLC[1] = true;//清除上电位读卡信号

                        //上电测试程序代码...

                        //R273--->上位机上电测试完成标志OPPLC[35]
                        ElecTestInfo = "无空缺位";
                        ElecTestStatus = "Normal";
                        IsReadID5 = false;
                    }
                    else if (AddressOfIDCard == 2 && IsReadID2)
                    {
                        RunningRecord($"载具{ID_Card_num} 到耐压测试位");
                        //TestResultSendComfirm = false;
                        //await Task.Run(async () =>
                        //{
                        //    while (!TestResultSendComfirm && IsAppRunning)
                        //    {
                        //        Send(Bcc("%01#WCSR04271"));
                        //        await Task.Delay(500);
                        //        Send(Bcc("%01#RCSR5062"));
                        //        await Task.Delay(500);
                        //    }
                        //});
                        OPPLC[23] = true;

                        //Send(Bcc("%01#WCSR04030"));
                        OPPLC[3] = true;
                    }
                    else if (AddressOfIDCard == 7 && IsReadID2)
                    {
                        RunningRecord($"载具{ID_Card_num} 到耐压测试二位");
                    }
                    else if (AddressOfIDCard == 3 && IsReadID3)
                    {
                        RunningRecord($"载具{ID_Card_num} 到性能(一)测试位");
                        //TestResultSendComfirm = false;
                        //await Task.Run(async () =>
                        //{
                        //    while (!TestResultSendComfirm && IsAppRunning)
                        //    {
                        //        Send(Bcc("%01#WCSR04281"));
                        //        await Task.Delay(500);
                        //        Send(Bcc("%01#RCSR5063"));
                        //        await Task.Delay(500);
                        //    }
                        //});
                        OPPLC[24] = true;
                        
                        //Send(Bcc("%01#WCSR04040"));
                        OPPLC[4] = true;
                    }
                    else if (AddressOfIDCard == 4 && IsReadID4)
                    {
                        RunningRecord($"载具{ID_Card_num} 到性能（二）测试位");

                        //TestResultSendComfirm = false;
                        //await Task.Run(async () =>
                        //{
                        //    while (!TestResultSendComfirm && IsAppRunning)
                        //    {
                        //        Send(Bcc("%01#WCSR04291"));
                        //        await Task.Delay(500);
                        //        Send(Bcc("%01#RCSR5064"));
                        //        await Task.Delay(500);
                        //    }
                        //});
                        OPPLC[25] = true;
                        
                        //Send(Bcc("%01#WCSR04050"));
                        OPPLC[5] = true;
                    }
                }
            }
            catch (Exception err)
            {
                IsReadID1 = false;
                IsReadID2 = false;
                IsReadID3 = false;
                IsReadID4 = false;
                IsReadID5 = false;
                RunningRecord(err.Message);
            }
        }

        private void UpdateProductInfo()
        {
            string tosql = "update ProductModel set ProductInfo = " + "'" + CurrentModel + "_" + WorkNum + "'" + "where ModelName = 'PN'";
            command = new SqlCommand(tosql, connect);//更新测试二的值
            if (command.ExecuteNonQuery() == -1)
            {
                MessageBox.Show("上传产品信息失败");
            }
        }

        private void VoltTestStatistics(string result)
        {
            var resultlist = result.ToCharArray();
            foreach (var each in resultlist)
            {
                if (each == '1') VoltPassNum += 1;
            }
        }

        private void FuncTestStatistics(string result)
        {
            var resultlist = result.ToCharArray();
            foreach (var each in resultlist)
            {
                if (each == '1') FuncPassNum += 1;
            }
        }

        private string Bcc(string cmd)
        {
            cmd = cmd.Trim();
            byte bcc = 0;
            byte[] cmdArr = Encoding.ASCII.GetBytes(cmd);
            for (int begin = 0; begin < cmdArr.Length; begin++)
            {
                bcc = (byte)(bcc ^ cmdArr[begin]);
            }
            return cmd + bcc.ToString("X2") + "\r";
        }

        /// <summary>
        /// 扫码枪线程
        /// </summary>
        private void HoneyWellThread()
        {
            Task.Run(() =>
            {
                serialPort2.PortName = Share.ShareModel.connectionConfig.HoneyWellPort;
                serialPort2.BaudRate = 9600;
                serialPort2.Parity = Parity.None;
                serialPort2.DataBits = 8;
                serialPort2.StopBits = StopBits.One;
                try
                {
                    if (!serialPort2.IsOpen)
                    {
                        serialPort2.Open();
                        RunningRecord("扫码枪串口通讯打开");
                        serialPort2.DataReceived += new SerialDataReceivedEventHandler(SerialReceive_HoneyWell);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message, "错误");
                }
            });
        }
        private async void SerialReceive_HoneyWell(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] tempmessage = new byte[1024];
            int num = serialPort2.BytesToRead;
            byte[] buf = new byte[num];
            serialPort2.Read(buf, 0, num);
            buffer.AddRange(buf);
            while (true)
            {
                int count = buffer.Count;
                if (buffer[count - 1] != 0X0D)
                {
                    break;
                }
                buffer.CopyTo(0, tempmessage, 0, count);
                buffer.RemoveRange(0, count);
                string HoneyWellMessage = Encoding.UTF8.GetString(tempmessage, 0, count);
                if (HoneyWellMessage.Contains("%"))
                {
                    if (MessageBox.Show("是否更新测试文件", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        ClearProductData();
                        string[] all = HoneyWellMessage.Split('%');
                        CurrentModel = all[0];
                        WorkNum = all[1];

                        try
                        {
                            JObject jobj = new JObject();
                            JObject jobj1 = new JObject();
                            string path = $"D:\\ACDC自动测试线配置\\{CurrentModel}.json";
                            if (File.Exists(path))
                            {
                                using (StreamReader stm = new StreamReader(path))
                                {
                                    using (JsonTextReader jsontext = new JsonTextReader(stm))
                                    {
                                        jobj1 = (JObject)JToken.ReadFrom(jsontext);
                                    }
                                }
                                TestConfig tConfig = jobj1[CurrentModel]?.ToObject<TestConfig>();
                                TestConfig = tConfig;
                            }
                            else
                            {
                                using (StreamReader stm = new StreamReader($"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}{"TESTCONFIG".Trim()}.json"))
                                {
                                    using (JsonTextReader jsontext = new JsonTextReader(stm))
                                    {
                                        jobj = (JObject)JToken.ReadFrom(jsontext);
                                    }
                                }
                                TestConfig tConfig = jobj["DefaultTestConfig"]?.ToObject<TestConfig>();
                                TestConfig = tConfig;
                            }

                        }
                        catch (Exception err)
                        {
                            MessageBox.Show($"{err.Message}；加载测试配置失败！");
                        }

                        //Send(Bcc("%01#WCSR04231"));
                        OPPLC[19] = true;
                        UpdateProductInfo();
                    }
                    else if (MessageBox.Show("是否更新测试文件", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    {
                        break;
                    }
                }
                else if (HoneyWellMessage.Contains("Admin"))
                {
                    if (string.IsNullOrEmpty(Board1.IDstring))
                    {
                        break;
                    }
                    if (!BlackList.Contains(Board1.IDstring))
                    {
                        RunningRecord($"载板{Board1.IDstring}未在黑名单无需解锁！");
                        break;
                    }
                    if (MessageBox.Show("是否解锁载板", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        //清除list中载板信息
                        //set R270 正常工作
                        BlackList.Remove(Board1.IDstring);
                        OPPLC[32] = true;
                    }
                    else if (MessageBox.Show("是否解锁载板", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                    {
                        break;
                    }
                }
                break;
            }
        }

        private void ClearProductData()
        {
            VoltPassNum = 0;
            FuncPassNum = 0;
            Voltage_Num = 0;
            Function_Num = 0;
        }

        private void RunningRecord(string message)
        {
            OperationLog = appendRecord.AppendString(message);
        }

        private void TestRecord(string message)
        {
            ShowMessageToTest = appendTestRecord.AppendString(message);
        }


        private void CheckThreadAlive()
        {
            Task.Factory.StartNew(async () =>
            {
                while (IsAppRunning)
                {
                    if (!IsOperatePLCTaskAlive)
                    {
                        eventAggregator.GetEvent<CurrentUserEvent>().Publish("CheckTaskClose");
                        CheckPLC();
                    }
                    if (!IsOperate8735TaskAlive)
                    {
                        OP8735();
                    }
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void OP8735()
        {
            Task.Factory.StartNew(async () =>
            {
                IsOperate8735TaskAlive = true;
                try
                {
                    while (IsAppRunning)
                    {
                        if (LoadMStatus.Equals("Warning"))
                        {
                            //蜂鸣
                        }
                        else
                        {
                            //取消蜂鸣
                        }

                        //做查询电压任务
                    }
                    IsOperate8735TaskAlive = false;
                }
                catch (Exception err)
                {
                    IsOperate8735TaskAlive = false;
                    RunningRecord(err.Message);
                }
            }, TaskCreationOptions.LongRunning);
        }
        
        #endregion

        #region navigation
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
        #endregion

    }
}
