using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakimHyperMarketExeOnServer
{
    public class DataLogin
    {
        public string Username { get; set; }
        public string password { get; set; }
        public int CompanyId { get; set; }
    }

    public class Login
    {
        public List<DataLogin> data { get; set; }
    }

    public class Datumresult
    {
        public string fSessionId { get; set; }
    }

    public class Resultlogin
    {
        public List<Datumresult> data { get; set; }
        public string url { get; set; }
        public int result { get; set; }
        public string message { get; set; }
    }

    public class ClsJsonData
    {
        public string USIN { get; set; }
        public string POSId { get; set; }
        public string DateTime { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public string PCTCode { get; set; }
        public decimal SalesValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxCharged { get; set; }
        public decimal TAmount { get; set; }
        public int InvoiceType { get; set; }
    }

    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public string POSID { get; set; }
        public string USIN { get; set; }
        public string DateTime { get; set; }
        public double TotalSaleValue { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalBillAmount { get; set; }
        public double TotalTaxCharged { get; set; }
        public int InvoiceType { get; set; }
        public int PaymentMode { get; set; }
        public List<InvoiceItems> Items { get; set; }
    }

    public class InvoiceItems
    {
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public double Quantity { get; set; }
        public double TotalAmount { get; set; }
        public double SaleValue { get; set; }
        public double TaxCharged { get; set; }
        public double TaxRate { get; set; }
        public string PCTCode { get; set; }
        public int InvoiceType { get; set; }
    }

    public class Temperatures
    {
        [JsonProperty("data")]
        public Datum[] Data { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("result")]
        public long Result { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class Datum
    {
        [JsonProperty("fSessionId")]
        public string FSessionId { get; set; }
    }

    public class Root
    {
        public string InvoiceNumber { get; set; }
        public string Code { get; set; }
        public string Response { get; set; }
        public object Errors { get; set; }
    }

    public class ESPRoot
    {
        public string InvoiceNumber { get; set; }
        public string Code { get; set; }
        public string Response { get; set; }
    }
}
