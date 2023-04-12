using ACDCTestSystemPart1.PublishEvent;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACDCTestSystemPart1.ViewModels
{
    public class LoginPageViewModel : BindableBase, INavigationAware, IConfirmNavigationRequest
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public LoginPageViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
        }

        private string account;
        public string Account
        {
            get
            {
                return account;
            }
            set
            {
                SetProperty(ref account, value);
            }
        }

        private string passwords;
        public string Passwords
        {
            get
            {
                return passwords;
            }
            set
            {
                SetProperty(ref passwords, value);
            }
        }

        private bool snackBar;
        public bool SnackBar
        {
            get
            {
                return snackBar;
            }
            set
            {
                SetProperty(ref snackBar, value);
            }
        }

        private string loginmessage;
        public string LoginMessage
        {
            get
            {
                return loginmessage;
            }
            set
            {
                SetProperty(ref loginmessage, value);
            }
        }

        private DelegateCommand logincommand;
        public DelegateCommand LoginCommand =>
            logincommand ?? (logincommand = new DelegateCommand(ExecuteLoginCommand));

        void ExecuteLoginCommand()
        {
            if (!string.IsNullOrEmpty(Passwords) && Account.Equals("操作员"))
            {
                if (Passwords.Equals("2"))
                {
                    Share.ShareModel.LoginToken = true;
                    Task.Factory.StartNew(async () =>
                    {
                        LoginMessage = "登录成功";
                        this.eventAggregator.GetEvent<CurrentUserEvent>().Publish("User");
                        Passwords = "";
                        SnackBar = true;
                        await Task.Delay(2000);
                        SnackBar = false;
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.regionManager.RequestNavigate("ContentRegion", "TestWindow", arg => { }, null);
                        }));
                    });
                }
                else
                {
                    Share.ShareModel.LoginToken = false;
                    Task.Factory.StartNew(async () =>
                    {
                        LoginMessage = "密码错误！";
                        Passwords = "";
                        SnackBar = true;
                        await Task.Delay(2000);
                        SnackBar = false;
                    });
                }
            }
            else if (!string.IsNullOrEmpty(Passwords) && Account.Equals("管理员"))
            {
                if (Passwords.Equals("1"))
                {
                    Share.ShareModel.IsAdmin = true;
                    Share.ShareModel.LoginToken = true;
                    Task.Factory.StartNew(async () =>
                    {
                        LoginMessage = "登录成功";
                        this.eventAggregator.GetEvent<CurrentUserEvent>().Publish("Admin");
                        Passwords = "";
                        SnackBar = true;
                        await Task.Delay(2000);
                        SnackBar = false;
                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.regionManager.RequestNavigate("ContentRegion", "ConfigWindow", arg => { }, null);
                        }));
                    });
                }
                else
                {
                    Share.ShareModel.LoginToken = false;
                    Share.ShareModel.IsAdmin = false;
                    Task.Factory.StartNew(async () =>
                    {
                        LoginMessage = "密码错误";
                        Passwords = "";
                        SnackBar = true;
                        await Task.Delay(2000);
                        SnackBar = false;
                    });
                }
            }
        }

        private DelegateCommand logoutcommand;
        public DelegateCommand LogoutCommand =>
            logoutcommand ?? (logoutcommand = new DelegateCommand(ExecuteLogoutCommand));

        void ExecuteLogoutCommand()
        {
            Share.ShareModel.LoginToken = false;
            Share.ShareModel.IsAdmin = false;
            if (string.IsNullOrEmpty(Account))
            {
                return;
            }
            Task.Factory.StartNew(async () =>
            {
                LoginMessage = "成功注销！";
                Passwords = "";
                SnackBar = true;
                await Task.Delay(2000);
                SnackBar = false;
            });
        }


        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (Share.ShareModel.LoginToken)
            {
                continuationCallback(true);
            }
            else
            {
                continuationCallback(false);
            }
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
            if (navigationContext.Parameters.ContainsKey("key"))
            {
                if (navigationContext.Parameters.GetValue<string>("key").Equals("Exit"))
                {
                    this.eventAggregator.GetEvent<CurrentUserEvent>().Publish("Logout");
                }
            }
        }
    }
}
