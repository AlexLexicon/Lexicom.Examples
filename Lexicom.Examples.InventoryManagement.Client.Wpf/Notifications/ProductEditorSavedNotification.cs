﻿using MediatR;

namespace Lexicom.Examples.InventoryManagement.Client.Wpf.Notifications;
public record class ProductEditorSavedNotification(Guid ProductId) : INotification;
