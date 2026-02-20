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

    resource "azurerm_windows_web_app" "web" {
      name                = "AppConfigWeb"
      resource_group_name = data.azurerm_resource_group.rg.name
      location            = azurerm_service_plan.plan.location
      service_plan_id     = azurerm_service_plan.plan.id

      site_config {}
    }