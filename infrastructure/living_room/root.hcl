generate "provider" {
  path      = "provider.tf"
  if_exists = "overwrite_terragrunt"
  contents  = <<EOF
provider "google" {
  project = "${local.project}"
  region = "${local.location}"
}
EOF
}

remote_state {
  backend = "gcs"
  config = {
    project = local.project
    bucket  = local.bucket_name
    location = local.location
    prefix   = "${basename(get_parent_terragrunt_dir())}/${path_relative_to_include()}"
  }
  generate = {
    path      = "backend.tf"
    if_exists = "overwrite_terragrunt"
  }
}

locals {
  location = "europe-west1"
  region = "eu"
  bucket_name = "bottlecap_infrastructure"
}