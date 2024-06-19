# Financial Instrument Price Service

## Overview

This project provides a service that offers REST API and WebSocket endpoints for live financial instrument prices sourced from a public data provider. It efficiently handles over 1,000 subscribers.

## Features

1. **REST API**:
   - **Get List of Instruments**: Endpoint to retrieve a list of available financial instruments.
   - **Get Current Price**: Endpoint to get the current price of a specific financial instrument.

2. **WebSocket Service**:
   - **Subscribe to Price Updates**: Subscribe to live price updates for specific financial instruments.
   - **Broadcast Price Updates**: Broadcast price updates to all subscribed clients.

3. **Data Source**:
   - Uses the Alpha Vantage API to fetch live price data.

4. **Performance**:
   - Efficiently manages 1,000+ WebSocket subscribers with a single connection to the data provider.

5. **Logging and Error Reporting**:
   - Implements event and error logging capabilities using Serilog.

## Technologies Used

- **.NET 8**
- **ASP.NET Core**
- **SignalR**
- **RestSharp**
- **Serilog**
- **Alpha Vantage API**

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- [Alpha Vantage API Key](https://www.alphavantage.co/support/#api-key)

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/kimtum/FinancialServices.git
   cd FinancialServices

2. **Open the project in your IDE (e.g., Visual Studio 2022)**.

3. **Set your Alpha Vantage API Key**:
   - Open `FinancialService.cs`.
   - Replace `"YOUR_ALPHA_VANTAGE_API_KEY"` with your actual Alpha Vantage API key.

4. **Restore dependencies**:
   - In Visual Studio, right-click on the solution in Solution Explorer and select **Restore NuGet Packages**.

5. **Build the project**:
   - Right-click on the solution and select **Build Solution**.

6. **Run the project**:
   - Press `F5` or click on the **Start Debugging** button.

## Usage

### REST API

1. **Get List of Instruments**:
   - **Endpoint**: `/financial/instruments`
   - **Method**: `GET`
   - **Description**: Returns a list of available financial instruments.

2. **Get Current Price**:
   - **Endpoint**: `/financial/price/{instrument}`
   - **Method**: `GET`
   - **Description**: Returns the current price of the specified financial instrument.

### WebSocket Service

1. **Connect to the WebSocket Hub**:
   - **URL**: `/priceHub`
   - **Description**: Connects to the WebSocket service for subscribing to price updates.

2. **Subscribe to Price Updates**:
   - **Method**: `Subscribe`
   - **Parameters**: `instrument` (string)
   - **Description**: Subscribes the client to receive updates for the specified financial instrument.

3. **Receive Price Updates**:
   - **Event**: `ReceivePriceUpdate`
   - **Parameters**: `instrument` (string), `price` (decimal)
   - **Description**: Receives live price updates for subscribed instruments.

## Logging

Logging is implemented using Serilog. Log messages are written to the console. To customize logging, modify the configuration in `Program.cs`.

