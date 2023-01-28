using System.Threading;
using System.Windows;

using TeachersScheduleParser.Runtime.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TeachersScheduleParser.Runtime.Controllers;
using TeachersScheduleParser.Runtime.Factories;
using TeachersScheduleParser.Runtime.Interfaces;
using TeachersScheduleParser.Runtime.Services;
using TeachersScheduleParser.Runtime.Structs;

namespace TeachersScheduleParser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static IHost? AppHost { get; private set; }

        private readonly CancellationTokenSource _cancellationTokenSource;
        
        private App()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            InitializeBuilder();
        }
        
        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync(_cancellationTokenSource.Token);

            var entryPointForm = AppHost.Services.GetRequiredService<MainWindow>();

            var initializables = AppHost.Services.GetServices<IInitializable>();

            foreach (var initializable in initializables)
            {
                initializable.Initialize();
            }

            entryPointForm.Show();
            
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync(_cancellationTokenSource.Token);
            
            base.OnExit(e);
        }

        private void InitializeBuilder()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices(((_, services) =>
            {
                services.AddSingleton<MainWindow>();

                services.AddDataSetFactory();

                services.AddDateFactory();

                services.AddSchedulesReaderService();

                services.AddSingleton<DataSetParsingService>();
                
                services.AddTransient<IFileReaderDataFactory<Schedule[], string>, TeachersSchedulesFactory>();

                services.AddTelegramBotSystem();
            })).Build();
        }
    }
}