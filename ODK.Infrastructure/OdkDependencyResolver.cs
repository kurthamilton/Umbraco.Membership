﻿using System.Configuration;
using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using ODK.Data.Events;
using ODK.Data.Payments;
using ODK.Umbraco.Events;
using ODK.Umbraco.Members;
using MvcDependencyResolver = System.Web.Mvc.IDependencyResolver;
using WebApiDependencyResolver = System.Web.Http.Dependencies.IDependencyResolver;

namespace ODK.Infrastructure
{
    public class OdkDependencyResolver
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();
        private IContainer _container;

        public OdkDependencyResolver(params Assembly[] webAssemblies)
        {
            _builder.RegisterControllers(webAssemblies);
            _builder.RegisterApiControllers(webAssemblies);

            RegisterServices();
            RegisterDataServices();
        }

        public void Register<T>(T reference)
        {
            _builder.Register(c => reference);
        }

        public MvcDependencyResolver GetMvcDependencyResolver()
        {
            return new AutofacDependencyResolver(GetContainer());
        }

        public WebApiDependencyResolver GetWebApiDependencyResolver()
        {
            return new AutofacWebApiDependencyResolver(GetContainer());
        }

        private IContainer GetContainer()
        {
            if (_container == null)
            {
                _container = _builder.Build();
            }

            return _container;
        }

        private void RegisterServices()
        {
            _builder.RegisterType<EventService>().InstancePerRequest();
            _builder.RegisterType<OdkMemberService>().InstancePerRequest();
        }

        private void RegisterDataServices()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString;
            _builder.RegisterInstance(new EventsDataService(connectionString)).SingleInstance();
            _builder.RegisterInstance(new PaymentsDataService(connectionString)).SingleInstance();
        }
    }
}