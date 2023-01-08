resource "google_project_service" "api" {
  service                    = "run.googleapis.com"
  disable_dependent_services = true
  disable_on_destroy         = false
}

resource "google_service_account" "service_account" {
  account_id   = var.cloud_run_name
  display_name = var.cloud_run_name
}

locals {
  service_account_roles = ["roles/run.invoker", "roles/secretmanager.secretAccessor"]
}

resource "google_project_iam_member" "service_account_iams" {
  count   = length(local.service_account_roles)
  project = google_cloud_run_service.default.project
  role    = local.service_account_roles[count.index]
  member  = "serviceAccount:${google_service_account.service_account.email}"
}

resource "google_cloud_run_service" "default" {
  name     = var.cloud_run_name
  location = var.cloud_run_location
  template {
    spec {
      containers {
        image = var.cloud_run_image_name
      }

      service_account_name = google_service_account.service_account.email
    }
  }
  traffic {
    percent         = 100
    latest_revision = true
  }

  lifecycle {
    ignore_changes = [
      template[0].spec[0].containers[0].image,
      template[0].spec[0].containers[0].env,
    ]
  }

  depends_on = [
    google_project_service.api
  ]
}

data "google_iam_policy" "noauth" {
  binding {
    role = "roles/run.invoker"
    members = [
      "allUsers",
    ]
  }
}

resource "google_cloud_run_service_iam_policy" "noauth" {
  location = google_cloud_run_service.default.location
  project  = google_cloud_run_service.default.project
  service  = google_cloud_run_service.default.name

  policy_data = data.google_iam_policy.noauth.policy_data
}