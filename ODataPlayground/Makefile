.PHONY: default build export copy run

default: build export copy run

build:
	cd ODataPlayground.Frontend && npx next build

export:
	cd ODataPlayground.Frontend && npx next export

copy:
	cd ODataPlayground.Server && cp -r ../ODataPlayground.Frontend/out/. ./ClientApp

run:
	cd ODataPlayground.Server && dotnet run

frontend-dev:
	cd ODataPlayground.Frontend && npm run dev
