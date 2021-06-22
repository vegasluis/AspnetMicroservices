using IdentityServer.Interfaces;
using IdentityServer.Repositories;
using IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddMvc();

            services.AddIdentityServer()
                    .AddTestUsers(TestUsers.Users)
                    .AddDeveloperSigningCredential();

            services.AddTransient<IMongoRepository, MongoRepository>();
            services.AddTransient<IClientStore, CustomClientStore>();
            services.AddTransient<IResourceStore, CustomResourceStore>();
            services.AddTransient<IPersistedGrantStore, CustomPersistedGrantStore>();

            services.AddAuthentication();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();            
            app.UseRouting();
            app.UseIdentityServer();
            app.UseStaticFiles();
            app.UseAuthorization();

            // --- Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing ---
            ConfigureMongoDriver2IgnoreExtraElements();

            // --- The following will do the initial DB population (If needed / first time) ---
            InitializeDatabase(app);

            app.UseEndpoints(endpoints =>
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                });
            });
        }

        #region Database
        private static async void InitializeDatabase(IApplicationBuilder app)
        {
            bool createdNewRepository = false;
            var repository = app.ApplicationServices.GetService<IMongoRepository>();

            //  --Client
            if (! await repository.CollectionExists<Client>())
            {
                foreach (var client in Config.GetClients())
                {
                    repository.Add<Client>(client);
                }
                createdNewRepository = true;
            }

            ////  --IdentityResource
            //if (!await repository.CollectionExists<IdentityResource>())
            //{
            //    foreach (var res in Config.GetIdentityResources())
            //    {
            //        repository.Add<IdentityResource>(res);
            //    }
            //    createdNewRepository = true;
            //}


            ////  --ApiResource
            //if (!await repository.CollectionExists<ApiResource>())
            //{
            //    foreach (var api in Config.GetApiResources())
            //    {
            //        repository.Add<ApiResource>(api);
            //    }
            //    createdNewRepository = true;
            //}

            //  --ApiScopes
            if (!await repository.CollectionExists<ApiScope>())
            {
                foreach (var scope in Config.GetApiScopes())
                {
                    repository.Add<ApiScope>(scope);
                }
                createdNewRepository = true;
            }

            // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
            if (createdNewRepository)
            {
                var newRepositoryMsg = $"Mongo Repository created/populated! Please restart you website, so Mongo driver will be configured  to ignore Extra Elements - e.g. IdentityServer \"_id\" ";
                throw new Exception(newRepositoryMsg);
            }
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiScope>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            //BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.SetIgnoreExtraElements(true);
            //});
            //BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            //{
            //    cm.AutoMap();
            //    cm.SetIgnoreExtraElements(true);
            //});
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });          
        }

        #endregion
    }
}
