# Auction & Marketplace System - Backend
**Hệ thống Đấu giá & Sàn thương mại điện tử (Auction & Marketplace Platform)**

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue.svg)
![Redis](https://img.shields.io/badge/Redis-7-red.svg)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED.svg)
![Status](https://img.shields.io/badge/status-production--ready-brightgreen)

> Nền tảng cho phép người dùng đăng bán sản phẩm, mua sắm trực tiếp hoặc tham gia đấu giá theo thời gian thực. Hệ thống xử lý luồng tiền an toàn với Redis Distributed Lock, ACID Transactions và cơ chế đóng băng ví thông minh.

Đây là repository **Backend** được xây dựng theo kiến trúc **Clean Architecture**, tập trung vào tính nhất quán dữ liệu tài chính dưới tải cao và concurrency.

## 🚀 Tính năng chính

### Sàn thương mại (Marketplace)
- Đăng bán sản phẩm theo danh mục, hỗ trợ ảnh và mô tả chi tiết
- Mua đứt (Purchase) với Redis Lock đảm bảo chỉ 1 người mua thành công
- Danh sách yêu thích (Wishlist) để theo dõi sản phẩm quan tâm
- Đánh giá sao + bình luận sau khi hoàn tất đơn hàng

### Đấu giá thời gian thực (Auction)
- Tạo phiên đấu giá với thời gian kết thúc tự động (Hangfire Background Job)
- Đặt giá (Bid) được bảo vệ bởi **Redis Distributed Lock** chống race condition
- Đóng băng tiền (Freeze Balance) khi bid, tự động hoàn trả khi bị vượt giá
- Hỗ trợ **tự nâng giá** mà không bị trừ kép tiền đóng băng
- Tự động đóng phiên, chuyển tiền cho người bán và tạo đơn hàng cho người thắng

### Ví điện tử & Giao dịch
- Ví nội bộ với cơ chế **Balance** (khả dụng) và **FrozenBalance** (đóng băng)
- Nạp tiền / Rút tiền được bọc trong DB Transaction (ACID)
- Lịch sử giao dịch chi tiết (TopUp, Withdraw, Purchase, BidFreeze, BidUnfreeze)

### Bảo mật & Xác thực
- JWT Access Token + Refresh Token (Redis Cache, TTL 7 ngày)
- OTP xác thực email (Single Use, TTL 2 phút)
- Blacklist token khi logout
- Mã hóa mật khẩu bằng BCrypt

### Realtime & Thông báo
- **SignalR** cho thông báo bid mới, bị vượt giá, phiên kết thúc
- Chat 1-1 giữa người mua và người bán
- Danh sách cuộc trò chuyện và lịch sử tin nhắn

### Quản trị
- Hệ thống Report (báo cáo vi phạm User/Item) với trạng thái Pending → Resolved
- Quản lý danh mục sản phẩm

## 🏗️ Kiến trúc hệ thống

```text
Dự án tuân thủ nghiêm ngặt Clean Architecture (Onion Architecture):

AuctionSys/
├── AuctionSys.Domain
│   └── Entities, Enums, Interfaces (Repository Contracts)
│
├── AuctionSys.Application
│   └── Use Cases, DTOs, Mappings, Common Models
│
├── AuctionSys.Infrastructure
│   └── EF Core, Repositories, UnitOfWork, External Services
│       (Redis, Hangfire, SignalR)
│
└── AuctionSys.Api
    └── Controllers, Hubs, Middleware, Program.cs
```

### Các pattern quan trọng
- **Repository + Unit of Work Pattern** — Quản lý transaction và đảm bảo tính toàn vẹn dữ liệu
- **Use Case Pattern (Thin Controller)** — Mỗi nghiệp vụ là một class độc lập, dễ test
- **Redis Distributed Lock** — Chống race condition cho mua hàng và đấu giá
- **Frozen Balance Mechanism** — Đóng băng tiền khi bid, hoàn trả khi bị vượt giá
- **AutoMapper** cho DTO mapping
- **Standardized ApiResponse\<T\>** cho mọi endpoint

## 🛠️ Công nghệ sử dụng

| Layer           | Technology                          |
|-----------------|-------------------------------------|
| Runtime         | .NET 9.0                            |
| Web API         | ASP.NET Core                        |
| ORM             | Entity Framework Core               |
| Database        | PostgreSQL 15                       |
| Distributed Lock| Redis (StackExchange.Redis)         |
| Authentication  | JWT + Refresh Token + BCrypt        |
| Background Jobs | Hangfire                            |
| Realtime        | SignalR                             |
| Containerize    | Docker + Docker Compose             |

## 📦 Cấu trúc thư mục

```text
AuctionSys/
├── AuctionSys.Domain/
│   ├── Entities/
│   │   ├── User, Wallet, WalletTransaction
│   │   ├── Item, Category, Order
│   │   ├── Auction, Bid, AuctionWatcher
│   │   ├── Review, Report, Wishlist
│   │   ├── Notification, ChatMessage
│   │   └── ...
│   ├── Enums/
│   └── Interfaces/
│
├── AuctionSys.Application/
│   ├── DTOs/
│   ├── Interfaces/UseCases/
│   ├── UseCases/
│   │   ├── Auth/        (Register, Login, OTP, Refresh, Logout)
│   │   ├── Wallet/      (TopUp, Withdraw, GetWallet, History)
│   │   ├── Item/        (Create, List, Detail, Purchase)
│   │   ├── Auction/     (Create, PlaceBid, Close, Watch)
│   │   ├── Chat/        (Send, History, Conversations)
│   │   ├── Review/      (Create, GetByUser)
│   │   ├── Report/      (Create, List, Resolve)
│   │   └── Notification/ (List, MarkRead, MarkAllRead)
│   ├── Mappings/
│   └── Common/
│
├── AuctionSys.Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Migrations/
│   ├── Repositories/
│   │   └── UnitOfWork.cs
│   └── Services/
│       ├── Auth/
│       └── HangfireBackgroundJobService.cs
│
├── AuctionSys.Api/
│   ├── Controllers/ (11 controllers)
│   ├── Hubs/
│   │   ├── AuctionHub.cs
│   │   └── ChatHub.cs
│   ├── Middlewares/
│   └── Program.cs
│
├── Dockerfile
├── docker-compose.yml
└── .gitignore
```

## 🚀 Bắt đầu nhanh (Local Development)

### 📋 Yêu cầu hệ thống
- .NET SDK 9.0
- Docker Desktop
- (Tùy chọn) PostgreSQL 15+ và Redis nếu chạy không qua Docker

### ▶️ Chạy bằng Docker (Khuyến nghị)

```bash
git clone <repository-url>
cd Auction-Marketplace-System-BE

# Khởi động tất cả services (PostgreSQL, Redis, API)
docker-compose up --build -d
```

Truy cập:
- **Swagger UI:** http://localhost:5000/swagger
- **Hangfire Dashboard:** http://localhost:5000/hangfire

### ▶️ Chạy thủ công (Development)

```bash
# 1. Khởi động PostgreSQL & Redis bằng Docker
docker-compose up db redis -d

# 2. Chạy API
cd AuctionSys.Api
dotnet run
```

## 📡 API Endpoints

| Module       | Method | Endpoint                           | Mô tả                    |
|--------------|--------|-------------------------------------|---------------------------|
| **Auth**     | POST   | `/api/Auth/send-otp`               | Gửi OTP xác thực email   |
|              | POST   | `/api/Auth/register`               | Đăng ký tài khoản        |
|              | POST   | `/api/Auth/login`                  | Đăng nhập                |
| **Wallets**  | GET    | `/api/Wallets`                     | Xem số dư ví             |
|              | POST   | `/api/Wallets/topup`               | Nạp tiền                 |
|              | POST   | `/api/Wallets/withdraw`            | Rút tiền                 |
| **Items**    | GET    | `/api/Items`                       | Danh sách sản phẩm       |
|              | POST   | `/api/Items`                       | Đăng bán sản phẩm        |
|              | POST   | `/api/Items/{id}/purchase`         | Mua đứt sản phẩm         |
| **Auctions** | GET    | `/api/Auctions`                    | Danh sách phiên đấu giá  |
|              | POST   | `/api/Auctions`                    | Tạo phiên đấu giá        |
|              | POST   | `/api/Auctions/{id}/bid`           | Ra giá                   |
| **Chat**     | GET    | `/api/Chat/conversations`          | Danh sách cuộc trò chuyện|
|              | POST   | `/api/Chat/send`                   | Gửi tin nhắn             |
| **Reports**  | POST   | `/api/Reports`                     | Báo cáo vi phạm          |

> Xem đầy đủ tại Swagger UI: http://localhost:5000/swagger
