using Bottlecap.EPaper.GoogleCloudStorage.Services;
using Bottlecap.EPaper.Services;
using Bottlecap.EPaper.Services.ImageProviders;

static string GetString(string key)
{
    var value = System.Environment.GetEnvironmentVariable(key);
    if (String.IsNullOrEmpty(value))
    {
        throw new Exception($"Environment variable '{key}' not found");
    }

    return value;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<IImageProvider>(new GoogleCloudImageProvider(GetString("BUCKET_NAME")))
    .AddSingleton<IEpaperImageService, EPaperImageService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
