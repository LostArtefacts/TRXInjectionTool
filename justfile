build: (publish)

restore:
    dotnet restore -v n

publish: restore
    dotnet publish -c Release -o out

test:
    out/TRXInjectionTool