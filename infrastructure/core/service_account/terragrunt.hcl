include "root" {
  path = find_in_parent_folders("root.hcl")
}

include "service_account" {
  path = "${dirname(find_in_parent_folders("root.hcl"))}/_envcommon/service_account.hcl"
}

inputs = {
  service_account_id    = "artifact-registry"
  service_account_display_name = "Artifact Registry"
}