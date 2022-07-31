resource google_storage_bucket default {
  name          = var.storage_bucket_name
  location      = var.storage_bucket_location
  force_destroy = true
}

resource "google_storage_bucket_iam_binding" "binding" {
  bucket = google_storage_bucket.default.name
  role = "roles/storage.objectViewer"
  members = var.storage_bucket_service_account_emails
}