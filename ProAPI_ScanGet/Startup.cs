using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Pro.Common;
using Pro.Common.Const;
using Pro.Data.Repositorys;
using Pro.Data.Repositorys.Implements;
using Pro.Model;
using Pro.Service;
using Pro.Service.WebData;
using Pro.Service.WebData.Implements;
using Pro.Service.Caching;
using Pro.Service.Implement;
using Pro.Service.Implements;
using Pro.Service.SubScanDataService;
using Pro.Service.SubScanDataService.Implements;

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

            services.AddScoped<INewStoryRepository, NewStoryRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddSingleton<IStoryTypeRepository, StoryTypeRepository>();
            services.AddScoped<IApplicationSettingRepository, ApplicationSettingRepository>();
            services.AddScoped<IResultScanDataRepository, ResultScanDataRepository>();
            services.AddScoped<IStoryFollowsRepository, StoryFollowsRepository>();
            services.AddScoped<IFileStoryRepository, FileStoryRepository>();
            services.AddScoped<ICacheProvider, InMemoryCacheProvider>();

            services.AddScoped<IScanDataService, ScanDataService>();
            services.AddScoped<IGetDataService, GetDataService>();
            services.AddScoped<IUploadImageService, UpFile2CloudinaryService>();
            services.AddScoped<IApplicationSettingService, ApplicationSettingService>();
            services.AddScoped<IPrepareService, PrepareService>();
            services.AddScoped<IGetRawDataService, GetRawDataService>();
            services.AddScoped<ISaveImage2Local, SaveImage2Local>();
            services.AddScoped<IUpData2DBService, UpData2DBService>();
            services.AddScoped<IStoryTypeService, StoryTypeService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<ICorrectInvalidDataService, CorrectInvalidDataService>();
            services.AddScoped<IStoryFollowsService, StoryFollowsService>();
            services.AddScoped<IFileStoryService, FileStoryService>();
            services.AddScoped<IGetDataFromWebService, GetDataFromWebService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "xStory", Version = "v1" });
            });
#if DEBUG
            LogHelper.InitLogHelper(Constants.DEBUG_LOG_FOLDER);
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
