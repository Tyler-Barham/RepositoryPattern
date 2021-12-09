using BooksApi.Models;
using BooksApi.Services;
using BooksApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BooksApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Read appsettings to objects
            var mongoSettings = Configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();
            var cassandraSettings = Configuration.GetSection(nameof(CassandraDBSettings)).Get<CassandraDBSettings>();

            // Any service with IRepository<Book> in its constructor will get this repo instace
            services.AddSingleton<IRepository<Book>>(sp => new MongoRepository<Book>(mongoSettings));

            services.AddSingleton<BookService>();

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());

            services.AddSwaggerDocument();
        }

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

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
