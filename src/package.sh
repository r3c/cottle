#!/bin/sh -e

base="$(dirname "$0")"

# Retrieve latest version from current HEAD
version="$(git --work-tree "$base" tag --points-at HEAD)"

if [ -z "$version" ]; then
	echo >&2 "warning: current HEAD doesn't point to a tag"
fi

dotnet pack -c Release "$base"/Cottle
