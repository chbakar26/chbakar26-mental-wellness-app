namespace MentalWellnessApp.Web.Models;

public enum ApprovalStatus
{
    Pending,
    Approved,
    Disapproved
}

public enum SessionLifecycleStatus
{
    Draft,
    Open,
    Completed,
    Archived
}

public enum SessionRequestStatus
{
    Pending,
    Approved,
    Denied
}
