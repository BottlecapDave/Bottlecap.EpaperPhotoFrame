include "root" {
  path = find_in_parent_folders("root.hcl")
  expose = true
}

include "artifact_registry" {
  path = "${dirname(find_in_parent_folders("root.hcl"))}/../_envcommon/artifact_registry.hcl"
}

dependency "service_account" {
  config_path = "../service_account"

  mock_outputs = {
    email = "temp_email"
  }
}

inputs = {
  artifact_registry_location    = include.root.locals.location
  artifact_registry_id    = "living-room-picture-frame"
  artifact_registry_service_account_email = dependency.service_account.outputs.email
}