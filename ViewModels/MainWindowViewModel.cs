using ACDCTestSystemPart1.PublishEvent;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace ACDCTestSystemPart1.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private bool open;
        public bool Open
        {
            get
            {
                return open;
            }
            set
            {
                SetProperty(ref open, value);
            }
        }

        private bool isAdmin;
        public bool IsAdmin
        {
            get
            {
                return isAdmin;
            }
            set
            {
                SetProperty(ref isAdmin, value);
            }
        }

        private string checktaskalive;
        public string CheckTaskAlive
        {
            get
            {
                return checktaskalive;
            }
            set
            {
                SetProperty(ref checktaskalive, value);
            }
        }

        private string user;
        public string User
        {
            get
            {
                return user;
            }
            set
            {
                SetProperty(ref user, value);
            }
        }


        private DelegateCommand logincommand;
        public DelegateCommand LoginCommand =>
            logincommand ?? (logincommand = new DelegateCommand(ExecuteLoginCommand));

        void ExecuteLoginCommand()
        {// 
            this.regionManager.RequestNavigate("ContentRegion", "LoginPage", arg => { }, null);
        }

        private DelegateCommand openConfigWindowCommand;
        public DelegateCommand OpenConfigWindowCommand =>
            openConfigWindowCommand ?? (openConfigWindowCommand = new DelegateCommand(ExecuteOpenConfigWindowCommand));

        void ExecuteOpenConfigWindowCommand()
        {
            this.regionManager.RequestNavigate("ContentRegion", "ConfigWindow", arg => { }, null);
        }

        private DelegateCommand openTestWindowCommand;
        public DelegateCommand OpenTestWindowCommand =>
            openTestWindowCommand ?? (openTestWindowCommand = new DelegateCommand(ExecuteOpenTestWindowCommand));

        void ExecuteOpenTestWindowCommand()
        {
            this.regionManager.RequestNavigate("ContentRegion", "TestWindow", arg => { }, null);
        }

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.GetEvent<CurrentUserEvent>().Subscribe(message =>
            {
                if (message.Equals("Admin"))
                {
                    IsAdmin = true;
                    User = "管理员";
                }
                else if (message.Equals("CheckTaskClose"))
                {
                    CheckTaskAlive = "检测程序已经结束";
                }
                else if (message.Equals("CheckTaskStart"))
                {
                    CheckTaskAlive = "检测程序运行中";
                }
                else if (message.Equals("User"))
                {
                    User = "操作员";
                }
                else if (message.Equals("Logout"))
                {
                    IsAdmin = false;
                }
            });
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                LoginCommand.Execute();
            }));
        }
    }
}
