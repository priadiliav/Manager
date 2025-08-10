SHELL := cmd.exe
.SHELLFLAGS := /C

ENV ?= default
RELEASE_NAME = manager
CHART_PATH = ./
VALUES_FILE = Values.yaml

PROXY_SERVICE = manager-server-api-service
PROXY_TARGET_PORT = 8080
PROXY_PORT = 8080

deploy:
	@helm list -q | findstr $(RELEASE_NAME) >nul 2>&1 && ( \
		echo Upgrading release $(RELEASE_NAME)... && \
		helm upgrade $(RELEASE_NAME) $(CHART_PATH) -f $(VALUES_FILE) \
	) || ( \
		echo Installing release $(RELEASE_NAME)... && \
		helm install $(RELEASE_NAME) $(CHART_PATH) -f $(VALUES_FILE) \
	)

proxy:
	@echo "Setting up proxy for $(RELEASE_NAME)..."
	@kubectl port-forward svc/$(PROXY_SERVICE) $(PROXY_PORT):$(PROXY_TARGET_PORT)


delete:
	@helm uninstall $(RELEASE_NAME) || echo Release $(RELEASE_NAME) not found or already deleted.
