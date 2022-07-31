include "root" {
  path = find_in_parent_folders("root.hcl")
  expose = true
}

include "storage" {
  path = "${dirname(find_in_parent_folders("root.hcl"))}/../_envcommon/storage.hcl"
}

dependency "service_account" {
  config_path = "../service_account"

  mock_outputs = {
    email = "temp_email"
  }
}

inputs = {
  storage_bucket_name = "living-room-picture-frame-contents"
  storage_bucket_location = include.root.locals.location
  storage_bucket_service_account_emails = ["serviceAccount:${dependency.service_account.outputs.email}"]
}