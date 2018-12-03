using Common.Interfaces.ServerManagersInterfaces;
using Common.RepositoryInterfaces;
using Db;
using Db.Repositories;
using Server.Managers;
using System;

using Unity;
using Unity.Lifetime;

namespace Server
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;

        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        /// 
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            //TODO: Register your type's mappings here.
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterType<IEmployeeManager, EmployeeManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ICustomerManager, CustomerManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ILineManager, LineManager>(new HierarchicalLifetimeManager());
            container.RegisterType<IPackageManager, PackageManager>(new HierarchicalLifetimeManager());
            container.RegisterType<IReceiptManager, ReceiptManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ISimulatorManager, SimulatorManager>(new HierarchicalLifetimeManager());
            container.RegisterType<IBiManager, BiManager>(new HierarchicalLifetimeManager());

            container.RegisterType<IUnitOfWork, UnitOfWork<CellularContext>>(new HierarchicalLifetimeManager());

            //container.RegisterType<ICustomerRepository, CustomerRepository>();
            //container.RegisterType<ILineRepository, LineRepository>();
            //container.RegisterType<ISmsRepository, SmsRepository>();
            //container.RegisterType<IPaymentRepository, PaymentRepository>();
            //container.RegisterType<IPackageRepository, PackageRepository>();
            //container.RegisterType<IPaymentRepository, PaymentRepository>();
            //container.RegisterType<IEmployeeRepository, EmployeeRepository>();
        }
    }
}