using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Pro.Common;
using Pro.Data.Repositorys;
using Pro.Data.Repositorys.Implements;
using Pro.Model;
using Pro.Service;
using Pro.Service.Caching;
using Pro.Service.Implements;

namespace xStory
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
            services.Configure<AppSettingData>(Configuration.GetSection(nameof(AppSettingData)));

            services.AddSingleton<IAppSettingData>(sp => sp.GetRequiredService<IOptions<AppSettingData>>().Value);
            //Repository
            services.AddScoped<INewStoryRepository, NewStoryRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();
            services.AddSingleton<ICommentRepository, CommentRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IStoryTypeRepository, StoryTypeRepository>();
            //Service
            services.AddScoped<IStorysService, StorysService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStoryTypeService, StoryTypeService>();

            services.AddAuthentication("tokenAuth")
            .AddScheme<TokenAuthenticationSchemeOptions, TokenAuthenticationService>("tokenAuth", ops => { });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "xStory", Version = "v1" });
            });
#if DEBUG
            LogHelper.InitLogHelper(@"C:\Logs\XStory\");
#else
            LogHelper.InitLogHelper();
#endif
            LogHelper.Info("App Started!");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "xStory v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder => builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
