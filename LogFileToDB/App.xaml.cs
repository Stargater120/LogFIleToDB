using Core;
using Database;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace LogFileToDB
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SQLHelper>();
            services.AddSingleton < DBContext>();
            services.AddSingleton<Repository>();
            services.AddSingleton<CommandRepository>();
            services.AddSingleton<QueryRepository>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<DisplayedLists>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
