using System;

namespace Inject
{
    /// <summary>
    /// Extension class that adds a set of convenience overloads to the <see cref="IInjectContainer"/> interface.
    /// </summary>
    public static class InjectContainerExtensions
    {
        /// <summary>
        /// Register a type in the container.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <typeparam name="TResolve">The type to resolve.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <exception cref="ArgumentException">Type is already registered.</exception>
        public static void Register<T, TResolve>(this IInjectContainer container)
            where TResolve : T
        {
            container.Register(typeof(T), typeof(TResolve));
        }

        /// <summary>
        /// Register an instance in the container.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <param name="instance">The instance to resolve.</param>
        /// <exception cref="ArgumentException">Type is already registered.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="instance"/> is null.</exception>
        public static void Register<T>(this IInjectContainer container, T instance)
        {
            container.Register(typeof(T), instance);
        }

        /// <summary>
        /// Resolve a type and return a concrete instance.
        /// </summary>
        /// <remarks>
        /// If the given type is not registered, the type is constructed using the public constructor.
        /// </remarks>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="container">The dependency injection container.</param>
        /// <returns>An instance of the type.</returns>
        /// <exception cref="ResolutionFailedException">A type failed to create or a circular reference is detected.</exception>
        public static T Resolve<T>(this IInjectContainer container)
        {
            return (T) container.Resolve(typeof (T));
        }
    }
}