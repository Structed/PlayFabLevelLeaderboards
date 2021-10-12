﻿variable "location" {
  type        = string
  description = "The location in which to create all resources"
  default     = "westeurope"
}

variable "prefix" {
  type        = string
  description = "Resource Name prefix (will be applied to all resource names, including the resource group"
  default     = "pfmatchhistory"
}

variable "tags" {
  type    = map(string)
  default = {}
}

variable "sp_client_id" {
  type        = string
  description = "Service Principal ID which is used to run the API"
}