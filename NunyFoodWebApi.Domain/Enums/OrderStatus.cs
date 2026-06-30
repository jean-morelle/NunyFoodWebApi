namespace NunyFoodWebApi.Domain.Enums;

public enum OrderStatus
{
    Created = 0,
    PendingPayment = 1,
    Paid = 2,
    Preparing = 3,
    Assigned = 4,
    InDelivery = 5,
    Delivered = 6,
    Confirmed = 7,
    Cancelled = 8
}
