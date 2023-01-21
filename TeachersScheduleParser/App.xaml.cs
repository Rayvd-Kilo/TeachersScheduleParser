using System.Threading;
using System.Windows;

using TeachersScheduleParser.Runtime.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TeachersScheduleParser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        private readonly CancellationTokenSource _cancellationTokenSource;
        
        private App()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            InitializeBuilder();
        }
        
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync(_cancellationTokenSource.Token);

            var entryPointForm = AppHost.Services.GetRequiredService<MainWindow>();
            
            entryPointForm.Show();
            
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StartAsync(_cancellationTokenSource.Token);
            
            base.OnExit(e);
        }

        private void InitializeBuilder()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices(((context, services) =>
            {
                services.AddSingleton<MainWindow>();

                services.AddDataSetFactory();

                services.AddDateFactory();
            })).Build();
        }
    }
}