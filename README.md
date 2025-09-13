# Bulky

Bulky is an **ASP.NET Core MVC e-commerce application** with features like product catalog, shopping cart, order management, and an admin panel.

---

## Features
- Product catalog with categories
- Shopping cart (add, update, remove items)
- User authentication & roles
- Order checkout & history
- Admin panel for products, orders, and users
- Responsive UI with Bootstrap

---

## Tech Stack
- **Backend:** ASP.NET Core MVC  
- **Database:** SQL Server + Entity Framework Core  
- **Auth:** ASP.NET Core Identity  
- **Frontend:** Bootstrap, Razor Views  
- **Architecture:** MVC  

---

## Getting Started

### Prerequisites
- .NET 8 SDK (or project version)  
- SQL Server (Express or higher)  
- Git  

### Setup
```bash
# clone the repo
git clone https://github.com/ahmedsaloma/Bulky.git
cd Bulky

# update database
dotnet ef database update

# run the app
dotnet run
