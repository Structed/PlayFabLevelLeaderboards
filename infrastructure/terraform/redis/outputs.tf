output "connection_strings" {
  sensitive = true
  value     = azurerm_redis_cache.redis.primary_connection_string
}