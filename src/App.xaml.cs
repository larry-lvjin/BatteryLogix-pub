using System.Configuration;
using System.Data;
using System.Windows;
using BatteryLogix.Services;

namespace BatteryLogix
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!LifetimeService.IsValid()) { Shutdown(); return; }
            new MainWindow().Show();
        }
    }

}
