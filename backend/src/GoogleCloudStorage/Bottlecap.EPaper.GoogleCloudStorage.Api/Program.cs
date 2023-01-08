using Bottlecap.EPaper.GoogleCloudStorage.Api;
using Bottlecap.EPaper.GoogleCloudStorage.Services;
using Bottlecap.EPaper.Services;
using Bottlecap.EPaper.Services.ImageProviders;
using Google.Apis.Auth.OAuth2;
using System.Text;

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

ApiConfig config = new()
{
    ApiKey = GetString("API_KEY")
};

builder.Services.AddSingleton(config);

builder.Services
    .AddSingleton<IImageProvider>(new GoogleCloudImageProvider
    (
        GetString("BUCKET_NAME"),
        GoogleCredential.FromJson(Encoding.UTF8.GetString(Convert.FromBase64String(GetString("GOOGLE_STORAGE_SERVICE_ACCOUNT"))))
    ))
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
