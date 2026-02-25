using AzureAppConfigurationWeb;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var enableAzureAppConfiguration = !builder.Environment.IsDevelopment();
var useAzureAppConfigurationMiddleware = false;

if (enableAzureAppConfiguration)
{
    var appConfigEndpoint = builder.Configuration["AppConfig:Endpoint"];
    if (!string.IsNullOrWhiteSpace(appConfigEndpoint))
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
            options.Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
                .Select("DisplaySettings:*"));

        builder.Services.AddAzureAppConfiguration();
        useAzureAppConfigurationMiddleware = true;
    }
}

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<DisplaySettings>(builder.Configuration.GetSection("DisplaySettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

if (!app.Environment.IsDevelopment() && useAzureAppConfigurationMiddleware)
{
    app.UseAzureAppConfiguration();
}

app.UseAuthorization();

app.MapRazorPages();

app.Run();