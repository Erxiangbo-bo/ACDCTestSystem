using ACDCTestSystemPart1.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace ACDCTestSystemPart1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<LoginPage>("LoginPage");
            containerRegistry.RegisterForNavigation<TestWindow>("TestWindow");
            containerRegistry.RegisterForNavigation<ConfigWindow>("ConfigWindow");
        }
    }
}
