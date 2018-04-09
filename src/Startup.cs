﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FindAlfaITBot.Infrastructure;
using FindAlfaITBot.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FindAlfaITBot
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables("ASPNETCORE_");

            Configuration = builder.Build();

            var date = DateTime.Now;
            Console.WriteLine($"Hello, I'm a bot! Today is {date.DayOfWeek}, it's {date:HH:mm} now.");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var token = Configuration["TELEGRAM_TOKEN"];

            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Telegram token must be not null");

            var connection = Configuration["MONGO"];

            if (!string.IsNullOrEmpty(connection))
                MongoDBHelper.ConfigureConnection(connection);

            TestDB();

            Console.WriteLine($"Connection string is {MongoDBHelper.GetConnectionName}");

            services.AddSingleton(new FindITBot(token));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }

        private static void TestDB()
        {
            var students = MongoDBHelper.All().Result;

            if (students.Count() == 0)
            {
                var student = new Student
                {
                    Id = Guid.NewGuid().ToString(),
                    EMail = "test@test.ru",
                    Name = "Test Student",
                    Profession = "Haskell",
                    University = "sgu"
                };

                MongoDBHelper.AddStudent(student);

                if (students.Count() == 0)
                    throw new Exception("Fail to start DB");
            }
        }
    }
}