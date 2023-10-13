namespace ConsoleApp1.Customer
{
    internal class CustomerServiceRejectedEvent
    {
        public Guid ProprietorLocationServiceID { get; set; }
        public string Reason { get; set; }
    }
}