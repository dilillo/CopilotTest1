namespace ConsoleApp1.Customer
{
    public class CustomerService
    {
        public Guid ProprietorLocationServiceID { get; set; }

        public CustomerServiceStatuses Status { get; set; }
    }

    public enum CustomerServiceStatuses
    {
        Requested,
        Accepted,
        Rejected,
        Suspended,
        Reinstated,
        Removed
    }
}
