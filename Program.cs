using Dapper;
using fitmate.api.Services;
using fitmate.api.Services.Exercise;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

SqlMapper.SetTypeMap(typeof(Dictionary<string, object>), new CustomPropertyTypeMap(
   typeof(Dictionary<string, object>),
   (type, columnName) => type.GetProperty(columnName.ToLower())));



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(57408); // Allows access from other devices
});


builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddSingleton<SharedHelper>();

builder.Services.AddScoped<IExerciseHelper, ExerciseHelper>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
app.UseWebSockets();
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
