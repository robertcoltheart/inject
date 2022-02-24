using System;
using Xunit;

namespace Inject
{
    public class InjectContainerTests
    {
        [Fact]
        public void RegisterNullTypeThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Register(null!, typeof(object)));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void RegisterTypeWithNullTypeThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Register(typeof(object), null!));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void RegisterInstanceNullThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Register(null!, new object()));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void RegisterInstanceWithNullInstanceThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Register<object>(null));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void RegisteredTypeDoesntInheritThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Register(typeof(IInterface1), typeof(Class2)));

            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void RegisteredInstanceDoesntInheritThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Register(typeof(IInterface1), new Class2()));

            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void RegisterSameTypeThrows()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, Class1>();

            var exception = Record.Exception(() => container.Register<IInterface1, Class1>());

            Assert.IsType<ArgumentException>(exception);
        }

        [Fact]
        public void ResolveNullTypeThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Resolve(null!));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void ResolveNonConcreteTypeThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Resolve<IInterface1>());

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void ResolveAbstractTypeThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Resolve<AbstractClass1>());

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void ResolveStaticTypeThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Resolve(typeof(StaticClass)));

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void CanRegisterTypeAsSingleton()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, Class1>();

            var instance1 = container.Resolve<IInterface1>();
            var instance2 = container.Resolve<IInterface1>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void ResolvesInstanceAsSingleton()
        {
            var container = new InjectContainer();

            var instance = new Class1();

            container.Register<IInterface1>(instance);

            var instance1 = container.Resolve<IInterface1>();
            var instance2 = container.Resolve<IInterface1>();

            Assert.Same(instance, instance1);
            Assert.Same(instance, instance2);
        }

        [Fact]
        public void CanResolveConcreteClassWithoutRegistering()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<Class1>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void CannotResolveAbstractClass()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, AbstractClass1>();

            var exception = Record.Exception(() => container.Resolve<AbstractClass1>());

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void CannotResolveInterface()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, IInherited1>();

            var exception = Record.Exception(() => container.Resolve<IInterface1>());

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void ResolveCircularReferenceThrows()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, CircularReference1>();
            container.Register<IInterface2, CircularReference2>();

            var exception = Record.Exception(() => container.Resolve<IInterface1>());

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void ResolvesConstructorParameters()
        {
            var container = new InjectContainer();
            container.Register<IInterfaceWithProperty, Class1WithProperty>();
            container.Register<IInterface2, Class2>();

            var instance = container.Resolve<IInterfaceWithProperty>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Arg);
        }

        [Fact]
        public void CanCreatePrivateNestedClass()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<PrivateClass>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void CreatingMultiplePublicConstructorClassThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Resolve<MultipleConstructorClass>());

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void CreatingNoPublicConstructorClassThrows()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Resolve<NoConstructorClass>());

            Assert.IsType<ResolutionFailedException>(exception);
        }

        [Fact]
        public void CanCreateOnePublicConstructorClassWithMultiPrivateConstructors()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<MultiplePrivateOnePublicConstructorClass>();

            Assert.NotNull(instance);
        }

        [Fact]
        public void CanCreateConcreteArgumentClass()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<ConcreteParameterClass>();

            Assert.NotNull(instance);
            Assert.NotNull(instance.Arg);
        }

        [Fact]
        public void ExceptionInConstructorThrowsInnerException()
        {
            var container = new InjectContainer();

            var exception = Record.Exception(() => container.Resolve<ConstructorExceptionClass>());

            Assert.NotNull(exception);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<InvalidOperationException>(exception.InnerException);
        }

        public interface IInterface1
        {
        }

        public interface IInherited1 : IInterface1
        {
        }

        public interface IInterface2
        {
        }

        public class Class1 : IInterface1
        {
        }

        public class Class2 : IInterface2
        {
        }

        public interface IInterfaceWithProperty
        {
            IInterface2 Arg { get; set; }
        }

        public class Class1WithProperty : IInterfaceWithProperty
        {
            public Class1WithProperty(IInterface2 arg)
            {
                Arg = arg;
            }

            public IInterface2 Arg { get; set; }
        }

        public abstract class AbstractClass1 : IInterface1
        {
        }

        public class CircularReference1 : IInterface1
        {
            public CircularReference1(IInterface2 arg)
            {
            }
        }

        public class CircularReference2 : IInterface2
        {
            public CircularReference2(IInterface1 arg)
            {
            }
        }

        public static class StaticClass
        {
        }

        private class PrivateClass
        {
        }

        public class MultipleConstructorClass
        {
            public MultipleConstructorClass()
            {
            }

            public MultipleConstructorClass(string arg)
            {
            }
        }

        public class MultiplePrivateOnePublicConstructorClass
        {
            public MultiplePrivateOnePublicConstructorClass()
            {
            }

            private MultiplePrivateOnePublicConstructorClass(string arg)
            {
            }

            private MultiplePrivateOnePublicConstructorClass(string arg1, string arg2)
            {
            }
        }

        public class NoConstructorClass
        {
            private NoConstructorClass()
            {
            }
        }

        public class ConcreteParameterClass
        {
            public object Arg;

            public ConcreteParameterClass(object arg)
            {
                Arg = arg;
            }
        }

        public class ConstructorExceptionClass
        {
            public ConstructorExceptionClass()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
