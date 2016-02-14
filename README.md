# Inject
A simple dependency injection container for registering and resolving types.

## Usage
```csharp
var container = new InjectContainer();
container.Register<IType, Type>();

IType instance = container.Resolve<IType>();
```
