using Caliburn.Micro;
using KiscoSchedule.ViewModels;
using Util = KiscoSchedule.Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using KiscoSchedule.Database.Services;
using KiscoSchedule.Shared.Models;
using System.Windows.Controls;
using KiscoSchedule.Services;

namespace KiscoSchedule
{
    class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();

            ConventionManager.AddElementConvention<PasswordBox>(
                PasswordBoxService.BoundPasswordProperty,
                "Password",
                "PasswordChanged");
        }

        /// <summary>
        /// Configures the bootstrapper
        /// </summary>
        protected override void Configure()
        {
            // Register the container
            container.Instance(container);

            // Add required singletons
            container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<IUser, User>()
                .Singleton<IDatabaseService, DatabaseService>();

            // Using LINQ combine the view with the view models
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        /// <summary>
        /// Called on start up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // Display the Shell View Model
            DisplayRootViewFor<ShellViewModel>();
        }

        /// <summary>
        /// Override the orignal function with our container
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetInstance(Type service, string key)
        {
            // Return the instance of a type and key
            return container.GetInstance(service, key);
        }

        /// <summary>
        /// Override the orignal function with our container
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            // return the instance of a type
            return container.GetAllInstances(service);
        }

        /// <summary>
        /// Override the orignal function with our container
        /// </summary>
        /// <param name="instance"></param>
        protected override void BuildUp(object instance)
        {
            // Push dependances to an existing instance
            container.BuildUp(instance);
        }
    }
}
