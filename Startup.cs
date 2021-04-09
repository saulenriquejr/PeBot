using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WelcomeUser.Common;
using WelcomeUser.Mail;
using WelcomeUser.Services;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;
using System;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public Startup(IConfiguration config)
        {
           Configuration = config;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddMvc(MvcOptions.EnableEndpointRouting = false);


            services.AddControllers().AddNewtonsoftJson();

            services.AddTransient<IMailService, MailService>();
            
            //services.AddTransient<IAppServices, AppServices>();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            //services.Configure<ApiKey>(Configuration.GetSection("AppSettings"));
            services.AddSingleton<AppServices>();
            services.AddSingleton<AppSettings>();
            services.AddSingleton(Configuration);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state.
            services.AddSingleton<UserState>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, WelcomeUserBot>();

            //Configure Services
            services.AddSingleton<BotServices>();

            //ConfigureDialog
            //ConfigureDialogs(services);

            //Configure State
            ConfigureState(services);
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["ConnectionStrings:storageAccount:blob"], preferMsi: true);
                builder.AddQueueServiceClient(Configuration["ConnectionStrings:storageAccount:queue"], preferMsi: true);
            });

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //services.AddTransient<IBot, DialogBot<MainDialog>>();
        }

        public void ConfigureDialogs(IServiceCollection services)
        {
            //services.AddSingleton<MainDialog>();
        }

        private void ConfigureState(IServiceCollection services)
        {
            //Create the storage we'll be using for User and Conversation State. (Memory is great for testing purposes.)
            //services.AddSingleton<IStorage, MemoryStorage>();
            var storageAccount = Configuration.GetSection("storageAccount").Value;
            var storageContainer = Configuration.GetSection("storageContainer").Value;
            services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount, storageContainer));

            //Create the User State
            services.AddSingleton<UserState>();

            //Create the Conversation state
            services.AddSingleton<ConversationState>();

            // Create an instance of the state service
            services.AddSingleton<StateService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthorization()
                .UseWebSockets()
            //app.UseHttpsRedirection();
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
    internal static class StartupExtensions
    {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddQueueServiceClient(serviceUri);
            }
            else
            {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
            }
        }
    }
}
