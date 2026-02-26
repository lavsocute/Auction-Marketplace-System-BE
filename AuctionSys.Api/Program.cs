using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AuctionSys.Api.Middlewares;
using AuctionSys.Domain.Interfaces;
using AuctionSys.Infrastructure.Data;
using AuctionSys.Infrastructure.Repositories;
using AuctionSys.Application.Interfaces.Auth;
using AuctionSys.Infrastructure.Services.Auth;
using AuctionSys.Application.Interfaces.UseCases.Auth;
using AuctionSys.Application.UseCases.Auth;
using AuctionSys.Application.Interfaces.UseCases.Category;
using AuctionSys.Application.UseCases.Category;
using AuctionSys.Application.Interfaces.UseCases.Wallet;
using AuctionSys.Application.UseCases.Wallet;
using AuctionSys.Application.Interfaces.UseCases.User;
using AuctionSys.Application.UseCases.User;
using AuctionSys.Application.Interfaces.UseCases.Item;
using AuctionSys.Application.UseCases.Item;
using AuctionSys.Application.Interfaces.UseCases.Wishlist;
using AuctionSys.Application.UseCases.Wishlist;
using AuctionSys.Application.Interfaces.UseCases.Auction;
using AuctionSys.Application.UseCases.Auction;
using AuctionSys.Application.Interfaces.UseCases.Review;
using AuctionSys.Application.UseCases.Review;
using AuctionSys.Application.Interfaces.UseCases.Notification;
using AuctionSys.Application.UseCases.Notification;
using AuctionSys.Application.Interfaces.UseCases.Report;
using AuctionSys.Application.UseCases.Report;
using AuctionSys.Application.Interfaces.UseCases.Chat;
using AuctionSys.Application.UseCases.Chat;
using AuctionSys.Application.Interfaces.Services;
using AuctionSys.Infrastructure.Services;
using AuctionSys.Api.Hubs;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

// Swagger with JWT config
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auction & Marketplace API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập token JWT theo định dạng: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Authentication Config
var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? throw new ArgumentException("Thiếu cấu hình JWT Secret");
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var blacklistService = context.HttpContext.RequestServices.GetRequiredService<ITokenBlacklistService>();
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            
            if (!string.IsNullOrEmpty(token) && await blacklistService.IsTokenBlacklistedAsync(token))
            {
                context.Fail("Token đã bị thu hồi (Blacklisted).");
            }
        }
    };
});

builder.Services.AddAuthorization();

// Redis Config
builder.Services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp => 
    StackExchange.Redis.ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Hangfire Config
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer();

builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IAuctionWatcherRepository, AuctionWatcherRepository>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IEmailService, MockEmailService>();
builder.Services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();

builder.Services.AddScoped<ISendOtpUseCase, SendOtpUseCase>();
builder.Services.AddScoped<IRegisterUseCase, RegisterUseCase>();
builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
builder.Services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
builder.Services.AddScoped<ILogoutUseCase, LogoutUseCase>();

builder.Services.AddScoped<IGetAllCategoriesUseCase, GetAllCategoriesUseCase>();
builder.Services.AddScoped<ICreateCategoryUseCase, CreateCategoryUseCase>();

builder.Services.AddScoped<IGetWalletUseCase, GetWalletUseCase>();
builder.Services.AddScoped<ITopUpUseCase, TopUpUseCase>();
builder.Services.AddScoped<IWithdrawUseCase, WithdrawUseCase>();
builder.Services.AddScoped<IGetTransactionHistoryUseCase, GetTransactionHistoryUseCase>();

builder.Services.AddScoped<IGetProfileUseCase, GetProfileUseCase>();
builder.Services.AddScoped<IUpdateProfileUseCase, UpdateProfileUseCase>();

builder.Services.AddScoped<IGetItemsUseCase, GetItemsUseCase>();
builder.Services.AddScoped<ICreateItemUseCase, CreateItemUseCase>();
builder.Services.AddScoped<IGetItemDetailUseCase, GetItemDetailUseCase>();
builder.Services.AddScoped<IPurchaseItemUseCase, PurchaseItemUseCase>();

builder.Services.AddScoped<IGetWishlistUseCase, GetWishlistUseCase>();
builder.Services.AddScoped<IAddToWishlistUseCase, AddToWishlistUseCase>();
builder.Services.AddScoped<IRemoveFromWishlistUseCase, RemoveFromWishlistUseCase>();

builder.Services.AddScoped<IGetAuctionsUseCase, GetAuctionsUseCase>();
builder.Services.AddScoped<ICreateAuctionUseCase, CreateAuctionUseCase>();
builder.Services.AddScoped<IGetAuctionDetailUseCase, GetAuctionDetailUseCase>();
builder.Services.AddScoped<IPlaceBidUseCase, PlaceBidUseCase>();
builder.Services.AddScoped<IWatchAuctionUseCase, WatchAuctionUseCase>();
builder.Services.AddScoped<IUnwatchAuctionUseCase, UnwatchAuctionUseCase>();
builder.Services.AddScoped<ICloseAuctionUseCase, CloseAuctionUseCase>();

builder.Services.AddScoped<ICreateReviewUseCase, CreateReviewUseCase>();
builder.Services.AddScoped<IGetUserReviewsUseCase, GetUserReviewsUseCase>();

builder.Services.AddScoped<IGetNotificationsUseCase, GetNotificationsUseCase>();
builder.Services.AddScoped<IMarkAsReadUseCase, MarkAsReadUseCase>();
builder.Services.AddScoped<IMarkAllAsReadUseCase, MarkAllAsReadUseCase>();

builder.Services.AddScoped<ICreateReportUseCase, CreateReportUseCase>();
builder.Services.AddScoped<IGetReportsUseCase, GetReportsUseCase>();
builder.Services.AddScoped<IResolveReportUseCase, ResolveReportUseCase>();

builder.Services.AddScoped<IGetConversationsUseCase, GetConversationsUseCase>();
builder.Services.AddScoped<IGetChatHistoryUseCase, GetChatHistoryUseCase>();
builder.Services.AddScoped<ISendMessageUseCase, SendMessageUseCase>();

builder.Services.AddScoped<IBackgroundJobService, HangfireBackgroundJobService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auction & Marketplace API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notification");
app.MapHub<ChatHub>("/hubs/chat");

app.Run();

public partial class Program { }