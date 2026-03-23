PROJECTS := $(shell find . -maxdepth 1 -type d -regex '\./[A-Z].+' -printf '%P ')
CONFIGURATION := Il2Cpp

.PHONY: build-all
build-all:
	@for i in $(PROJECTS); do \
		$(MAKE) -C $$i PROJECT_ID=$$i CONFIGURATION=$(CONFIGURATION) build; \
	done
