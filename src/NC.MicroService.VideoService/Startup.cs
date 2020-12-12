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
using NC.MicroService.Infrastructure.Consul;
using NC.MicroService.VideoService.Services;
using NC.MicroService.VideoService.Repositories;
using NC.MicroService.VideoService.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NC.MicroService.VideoService
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

            // 1. ע�����ݿ�������
            services.AddDbContext<CoreContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));// AiConnection DefaultConnection
            });

            // 2. ע���Ŷ�service
            services.AddScoped<IVideoService, Services.VideoService>();

            // 3. ע���ŶӲִ�
            services.AddScoped<IVideoRepository, VideoRepository>();

            // 4. ע��ӳ��
            // services.AddAutoMapper();

            // 5. ע��Consulע�����
            services.AddConsulRegistry(Configuration);

            //// 6.У��AccessToken,������У�����Ľ���У��-- > Ocelot ���ؼ�����Ȩ��֤
            //services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //        .AddIdentityServerAuthentication(options =>
            //        {
            //            options.Authority = "https://192.168.2.107:5005"; // 1. ��Ȩ���ĵ�ַ
            //            options.ApiName = "VideoService"; // 2. api����(��Ŀ��������)
            //            options.RequireHttpsMetadata = true; // 3. httpsԪ���ݣ�����Ҫ
            //            options.JwtBackChannelHandler = GetHandler(); // 4. �Զ��� HttpClientHandler 
            //        });

            //// 7. ע��Saga�ֲ�ʽ����
            //services.AddOmegaCore(options =>
            //{
            //    options.GrpcServerAddress = "192.168.223.181:8080"; // 7.1 Э�����ĵ�ַ
            //    options.InstanceId = "VideoService-ID"; // 7.2 ����ʵ��ID -- ���ڼ�Ⱥ
            //    options.ServiceName = "VideoService"; // 7.3 ��������
            //});

            // 8. �����¼����� CAP
            services.AddCap(options =>
            {
                // 8.1 ʹ���ڴ�洢��Ϣ����Ϣ����ʧ�ܴ�����
                //options.UseInMemoryStorage();
                // ʹ��EntityFramework���д洢����
                options.UseEntityFramework<CoreContext>();
                // ʹ��MySql����������
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));// AiConnection DefaultConnection

                // 8.2 ʹ��RabbitMQ�����¼����Ĵ���
                options.UseRabbitMQ(options =>
                {
                    options.HostName = "172.17.225.138";
                    options.UserName = "mq";
                    options.Password = "123456";
                    options.Port = 5672;
                    options.VirtualHost = "/";
                });

                // 8.3 ���ö�ʱ����������
                // ����ʧ������ʱ�������룩�����������ø�ʱ��
                // ����CAPĬ���߼����ɣ��������Դ�����Ϊ5��ʱ��CAP���Ȼ���60������3�Σ���Ȼʧ��ʱ��ִ�г�ʱ�����ߣ�3���ӣ���֮���ٽ������ԡ�
                // options.FailedRetryInterval = 2;  
                options.FailedRetryCount = 5; // ����5������ʧ��ʱ��ֻ�ܽ����˹���Ԥ���ڲ������ٴ�������Ϣ��

                // 8.4 ����CAP��̨���ҳ�棨�˹�������
                options.UseDashboard(); // CAP ��̨�������棨·�ɵ�ַ��/cap��
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 1. Consul����ע��
            app.UseConsulRegistry();

            app.UseRouting();

            // 1. ����������֤ --> Ocelot ���ؼ�����Ȩ��֤
            //app.UseAuthentication();

            // 2. ʹ����Ȩ
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}