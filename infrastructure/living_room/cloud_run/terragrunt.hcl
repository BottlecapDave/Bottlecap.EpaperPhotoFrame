include "root" {
  path = find_in_parent_folders("root.hcl")
  expose = true
}

include "cloud_run" {
  path = "${dirname(find_in_parent_folders("root.hcl"))}/../_envcommon/cloud_run.hcl"
}

dependency "service_account" {
  config_path = "../service_account"

  mock_outputs = {
    email = "temp_email"
  }
}

inputs = {
  cloud_run_name = "living-room-picture-frame-contents"
  cloud_run_location = include.root.locals.location
  cloud_run_image_name = "gcr.io/cloudrun/hello"
  cloud_run_service_account_email = dependency.service_account.outputs.email
}