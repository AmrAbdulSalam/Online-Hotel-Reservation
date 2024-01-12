# Travel and Accommodation Booking Platform using ASP.NET with Clean Architecture
The project is an ASP.NET hotel reservation system that employs the 
Clean Architecture design principles.
It utilizes Entity Framework Core and API to facilitate functionalities for managing
hotels, rooms, reservations, payments, and user accounts.
This project aims to provide a solid foundation for a comprehensive travel and accommodation booking platform.

## Table of Contents
1.   [Features](#features)
2.   [Architecture Overview](#architecture-overview)
3.   [Admin Functionality](#admin-functionality)
4.   [Reservation and Payment Handling](#reservation-and-payment-handling)
5.   [Hotel Reservation Database ERD](#hotel-reservation-database-erd)
6.   [Getting Started](#getting-started)
7.   [Dependencies](#dependencies)
8.   [Usage](#usage)
9.   [Testing](#testing)
10.  [Contributing](#contributing)

## Features
The ASP.NET Library API offers the following key features:

- **Hotel Management**: Admins can create, update, and view hotel details including name, location, description, and images.
- **Room Management**: Admins can manage room inventory, types, capacities, and pricing.
- **City Information**: Admins can add, edit, and view details of cities including name, country, post office, and currency.
- **Reservation Handling**: Users can make reservations, view booking details, and manage bookings.
- **Payment Processing**: Handle payment transactions for reservations made by users.
- **Multi-Currency Support**: Manage different currencies for various cities.

-   **Authentication**: Secure API endpoints using JWT authentication
JSON Web Token (JWT) was used rather than sessions because it's more simple since it's stateless and more suitable for APIs,
Since there's no need to connect to a central Authentication provider to make sure if the user is Authenticated
 
- **Authorization**: Secure API endpoints by Roles (User,Admin) and policies.
  
- **Validation**: Perform input validation to ensure data integrity : Using fluent validation

- Encryption Service :<br>
  Encrypt: Utilizes AES encryption to transform plaintext into an encrypted format.<br>
  Decrypt: Performs decryption of encrypted text to retrieve the original plaintext.

- **PDF Creator**:<br>

  The PDF Creator facilitates the generation of comprehensive PDF documents related to reservations, offering features like:
  - **Booking Confirmations**: Generates detailed booking confirmations for users.
  - **Invoice Generation**: Creates invoices for confirmed reservations.
 
- **Email Service**:<br>

  The Email Service enables seamless communication by providing functionalities such as:
  - **Reservation Confirmation Emails**: Automatically sends confirmation emails to users upon successful reservation.
  - PDF **Generation**: Sends an email with all reservation deatils inside the PDF as the following picture :

    ![Screen Shot 2024-01-10 at 10 58 36 PM](https://github.com/AmrAbdulSalam/Online-Hotel-Reservation/assets/46429001/9c5a0e66-0bc8-4aa0-a84b-3b5393915d19)


## Architecture Overview

The ASP.NET Library API follows the Clean Architecture principles, which promote a clear separation of concerns and modular design. The architecture is organized into the following layers:

1.  **Presentation Layer**: This layer contains the API controllers responsible for handling incoming HTTP requests and returning appropriate responses. It is the entry point for external clients.
    
2.  **Application Layer**: This layer contains application-specific business logic and use cases. It coordinates the interactions between the Presentation and Domain layers.
    
3.  **Domain Layer**: This layer represents the core of the application and encapsulates the business rules and domain models. It contains modeles, value objects, and domain services.
    
4.  **Infrastructure Layer**: This layer provides implementation details and external dependencies. It includes database access, external services, and other infrastructure-related concerns.
    

The use of interfaces and dependency injection ensures loose coupling between layers, making it easier to replace or modify components without affecting other parts of the application.

 Clean Architecture offers many benefits to achieve separation of concerns, maintainability and testability on large and complex projects.

## Admin Functionality

The system includes an admin role that manages hotel-related information:

- **Hotel Management**: Admins can add, edit, and delete hotel details, including name, location, description, and images.
- **Room Management**: Admins can manage room inventory, types, capacities, and pricing for each hotel.
- **City Information**: Admins have access to add, edit, or remove city details, including currency information.


## **Reservation and Payment Handling**

Users can make reservations for hotel rooms, manage bookings, and handle payment transactions within the system. 
The application manages reservation details, including check-in/check-out dates, room types, and prices. 
Payment processing is facilitated for confirmed reservations.

## **Hotel Reservation Database ERD**

<img width="1119" alt="Screen Shot 2023-12-30 at 2 26 09 PM" src="https://github.com/AmrAbdulSalam/Online-Hotel-Reservation/assets/46429001/0117c973-5d40-4160-bc28-229ca145da2d">

## Getting Started

To get started with the ASP.NET Library API, follow these steps:

1.  Install Docker.

2.  Clone repository : `git clone https://github.com/AmrAbdulSalam/Online-Hotel-Reservation`.

3.  Navigate to the project directory: `cd .\HotelReservation`.

4.  Install the dependencies: `dotnet restore`.

5.  Set up the necessary information in the `appsettings.json` file.

6.  Run database migrations :  `dotnet ef database update`.

7.  Build and run the application :

- ##### for development  : `dotnet run`.

- ##### for production  : `docker compose up`.

8.  Use the API :

- ##### For Development  : `https://localhost:7072` 

- ##### For Production  : `https://localhost:8088` 


## Dependencies

The ASP.NET Library API has the following dependencies:

-   ASP.NET Core (version 6.X.X)
-   Entity Framework Core (version 6.X.X)
-   Swagger UI (version 3.0.X)
-   Other NuGet packages as specified in the project files.

## Usage
Once the API is up and running, you can access the available endpoints using a tool like cURL, Postman, or any other HTTP client. The API documentation and interactive testing can be accessed via Swagger UI, which is available at http://localhost:7072/swagger.

Before accessing the protected endpoints, make sure to obtain an authentication token by calling the appropriate authentication endpoint (`/api/authentication`). Include the token in the headers of subsequent requests for authorized operations.

## Testing
- **Unit Testing** : The services functionality were tested with mocked repositories using xUnit and Moq.

- **API-Integration Testing** : The controllers - api end points was tested through HTTP Client using xUnit.


## Contributing

Contributions to the ASP.NET Library API are welcome! If you find any issues or have suggestions for improvements, please submit a pull request or create an issue on the project repository.
