using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inject.Properties;

namespace Inject
{
    /// <summary>
    /// A dependency injection container that stores type registrations and instances, and creates new types;
    /// </summary>
    public class InjectContainer : IInjectContainer
    {
        private readonly Dictionary<Type, Type> _registeredTypes = new Dictionary<Type, Type>();
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        private object CreateType(Type type, Stack<Type> activating)
        {
            ConstructorInfo[] constructors = type.GetConstructors();

            if (constructors.Length != 1)
                throw new ResolutionFailedException(string.Format(Resources.ResolutionFailed_WrongNumberConstructors, type));

            object[] parameters = constructors.First()
                .GetParameters()
                .Select(x => Resolve(x.ParameterType, activating))
                .ToArray();
            
            try
            {
                return Activator.CreateInstance(type, parameters);
            }
            catch (TargetInvocationException ex)
            {
                throw new ResolutionFailedException(string.Format(Resources.ResolutionFailed_CreationError, type), ex.InnerException);
            }
        }

        /// <summary>
        /// Register a type in the container.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="resolveType">The type to resolve.</param>
        /// <exception cref="ArgumentException">Type is already registered or <paramref name="resolveType"/> does not derive from <paramref name="type"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="resolveType"/> is null.</exception>
        public void Register(Type type, Type resolveType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (resolveType == null)
                throw new ArgumentNullException(nameof(resolveType));

            if (_registeredTypes.ContainsKey(type))
                throw new ArgumentException(string.Format(Resources.Argument_TypeAlreadyRegistered, type), nameof(type));

            if (!type.IsAssignableFrom(resolveType))
                throw new ArgumentException(string.Format(Resources.Argument_TypeShouldInherit, type, type));

            _registeredTypes[type] = resolveType;
        }

        /// <summary>
        /// Register an instance in the container.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="instance">The instance to resolve.</param>
        /// <exception cref="ArgumentException">Type is already registered or instance is not of the same type as <paramref name="type"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="instance"/> is null.</exception>
        public void Register(Type type, object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            if (_instances.ContainsKey(type))
                throw new ArgumentException(string.Format(Resources.Argument_TypeAlreadyRegistered, type), nameof(type));

            if (!type.IsInstanceOfType(instance))
                throw new ArgumentException(string.Format(Resources.Argument_TypeShouldInherit, instance.GetType(), type));

            _instances[type] = instance;
        }
        
        /// <summary>
        /// Resolve a type and return a concrete instance.
        /// </summary>
        /// <remarks>
        /// If the given type is not registered, the type is constructed using the public constructor.
        /// </remarks>
        /// <param name="type">The type to resolve.</param>
        /// <returns>An instance of the type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/>Type is null.</exception>
        /// <exception cref="ResolutionFailedException">A type failed to create or a circular reference is detected.</exception>
        public object Resolve(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return Resolve(type, new Stack<Type>());
        }

        private object Resolve(Type type, Stack<Type> activating)
        {
            object instance;

            if (_instances.TryGetValue(type, out instance))
                return instance;

            Type resolveType;

            if (!_registeredTypes.TryGetValue(type, out resolveType))
                resolveType = type;

            ValidateResolution(type, resolveType, activating);

            activating.Push(resolveType);

            instance = CreateType(resolveType, activating);
            _instances[type] = instance;

            activating.Pop();

            return instance;
        }

        private void ValidateResolution(Type type, Type resolveType, Stack<Type> activating)
        {
            if (activating.Contains(resolveType))
                throw new ResolutionFailedException(string.Format(Resources.ResolutionFailed_CircularReference, type, string.Join(Environment.NewLine, activating.Select(x => x.ToString()).ToArray())));

            if (resolveType != null && (resolveType.IsInterface || resolveType.IsAbstract))
                throw new ResolutionFailedException(string.Format(Resources.ResolutionFailed_CannotCreateInterface, resolveType));
        }
    }
}