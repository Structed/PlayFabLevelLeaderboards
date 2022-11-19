resource "azurerm_redis_cache" "redis" {
  name                = "example-cache"
  location            = var.resource_group.location
  resource_group_name = var.resource_group.name
  capacity            = var.capacity
  family              = var.family
  sku_name            = var.sku_name
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"

  redis_configuration {
  }
}