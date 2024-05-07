using MediatR;

namespace Lexicom.Examples.InventoryManagement.Client.Application.Notifications;
public record class NewProductNotification(Guid Id) : INotification;
