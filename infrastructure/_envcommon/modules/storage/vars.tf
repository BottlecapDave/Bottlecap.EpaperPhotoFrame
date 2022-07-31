variable storage_bucket_name {
  type = string
  description = "The name of the bucket"
}

variable storage_bucket_location {
  type = string
  description = "The location the bucket should be created within"
}

variable storage_bucket_service_account_emails {
  type = list(string)
  description = "The emails that should have read access to the bucket"
}