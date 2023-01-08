resource "google_project_service" "api" {
  service = "run.googleapis.com"
  disable_dependent_services = true
  disable_on_destroy = false
}

resource google_service_account service_account {
  account_id   = var.cloud_run_name
  display_name = var.cloud_run_name
}

resource "google_cloud_run_service" "default" {
  name     = var.cloud_run_name
  location = var.cloud_run_location
  template {
    spec {
      containers {
          image = var.cloud_run_image_name
      }

      service_account_name  = google_service_account.service_account.email
    }
  }
  traffic {
    percent         = 100
    latest_revision = true
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
  location    = google_cloud_run_service.default.location
  project     = google_cloud_run_service.default.project
  service     = google_cloud_run_service.default.name

  policy_data = data.google_iam_policy.noauth.policy_data
}