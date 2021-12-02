using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApi.Authentication;
using WebApi.Common.JsonConvert;
using WebApi.Service;

namespace WebApi
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
            // 添加Swagger
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API Demo", Version = "v1" });
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                // 使用反射获取xml文件。并构造出文件的路径
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 启用xml注释. 该方法第二个参数启用控制器的注释，默认为false.
                options.IncludeXmlComments(xmlPath, true);
            });

            services.AddControllers(options => {
                //options.Filters.Add(typeof(CustomerExceptionFilter));
            }).AddJsonOptions(config => {
                config.JsonSerializerOptions.WriteIndented = true;
                config.JsonSerializerOptions.Converters.Add(new DateTimeConverter());// 日期格式转换器
                config.JsonSerializerOptions.Converters.Add(new BytesToBase64Converter());
                config.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());// 枚举转换器
                config.JsonSerializerOptions.Converters.Add(new DataTableConverter());// DataTable
                                                                                      //config.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                config.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddCors(options => {
                options.AddPolicy("any", builder => {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyHeader(); //允许任何来源的主机访问
                    //builder.WithOrigins("localhost:5000")//设置允许访问的域
                });
            });

            services.AddAuthentication(options => {
                options.AddScheme<HeaderAuth>(GuidToken.GUID_TOKEN_NAME, "Default Guid Token");
                options.DefaultAuthenticateScheme = GuidToken.GUID_TOKEN_NAME;
                options.DefaultChallengeScheme = GuidToken.GUID_TOKEN_NAME;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // 添加Swagger有关中间件
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Demo v1");
                });
            }


            //var options = new RewriteOptions()
            //    .AddRedirect("^login", "/");
            ////.AddRewrite("login", "/", skipRemainingRules: true);
            //app.UseRewriter(options);

            //app.UseStaticFiles();

            //app.UseFileServer();

            app.UseCors("any");

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
