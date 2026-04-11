#!/usr/bin/env bash

set -e

declare -r NEXUS_API_URL="https://api.nexusmods.com/v3"
declare -r BUILD_IL2CPP="Il2Cpp"
declare -r BUILD_MONO="Mono"

if [ -z "$1" ] || [ -z "$2" ] || [ -z "${NEXUS_API_KEY}" ]; then
  echo "Nope.  Try again." >&2
  exit 1
fi

declare -r MOD_NAME="$1"
declare -r MOD_VERSION="$2"

declare -r TARGET_DIR_PATH="target/${MOD_NAME}/nexus";


function getUploadGroupId() {
  jq -r ".groupIds.$(tr A-Z a-z <<< $1)" mods/${MOD_NAME}/meta/nexus.json \
    | tr -d '\n'
}

function getUploadID() {
  local -r buildType="$1"

  jq -r '.data.id' upload-response-${MOD_NAME}-${buildType}.json | tr -d '\n'
}

function createUpload() {
  local -r buildType="$1"

  local -r fileName="${MOD_NAME}-${buildType}-${MOD_VERSION}.zip"
  local -ri fileSize=$(stat -c%s "${TARGET_DIR_PATH}/${fileName}" | tr -d '\n')

  curl -s \
       -XPOST \
       -H"apikey: ${NEXUS_API_KEY}" \
       -H"Content-Type: application/json" \
       -d"$(jq -c --arg name "${fileName}" --argjson size ${fileSize} '{ filename: $name, size_bytes: $size }' <<< '{}')" \
      "${NEXUS_API_URL}/uploads" \
      > upload-response-${MOD_NAME}-${buildType}.json
}

function createUpdateGroup() {
  local -r buildType="$1"
  local -r groupId=$(getUploadGroupId $buildType)
  local -r groupName="${MOD_NAME}-${buildType}"
  local -r uploadId="$(getUploadID $buildType)"

  local -r requestBody="$( \
    jq -c \
       --arg upload "${uploadId}" \
       --arg name "${groupName}" \
       --arg version "$(tr -d 'v' <<< ${MOD_VERSION})" \
       '{ upload_id: $upload, name: $name, version: $version, file_category: "main" }' \
       <<< '{}' \
  )"

  echo $requestBody

#  curl -s \
#       -XPOST \
#       -H"apikey: ${NEXUS_API_KEY}" \
#       -H"Content-Type: application/json" \
#       -d"$(jq -c --arg name "${groupName}" --arg )"
}

function getPresignedURL() {
  local -r buildType="$1"

  jq -r '.data.presigned_url' upload-response-${MOD_NAME}-${buildType}.json | tr -d '\n'
}

createUpdateGroup $BUILD_MONO
