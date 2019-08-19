using System.Windows;

namespace CardQuery
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var dispatcher = Dispatcher;
            if (dispatcher != null)
            {
                dispatcher.UnhandledException += Dispatcher_UnhandledException;
            }
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            e.Handled = true;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Title = "Card Query For Hearthstone (QQGroup 146605007)";
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mainWindow.Show();
        }
    }
}
