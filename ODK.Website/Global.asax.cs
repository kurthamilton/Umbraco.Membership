using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using ODK.Data.Payments;
using ODK.Umbraco.Members;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace ODK.Website
{
    public class OdkApplication : UmbracoApplication
    {
        protected override void OnApplicationStarted(object sender, EventArgs e)
        {
            base.OnApplicationStarted(sender, e);

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(OdkApplication).Assembly);
            builder.RegisterApiControllers(typeof(OdkApplication).Assembly);

            builder.RegisterControllers(typeof(UmbracoApplication).Assembly);
            builder.RegisterApiControllers(typeof(UmbracoApplication).Assembly);

            RegisterUmbracoServices(builder);
            RegisterServices(builder);
            RegisterDataServices(builder);

            IContainer container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterUmbracoServices(ContainerBuilder builder)
        {
            builder.Register(c => ApplicationContext.Current.Services.MediaService);
            builder.Register(c => ApplicationContext.Current.Services.MemberService);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<OdkMemberService>().InstancePerRequest();
        }

        private static void RegisterDataServices(ContainerBuilder builder)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString;
            builder.RegisterInstance(new PaymentsDataService(connectionString)).SingleInstance();
        }
    }
}