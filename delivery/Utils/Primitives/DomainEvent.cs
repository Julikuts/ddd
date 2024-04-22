using MediatR;

namespace Primitives
{
    public interface IDomainEvent : INotification
    {
	
	public Guid Id { get; }
    public string Name { get; }
    }
}