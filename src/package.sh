#!/bin/sh -e

base="$(dirname "$0")"

# Retreive latest version from current HEAD
version="$(git --work-tree "$base" tag --points-at HEAD)"

if [ -z "$version" ]; then
	echo >&2 "error: current HEAD doesn't point to a tag"
	exit 1
fi

dotnet pack -c Release "$base"/Cottle
