.PHONY: lint
lint:
	$(MAKE) -C Frontend lint
	$(MAKE) -C AI lint
