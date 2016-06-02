using System;
using NUnit.Framework;

namespace Inject
{
    [TestFixture]
    public class InjectContainerTests
    {
        [Test]
        public void RegisterNullTypeThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Register(null, typeof(object)), Throws.ArgumentNullException);
        }

        [Test]
        public void RegisterTypeWithNullTypeThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Register(typeof(object), null), Throws.ArgumentNullException);
        }

        [Test]
        public void RegisterInstanceNullThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Register(null, new object()), Throws.ArgumentNullException);
        }

        [Test]
        public void RegisterInstanceWithNullInstanceThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Register<object>(null), Throws.ArgumentNullException);
        }

        [Test]
        public void RegisteredTypeDoesntInheritThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Register(typeof(IInterface1), typeof(Class2)), Throws.ArgumentException);
        }

        [Test]
        public void RegisteredInstanceDoesntInheritThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Register(typeof(IInterface1), new Class2()), Throws.ArgumentException);
        }

        [Test]
        public void RegisterSameTypeThrows()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, Class1>();

            Assert.That(() => container.Register<IInterface1, Class1>(), Throws.ArgumentException);
        }

        [Test]
        public void ResolveNullTypeThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Resolve(null), Throws.ArgumentNullException);
        }

        [Test]
        public void ResolveNonConcreteTypeThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Resolve<IInterface1>(), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void ResolveAbstractTypeThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Resolve<AbstractClass1>(), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void ResolveStaticTypeThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Resolve(typeof(StaticClass)), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void CanRegisterTypeAsSingleton()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, Class1>();

            var instance1 = container.Resolve<IInterface1>();
            var instance2 = container.Resolve<IInterface1>();

            Assert.AreSame(instance1, instance2);
        }

        [Test]
        public void ResolvesInstanceAsSingleton()
        {
            var container = new InjectContainer();

            var instance = new Class1();

            container.Register<IInterface1>(instance);

            var instance1 = container.Resolve<IInterface1>();
            var instance2 = container.Resolve<IInterface1>();

            Assert.AreSame(instance, instance1);
            Assert.AreSame(instance, instance2);
        }

        [Test]
        public void CanResolveConcreteClassWithoutRegistering()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<Class1>();

            Assert.IsNotNull(instance);
        }

        [Test]
        public void CannotResolveAbstractClass()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, AbstractClass1>();

            Assert.That(() => container.Resolve<AbstractClass1>(), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void CannotResolveInterface()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, IInherited1>();

            Assert.That(() => container.Resolve<IInterface1>(), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void ResolveCircularReferenceThrows()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, CircularReference1>();
            container.Register<IInterface2, CircularReference2>();

            Assert.That(() => container.Resolve<IInterface1>(), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void ResolvesConstructorParameters()
        {
            var container = new InjectContainer();
            container.Register<IInterfaceWithProperty, Class1WithProperty>();
            container.Register<IInterface2, Class2>();

            var instance = container.Resolve<IInterfaceWithProperty>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Arg);
        }

        [Test]
        public void CanCreatePrivateNestedClass()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<PrivateClass>();

            Assert.IsNotNull(instance);
        }

        [Test]
        public void CreatingMultiplePublicConstructorClassThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Resolve<MultipleConstructorClass>(), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void CreatingNoPublicConstructorClassThrows()
        {
            var container = new InjectContainer();

            Assert.That(() => container.Resolve<NoConstructorClass>(), Throws.InstanceOf<ResolutionFailedException>());
        }

        [Test]
        public void CanCreateOnePublicConstructorClassWithMultiPrivateConstructors()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<MultiplePrivateOnePublicConstructorClass>();

            Assert.IsNotNull(instance);
        }

        [Test]
        public void CanCreateConcreteArgumentClass()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<ConcreteParameterClass>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Arg);
        }

        [Test]
        public void ExceptionInConstructorThrowsInnerException()
        {
            try
            {
                new InjectContainer().Resolve<ConstructorExceptionClass>();
                Assert.Fail();
            }
            catch (ResolutionFailedException ex)
            {
                Assert.IsNotNull(ex.InnerException);
                Assert.AreEqual(typeof(InvalidOperationException), ex.InnerException.GetType());
            }
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
