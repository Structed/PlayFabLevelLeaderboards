output "primary_connection_string" {
  sensitive = true
  value     = azurerm_redis_cache.redis.primary_connection_string
}