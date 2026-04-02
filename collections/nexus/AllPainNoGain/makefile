.ONESHELL:

GAME_VERSION := Il2Cpp
POST_INPUT := collection-$(shell echo $(GAME_VERSION) | tr A-Z a-z).yml

API_BASE := https://api.nexusmods.com/v3

.PHONY: default
default:
	@echo "GAME_VERSION := $(GAME_VERSION)"
	@echo "POST_INPUT   := $(POST_INPUT)"

.PHONY: generate-post-json
generate-post-json: request-body/post.json

.PHONY: post-collection
post-collection: request-body/post.json
	@curl -i -XPOST \
		-H"apikey: $${NEXUS_API_KEY}" \
		-H"Content-Type: application/json" \
		-d@$< \
		$(API_BASE)/collections

.PHONY: clean
clean:
	@rm -rf request-body

request-body/post.json: $(POST_INPUT) request-body .venv/bin/yq
	@echo "generating collection post request body"
	@.venv/bin/yq -c '.' $< > $@

request-body:
	@mkdir request-body

.venv/bin/yq: .venv
	@echo "Installing yq"
	@source .venv/bin/activate
	@python3 -m pip install -r requirements.txt

.venv:
	@echo "Setting up Python3 virtual env"
	@python3 -m venv .venv
