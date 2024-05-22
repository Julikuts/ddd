using MediatR;

namespace Primitives
{
    public interface IDomainEvent : INotification
    {
         public Guid EventId { get; set ; }  
         
    }
 
    public abstract record DomainEvent : IDomainEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
    }

     
}