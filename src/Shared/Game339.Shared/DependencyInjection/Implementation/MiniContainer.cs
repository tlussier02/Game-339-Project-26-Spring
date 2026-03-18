using System.Collections.Concurrent;

namespace Game339.Shared.DependencyInjection.Implementation;

public sealed class MiniContainer : IMiniContainer
{
    private readonly ConcurrentDictionary<Type, Func<IMiniContainer, object>> _registrations = new();

    public IMiniContainer RegisterSingletonInstance<TInterface>(TInterface instance) where TInterface : class
    {
        if (instance is null)
        {
            throw new ArgumentNullException(nameof(instance));
        }

        return Register(typeof(TInterface), _ => instance);
    }

    public IMiniContainer RegisterSingletonFactory<TInterface>(Func<IMiniContainer, TInterface> factory) where TInterface : class
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        var lazy = new Lazy<object>(() => factory(this));
        return Register(typeof(TInterface), _ => lazy.Value);
    }

    public IMiniContainer RegisterTransientFactory<TInterface>(Func<IMiniContainer, TInterface> factory) where TInterface : class
    {
        if (factory is null)
        {
            throw new ArgumentNullException(nameof(factory));
        }

        return Register(typeof(TInterface), _ => factory(this));
    }

    public TInterface Resolve<TInterface>() where TInterface : class
    {
        return (TInterface)Resolve(typeof(TInterface));
    }

    public object Resolve(Type type)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (type == typeof(IMiniContainer))
        {
            return this;
        }

        if (_registrations.TryGetValue(type, out var factory))
        {
            return factory(this);
        }

        throw new InvalidOperationException($"Type {type.FullName} is not registered.");
    }

    private IMiniContainer Register(Type type, Func<IMiniContainer, object> factory)
    {
        if (!_registrations.TryAdd(type, factory))
        {
            throw new DuplicateRegistrationException(type);
        }

        return this;
    }
}
