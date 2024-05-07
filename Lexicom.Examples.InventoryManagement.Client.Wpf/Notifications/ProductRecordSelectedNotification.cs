using MediatR;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.Notifications;
public record class ProductRecordSelectedNotification(Guid? ProductId) : INotification;
