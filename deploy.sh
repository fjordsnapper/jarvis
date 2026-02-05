#!/bin/bash
# Jarvis Azure Deployment Script
# This script deploys the infrastructure and application to Azure

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Configuration
RESOURCE_GROUP=${AZURE_RESOURCE_GROUP:-"jarvis-rg"}
LOCATION=${AZURE_LOCATION:-"eastus"}
APP_SERVICE_NAME=${APP_SERVICE_NAME:-"jarvis-app-dev"}
BASE_NAME=${BASE_NAME:-"jarvis"}
ENVIRONMENT=${ENVIRONMENT:-"dev"}

print_status() {
  echo -e "${GREEN}[INFO]${NC} $1"
}

print_error() {
  echo -e "${RED}[ERROR]${NC} $1"
}

print_warning() {
  echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
  print_status "Checking prerequisites..."

  if ! command -v az &> /dev/null; then
    print_error "Azure CLI not found. Please install it from https://learn.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
  fi

  if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK not found. Please install it from https://dotnet.microsoft.com/download"
    exit 1
  fi

  print_status "Prerequisites check passed"
}

# Authenticate with Azure
authenticate() {
  print_status "Checking Azure authentication..."

  if ! az account show &> /dev/null; then
    print_status "Not authenticated. Running 'az login'..."
    az login
  else
    current_user=$(az account show -o tsv --query user.name)
    print_status "Already authenticated as: $current_user"
  fi
}

# Create resource group
create_resource_group() {
  print_status "Creating resource group: $RESOURCE_GROUP"

  if az group exists --name "$RESOURCE_GROUP" | grep -q false; then
    az group create \
      --name "$RESOURCE_GROUP" \
      --location "$LOCATION"
    print_status "Resource group created: $RESOURCE_GROUP"
  else
    print_status "Resource group already exists: $RESOURCE_GROUP"
  fi
}

# Deploy infrastructure
deploy_infrastructure() {
  print_status "Deploying infrastructure..."

  az deployment group create \
    --resource-group "$RESOURCE_GROUP" \
    --template-file ./infra/main.bicep \
    --parameters ./infra/parameters.json \
    --parameters baseName="$BASE_NAME" environment="$ENVIRONMENT" location="$LOCATION"

  print_status "Infrastructure deployed successfully"
}

# Build and publish application
build_application() {
  print_status "Building .NET application..."

  dotnet build src/Jarvis.Api/Jarvis.Api.csproj --configuration Release

  print_status "Publishing application..."

  dotnet publish src/Jarvis.Api/Jarvis.Api.csproj \
    --configuration Release \
    --output ./publish \
    --no-build

  print_status "Application build and publish completed"
}

# Deploy application
deploy_application() {
  print_status "Deploying application to App Service: $APP_SERVICE_NAME"

  # Create ZIP package
  cd ./publish
  zip -r ../jarvis-app.zip .
  cd ..

  # Get the App Service name from the Bicep output
  app_service=$(az deployment group show \
    --resource-group "$RESOURCE_GROUP" \
    --name main \
    -o tsv --query properties.outputs.appServiceList.value 2>/dev/null || echo "$APP_SERVICE_NAME")

  # Deploy the ZIP to App Service
  az webapp deployment source config-zip \
    --resource-group "$RESOURCE_GROUP" \
    --name "$app_service" \
    --src-path ./jarvis-app.zip

  print_status "Application deployed successfully"
}

# Run tests
run_tests() {
  print_status "Running tests..."

  dotnet test tests/Jarvis.Tests/Jarvis.Tests.csproj \
    --configuration Release \
    --verbosity normal

  print_status "Tests completed"
}

# Main deployment flow
main() {
  echo "========================================="
  echo "Jarvis Azure Deployment Script"
  echo "========================================="
  echo ""
  echo "Configuration:"
  echo "  Resource Group: $RESOURCE_GROUP"
  echo "  Location: $LOCATION"
  echo "  Environment: $ENVIRONMENT"
  echo "  Base Name: $BASE_NAME"
  echo ""

  check_prerequisites
  authenticate
  create_resource_group

  read -p "Run tests? (y/n) " -n 1 -r
  echo
  if [[ $REPLY =~ ^[Yy]$ ]]; then
    run_tests
  fi

  read -p "Deploy infrastructure? (y/n) " -n 1 -r
  echo
  if [[ $REPLY =~ ^[Yy]$ ]]; then
    deploy_infrastructure
  fi

  read -p "Build and deploy application? (y/n) " -n 1 -r
  echo
  if [[ $REPLY =~ ^[Yy]$ ]]; then
    build_application
    deploy_application
  fi

  echo ""
  echo "========================================="
  print_status "Deployment process completed!"
  echo "========================================="
}

# Run main function
main
