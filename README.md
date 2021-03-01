# PaymentGatewayAPI
Payment Gateway API project acts as a gateway to process payments using Bank Client.
* paymentgatewayAPI/swagger gives the API documentation.
* paymentgatewayAPI/metrics shows the metrics for the API use.

Note : The BankClient is mocked and just returns dummy Payment details.

## PaymentGatewayAPI.Client
This is a console applicaiton that communicates with both the identity Server and PaymentGatewayAPI to get and post payments.

## How to Run the projects
Startup multiple projects : 
* Bank.API - Mock API to process payments
* IdentityServer - grants auth tokens for accessing PaymentGateway.API
* PaymentGateway.API - Accepts payment requests that are authorised
* PaymentGateway.Client - This connects to the PaymentGateway.API to get and post dummy payments.


