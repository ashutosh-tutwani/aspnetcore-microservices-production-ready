# aspnetcore-microservices-production-ready
### Comming Soon, Stay Tuned
Production ready .NET 6 microservices integrated with OpenAPI. 

## Cloud Provider
### We have used cloud agnostic approach to build/deploy the app. Means you can leverage any cloud provider you like (AWS, Azure, GCP, etc..).
This app uses AWS as the default Cloud Provider. 
But you can change the cloud provider with just one single change in appsettings.

AWS Services used: SNS, SQS, Parameter Store/ Secrets Manager, Cloudwatch


## Identity (Authentication and Autharization): 
This app uses OIDC protocol with OAuth 2.0.
For Identity Provider its been configured to use Auth0 (by Okta)

## Messaging:
This application uses AWS SNS topics and AWS SQS as a messaging broker.
AWS SNS is used to fan out the events/messages to different consumers or subscribers. Whereas AWS SQS is a message queue service.

## Observaibility:
This application uses OpenTelemetry to collect telemetry data from different distributed systems in order to troubleshoot, debug and manage applications.
For logging, this application uses Serilog.

## Search:
The application is using ElasticSearch as a search engine.

## Tests:
XUnit and Moq are being used as Unit testing framework.

## Services
### User Management API

### Reporting API

## Background Services (Workers)

## Deployment
This app is deployed using AWS EKS. 

