namespace FastPosFrontend.sse
{
    public class SSEventType
    {
        public const string CREATE_ORDER = "Create.Order";
        public const string UPDATE_ORDER = "Update.Order";
        public const string DELETE_ORDER = "Delete.Order";
        public const string PAY_ORDER = "Pay.Order";
        public const string CANCEL_ORDER = "Cancel.Order";
        public const string LOCK_ORDER = "Lock.Order";

    }
}
