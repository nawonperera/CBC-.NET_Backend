using CristalBeautyAPI.Middleware;
using CristalBeautyAPI.Models.Dto;
using CristalBeautyAPI.Repositories;
using CristalBeautyAPI.Repositories.IRepository;
using CristalBeautyAPI.Services;
using CristalBeautyAPI.Services.IServices;
using Microsoft.Extensions.AI;
using Microsoft.OpenApi;
using OpenAI;
using Qdrant.Client;
using System.ClientModel;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddSingleton<MongoDbService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductAIRepository, ProductAIRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IMessageService, MessageService>();

var credential = new ApiKeyCredential(builder.Configuration["DeepInfra:ApiKey"] ?? throw new InvalidOperationException("Missing configuration: DeepInfra:ApiKey."));

var options = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://api.deepinfra.com/v1/openai")
};

var openAiClient = new OpenAIClient(credential, options);

IChatClient chatClient = openAiClient.GetChatClient("google/gemini-3.1-flash-lite").AsIChatClient();

var embeddingGenerator = openAiClient.GetEmbeddingClient("sentence-transformers/all-MiniLM-L12-v2").AsIEmbeddingGenerator();

builder.Services.AddChatClient(chatClient).UseFunctionInvocation();
builder.Services.AddEmbeddingGenerator(embeddingGenerator);

builder.Services.AddSingleton<QdrantClient>(sp =>
{
    var uri = new Uri(builder.Configuration["Qdrant:Endpoint"]!);
    var apiKey = builder.Configuration["Qdrant:ApiKey"];

    return new QdrantClient(uri, apiKey: apiKey);
});

builder.Services.AddQdrantCollection<ulong, ProductVectorDTO>("product-vectors");

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "CristalBeautyAPI";
        document.Info.Version = "v1";

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        };

        document.Security ??= [];
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
    });
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Running in Development!");
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CristalBeautyAPI v1");
    });
}

app.UseCors();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run("http://localhost:3000");
