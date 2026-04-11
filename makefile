PROJECTS := $(shell find ./mods -maxdepth 1 -type d -printf '%P ')
PROJECT_PATHS := $(addprefix mods/,$(PROJECTS))
CONFIGURATION := Il2Cpp

.PHONY: default
default:
	@echo "PROJECTS = $(PROJECTS)"
	@echo "CONFIGURATION = $(CONFIGURATION)"

.PHONY: build-all
build-all:
	@for i in $(PROJECTS); do \
		$(MAKE) -C mods/$$i PROJECT_ID=$$i CONFIGURATION=$(CONFIGURATION) build; \
	done

.PHONY: install
install:
	@$(MAKE) -C mods/$(PROJECT) PROJECT_ID=$(PROJECT) install

.PHONY: clean-all
clean-all:
	@for project in $(PROJECTS); do \
		rm -rf "mods/$${project}/bin" "mods/$${project}/obj"; \
	done