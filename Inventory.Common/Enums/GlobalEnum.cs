namespace Inventory.Common.Enums;

public static class GlobalEnum
{
    public enum UserRole
    {
        SuperAdmin = 1,
        Admin = 2,
        User = 3
    }

    public enum OperationType
    {
        Create,
        Update,
        Delete,
        View
    }

    public enum ActivityLogModule
    {
        User = 1,
        Category = 2,
        Product = 3,
        Order = 4,
    }

    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Completed = 4
    }
}
