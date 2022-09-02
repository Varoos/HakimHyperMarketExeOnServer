using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakimHyperMarketExeOnServer
{
    public class POSSalesData
    {
        public class Header
        {
            public string DocNo { get; set; }
            public int Date { get; set; }
            public int SalesAC__Id { get; set; }
            public int CustomerAC__Id { get; set; }
            public int Currency__Id { get; set; }
            public double ExchangeRate { get; set; }
            public int Outlet__Id { get; set; }
            public int Counter__Id { get; set; }
            public int Member__Id { get; set; }
            public int Employee__Id { get; set; }
            public int Table__Id { get; set; }
            public int Guest__Id { get; set; }
            public string sPreOrderBillReferenceNo { get; set; }
            public int iShift { get; set; }
            public string sNarration { get; set; }
            public int iTransactionType { get; set; }
            public int iSelectedEmployee__Id { get; set; }
            public int iCreatedUser { get; set; }
            public string sMessageGreeting { get; set; }
            public string sSpecialInstruction { get; set; }
            public string sTokenNumber { get; set; }
            public int iProductionOutlet__Id { get; set; }
            public string iDeliveryType { get; set; }
            public string sPhoneNumber { get; set; }
            public string dTotalSaving { get; set; }
            public string sPosOrderTypeName { get; set; }
            public string FBRInvNo { get; set; }
            public double Net { get; set; }

            public int HeaderId { get; set; }
        }

        public class Body
        {
            public int Item__Id { get; set; }
            public string Item__Name { get; set; }
            public string Item__Code { get; set; }
            public double iSetQty { get; set; }
            public int Unit__Id { get; set; }
            public double Quantity { get; set; }
            public double Rate { get; set; }
            public double Gross { get; set; }
            [JsonProperty("Discount")]
            public Discount Discount { get; set; }

            [JsonProperty("Disc%")]
            public DiscPerc DiscPerc { get; set; }

            [JsonProperty("Total Discount")]
            public TotalDisc TotalDisc { get; set; }

            [JsonProperty("FNet")]
            public FNet FNet { get; set; }

            [JsonProperty("GST%")]
            public GSTPerc GSTPerc { get; set; }

            public double iProductType { get; set; }
            public string sLineNarration { get; set; }
            public string sComplimentaryRemarks { get; set; }
            public int TransactionId { get; set; }
        }

        public class Discount
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class DiscPerc
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class TotalDisc
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class FNet
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }

        public class GSTPerc
        {
            public double Input { get; set; }
            public string FieldName { get; set; }
            public int FieldId { get; set; }
            public int ColMap { get; set; }
            public double Value { get; set; }
        }


        public class Datum
        {
            public List<Body> Body { get; set; }
            public Header Header { get; set; }
        }

        public class Root
        {
            public List<Datum> data { get; set; }
            public string url { get; set; }
            public int result { get; set; }
            public object message { get; set; }
        }

        public class ProductRoot
        {
            public List<ProdDatum> data { get; set; }
            public string url { get; set; }
            public int result { get; set; }
            public object message { get; set; }
        }

        public class ProdDatum
        {
            public int iMasterId { get; set; }
            public string PCTCode { get; set; }
        }


    }


    public class Table
    {
        public int iHeaderId { get; set; }
    }


    public class HashData
    {
        public List<Hashtable> data { get; set; }
        public string url { get; set; }
        public int result { get; set; }
        public object message { get; set; }
    }
}
