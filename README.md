# FileGate

FileGate is an "simple" solution to share files from one computer to another

## NOTE

This is still WIP and depending on my free-time I will complete it sooner or later

## Usage

Build and Run Api project on host computer 

```bash
dotnet build ../FileGate.Api/FileGate.Api.csproj

./bin/Debug/netcoreapp3.0/FileGate.Api
```

Then - Start a client on machine that should share files

```bash
dotnet build ../FileGate.Client/FileGate.Client.csproj

./bin/Debug/netcoreapp3.0/FileGate.Client -p {path-to-sharing-folder}
```

After that you should be able to request a list of files in sharing folder and download them

To see api-paths open '/swagger'

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.


## License
[MIT](https://choosealicense.com/licenses/mit/)