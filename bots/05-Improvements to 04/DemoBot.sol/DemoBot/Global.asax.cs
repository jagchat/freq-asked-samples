using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;

namespace DemoBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //enables voice for formflow/formbuilder in the bot
            var builder = new ContainerBuilder();
            builder
                .RegisterType<TextToSpeakActivityMapper>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Update(Conversation.Container);
        }
    }
}
