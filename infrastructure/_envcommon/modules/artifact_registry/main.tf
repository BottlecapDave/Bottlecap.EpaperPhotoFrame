resource "google_project_service" "api" {
  service = "artifactregistry.googleapis.com"
  disable_dependent_services = false
  disable_on_destroy = false
}

resource "google_artifact_registry_repository" "default" {
  location      = var.artifact_registry_location
  repository_id = var.artifact_registry_id
  format        = "DOCKER"

  depends_on = [
    google_project_service.api
  ]
}

resource "google_artifact_registry_repository_iam_member" "member" {
  project = google_artifact_registry_repository.default.project
  location = google_artifact_registry_repository.default.location
  repository = google_artifact_registry_repository.default.name
  role = "roles/artifactregistry.repoAdmin"
  member = "serviceAccount:${var.artifact_registry_service_account_email}"
}