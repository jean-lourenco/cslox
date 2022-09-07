build:
	dotnet build

run:
	dotnet run --project src/Lox.csproj

test:
	dotnet test

.PHONY: build run test