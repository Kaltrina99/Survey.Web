using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Survey.Core.Interfaces;
using Survey.Infrastructure.Data;
using Survey.Infrastructure.Repositories;

namespace Survey.Web
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Transient);
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddControllersWithViews();
            services.AddDefaultIdentity<IdentityUser>().AddRoles<IdentityRole>()  
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().AddDefaultUI();
            services.AddLogging();
            services.AddSingleton(typeof(IHttpContextAccessor), typeof(HttpContextAccessor));
            services.AddAuthentication()
               .AddGoogle(options =>
               {
                   IConfigurationSection googleAuthSection = Configuration.GetSection("Authentication:Google");

                   options.ClientId = googleAuthSection["ClientId"];
                   options.ClientSecret = googleAuthSection["ClientSecret"];
               });

            services.AddRazorPages()
            .AddSessionStateTempDataProvider();
            services.AddSession();
            //services.AddScoped<IPermissionRole, PermissonRoleRepository>();
            //services.AddScoped<ISurveyResults, SurveyResultsRepository>();
            //services.AddScoped<ISurveySubmission, SubmissionRepository>();
            //services.AddScoped<IImageProfile, ImageProfileRepository>();
            services.AddScoped<IProjectCategory, ProjectCategoryRepository>();
            services.AddScoped<IProjects, ProjectsRepository>();
            //services.AddScoped<IForms, FormsRepository>();
            //services.AddScoped<IQuestions, QuestionsRepository>();
            //services.AddScoped<IQuestionOptions, QuestionOptionsRepository>();
            //services.AddScoped<IAnswers, AnswersRepository>();
            //services.AddScoped<ISkipLogic, SkipLogicRepository>();
            //services.AddScoped<IDataset, DatasetRepository>();
            //services.AddScoped<IEnrollDataset, EnrollDatasetRepository>();
            //services.AddScoped<ICases, CasesRepository>();
            //services.AddScoped<ICasesExcelHeaders, CasesExcelHeadersRepository>();
            //services.AddScoped<ICasesExcelData, CasesExcelDataRepository>();
            //services.AddScoped<ICaseAssignedUsers, CaseAssignedUsersRepository>();
            //services.AddScoped<ICaseAssignedForms, CaseAssignedFormsRepository>();
            //services.AddScoped<ISurveyResultDownload, SurveyResultDownload>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
