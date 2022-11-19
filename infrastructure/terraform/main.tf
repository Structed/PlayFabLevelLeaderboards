# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 2.80"
    }
  }

  backend "azurerm" {}
}

provider "azurerm" {
  features {}
}

locals {
  prefix = "pfmatchhistory"
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.prefix}-rg"
  location = var.location
}

resource "azurerm_storage_account" "storage" {
  name                     = replace("${var.prefix}storage", "-", "")
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_kind             = "StorageV2"
  account_tier             = "Standard"
  account_replication_type = "LRS"
  access_tier              = "Hot"
}

module "redis" {
  source         = "./redis"
  resource_group = azurerm_resource_group.rg
  tags           = var.tags
  prefix         = var.prefix
}

resource "azurerm_app_service_plan" "app_service_plan" {
  name                = "${var.prefix}-app-service-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

locals {
  function_name = "${var.prefix}-function"
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.prefix}-law"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "apinsights_publicapi" {
  name                = "${local.function_name}-appinsights"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  workspace_id        = azurerm_log_analytics_workspace.law.id
  application_type    = "web"
}

resource "azurerm_function_app" "function_publicapi" {
  name                       = local.function_name
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.storage.name
  storage_account_access_key = azurerm_storage_account.storage.primary_access_key

  enabled                = true
  enable_builtin_logging = true
  https_only             = true
  version                = "~4"

  # https://docs.microsoft.com/en-us/azure/azure-functions/functions-app-settings
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"  = "${azurerm_application_insights.apinsights_publicapi.instrumentation_key}"
    "WEBSITE_ENABLE_SYNC_UPDATE_SITE" = 1
    "WEBSITE_RUN_FROM_PACKAGE"        = 1
    "FUNCTIONS_WORKER_RUNTIME"        = "dotnet"
    "REDIS_CONNECTION_STRING"         = module.redis.connection_strings[0]
    "PlayFabTitleId"                  = var.pf_title_id
    "PlayFabDeveloperSecret"          = var.pf_developer_secret
  }

  site_config {
    ftps_state = "Disabled"
    //    ip_restriction = []
    min_tls_version = "1.2"
  }
}
