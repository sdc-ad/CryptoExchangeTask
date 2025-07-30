# Crypto Exchange Task

## Projects
This solution consists of the following projects:

### CryptoExchangeTask.Core
Contains the code to read the exchange JSON and to calculate the order plans based on a user request

### CryptoExchangeTask.Core.UnitTests
Contains unit tests for the calculation algorithm

### CryptoExchangeTask.Console
A simple console wrapper around the core calculation. The requested order is passed on the command line in the form: `<buy|sell> <amount>`.

For example:
* dotnet run buy 1.2
* CryptoExchangeTask.Console.exe sell 0.5

The exchange JSON files are copied into the Data folder by the build. You can replace these files after building for different exchange data.

### CryptoExchangeTask.WebApi
A simple HTTP wrapper around the core calculation. Requests are of the form `GET /plan/<buy|sell>?amount=<amount>`.

For example:
* GET /plan/buy?amount=1.2
* GET /plan/sell?amount=0.5

The project also serves an Open API JSON on `/openapi/v1.json` and the file `CryptoExchangeTask.WebAPI.http` can be used to submit requests to the service too.

## Docker Support
The Web API project contains a Dockerfile and there is a docker-compose file at the root of this solution. Running `docker compose up` in this folder will build and run the Web API in Docker. It will expose the service on ports 8080 (http) and 8081 (https).

A self signed HTTPS certificate is already included in this repository.