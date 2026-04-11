PROJECT_ID := $(shell basename ${PWD})
VERSION := $(shell grep 'MelonInfo' src/$(PROJECT_ID)/Mod.cs | cut -d'"' -f2 | tr -d '\n')

MOD_URL := https://github.com/Foxcapades/schedule-1-mods

LIFECYCLE := DEBUG
CONFIGURATION := Il2Cpp

LOWER_CONFIG := $(shell echo $(CONFIGURATION) | tr A-Z a-z)

OUTPUT_DLL := $(PROJECT_ID).$(CONFIGURATION).dll

TARGET_DIRECTORY := $(shell realpath ${PWD}/../../target)/$(PROJECT_ID)
INSTALL_DIRECTORY := ${INSTALL_DIRECTORY}

CLEAN_TARGETS := bin obj

ifeq ($(CONFIGURATION),Il2Cpp)
	NET_PATH := net6.0
else
	NET_PATH := netstandard2.1
endif

INPUT_FILES := $(shell find src -type f -name '*.cs') $(shell find ../../lib/Fxcpds -type f -name '*.cs')

BUILD_TARGET := bin/$(CONFIGURATION)/$(NET_PATH)/$(PROJECT_ID).dll
OUTPUT_TARGET := $(TARGET_DIRECTORY)/$(OUTPUT_DLL)

ZIP_NAME := $(PROJECT_ID)-$(CONFIGURATION)-v$(VERSION).zip

.PHONY: default
default:
	@echo "PROJECT_ID        = $(PROJECT_ID)"
	@echo "VERSION           = $(VERSION)"
	@echo "LIFECYCLE         = $(LIFECYCLE)"
	@echo "CONFIGURATION     = $(CONFIGURATION)"
	@echo "LOWER_CONFIG      = $(LOWER_CONFIG)"
	@echo "OUTPUT_DLL        = $(OUTPUT_DLL)"
	@echo "TARGET_DIRECTORY  = $(TARGET_DIRECTORY)"
	@echo "INSTALL_DIRECTORY = $(INSTALL_DIRECTORY)"
	@echo "NET_PATH          = $(NET_PATH)"
	@echo "INPUT_FILES       = $(INPUT_FILES)"
	@echo "BUILD_TARGET      = $(BUILD_TARGET)"
	@echo "OUTPUT_TARGET     = $(OUTPUT_TARGET)"
	@echo "NM_RELEASE_ZIP    = $(NM_RELEASE_ZIP)"

################################################################################
#
# General Build
#
################################################################################

.PHONY: build
build: $(OUTPUT_TARGET)

.PHONY: package
package: $(NM_RELEASE_ZIP)

.PHONY: build-mono
build-mono:
	@$(MAKE) CONFIGURATION=Mono LIFECYCLE=$(LIFECYCLE) build

.PHONY: package-mono
package-mono:
	@$(MAKE) CONFIGURATION=Mono LIFECYCLE=$(LIFECYCLE) package

.PHONY: build-il2cpp
build-il2cpp:
	@$(MAKE) CONFIGURATION=Il2Cpp LIFECYCLE=$(LIFECYCLE) build

.PHONY: package-il2cpp
package-il2cpp:
	@$(MAKE) CONFIGURATION=Il2Cpp LIFECYCLE=$(LIFECYCLE) package

.PHONY: clean
clean:
	@rm -rf $(CLEAN_TARGETS)

.PHONY: install
install: build
	@if [ ! -z "$(INSTALL_DIRECTORY)" ]; then \
		cp $(OUTPUT_TARGET) "$(INSTALL_DIRECTORY)"; \
		echo "Installing $(notdir $(OUTPUT_TARGET))"; \
	fi

$(BUILD_TARGET): $(INPUT_FILES)
	@echo "building $@ using configuration $(CONFIGURATION) for lifecycle $(LIFECYCLE)"
	@dotnet build --configuration $(CONFIGURATION) --property:Lifecycle=$(LIFECYCLE) --verbosity m

$(OUTPUT_TARGET): $(BUILD_TARGET)
	@mkdir -p $(TARGET_DIRECTORY)
	@cp $(BUILD_TARGET) $(OUTPUT_TARGET)

################################################################################
#
# Nexus Mods
#
################################################################################

