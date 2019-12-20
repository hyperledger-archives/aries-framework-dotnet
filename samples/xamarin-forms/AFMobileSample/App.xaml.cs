﻿using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Hyperledger.Aries.Features.DidExchange;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace AFMobileSample
{
    public partial class App : Application
    {
        internal static IContainer Container { get; private set; }

        public App()
        {
            InitializeComponent();
            RegisterContainer();

            MainPage = new MainPage();
        }

        private void RegisterContainer()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            // Initialize Autofac
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(typeof(IConnectionService).Assembly)
                .AsImplementedInterfaces();
            builder.RegisterType<HttpClient>().AsSelf();

            builder.Populate(services);

            // Build the final container
            Container = builder.Build();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
