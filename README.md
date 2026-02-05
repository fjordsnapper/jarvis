# Jarvis

A .NET 10 REST API service, designed to be deployed on Azure App Service with infrastructure managed through Bicep.

## Overview

Jarvis is a modern REST API built with .NET 10, deployed to Azure App Service, and provisioned with infrastructure-as-code using Bicep templates. This project includes automated CI/CD workflows for building, testing, and deploying to Azure.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Git](https://git-scm.com/)
- Azure subscription with appropriate permissions

## Project Structure

```
Jarvis/
├── src/
│   └── Jarvis.Api/          # Main REST API project
├── tests/
│   └── Jarvis.Tests/        # Unit and integration tests
├── infra/
│   ├── main.bicep           # Main Bicep template
│   ├── parameters.json       # Parameter values for deployment
│   └── outputs.bicep        # Output definitions
├── .github/
│   └── workflows/
│       └── deploy.yml       # CI/CD pipeline
└── README.md                # This file
```

## Getting Started

### Local Development

1. Clone the repository:
```bash
git clone <repository-url>
cd Jarvis
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run tests:
```bash
dotnet test
```

5. Run the API locally:
```bash
cd src/Jarvis.Api
dotnet run
```

The API will be available at `https://localhost:5001` (or the configured port).

## Azure Deployment

### Prerequisites

1. Authenticate with Azure:
```bash
az login
```

2. Set your default subscription:
```bash
az account set --subscription <subscription-id>
```

### Deploy Infrastructure

Deploy the Bicep template to create Azure resources:

```bash
az deployment group create \
  --resource-group <resource-group-name> \
  --template-file infra/main.bicep \
  --parameters infra/parameters.json
```

### Deploy Application

Build and publish the application:

```bash
dotnet publish -c Release -o publish
```

Deploy to App Service:

```bash
az webapp deployment source config-zip \
  --resource-group <resource-group-name> \
  --name <app-service-name> \
  --src-path ./publish.zip
```

## CI/CD Pipeline

This repository includes a GitHub Actions workflow that automatically:
1. Builds the .NET application
2. Runs tests
3. Publishes artifacts
4. Deploys to Azure App Service

Configure the following GitHub secrets:
- `AZURE_SUBSCRIPTION_ID`: Your Azure subscription ID
- `AZURE_RESOURCE_GROUP`: Target resource group name
- `AZURE_APP_SERVICE_NAME`: App Service instance name

## Development

### Adding Features

- Add endpoints in `src/Jarvis.Api/Controllers/`
- Add business logic in `src/Jarvis.Api/Services/`
- Write tests in `tests/Jarvis.Tests/`

### Running Tests

```bash
dotnet test
```

## Contributing

Please follow these guidelines:
1. Create a feature branch from `main`
2. Make your changes
3. Add/update tests as needed
4. Submit a pull request with a clear description

## Infrastructure

The Azure infrastructure is defined in Bicep templates:
- **App Service Plan**: Hosts the web application
- **App Service**: Runs the REST API
- **Storage Account**: For persistent data storage
- **Application Insights**: For monitoring and diagnostics

## Monitoring

Application Insights is integrated for monitoring:
- View logs: `az webapp log tail --resource-group <rg> --name <app-name>`
- Check metrics in Azure Portal

## Troubleshooting

### Authentication Issues
```bash
az login
az account show
```

### Deployment Failures
```bash
az webapp deployment slot swap --resource-group <rg> --name <app-name>
az webapp log download --resource-group <rg> --name <app-name>
```

## License

[Specify your license here]

## Support

For issues, questions, or feedback, please create an issue in the GitHub repository.
