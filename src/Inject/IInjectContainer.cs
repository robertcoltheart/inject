using System;

namespace Inject
{
    /// <summary>
    /// A dependency injection container that stores type registrations and instances, and creates new types.
    /// </summary>
    public interface IInjectContainer
    {
        /// <summary>
        /// Register a type in the container.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="resolveType">The type to resolve.</param>
        /// <exception cref="ArgumentException">Type is already registered or <paramref name="resolveType"/> does not derive from <paramref name="type"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="resolveType"/> is null.</exception>
        void Register(Type type, Type resolveType);
        
        /// <summary>
        /// Register an instance in the container.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="instance">The instance to resolve.</param>
        /// <exception cref="ArgumentException">Type is already registered or instance is not of the same type as <paramref name="type"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> or <paramref name="instance"/> is null.</exception>
        void Register(Type type, object instance);
        
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
        object Resolve(Type type);
    }
}