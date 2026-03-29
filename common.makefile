PROJECT_ID := $(shell basename ${PWD})
VERSION := $(shell grep 'MelonInfo' $(PROJECT_ID)/Mod.cs | cut -d'"' -f2 | tr -d '\n')

LIFECYCLE := RELEASE
CONFIGURATION := Il2Cpp

LOWER_CONFIG := $(shell echo $(CONFIGURATION) | tr A-Z a-z)

BUILD_DLL  := $(PROJECT_ID).dll
OUTPUT_DLL := $(PROJECT_ID).$(CONFIGURATION).dll

TARGET_DIRECTORY := target
INSTALL_DIRECTORY := ${INSTALL_DIRECTORY}

ifeq ($(CONFIGURATION),Il2Cpp)
	NET_PATH := net6.0
else
	NET_PATH := netstandard2.1
endif

BUILD_TARGET := bin/$(CONFIGURATION)/$(NET_PATH)/$(BUILD_DLL)
OUTPUT_TARGET := $(TARGET_DIRECTORY)/$(OUTPUT_DLL)
RELEASE_ZIP := $(TARGET_DIRECTORY)/$(shell echo $(PROJECT_ID) | tr -d a-z | tr A-Z a-z)-$(LOWER_CONFIG)-v$(VERSION).zip

.PHONY: default
default:
	@echo "PROJECT_ID        = $(PROJECT_ID)"
	@echo "VERSION           = $(VERSION)"
	@echo "LIFECYCLE         = $(LIFECYCLE)"
	@echo "CONFIGURATION     = $(CONFIGURATION)"
	@echo "LOWER_CONFIG      = $(LOWER_CONFIG)"
	@echo "BUILD_DLL         = $(BUILD_DLL)"
	@echo "OUTPUT_DLL        = $(OUTPUT_DLL)"
	@echo "TARGET_DIRECTORY  = $(TARGET_DIRECTORY)"
	@echo "INSTALL_DIRECTORY = $(INSTALL_DIRECTORY)"
	@echo "NET_PATH          = $(NET_PATH)"
	@echo "BUILD_TARGET      = $(BUILD_TARGET)"
	@echo "OUTPUT_TARGET     = $(OUTPUT_TARGET)"
	@echo "RELEASE_ZIP       = $(RELEASE_ZIP)"

.PHONY: build
build: $(OUTPUT_TARGET)

.PHONY: package
package: $(RELEASE_ZIP)

.PHONY: install
install: build
	@if [ ! -z "$(INSTALL_DIRECTORY)" ]; then \
		cp $(OUTPUT_TARGET) "$(INSTALL_DIRECTORY)"; \
		echo "Installing $(notdir $(OUTPUT_TARGET))"; \
	fi

.PHONY: release
release: package-il2cpp package-mono

.PHONY: build-mono
build-mono:
	@$(MAKE) CONFIGURATION=Mono build

.PHONY: package-mono
package-mono:
	@$(MAKE) CONFIGURATION=Mono package

.PHONY: build-mono
build-il2cpp:
	@$(MAKE) CONFIGURATION=Il2Cpp build

.PHONY: package-mono
package-il2cpp:
	@$(MAKE) CONFIGURATION=Il2Cpp package

$(BUILD_TARGET):
	dotnet build --configuration $(CONFIGURATION) --property:Lifecycle=$(LIFECYCLE)

$(OUTPUT_TARGET): $(BUILD_TARGET)
	@mkdir -p $(TARGET_DIRECTORY)
	@cp $(BUILD_TARGET) $(OUTPUT_TARGET)

$(RELEASE_ZIP): $(OUTPUT_TARGET)
	@echo "Zipping $(OUTPUT_DLL)"
	@cd $(TARGET_DIRECTORY) && zip -9 $(@F) $(OUTPUT_DLL)
