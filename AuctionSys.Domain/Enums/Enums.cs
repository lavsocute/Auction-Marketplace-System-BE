namespace AuctionSys.Domain.Enums;

public enum ListType
{
    FixedPrice,
    Auction
}

public enum ItemStatus
{
    Available,
    Sold,
    InAuction,
    Expired,
    InUserSteamInventory,
    PendingDeposit,
    InBotInventory,
    TradeLocked,
    PendingWithdraw,
    Withdrawn
}

public enum SkinExterior
{
    NotApplicable,
    FactoryNew,
    MinimalWear,
    FieldTested,
    WellWorn,
    BattleScarred
}

public enum AuctionStatus
{
    Active,
    Ended,
    Cancelled
}

public enum TransactionType
{
    TopUp,
    Withdraw,
    Purchase,
    Sale,
    BidFreeze,
    BidUnfreeze,
    AuctionWin,
    AuctionRefund
}

public enum OrderStatus
{
    Completed,
    Cancelled
}

public enum NotificationType
{
    Outbid,
    AuctionWon,
    AuctionEnded,
    NewMessage,
    OrderCompleted,
    ReviewReceived
}

public enum ReportTargetType
{
    Item,
    User
}

public enum ReportStatus
{
    Pending,
    Reviewed,
    Resolved
}
