include "root" {
  path = find_in_parent_folders("root.hcl")
  expose = true
}

include "storage" {
  path = "${dirname(find_in_parent_folders("root.hcl"))}/_envcommon/storage.hcl"
}

inputs = {
  storage_bucket_name = "picture-frame-contents"
  storage_bucket_location = include.root.locals.location
}