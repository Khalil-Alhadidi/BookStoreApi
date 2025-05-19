# BookStoreApi

## Overview
BookStoreApi is a .NET Core Web API project that provides endpoints for managing books and user authentication. It is built using the latest .NET 9.0 framework and follows a clean architecture approach.
Created by Cursor (90%) and the rest by me (Khalil ^__^).

## Features
- **Book Management:** CRUD operations for books.
- **User Authentication:** JWT-based authentication with refresh token support.
- **Health Checks:** Endpoint to check the health of the application.

## Prerequisites
- .NET 9.0 SDK
- SQL Server (or any compatible database)

## Getting Started
1. **Clone the Repository:**
   ```bash
   git clone <repository-url>
   cd BookStoreApi
   ```

2. **Restore Dependencies:**
   ```bash
   dotnet restore
   ```

3. **Update Database:**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```

5. **Access the API:**
   - The API will be available at `https://localhost:5136`.

## API Endpoints
- **GET /api/books:** Retrieve all books.
- **GET /api/books/{id}:** Retrieve a specific book by ID.
- **GET /api/books/search?title={title}:** Search for books by title.
- **POST /api/books:** Create a new book.
- **PUT /api/books/{id}:** Update an existing book.
- **DELETE /api/books/{id}:** Delete a book.
- **GET /health:** Check the health of the application.
- **POST /api/auth/refresh:** Refresh the authentication token.

## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

## License
This project is licensed under the MIT License. 