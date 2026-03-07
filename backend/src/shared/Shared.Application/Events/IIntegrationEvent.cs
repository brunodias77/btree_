using Shared.Domain.Events;

namespace Shared.Application.Events;

// 3. [Evento com consequência em outro módulo]
// Chamado de Integration Event. Serve para comunicar módulos diferentes sem acoplá-los.
public interface IIntegrationEvent : IEvent
{
    
}