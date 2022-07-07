using AWSServerlessInvoice.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SR.IntacctWrapper.NET.Enums;
using SR.IntacctWrapper.NET.Interfaces.APIHelper;
using SR.IntacctWrapper.NET.Interfaces.Managers;
using SR.IntacctWrapper.NET.Interfaces.Objects;
using SR.IntacctWrapper.NET.Objects;
using SR.IntacctWrapper.NET.Objects.APIHelper;
using SR.IntacctWrapper.NET.Options;
using SR.IntacctWrapper.NET.StaticContent;

namespace AWSServerlessInvoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ISOTransactionManager _sOTransactionManager;
        private readonly IInvoiceManager _invoiceManager;

        public OrderController(ISOTransactionManager sOTransactionManager,IInvoiceManager invoiceManager)
        {
            _sOTransactionManager = sOTransactionManager;
            _invoiceManager = invoiceManager;
        }
        [HttpPost]
        public IActionResult Post([FromBody]OrderInput orderInput)
        {
            var query = new QueryFilter
            {
                Field = "RECORDNO",
                Operator = FilterOperator.Equal,
                Value = orderInput.RecordId
            };
            var filter = new LogicalNode { Filters = new List<IQueryFilter> { query } };
            var sorting = new Dictionary<string, string>();
            sorting.Add(SOTransactionXmlFields.DocumentNumber, "desc");
            var fields = new List<string> {
                SOTransactionXmlFields.DocumentNumber,
                SOTransactionXmlFields.CustomerId,
                SOTransactionXmlFields.TransactionId
            };

            var transactions = _sOTransactionManager.GetSOTranactions(filter, sorting, fields);
            // include created from sales order key {Sales Order-SO0063}
            // use sales invoice transaction type
            var fullTransactionList = _sOTransactionManager.GetFullSOTransactions(new List<string> { transactions.First().TransactionId });

            var transaction = fullTransactionList.FirstOrDefault();
            if(transaction is null)
                return NotFound();

            var invoice = ConvertToInvoice(transaction);

            var response = _sOTransactionManager.CreateSOTransactions(new List<ISOTransaction> { invoice });
            
            return Ok();
        }

        private ISOTransaction ConvertToInvoice(ISOTransaction transaction)
        {
            var invoice = new SOTransaction()
            {
                CustomerId = transaction.CustomerId,
                DateCreated = DateTime.Now,
                DateDue = DateTime.Now.AddDays(30),
                ControlID = DateTime.Now.Ticks.ToString(),
                BaseCurr = transaction.BaseCurr,
                Currency = transaction.Currency,
                ExchRate = transaction.ExchRate,
                SOTransItems = new List<ISOTransItem>(),
                TransactionType = "Sales Invoice",
                CreatedFrom = $"Sales Order-{transaction.DocumentNumber}"
            };
            foreach (var line in transaction.SOTransItems)
            {
                invoice.SOTransItems.Add(new SOTransItem
                {
                    Quantity = line.Quantity,
                    ItemId = line.ItemId,
                    WarehouseId = line.WarehouseId,
                    LocationId = line.LocationId,
                    Unit = line.Unit
                });
            }

            return invoice;
        }
    }
}
