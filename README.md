# JWT Examples

Use System.IdentityModel.Tokens.Jwt to create a HS256-signed JWT in a CLI, using .NET
Core on MacOS.

Follow along with [this document](https://docs.microsoft.com/en-us/dotnet/core/tutorials/using-with-xplat-cli)

## Disclaimer

This example is not an official Google product, nor is it part of an official Google product.


## Building

```
 alias dotnet=/usr/local/share/dotnet/dotnet
 dotnet build
```

## Running

```
dotnet run JwtGenerator --name foo --expiry 3600
```


## License

This material is copyright 2018, Google Inc.
and is licensed under the Apache 2.0 license. See the [LICENSE](LICENSE) file.
