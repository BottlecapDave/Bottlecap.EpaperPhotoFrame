resource "google_project_service" "secretmanager" {
  service                    = "secretmanager.googleapis.com"
  disable_dependent_services = false
  disable_on_destroy         = false
}

resource "google_storage_bucket" "default" {
  name          = var.storage_bucket_name
  location      = var.storage_bucket_location
  force_destroy = true
}

resource "google_service_account" "service_account" {
  account_id   = var.storage_bucket_name
  display_name = var.storage_bucket_name
}

resource "google_storage_bucket_iam_binding" "binding" {
  bucket  = google_storage_bucket.default.name
  role    = "roles/storage.objectViewer"
  members = ["serviceAccount:${google_service_account.service_account.email}"]
}

resource "google_service_account_key" "key" {
  service_account_id = google_service_account.service_account.name
}

resource "google_secret_manager_secret" "secret" {
  secret_id = "GOOGLE_STORAGE_SERVICE_ACCOUNT"

  replication {
    automatic = true
  }

  depends_on = [google_project_service.secretmanager]
}


resource "google_secret_manager_secret_version" "version" {
  secret = google_secret_manager_secret.secret.id

  secret_data = google_service_account_key.key.private_key
}