Bulky - E-commerce Web Application
Bulky is a dynamic and robust e-commerce web application built with ASP.NET Core MVC. It provides a full-featured platform for browsing, purchasing, and managing products, designed to be scalable and easy to maintain. This project demonstrates key principles of modern web development using the .NET ecosystem.

🌟 Features
Product Catalog: Browse products by category with detailed descriptions, images, and pricing.

Shopping Cart: A fully functional shopping cart that allows users to add, update, and remove items.

User Authentication: Secure user registration and login system.

Order Management: A complete order processing workflow, from checkout to shipping.

Admin Panel: A dedicated area for administrators to manage products, categories, orders, and users.

Responsive Design: A clean and modern UI that works seamlessly across desktops, tablets, and mobile devices.

🛠️ Technologies Used
Backend: ASP.NET Core MVC

Database: Entity Framework Core with SQL Server

Frontend: HTML, CSS, Bootstrap

Authentication: ASP.NET Core Identity

Architecture: Model-View-Controller (MVC)

📋 Prerequisites
Before you begin, ensure you have the following installed on your system:

.NET 8 SDK (or the version used in the project)

SQL Server Express or another compatible SQL database.

Git

🚀 Getting Started
Follow these steps to get a local copy of the project up and running.

Clone the repository:

git clone [https://github.com/ahmedsaloma/Bulky.git](https://github.com/ahmedsaloma/Bulky.git)
cd Bulky

Configure the Database Connection:

Open the appsettings.json file in the main project directory.

Locate the ConnectionStrings section and modify the DefaultConnection string to point to your local SQL Server instance.

"ConnectionStrings": {
  "DefaultConnection": "Server=your_server_name;Database=BulkyDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}

Apply Database Migrations:

Open a terminal or command prompt in the project's root directory.

Run the following command to create the database and apply the initial schema:

dotnet ef database update

Run the Application:

You can run the application using the .NET CLI:

dotnet run

Alternatively, you can run it from Visual Studio or your preferred IDE.

Access the Application:

Once the application is running, open your web browser and navigate to https://localhost:5001 or http://localhost:5000.

📖 Usage
Browse Products: Navigate to the home page to see the list of available products.

Register/Login: Create a new account or log in with an existing one to start shopping.

Admin Access: To access the admin panel, you may need to assign an "Admin" role to a user directly in the database after registration.

🤝 Contributing
Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are greatly appreciated.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".

Fork the Project

Create your Feature Branch (git checkout -b feature/AmazingFeature)

Commit your Changes (git commit -m 'Add some AmazingFeature')

Push to the Branch (git push origin feature/AmazingFeature)

Open a Pull Request

