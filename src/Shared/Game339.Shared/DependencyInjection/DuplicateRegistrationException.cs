namespace Game339.Shared.DependencyInjection;

public sealed class DuplicateRegistrationException : InvalidOperationException
{
    public DuplicateRegistrationException(Type registeredType)
        : base($"A registration for {registeredType.FullName} already exists.")
    {
        RegisteredType = registeredType ?? throw new ArgumentNullException(nameof(registeredType));
    }

    public Type RegisteredType { get; }
}
