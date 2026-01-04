# FullStack URL-Shortener

A full-stack URL Shortener application built with ASP.NET Core (Backend) and Angular (Frontend). 
Application allows users to shorten long URLs, manage their links, and admin features (e.g. change the about section about the algorithm).
Has a secure login system using Identity and a SQLite database.

---

## Tech Stack I used
* **Backend: ASP.NET Core Web API 9.0 (C#)**
* **Frontend: Angular 19+ (TypeScript, Bootstrap)**
* **Database: SQLite (Entity Framework Core Code-First approach)**
* **Testing: xUnit, Moq**
* **Authentication: ASP.NET Core Identity (Cookie-based Auth with CORS support)**

## Features
* **Public Access: View list of shortened URLs.**
* **User Accounts: Login (seeded users available)**
  * **Admin:**
    * **E-mail: admin@vitalii.com**
    * **Password: Vitalii123!**
  * **Plain User:**
    * **E-mail: user@vitalii.com**
    * **Password: User123!**
* **Role-Based Access Control:**
  * **Users: Create short links, delete only their own links, view info of their own links.**
  * **Admins: Delete any link, View any link, edit the "About" page content.**

* **Smart Redirection: Navigate to http://localhost:5152/{shortCode} to be redirected to the original URL.**
* **Duplicate Prevention: Prevents creating duplicate short codes for the same URL.**

## How to run the application?

### Prerequisites
* .NET 9.0 SDK
* Node.js (LTS version recommended)
* Angular CLI (`npm install -g @angular/cli`)

### 1. Running the Database
The project uses SQLite. You need to apply migrations to create the database file (app.db).
1. Navigate to the backend folder:
   ```bash
   cd URLShortener
   ```
2. Restore dependencies:
  ```bash
  dotnet restore
  ```
3. Update the database (Runs migrations & creates app.db):
   ```bash
   dotnet ef database update
   ```
   Note: If you don't have the EF tool installed, run: 
   ```bash
   dotnet tool install --global dotnet-ef
   ```
4. Run the Backend Server:
   ```bash
   dotnet run
   ``` 
The API will start at http://localhost:5152.
*Note: On the first run, the app will automatically seed default users.*



