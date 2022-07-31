setup-dotnet:
	asdf plugin add dotnet-core || true

setup-terraform:
	asdf plugin add terraform || true

setup-terragrunt:
	asdf plugin add terragrunt || true

setup: setup-dotnet setup-terraform setup-terragrunt
	asdf install

start: setup