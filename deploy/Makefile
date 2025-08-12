SHELL := cmd.exe
.SHELLFLAGS := /C

ENV ?= default
RELEASE_NAME = manager
CHART_PATH = ./
VALUES_FILE = Values.yaml

PROXY_CONTROLLER = ingress-nginx-controller-56d7c84fd4-mm4p8

deploy:
	@helm list -q | findstr $(RELEASE_NAME) >nul 2>&1 && ( \
		echo Upgrading release $(RELEASE_NAME)... && \
		helm upgrade $(RELEASE_NAME) $(CHART_PATH) -f $(VALUES_FILE) \
	) || ( \
		echo Installing release $(RELEASE_NAME)... && \
		helm install $(RELEASE_NAME) $(CHART_PATH) -f $(VALUES_FILE) \
	)

proxy:
	@kubectl port-forward -n ingress-nginx pod/$(PROXY_CONTROLLER) 8080:80

delete:
	@helm uninstall $(RELEASE_NAME) || echo Release $(RELEASE_NAME) not found or already deleted.
