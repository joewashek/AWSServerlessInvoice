namespace AWSServerlessInvoice.Services
{
    public interface IOrderQueue
    {
        Task AddToQueueAsync(string value);
    }
}
