namespace Game339.Shared.DependencyInjection;

public interface IMiniContainer
{
    IMiniContainer RegisterSingletonInstance<TInterface>(TInterface instance) where TInterface : class;
    IMiniContainer RegisterSingletonFactory<TInterface>(Func<IMiniContainer, TInterface> factory) where TInterface : class;
    IMiniContainer RegisterTransientFactory<TInterface>(Func<IMiniContainer, TInterface> factory) where TInterface : class;
    TInterface Resolve<TInterface>() where TInterface : class;
    object Resolve(Type type);
}
