variable "resource_group" {
  description = "The resource group"
}

variable "prefix" {
  type        = string
  description = "Resource Name prefix (will be applied to all resource names)"
}

variable "tags" {
  type        = map(string)
  default     = {}
  description = "Tags to be added to the resource"
}

variable "capacity" {
  type        = number
  default     = 0
  description = "(Required) The size of the Redis cache to deploy. Valid values for a SKU family of C (Basic/Standard) are 0, 1, 2, 3, 4, 5, 6, and for P (Premium) family are 1, 2, 3, 4, 5."
}

variable "family" {
  type        = string
  default     = "C"
  description = "(Optional) The SKU family to use. Valid values are C (for Basic/Standard SKUs) and P (for Premium SKUs)."
}

variable "sku_name" {
  type        = string
  default     = "Basic"
  description = "(Optional) The type of Redis cache to deploy. Valid values are Basic, Standard, and Premium."
}