# Customer Orders API

This is a simple API for managing customer orders.

## Features

- Create a new order
- Get an order
- Get all orders for a specific customer
- Update an existing order
  
## Built With

- [ASP.NET Core Web API]([https://www.fastify.io/](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api)): controller-based web API

## API Endpoints

* POST /orders: Create a new order
* GET /orders/{orderId}: Get an individual order
* PUT /orders/{orderId}: Update an existing order
* GET /orders?customerId={customerId}: Get all orders for a specific customer