NM_DIR          := $(TARGET_DIRECTORY)/nexus
NM_TARGET_DIR   := $(NM_DIR)/$(LOWER_CONFIG)
NM_DLL          := $(NM_TARGET_DIR)/$(OUTPUT_DLL)
NM_ZIP_CONTENTS := $(OUTPUT_DLL)
NM_RELEASE_ZIP  := $(NM_DIR)/$(ZIP_NAME)

.PHONY: nm-clean
nm-clean: clean
	@rm -rf $(NM_DIR)

.PHONY: nm-package
nm-package: $(NM_RELEASE_ZIP)

.PHONY: nm-release
nm-release: nm-clean nm-release-dirty

.PHONY: nm-release-dirty
nm-release-dirty:
	@$(MAKE) --no-print-directory CONFIGURATION=Il2Cpp LIFECYCLE=RELEASE nm-package
	@$(MAKE) --no-print-directory CONFIGURATION=Mono   LIFECYCLE=RELEASE nm-package


$(NM_TARGET_DIR):
	@mkdir -p $@

$(NM_DLL): $(OUTPUT_TARGET) $(NM_TARGET_DIR)
	@cp $< $@

$(NM_RELEASE_ZIP): $(NM_DLL)
	@echo "packing $(notdir $@) for nexus"
	@cd $(NM_TARGET_DIR) \
		&& zip -9 $(ZIP_NAME) $(NM_ZIP_CONTENTS) \
		&& mv $(ZIP_NAME) $(NM_DIR)


################################################################################
#
# Thunderstore
#
################################################################################

TS_DIR          := $(TARGET_DIRECTORY)/thunderstore
TS_TARGET_DIR   := $(TS_DIR)/$(LOWER_CONFIG)
TS_DLL          := $(TS_TARGET_DIR)/Mods/$(OUTPUT_DLL)
TS_ZIP_CONTENTS := Mods/$(OUTPUT_DLL) icon.png README.md manifest.json
TS_RELEASE_ZIP  := $(TS_DIR)/$(ZIP_NAME)

.PHONY: ts-manifest
ts-manifest: $(TS_TARGET_DIR)/manifest.json

.PHONY: ts-readme
ts-readme: $(TS_TARGET_DIR)/README.md

.PHONY: ts-package
ts-package: $(TS_RELEASE_ZIP)

.PHONY: ts-release
ts-release: ts-clean ts-release-dirty

.PHONY: ts-release-dirty
ts-release-dirty:
	@$(MAKE) --no-print-directory CONFIGURATION=Il2Cpp LIFECYCLE=RELEASE ts-package
	@$(MAKE) --no-print-directory CONFIGURATION=Mono   LIFECYCLE=RELEASE ts-package

.PHONY: ts-clean
ts-clean: clean
	@rm -rf $(TS_DIR)

$(TS_TARGET_DIR):
	@mkdir -p $@

$(TS_TARGET_DIR)/icon.png: docs/assets/ts-icon.png
	@cp $< $@

$(TS_TARGET_DIR)/README.md: readme.adoc $(TS_TARGET_DIR)
	@sed 's/^= /# /g;s/^== /## /g;s/^=== /### /g;s/^\*\*/    */g' $< > $@

$(TS_TARGET_DIR)/manifest.json: meta/ts-manifest.json $(TS_TARGET_DIR)
	@jq --arg name $(PROJECT_ID)$(CONFIGURATION) \
		--arg version $(VERSION) \
		--arg url "$(MOD_URL)" \
		--argjson deps '[ "LavaGang-MelonLoader-0.7.2" ]' \
		'.name = $$name | .version_number = $$version | .website_url = $$url | .dependencies = $$deps' \
		$< > $@

$(TS_DLL): $(OUTPUT_TARGET) $(TS_TARGET_DIR)
	@mkdir -p $(dir $@)
	@cp $< $@

$(TS_RELEASE_ZIP): $(addprefix $(TS_TARGET_DIR)/,$(TS_ZIP_CONTENTS))
	@echo "packing $(notdir $@) for thunderstore"
	@cd $(TS_TARGET_DIR) \
		&& zip -9r $(ZIP_NAME) $(TS_ZIP_CONTENTS) \
		&& mv $(ZIP_NAME) $(TS_DIR)


################################################################################
#
# Full Targets
#
################################################################################

.PHONY: release
release: LIFECYCLE := RELEASE
release: ts-clean nm-clean ts-release-dirty nm-release-dirty
