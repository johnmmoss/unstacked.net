terraform {

      required_providers {
        azurerm = {
          source  = "hashicorp/azurerm"
          version = "~> 4.61.0"
        }
      }
      
      backend "azurerm" {
        resource_group_name  = "unstacked-appconfig-rg-dev"
        storage_account_name = "tfstatestorage548166923"
        container_name       = "tfstate"
        key                  = "terraform.tfstate"
      }
  }

    provider "azurerm" {
      features {}
    }

    data "azurerm_resource_group" "rg" {
      name = "unstacked-appconfig-rg-dev"
    }

    resource "azurerm_service_plan" "plan" {
      name                = "AppConfigWebPlan"
      resource_group_name = data.azurerm_resource_group.rg.name
      location            = data.azurerm_resource_group.rg.location
      sku_name            = "F1"
      os_type             = "Windows"
    }

    resource "azurerm_app_configuration" "appconfig" {
      name                = "appconfigwebdev"
      resource_group_name = data.azurerm_resource_group.rg.name
      location            = data.azurerm_resource_group.rg.location
      sku                 = "free"
    }

    resource "azurerm_windows_web_app" "web" {
      name                = "AppConfigWeb"
      resource_group_name = data.azurerm_resource_group.rg.name
      location            = azurerm_service_plan.plan.location
      service_plan_id     = azurerm_service_plan.plan.id

      identity {
        type = "SystemAssigned"
      }

      app_settings = {
        "AppConfig:Endpoint" = azurerm_app_configuration.appconfig.endpoint
      }

      site_config {
		always_on           		= false
        
			application_stack {
			  current_stack = "dotnet"
			  dotnet_version = "v8.0" 
			}
		}
    }

    resource "azurerm_role_assignment" "web_appconfig_data_reader" {
      scope                = azurerm_app_configuration.appconfig.id
      role_definition_name = "App Configuration Data Reader"
      principal_id         = azurerm_windows_web_app.web.identity[0].principal_id

      depends_on = [azurerm_windows_web_app.web]
    }

    output "app_configuration_endpoint" {
      value = azurerm_app_configuration.appconfig.endpoint
    }

    output "app_configuration_name" {
      value = azurerm_app_configuration.appconfig.name
    }