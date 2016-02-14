using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inject
{
    [TestClass]
    public class InjectContainerTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterNullTypeThrows()
        {
            new InjectContainer().Register(null, typeof(object));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterTypeWithNullTypeThrows()
        {
            new InjectContainer().Register(typeof(object), null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterInstanceNullThrows()
        {
            new InjectContainer().Register(null, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterInstanceWithNullInstanceThrows()
        {
            new InjectContainer().Register<object>(null);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredTypeDoesntInheritThrows()
        {
            new InjectContainer().Register(typeof(IInterface1), typeof(Class2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisteredInstanceDoesntInheritThrows()
        {
            new InjectContainer().Register(typeof(IInterface1), new Class2());
        }
        
        [TestMethod]
        public void RegisterSameTypeThrows()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, Class1>();

            try
            {
                container.Register<IInterface1, Class1>();
                Assert.Fail();
            }
            catch (ArgumentException)
            {
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolveNullTypeThrows()
        {
            new InjectContainer().Resolve(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ResolveNonConcreteTypeThrows()
        {
            new InjectContainer().Resolve<IInterface1>();
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ResolveAbstractTypeThrows()
        {
            new InjectContainer().Resolve<AbstractClass1>();
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ResolveStaticTypeThrows()
        {
            new InjectContainer().Resolve(typeof(StaticClass));
        }

        [TestMethod]
        public void CanRegisterTypeAsSingleton()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, Class1>();

            var instance1 = container.Resolve<IInterface1>();
            var instance2 = container.Resolve<IInterface1>();

            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
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
        
        [TestMethod]
        public void CanResolveConcreteClassWithoutRegistering()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<Class1>();
            
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void CannotResolveAbstractClass()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, AbstractClass1>();

            container.Resolve<AbstractClass1>();
        }
        
        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void CannotResolveInterface()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, IInherited1>();

            container.Resolve<IInterface1>();
        }
        
        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ResolveCircularReferenceThrows()
        {
            var container = new InjectContainer();
            container.Register<IInterface1, CircularReference1>();
            container.Register<IInterface2, CircularReference2>();

            container.Resolve<IInterface1>();
        }

        [TestMethod]
        public void ResolvesConstructorParameters()
        {
            var container = new InjectContainer();
            container.Register<IInterfaceWithProperty, Class1WithProperty>();
            container.Register<IInterface2, Class2>();

            var instance = container.Resolve<IInterfaceWithProperty>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Arg);
        }

        [TestMethod]
        public void CanCreatePrivateNestedClass()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<PrivateClass>();

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void CreatingMultiplePublicConstructorClassThrows()
        {
            var container = new InjectContainer();

            container.Resolve<MultipleConstructorClass>();
        }

        [TestMethod]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void CreatingNoPublicConstructorClassThrows()
        {
            var container = new InjectContainer();

            container.Resolve<NoConstructorClass>();
        }

        [TestMethod]
        public void CanCreateOnePublicConstructorClassWithMultiPrivateConstructors()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<MultiplePrivateOnePublicConstructorClass>();

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void CanCreateConcreteArgumentClass()
        {
            var container = new InjectContainer();
            var instance = container.Resolve<ConcreteParameterClass>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Arg);
        }

        [TestMethod]
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