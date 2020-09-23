using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Shop.Data;

namespace Shop
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
            services.AddCors();

            // Comprime os arquivos json enviados ao navegador que ira descompactar automaticamente
            services.AddResponseCompression( options => {
                options.Providers.Add<GzipCompressionProvider>(); 
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {"application/json"});
            });

            services.AddControllers();

            IdentityModelEventSource.ShowPII = true;

            var key = Encoding.ASCII.GetBytes(Settings.Secret);

            services.AddAuthentication( x => {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(y => {
                    y.RequireHttpsMetadata = false;
                    y.SaveToken = true;
                    y.TokenValidationParameters = new TokenValidationParameters{
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                };
            });
            
            //services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));

            services.AddDbContext<DataContext>(opt => opt.UseSqlServer(
                Configuration.GetConnectionString("connectionString")
            ));

            //A conecção sera fechada apos o uso...
            services.AddScoped<DataContext, DataContext>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors( x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
            );

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

             app.UseEndpoints(endpoints =>
             {
                endpoints.MapControllers();
            });

           
        }
    }
}
