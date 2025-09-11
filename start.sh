#!/bin/bash
set -e

command_exists() {
    command -v "$1" >/dev/null 2>&1
}

DEPENDENCIES=("dotnet" "git")
for dep in "${DEPENDENCIES[@]}"; do
    if ! command_exists "$dep"; then
        echo "Error: $dep is not installed. Please install it first."
        exit 1
    fi
done

cd Wyvern.API || { echo "Error: Could not enter Wyvern.API directory"; exit 1; }

dotnet restore
dotnet run
