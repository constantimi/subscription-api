# Clean Architecture API

This API manages SMS-based subscription services, enabling customers to subscribe, unsubscribe, and view their active subscriptions while applying promotional discounts.

## Table of Contents

| Section                                               | Description                                           |
| ----------------------------------------------------- | ----------------------------------------------------- |
| [1. Overview](#overview)                              | Introduction to the Subscription API and its purpose. |
| [2. Key Features](#key-features)                      | Main functionalities of the API.                      |
| [3. Endpoints](#endpoints)                            | API endpoints and their usage.                        |
| [4. Discount Rules](#discount-rules)                  | Rules for applying discounts.                         |
| [5. Architecture & Patterns](#architecture--patterns) | Design principles and patterns used.                  |
| [6. Prerequisites](#prerequisites)                    | Requirements to run the API.                          |
| [7. Getting Started](#getting-started)                | Steps to set up and run the API.                      |
| [8. Testing](#testing)                                | Testing strategies and edge cases.                    |
| [9. Example Scenarios](#example-scenarios)            | Sample use cases and expected outcomes.               |

## Overview

The Subscription API is built using .NET 8, following Clean Architecture principles. It separates core logic, application workflows, and infrastructure concerns, ensuring scalability and maintainability.

### Purpose

The API handles:

-   Subscription management for services.
-   Automatic discount calculation based on defined rules.
-   Idempotent operations to prevent duplicate requests.

## Key Features

1. **Subscription Management**

    - Add, remove, and query subscriptions.
    - Track active subscriptions and calculate total costs.

2. **Promotions and Discounts**

    - Automatically apply discounts based on defined rules.
    - Resolve overlapping discounts.

3. **Idempotency**

    - Ensures operations are not repeated unnecessarily.

4. **Extensibility**

    - Modular patterns allow easy addition of new discount rules or subscription types.

5. **Swagger Documentation**
    - Comprehensive API documentation with request/response examples.

## Endpoints

#### **`POST /subscribe`**

Subscribe a customer to a service.

-   **Input Parameters**:
    -   `customer_phone_number` (string): Unique customer identifier.
    -   `service_id` (string): Unique service identifier.
    -   `duration_months` (int): Number of months for the subscription.

#### **`POST /unsubscribe`**

Remove a customer's subscription.

-   **Input Parameters**:
    -   `customer_phone_number` (string): Unique customer identifier.
    -   `service_id` (string): Unique service identifier.

#### **`GET /subscription-summary`**

Retrieve a summary of the customer's active subscriptions.

-   **Input Parameters**:

    -   `customer_phone_number` (string): Unique customer identifier.

-   **Response**:
    -   List of subscribed services.
    -   Total cost before and after discounts.
    -   Itemized discount details.

## Discount Rules

1. **Service Pair Promotion**  
   Subscribe to **Health&Lifestyle** and **Magazines and News**, and get **Health&Lifestyle** free for one month.

2. **Quantity-Based Discounts**  
   Subscribe to **3 or more services**, and receive a **10% discount** on the total cost.

3. **Bundled Discounts**  
   Subscribe to **Gaming+** and **eLearning Portal**, and get a **flat €5 discount** on the combined cost.

4. **Upfront Subscription Bonus**  
   Subscribe to any service for **5 months upfront**, and get the **6th month free** for that service.

## Architecture & Patterns

### 1. Clean Architecture

-   **Domain**: Business logic and core entities.
-   **Application**: Use cases and workflow logic.
-   **Infrastructure**: External concerns like database interactions.
-   **API**: HTTP layer and controllers.

### 2. CQRS with MediatR

Command and Query Responsibility Segregation pattern separates read and update operations for a data store. Implementing CQRS in your application can maximize its performance, scalability, and security. The flexibility created by migrating to CQRS allows a system to better evolve over time and prevents update commands from causing merge conflicts at the domain level.

-   **Commands**: Handle state-changing operations (e.g., `SubscribeCommand`).
-   **Queries**: Handle read-only operations (e.g., `GetSubscriptionSummaryQuery`).

### 3. Repository and UnitOfWork Pattern

-   The repository and unit of work patterns are intended to create an abstraction layer between the data access layer and the business logic layer of an application. Implementing these patterns can ensure testability and separation of concerns

### 4. Middleware Pipeline

-   Handles global exceptions and idempotency checks.

## Prerequisites

-   **.NET 8 SDK**: Ensure .NET 8 is installed. [Download .NET 8 SDK](https://dotnet.microsoft.com/download/dotnet).
-   **Visual Studio or VS Code** (Optional): IDE support for .NET projects.

## Getting Started

1. **Install Dependencies**  
   Restore NuGet packages:

    ```bash
    dotnet restore SubscriptionAPI.sln
    ```

2. **Build the Project**  
   Compile the code:

    ```bash
    dotnet build SubscriptionAPI/SubscriptionAPI.csproj
    ```

3. **Run the Project**  
   Start the API:

    ```bash
    dotnet run --project SubscriptionAPI/SubscriptionAPI.csproj
    ```

4. **Access Swagger UI**  
   Interact with the API at [Swagger UI](http://localhost:5170/swagger).

## Testing

### Unit Tests

-   **Add Subscriptions**: Ensure customers can subscribe successfully.
-   **Remove Subscriptions**: Validate unsubscription logic.
-   **Query Subscriptions**: Test the subscription summary endpoint.
-   **Discount Rules**: Verify that discounts are applied correctly.

### Edge Cases

-   No active subscriptions.
-   Subscribing to the same service multiple times.
-   Overlapping discounts.

## Example Scenarios

### Scenario 1: Gaming+ and eLearning Portal

-   **Subscriptions**:
    -   Gaming+ (5 months).
    -   eLearning Portal (2 months).
-   **Expected**:
    -   Total Cost: €75 (Gaming+) + €20 (eLearning) = €95.
    -   Discounts: €15 (Upfront Bonus) + €5 (Bundled Discount).
    -   **Final Cost**: €75.

### Scenario 2: Health&Lifestyle, Magazines, Gaming+

-   **Subscriptions**:
    -   Health&Lifestyle (2 months).
    -   Magazines (1 month).
    -   Gaming+ (1 month).
-   **Expected**:
    -   Total Cost: €35.
    -   Discounts: €12 (Service Pair) + €2.30 (Quantity Discount).
    -   Final Cost: €20.70.
