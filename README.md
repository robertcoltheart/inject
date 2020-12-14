# Inject [![Build](https://img.shields.io/github/workflow/status/robertcoltheart/Inject/build?style=flat-square)](https://github.com/robertcoltheart/Inject/actions?query=workflow:build) [![NuGet](https://img.shields.io/nuget/v/Inject.svg?style=flat-square)](https://www.nuget.org/packages/Inject)

A dependency injection container for registering and resolving types.

## Usage
Install the package from NuGet with `nuget install Inject`.

Types and instances can be registered in the container, as in the example below. You can also resolve types without registering them, provided that they have only 1 constructor and all of the parameters can also be resolved. The container will throw an exception if any circular dependencies are found.

```csharp
var container = new InjectContainer();
container.Register<IType, Type>();

IType instance = container.Resolve<IType>();
```

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on how to contribute to this project.

## License
Inject is released under the [MIT License](LICENSE)
