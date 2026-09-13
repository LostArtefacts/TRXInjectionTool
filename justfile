build: (publish)

restore:
    git submodule update --init --recursive
    dotnet restore -v n

publish: restore
    dotnet publish -c Release -o out

test *args:
    out/TRXInjectionTool {{args}}
