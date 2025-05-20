using EquipmentRental.DataAccess.Models;

public class NotificationService
{
    private readonly EquipmentRentalDbContext _context;

    public NotificationService(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a notification for the specified user.
    /// </summary>
    public async Task CreateNotification(int userId, string message, string type)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Type = type,
            IsRead = false,
            CreatedAt = DateTime.Now
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}
