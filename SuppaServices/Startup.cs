using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using EasyRpc.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleFixture;
using SuppaServices.Interfaces.Personnel;
using SuppaServices.Server.Configuration;
using SuppaServices.Server.DataAccess;
using SuppaServices.Server.Filter;
using SuppaServices.Server.Repository;
using SuppaServices.Server.Services;

namespace SuppaServices.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddJsonRpc();

            services.AddTransient<IPersonnelRepository, PersonnelRepository>();
            services.AddTransient<IConnectionFactory, SqliteConnectionFactory>();
            services.AddScoped<IConnectionManager, ConnectionManager>();

            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseJsonRpc("/",
                api =>
                {
                    api.ApplyFilter<TransactionFilter>(m =>
                        m.GetCustomAttributes(typeof(TransactionAttribute), true).Length > 0);

                    api.ExposeAssemblyContaining<Startup>().Where(TypesThat.AreInTheSameNamespaceAs<BitmapService>());
                });

            CreateDatabase(app);
        }

        private void CreateDatabase(IApplicationBuilder app)
        {
            SQLitePCL.Batteries.Init();

            using var scope = app.ApplicationServices.CreateScope();

            var connectionManager = scope.ServiceProvider.GetService<IConnectionManager>();

            connectionManager.BeginTransaction();

            using var c = connectionManager.GetConnection();

            c.Connection.Execute(
                "CREATE TABLE IF NOT EXISTS Personnel (PersonnelId INTEGER PRIMARY KEY AUTOINCREMENT, FirstName TEXT, LastName TEXT, DateOfBirth TEXT, EmploymentDate TEXT);");

            GeneratePersonnel(scope).Wait();

            connectionManager.CommitTransaction();
        }

        private async Task GeneratePersonnel(IServiceScope scope)
        {
            var personnelRepository = scope.ServiceProvider.GetService<IPersonnelRepository>();

            if ((await personnelRepository.GetPersonnelListEntries("")).Any())
            {
                return;
            }

            var fixture = new Fixture();

            for (var i = 0; i < 20; i++)
            {
                var personnel = fixture.Generate<PersonnelEntry>(constraints: new { minDate = new DateTime(1960,1,1),  maxDate = new DateTime(1990, 1, 1) });

                personnel.EmploymentDate =
                    fixture.Generate<DateTime>(constraints: new { minDate = personnel.DateOfBirth.Value.AddYears(20), maxDate = DateTime.Today });

                await personnelRepository.AddPersonnelEntry(personnel);
            }
        }
    }
}
