namespace AWSServerlessInvoice.Models
{
    public class ReadByQueryParameters
    {
        public string Object { get; set; } = "";
        public string Fields { get; set; } = "";
        public string Query { get; set; } = "";
        public int PageSize { get; set; }
    }
}
