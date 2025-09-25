using Assignment_12._3._2.Data;
using Assignment_12._3._2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assignment_12._3._2
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddDbContext<TaskDataContext>(options =>
            {
                options.UseSqlite("Data Source = tasks.db");
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TaskDataContext>();
                context.Database.EnsureCreated();

                // If no data in database, seed data
                if (!context.Tasks.Any())
                {
                    context.Tasks.AddRange(
                        new TaskToDo { TaskDescription = "Take out trash" },
                        new TaskToDo { TaskDescription="Pay bills"}
                        );
                    context.SaveChangesAsync();
                }
            }

            return app;
        }
    }
}
