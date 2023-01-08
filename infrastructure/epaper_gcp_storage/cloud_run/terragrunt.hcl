include "root" {
  path = find_in_parent_folders("root.hcl")
  expose = true
}

include "cloud_run" {
  path = "${dirname(find_in_parent_folders("root.hcl"))}/_envcommon/cloud_run.hcl"
}

inputs = {
  cloud_run_name = "gcp-picture-frame"
  cloud_run_location = include.root.locals.location
  cloud_run_image_name = "gcr.io/cloudrun/hello"
}