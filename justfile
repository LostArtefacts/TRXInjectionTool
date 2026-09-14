build: (publish)

restore:
    git submodule update --init --recursive
    dotnet restore -v n

publish: restore
    dotnet publish src/TRXInjectionTool.csproj -c Release -o out
    mkdir -p out/Plugins
    for p in TR1 TR2 TR3 TR4 TRX; do dotnet build builders/$p -c Release && cp builders/$p/bin/Release/TRXBuilders.$p.dll out/Plugins/; done

test *args:
    out/TRXInjectionTool {{args}}
