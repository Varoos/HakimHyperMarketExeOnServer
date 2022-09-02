using Focus.Common.DataStructs;
using Focus.Pos.BusinessObjects;
using Focus.Transactions.DataStructs;
using Focus.TranSettings.DataStructs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;

namespace HakimHyperMarketExeOnServer
{
    public class ClsExecuteonServer
    {
        int VoucherType = 0;
        int companyid = 0;
        string VoucherNo = "";
        string DateTime = "";

        public bool POSSales(Transaction objTrans, VWVoucherSettings objSettings, int iFieldId, string strFieldName, int iRowindex)
        {
            Clsdata.LogFile("POSSalesExeserver", "Execute on server calling Companyid:" + iFieldId);

            VoucherType = Convert.ToInt32(objTrans.Header.VoucherType);
            companyid = iFieldId;
            VoucherNo = Convert.ToString(objTrans.Header.DocNo);
            Clsdata.LogFile("POSSalesExeserver", "Vouchertype:" + VoucherType);
            Clsdata.LogFile("POSSalesExeserver", "companyid:" + companyid);
            Clsdata.LogFile("POSSalesExeserver", "VoucherNo:" + VoucherNo);
            try
            {
                #region Update
                string updateQuery = String.Format("UPDATE f  SET FBRInvNo = '327788665512301/07/2021' from tCore_Header_0 h INNER JOIN tCore_HeaderData" + VoucherType + "_0 f ON h.iHeaderId = f.iHeaderId where iVoucherType = " + VoucherType + " and sVoucherNo = '" + VoucherNo + "'");
                int j = Clsdata.GetExecute(updateQuery, companyid);
                if (j == 1)
                {
                    Clsdata.LogFile("POSSalesExeserver", "POSSales Voucher Updated Successfully");
                }
                #endregion
            }
            catch (Exception ex)
            {
                Clsdata.LogFile("POSSalesExeserver", "POSSales exception:" + ex.Message);
            }
            return true;
        }

        public IdNamePair POSSalesNew(Transaction SalesInvoiceInput, Output SalesInvoiceOutput, Transaction CreditNoteInput, Output CreditNoteOutput, BillSettlementDetails BillSetmntDtlsInput, BillPrinting BillPrintData, PaymentDetails BillSettlementPaymentDetailsInput, int ComapnyId, string CompanyCode)
        {
            string errors1 = "";
            string Message = "";
            int HeaderId = 0;

            Clsdata.LogFile("POSSalesExeserver", "Execute on server calling Companyid: " + ComapnyId);

            try
            {
                POSSalesData.Root objResponse = new POSSalesData.Root();
                BL_Config configMan = new BL_Config(this.GetType().Assembly.Location);
                string ServerAPIIP = configMan.GetAppSetting("Server_API_IP");
                var FBRInvoiceNo = "";
                var ESPInvoiceNo = "";
                companyid = ComapnyId;
                string SessionId = "";
                int fieldId = Convert.ToInt32(configMan.GetAppSetting("FieldId"));//1672;
                string fieldName = "FBRInvNo";
                string TagValue = "";// "327788665512301/07/2021";

                if (companyid != 0)
                {
                    SessionId = GetSessionId(companyid);
                }
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew SessionId: " + SessionId);

                if (SalesInvoiceInput != null && SalesInvoiceInput.Header != null)
                {
                    VoucherType = Convert.ToInt32(SalesInvoiceInput.Header.VoucherType);
                    VoucherNo = Convert.ToString(SalesInvoiceInput.Header.DocNo);

                    DateTime dt = GetIntToDate(SalesInvoiceInput.Header.Date);
                    DateTime = dt.ToString("MM-dd-yyyy");
                }
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Vouchertype: " + VoucherType);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew VoucherNo: " + VoucherNo);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew DateTime: " + DateTime);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew companyid: " + companyid);

                #region JSON CREATION
                List<ClsJsonData> DList = new List<ClsJsonData>();

                #region LoadVoucher
                HashData objHashRequest = new HashData();
                Hashtable objHash = new Hashtable();
                objHash.Add("Query", "select iHeaderId from tCore_Header_0 where iVoucherType="+ VoucherType + " and sVoucherNo='" + VoucherNo + "'");
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string sContent1 = JsonConvert.SerializeObject(objHashRequest);
                using (var client = new WebClient())
                {
                    client.Headers.Add("fSessionId", SessionId);
                    client.Headers.Add("Content-Type", "application/json");
                    string sUrl = "http://" + ServerAPIIP + "/Focus8API/utility/ExecuteSqlQuery";
                    string strResponse = client.UploadString(sUrl, sContent1);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew string URL : --" + strResponse);
                    QueryClass.Root objHashResponse = JsonConvert.DeserializeObject<QueryClass.Root>(strResponse);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew string URL : --" + objHashResponse.data[0].Table[0].iHeaderId);
                    HeaderId = objHashResponse.data[0].Table[0].iHeaderId;
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Voucher HeaderId : --" + HeaderId);
                }

                using (var client = new WebClient())
                {
                    client.Headers.Add("fSessionId", SessionId);
                    string sUrl = "";
                    if (VoucherType== 3331)
                    {
                        sUrl = "http://" + ServerAPIIP + "/Focus8API/Screen/Transactions/POS Sales/" + VoucherNo;
                    }
                    else if (VoucherType == 1792)
                    {
                        sUrl = "http://" + ServerAPIIP + "/Focus8API/Screen/Transactions/POS - Sales Returns/" + VoucherNo;
                        fieldId = fieldId + 1;
                    }
                    
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew string URL --" + VoucherNo + sUrl);
                    var strResponse = client.DownloadString(sUrl);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew stringstrResponse for Voucher No :" + VoucherNo + " --" + strResponse);
                    objResponse = JsonConvert.DeserializeObject<POSSalesData.Root>(strResponse);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew DeserializeObject for Voucher No : " + VoucherNo + " --" + objResponse);
                }

                #endregion

                List<InvoiceItems> lst = new List<InvoiceItems>();
                Invoice objInv = new Invoice();
                objInv.InvoiceNumber = "";
                objInv.POSID = "932400";
                objInv.USIN = VoucherNo;
                objInv.DateTime = DateTime;
                objInv.InvoiceType = 1;
                objInv.PaymentMode = 1;

                objInv.TotalSaleValue = objResponse.data[0].Body.Sum(_ => _.FNet.Input);
                objInv.TotalQuantity = objResponse.data[0].Body.Sum(_ => _.Quantity);
                objInv.TotalTaxCharged = objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);
                objInv.TotalBillAmount = objResponse.data[0].Body.Sum(_ => _.FNet.Input) + objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);

                foreach (var item in objResponse.data[0].Body)
                {
                    int ProductId = item.Item__Id;
                    //POSSalesData.ProductRoot objProdResponse = new POSSalesData.ProductRoot();
                    //using (var client = new WebClient())
                    //{
                    //    client.Headers.Add("fSessionId", SessionId);
                    //    string sUrl = "http://" + ServerAPIIP + "/Focus8API/List/Masters/Core__Product?fields=iMasterId,PCTCode&where=iMasterId=" + ProductId;
                    //    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew string URL -- /n" + ProductId + sUrl);
                    //    var strResponse = client.DownloadString(sUrl);
                    //    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Product stringstrResponse for ProductId : " + ProductId + " --" + strResponse);
                    //    objProdResponse = JsonConvert.DeserializeObject<POSSalesData.ProductRoot>(strResponse);
                    //    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Product DeserializeObject for ProductId : " + ProductId + " --" + objProdResponse);
                    //}

                    InvoiceItems objitem = new InvoiceItems();
                    objitem.ItemName = item.Item__Name;
                    objitem.ItemCode = item.Item__Code;
                    objitem.PCTCode = "01011000";//objProdResponse.data[0].PCTCode;
                    objitem.Quantity = item.Quantity;
                    objitem.SaleValue = item.FNet.Input;
                    objitem.TaxCharged = item.GSTPerc.Value;
                    objitem.TotalAmount = item.FNet.Input + item.GSTPerc.Value;
                    objitem.TaxRate = item.GSTPerc.Input;
                    objitem.InvoiceType = 1;
                    lst.Add(objitem);
                }
                objInv.Items = lst;

                string ClientAPIURL = configMan.GetAppSetting("ClientAPIURL");
                Clsdata.LogFile("POSSalesExeserver", "ClientAPIURL Data:" + ClientAPIURL);
                HttpClient Client = new HttpClient();
                var json2 = new JavaScriptSerializer().Serialize(objInv);
                Clsdata.LogFile("POSSalesExeserver", "Json Data:" + json2);
                var content = new StringContent(json2, Encoding.UTF8, "application/json");
                HttpResponseMessage response = Client.PostAsync(ClientAPIURL, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response from API");
                    Console.WriteLine("———————————————");
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Clsdata.LogFile("POSSalesExeserver", "API Response:" + response.Content.ReadAsStringAsync().Result);

                    Root objRootResponse = new Root();
                    var Result = response.Content.ReadAsStringAsync().Result;
                    objRootResponse = JsonConvert.DeserializeObject<Root>(Result);
                    Clsdata.LogFile("POSSalesExeserver", "API DeserializeObject Response: " + objRootResponse);

                    FBRInvoiceNo = objRootResponse.InvoiceNumber;
                    Clsdata.LogFile("POSSalesExeserver", "FBRInvoiceNo : " + FBRInvoiceNo);

                    #region VoucherUpdateQuery
                    HashData objHashRequest1 = new HashData();
                    Hashtable objHash1 = new Hashtable();
                    if (VoucherType == 3331)
                    {
                        objHash1.Add("Query", "update tCore_HeaderData3331_0 set FBRInvNo='" + FBRInvoiceNo + "'  where iHeaderId=" + HeaderId + "");
                    }
                    else if (VoucherType == 1792)
                    {
                        objHash1.Add("Query", "update tCore_HeaderData1792_0 set FBRInvNo='" + FBRInvoiceNo + "'  where iHeaderId=" + HeaderId + "");
                    }
                    
                    List<Hashtable> lstHash1 = new List<Hashtable>();
                    lstHash1.Add(objHash1);
                    objHashRequest1.data = lstHash1;
                    string sContent = JsonConvert.SerializeObject(objHashRequest1);
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("fSessionId", SessionId);
                        client.Headers.Add("Content-Type", "application/json");
                        string sUrl = "http://" + ServerAPIIP + "/Focus8API/utility/ExecuteNonQuery";
                        string strResponse = client.UploadString(sUrl, sContent);
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Update Response : --" + strResponse);
                        QueryClass.UpdateRoot objHashResponse = JsonConvert.DeserializeObject<QueryClass.UpdateRoot>(strResponse);
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Update Result: --" + objHashResponse.data[0].Result);
                        int UpdateResult = objHashResponse.data[0].Result;
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Voucher UpdateResult : --" + UpdateResult);
                    }

                    #endregion

                    #region BillPrintPDFCode
                    TagValue = FBRInvoiceNo;// "327788665512301/07/2021";
                    Clsdata.LogFile("POSSalesExeserver", "fieldId: " + fieldId + " FieldName: " + fieldName + " Tag: " + TagValue);
                    if (BillPrintData == null)
                    {
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew BillPrintData is null");
                        return new IdNamePair();
                    }
                    if (BillPrintData.HeaderExtraFieldValues == null)
                    {
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew BillPrintData.HeaderExtraFieldValues is null");
                        return new IdNamePair();
                    }
                    BillPrintData.HeaderExtraFieldValues = new List<IdNamePair>();
                    BillPrintData.HeaderExtraFieldValues.Add(new IdNamePair { ID = fieldId, Name = fieldName, Tag = TagValue });
                    #endregion

                    #region ESR Post Code
                    lst = new List<InvoiceItems>();
                    objInv = new Invoice();
                    objInv.InvoiceNumber = FBRInvoiceNo;
                    objInv.POSID = "932400";
                    objInv.USIN = VoucherNo;
                    objInv.DateTime = DateTime;
                    objInv.InvoiceType = 1;
                    objInv.PaymentMode = 1;

                    objInv.TotalSaleValue = objResponse.data[0].Body.Sum(_ => _.FNet.Input);
                    objInv.TotalQuantity = objResponse.data[0].Body.Sum(_ => _.Quantity);
                    objInv.TotalTaxCharged = objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);
                    objInv.TotalBillAmount = objResponse.data[0].Body.Sum(_ => _.FNet.Input) + objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);

                    foreach (var item in objResponse.data[0].Body)
                    {
                        int ProductId = item.Item__Id;
                        //POSSalesData.ProductRoot objProdResponse = new POSSalesData.ProductRoot();
                        //using (var client = new WebClient())
                        //{
                        //    client.Headers.Add("fSessionId", SessionId);
                        //    string sUrl = "http://" + ServerAPIIP + "/Focus8API/List/Masters/Core__Product?fields=iMasterId,PCTCode&where=iMasterId=" + ProductId;
                        //    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew string URL -- /n" + ProductId + sUrl);
                        //    var strResponse = client.DownloadString(sUrl);
                        //    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Product stringstrResponse for ProductId : " + ProductId + " --" + strResponse);
                        //    objProdResponse = JsonConvert.DeserializeObject<POSSalesData.ProductRoot>(strResponse);
                        //    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Product DeserializeObject for ProductId : " + ProductId + " --" + objProdResponse);
                        //}

                        InvoiceItems objitem = new InvoiceItems();
                        objitem.ItemName = item.Item__Name;
                        objitem.ItemCode = item.Item__Code;
                        objitem.PCTCode = "01011000";//objProdResponse.data[0].PCTCode;
                        objitem.Quantity = item.Quantity;
                        objitem.SaleValue = item.FNet.Input;
                        objitem.TaxCharged = item.GSTPerc.Value;
                        objitem.TotalAmount = item.FNet.Input + item.GSTPerc.Value;
                        objitem.TaxRate = item.GSTPerc.Input;
                        objitem.InvoiceType = 1;
                        lst.Add(objitem);
                    }
                    objInv.Items = lst;

                    string ClientPOSTURL = configMan.GetAppSetting("ClientPOSTURL");
                    Clsdata.LogFile("POSSalesExeserver", "ClientPOSTURL Data:" + ClientPOSTURL);
                    HttpClient Client1 = new HttpClient();
                    var json21 = new JavaScriptSerializer().Serialize(objInv);
                    Clsdata.LogFile("POSSalesExeserver", "Json Data:" + json21);
                    var content1 = new StringContent(json21, Encoding.UTF8, "application/json");
                    Clsdata.LogFile("POSSalesExeserver", "content1: " + content1);
                    HttpResponseMessage response1 = Client1.PostAsync(ClientPOSTURL, content1).Result;
                    try
                    {
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST Before Response: " + response1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST response1:" + response1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST After Response: " + response1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST Before Result Response: " + response1.Content.ReadAsStringAsync().Result);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST Response:" + response1.Content.ReadAsStringAsync().Result);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST After Result Response: " + response1.Content.ReadAsStringAsync().Result);

                        ESPRoot objESPRootResponse = new ESPRoot();
                        var Result1 = response1.Content.ReadAsStringAsync().Result;
                        objESPRootResponse = JsonConvert.DeserializeObject<ESPRoot>(Result1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST DeserializeObject Response: " + objESPRootResponse);

                        ESPInvoiceNo = objESPRootResponse.InvoiceNumber;
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST InvoiceNo : " + ESPInvoiceNo);

                        //if (response1.IsSuccessStatusCode)
                        //{
                        //    Console.WriteLine("Response from ESP POST");
                        //    Console.WriteLine("———————————————");
                        //    Console.WriteLine(response1.Content.ReadAsStringAsync().Result);
                        //    Clsdata.LogFile("POSSalesExeserver", "ESP POST Response:" + response1.Content.ReadAsStringAsync().Result);

                        //    //objESPRootResponse = new ESPRoot();
                        //    //Result1 = response1.Content.ReadAsStringAsync().Result;
                        //    //objESPRootResponse = JsonConvert.DeserializeObject<ESPRoot>(Result1);
                        //    //Clsdata.LogFile("POSSalesExeserver", "ESP POST DeserializeObject Response: " + objESPRootResponse);

                        //    //ESPInvoiceNo = objESPRootResponse.InvoiceNumber;
                        //    //Clsdata.LogFile("POSSalesExeserver", "ESP POST InvoiceNo : " + ESPInvoiceNo);
                        //}
                    }
                    catch (Exception ex)
                    {
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew POSSales exception:" + ex.Message);
                    }
                    #endregion
                }
                #endregion

                #region VoucherResave
                //string POSSales = "http://" + ServerAPIIP + "/Focus8API/ Transactions/Vouchers/POS Sales";
                //#region Header
                //Hashtable header = new Hashtable
                //{
                //    //mandatory
                //    { "HeaderId", HeaderId},
                //    { "VoucherNo", VoucherNo},
                //    { "Date",objResponse.data[0].Header.Date },
                //    { "SalesAC", objResponse.data[0].Header.SalesAC__Id },
                //    { "CustomerAC", objResponse.data[0].Header.CustomerAC__Id },
                //    { "Currency", objResponse.data[0].Header.Currency__Id},
                //    { "ExchangeRate", objResponse.data[0].Header.ExchangeRate},
                //    { "Outlet", objResponse.data[0].Header.Outlet__Id},
                //    {"Counter", objResponse.data[0].Header.Counter__Id},
                //    {"Member", objResponse.data[0].Header.Member__Id },
                //    {"Employee", objResponse.data[0].Header.Employee__Id },
                //    { "Table", objResponse.data[0].Header.Table__Id },
                //    {"Guest",objResponse.data[0].Header.Guest__Id },
                //    {"sPreOrderBillReferenceNo",objResponse.data[0].Header.sPreOrderBillReferenceNo},
                //    {"iShift", objResponse.data[0].Header.iShift},
                //    {"sNarration",objResponse.data[0].Header.sNarration},
                //    {"iTransactionType", objResponse.data[0].Header.iTransactionType},
                //    {"iSelectedEmployee",objResponse.data[0].Header.iSelectedEmployee__Id},
                //    {"iCreatedUser", objResponse.data[0].Header.iCreatedUser},
                //    {"sMessageGreeting", objResponse.data[0].Header.sMessageGreeting},
                //    {"sSpecialInstruction",objResponse.data[0].Header.sSpecialInstruction},
                //    {"sTokenNumber", objResponse.data[0].Header.sTokenNumber},
                //    {"iProductionOutlet", objResponse.data[0].Header.iProductionOutlet__Id},
                //    {"iDeliveryType",objResponse.data[0].Header.iDeliveryType},
                //    {"sPhoneNumber", objResponse.data[0].Header.sPhoneNumber},
                //    {"dTotalSaving", objResponse.data[0].Header.dTotalSaving},
                //    {"sPosOrderTypeName",objResponse.data[0].Header.sPosOrderTypeName},
                //    {"FBRInvNo",FBRInvoiceNo}
                //};
                //#endregion

                //List<Hashtable> body = new List<Hashtable>();
                //#region Body
                //Hashtable row1 = new Hashtable();
                //foreach (var item in objResponse.data[0].Body)
                //{
                //    row1 = new Hashtable
                //    {
                //        { "Item",item.Item__Id},
                //        { "iSetQty", item.iSetQty },
                //        { "Unit", item.Unit__Id },
                //        { "Quantity", item.Quantity },
                //        { "Rate", item.Rate },
                //        { "Gross", item.Gross},
                //        { "Discount", item.Discount},
                //        { "Disc%", item.DiscPerc},
                //        { "Total Discount", item.TotalDisc},
                //        { "FNet", item.FNet},
                //        { "GST%", item.GSTPerc},
                //        { "iProductType", item.iProductType},
                //        { "sLineNarration", item.sLineNarration},
                //        { "sComplimentaryRemarks", item.sComplimentaryRemarks},
                //        { "TransactionId", item.TransactionId},
                //    };
                //    body.Add(row1);
                //}
                //#endregion

                //var postingData = new PostingData();
                //postingData.data.Add(new Hashtable { { "Header", header }, { "Body", body } });
                //string sContent = JsonConvert.SerializeObject(postingData);
                //Clsdata.LogFile("POSSalesExeserver", "Voucher Update  sContent:" + sContent);
                //errors1 = "";
                //#region Response
                //var Vresponse = Focus8API.Post(POSSales, sContent, SessionId, ref errors1);
                //if (Vresponse != null)
                //{
                //    var responseData = JsonConvert.DeserializeObject<APIResponse.PostResponse>(Vresponse);
                //    if (responseData.result == -1)
                //    {
                //        Message = $"Posting Failed: {responseData.message } \n";
                //        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Posting Failed: " + Message);
                //    }
                //    else
                //    {
                //        Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Voucher Updated Succesfully for Vocherno : " + VoucherNo);
                //        Message = "Updated Successfully";
                //    }
                //}
                //#endregion
                #endregion
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew After BillPrintData.HeaderExtraFieldValues Method");
                return new IdNamePair { ID = fieldId, Name = fieldName, Tag = TagValue };
            }
            catch (Exception ex)
            {
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew POSSales exception:" + ex.Message);

                return new IdNamePair();
            }
        }

        public Date GetIntToDate(int iDate)
        {
            try
            {
                return (new Date(iDate, CalendarType.Gregorean));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string getServiceLink()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string strFileName = "";
            string sAppPath = BL_Configdata.Focus8Path;
            strFileName = sAppPath + "\\ERPXML\\ServerSettings.xml";

            xmlDoc.Load(strFileName);
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/ServSetting/MasterServer/ServerName");
            string strValue;
            XmlNode node = nodeList[0];
            if (node != null)
                strValue = node.InnerText;
            else
                strValue = "";
            return strValue;
        }

        public string GetSessionId(int CompId)
        {
            string sSessionId = "";
            try
            {
                int ccode = CompId;
                BL_Config configMan = new BL_Config(this.GetType().Assembly.Location);
                string ServerAPIIP = configMan.GetAppSetting("Server_API_IP");
                string User_Name = configMan.GetAppSetting("UserName");
                string Password = configMan.GetAppSetting("Password");

                //string User_Name = "su";
                //string Password = "su";
                //string ServerAPIIP = "localhost";
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew POSSales Session User_Name:" + User_Name);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew POSSales Session Password:" + Password);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew POSSales Session ServerAPIIP:" + ServerAPIIP);

                DataLogin datanum = new DataLogin();
                datanum.CompanyId = ccode;
                datanum.Username = User_Name;
                datanum.password = Password;
                List<DataLogin> lstd = new List<DataLogin>();
                lstd.Add(datanum);
                Login lngdata = new Login();
                lngdata.data = lstd;
                string sContent = JsonConvert.SerializeObject(lngdata);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew POSSales Session sContent:" + sContent);
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("Content-Type", "application/json");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    var arrResponse = client.UploadString("http://" + ServerAPIIP + "/Focus8API/Login", sContent);
                    //returnObject = new clsDeserialize().Deserialize<RootObject>(arrResponse);
                    Resultlogin lng = JsonConvert.DeserializeObject<Resultlogin>(arrResponse);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesNew POSSales Session Resultlogin:" + lng);

                    sSessionId = lng.data[0].fSessionId;
                }

                return sSessionId;
            }
            catch (Exception ex)
            {
                Clsdata.LogFile("POSSalesExeserver", "POSSalesNew Exception:" + ex.ToString());
            }
            return sSessionId;
        }
        public IdNamePair POSSalesRet(Transaction SalesReturnInput, Output SalesReturnOutput, Transaction SalesJournalInput, Output SalesJournalOutput, PaymentDetails RefundSetmntDtlsInput, BillPrinting BillPrintData, int iSalesReturnPrintFormatId, int iSaleReturnCopiesToPrint, IdNamePair[] ProductMasterFields, List<TemplateFields> bodyExtraTemplateFieldsForPrint, List<CustLineWiseUIR> ProductDetails, int ICompID, string CompanyCode )
        {
            string errors1 = "";
            string Message = "";
            int HeaderId = 0;
            var responseData = (VoucherPostStatus)SalesReturnOutput.ReturnData;
            Clsdata.LogFile("POSSalesExeserver", "Execute on server calling Companyid: " + ICompID);
            Clsdata.LogFile("POSSalesExeserver", "POSSalesRet VoucherNo: " + responseData.VoucherNo);
            
            
            try
            {
                POSSalesData.Root objResponse = new POSSalesData.Root();
                BL_Config configMan = new BL_Config(this.GetType().Assembly.Location);
                string ServerAPIIP = configMan.GetAppSetting("Server_API_IP");
                var FBRInvoiceNo = "";
                var ESPInvoiceNo = "";
                companyid = ICompID;
                string SessionId = "";
                int fieldId = Convert.ToInt32(configMan.GetAppSetting("FieldId"));//1672;
                string fieldName = "FBRInvNo";
                string TagValue = "";// "327788665512301/07/2021";

                if (companyid != 0)
                {
                    SessionId = GetSessionId(companyid);
                }
                Clsdata.LogFile("POSSalesExeserver", "POSSalesRet SessionId: " + SessionId);

                if (SalesReturnInput != null && SalesReturnInput.Header != null)
                {
                    VoucherType = Convert.ToInt32(SalesReturnInput.Header.VoucherType);
                    VoucherNo = Convert.ToString(responseData.VoucherNo);

                    DateTime dt = GetIntToDate(SalesReturnInput.Header.Date);
                    DateTime = dt.ToString("MM-dd-yyyy");
                }
                Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Vouchertype: " + VoucherType);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesRet VoucherNo: " + VoucherNo);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesRet DateTime: " + DateTime);
                Clsdata.LogFile("POSSalesExeserver", "POSSalesRet companyid: " + companyid);

                #region JSON CREATION
                List<ClsJsonData> DList = new List<ClsJsonData>();

                #region LoadVoucher
                HashData objHashRequest = new HashData();
                Hashtable objHash = new Hashtable();
                objHash.Add("Query", "select iHeaderId from tCore_Header_0 where iVoucherType=" + VoucherType + " and sVoucherNo='" + VoucherNo + "'");
                List<Hashtable> lstHash = new List<Hashtable>();
                lstHash.Add(objHash);
                objHashRequest.data = lstHash;
                string sContent1 = JsonConvert.SerializeObject(objHashRequest);
                using (var client = new WebClient())
                {
                    client.Headers.Add("fSessionId", SessionId);
                    client.Headers.Add("Content-Type", "application/json");
                    string sUrl = "http://" + ServerAPIIP + "/Focus8API/utility/ExecuteSqlQuery";
                    string strResponse = client.UploadString(sUrl, sContent1);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet string URL : --" + strResponse);
                    QueryClass.Root objHashResponse = JsonConvert.DeserializeObject<QueryClass.Root>(strResponse);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet string URL : --" + objHashResponse.data[0].Table[0].iHeaderId);
                    HeaderId = objHashResponse.data[0].Table[0].iHeaderId;
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Voucher HeaderId : --" + HeaderId);
                }

                using (var client = new WebClient())
                {
                    client.Headers.Add("fSessionId", SessionId);
                    string sUrl = "";
                    if (VoucherType == 3331)
                    {
                        sUrl = "http://" + ServerAPIIP + "/Focus8API/Screen/Transactions/POS Sales/" + VoucherNo;
                    }
                    else if (VoucherType == 1792)
                    {
                        sUrl = "http://" + ServerAPIIP + "/Focus8API/Screen/Transactions/POS - Sales Returns/" + VoucherNo;
                        fieldId = fieldId + 1;
                    }

                    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet string URL --" + VoucherNo + sUrl);
                    var strResponse = client.DownloadString(sUrl);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet stringstrResponse for Voucher No :" + VoucherNo + " --" + strResponse);
                    objResponse = JsonConvert.DeserializeObject<POSSalesData.Root>(strResponse);
                    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet DeserializeObject for Voucher No : " + VoucherNo + " --" + objResponse);
                }

                #endregion

                List<InvoiceItems> lst = new List<InvoiceItems>();
                Invoice objInv = new Invoice();
                objInv.InvoiceNumber = "";
                objInv.POSID = "932400";
                objInv.USIN = VoucherNo;
                objInv.DateTime = DateTime;
                objInv.InvoiceType = 3;
                objInv.PaymentMode = 1;

                objInv.TotalSaleValue = objResponse.data[0].Body.Sum(_ => _.FNet.Input);
                objInv.TotalQuantity = objResponse.data[0].Body.Sum(_ => _.Quantity);
                objInv.TotalTaxCharged = objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);
                objInv.TotalBillAmount = objResponse.data[0].Body.Sum(_ => _.FNet.Input) + objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);

                foreach (var item in objResponse.data[0].Body)
                {
                    int ProductId = item.Item__Id;
                    //POSSalesData.ProductRoot objProdResponse = new POSSalesData.ProductRoot();
                    //using (var client = new WebClient())
                    //{
                    //    client.Headers.Add("fSessionId", SessionId);
                    //    string sUrl = "http://" + ServerAPIIP + "/Focus8API/List/Masters/Core__Product?fields=iMasterId,PCTCode&where=iMasterId=" + ProductId;
                    //    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet string URL -- /n" + ProductId + sUrl);
                    //    var strResponse = client.DownloadString(sUrl);
                    //    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Product stringstrResponse for ProductId : " + ProductId + " --" + strResponse);
                    //    objProdResponse = JsonConvert.DeserializeObject<POSSalesData.ProductRoot>(strResponse);
                    //    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Product DeserializeObject for ProductId : " + ProductId + " --" + objProdResponse);
                    //}

                    InvoiceItems objitem = new InvoiceItems();
                    objitem.ItemName = item.Item__Name;
                    objitem.ItemCode = item.Item__Code;
                    objitem.PCTCode = "01011000"; //objProdResponse.data[0].PCTCode;
                    objitem.Quantity = item.Quantity;
                    objitem.SaleValue = item.FNet.Input;
                    objitem.TaxCharged = item.GSTPerc.Value;
                    objitem.TotalAmount = item.FNet.Input + item.GSTPerc.Value;
                    objitem.TaxRate = item.GSTPerc.Input;
                    objitem.InvoiceType = 3;
                    lst.Add(objitem);
                }
                objInv.Items = lst;

                string ClientAPIURL = configMan.GetAppSetting("ClientAPIURL");
                Clsdata.LogFile("POSSalesExeserver", "ClientAPIURL Data:" + ClientAPIURL);
                HttpClient Client = new HttpClient();
                var json2 = new JavaScriptSerializer().Serialize(objInv);
                Clsdata.LogFile("POSSalesExeserver", "Json Data:" + json2);
                var content = new StringContent(json2, Encoding.UTF8, "application/json");
                HttpResponseMessage response = Client.PostAsync(ClientAPIURL, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response from API");
                    Console.WriteLine("———————————————");
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    Clsdata.LogFile("POSSalesExeserver", "API Response:" + response.Content.ReadAsStringAsync().Result);

                    Root objRootResponse = new Root();
                    var Result = response.Content.ReadAsStringAsync().Result;
                    objRootResponse = JsonConvert.DeserializeObject<Root>(Result);
                    Clsdata.LogFile("POSSalesExeserver", "API DeserializeObject Response: " + objRootResponse);

                    FBRInvoiceNo = objRootResponse.InvoiceNumber;
                    Clsdata.LogFile("POSSalesExeserver", "FBRInvoiceNo : " + FBRInvoiceNo);

                    #region VoucherUpdateQuery
                    HashData objHashRequest1 = new HashData();
                    Hashtable objHash1 = new Hashtable();
                    if (VoucherType == 3331)
                    {
                        objHash1.Add("Query", "update tCore_HeaderData3331_0 set FBRInvNo='" + FBRInvoiceNo + "'  where iHeaderId=" + HeaderId + "");
                    }
                    else if (VoucherType == 1792)
                    {
                        objHash1.Add("Query", "update tCore_HeaderData1792_0 set FBRInvNo='" + FBRInvoiceNo + "'  where iHeaderId=" + HeaderId + "");
                    }

                    List<Hashtable> lstHash1 = new List<Hashtable>();
                    lstHash1.Add(objHash1);
                    objHashRequest1.data = lstHash1;
                    string sContent = JsonConvert.SerializeObject(objHashRequest1);
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("fSessionId", SessionId);
                        client.Headers.Add("Content-Type", "application/json");
                        string sUrl = "http://" + ServerAPIIP + "/Focus8API/utility/ExecuteNonQuery";
                        string strResponse = client.UploadString(sUrl, sContent);
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Update Response : --" + strResponse);
                        QueryClass.UpdateRoot objHashResponse = JsonConvert.DeserializeObject<QueryClass.UpdateRoot>(strResponse);
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Update Result: --" + objHashResponse.data[0].Result);
                        int UpdateResult = objHashResponse.data[0].Result;
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Voucher UpdateResult : --" + UpdateResult);
                    }

                    #endregion

                    #region BillPrintPDFCode
                    TagValue = FBRInvoiceNo;// "327788665512301/07/2021";
                    Clsdata.LogFile("POSSalesExeserver", "fieldId: " + fieldId + " FieldName: " + fieldName + " Tag: " + TagValue);
                    if (BillPrintData == null)
                    {
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet BillPrintData is null");
                        return new IdNamePair();
                    }
                    if (BillPrintData.HeaderExtraFieldValues == null)
                    {
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet BillPrintData.HeaderExtraFieldValues is null");
                        return new IdNamePair();
                    }
                    BillPrintData.HeaderExtraFieldValues = new List<IdNamePair>();
                    BillPrintData.HeaderExtraFieldValues.Add(new IdNamePair { ID = fieldId, Name = fieldName, Tag = TagValue });
                    #endregion

                    #region ESR Post Code
                    lst = new List<InvoiceItems>();
                    objInv = new Invoice();
                    objInv.InvoiceNumber = FBRInvoiceNo;
                    objInv.POSID = "932400";
                    objInv.USIN = VoucherNo;
                    objInv.DateTime = DateTime;
                    objInv.InvoiceType = 3;
                    objInv.PaymentMode = 1;

                    objInv.TotalSaleValue = objResponse.data[0].Body.Sum(_ => _.FNet.Input);
                    objInv.TotalQuantity = objResponse.data[0].Body.Sum(_ => _.Quantity);
                    objInv.TotalTaxCharged = objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);
                    objInv.TotalBillAmount = objResponse.data[0].Body.Sum(_ => _.FNet.Input) + objResponse.data[0].Body.Sum(_ => _.GSTPerc.Value);

                    foreach (var item in objResponse.data[0].Body)
                    {
                        int ProductId = item.Item__Id;
                        //POSSalesData.ProductRoot objProdResponse = new POSSalesData.ProductRoot();
                        //using (var client = new WebClient())
                        //{
                        //    client.Headers.Add("fSessionId", SessionId);
                        //    string sUrl = "http://" + ServerAPIIP + "/Focus8API/List/Masters/Core__Product?fields=iMasterId,PCTCode&where=iMasterId=" + ProductId;
                        //    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet string URL -- /n" + ProductId + sUrl);
                        //    var strResponse = client.DownloadString(sUrl);
                        //    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Product stringstrResponse for ProductId : " + ProductId + " --" + strResponse);
                        //    objProdResponse = JsonConvert.DeserializeObject<POSSalesData.ProductRoot>(strResponse);
                        //    Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Product DeserializeObject for ProductId : " + ProductId + " --" + objProdResponse);
                        //}

                        InvoiceItems objitem = new InvoiceItems();
                        objitem.ItemName = item.Item__Name;
                        objitem.ItemCode = item.Item__Code;
                        objitem.PCTCode = "01011000"; //objProdResponse.data[0].PCTCode;
                        objitem.Quantity = item.Quantity;
                        objitem.SaleValue = item.FNet.Input;
                        objitem.TaxCharged = item.GSTPerc.Value;
                        objitem.TotalAmount = item.FNet.Input + item.GSTPerc.Value;
                        objitem.TaxRate = item.GSTPerc.Input;
                        objitem.InvoiceType = 3;
                        lst.Add(objitem);
                    }
                    objInv.Items = lst;

                    string ClientPOSTURL = configMan.GetAppSetting("ClientPOSTURL");
                    Clsdata.LogFile("POSSalesExeserver", "ClientPOSTURL Data:" + ClientPOSTURL);
                    HttpClient Client1 = new HttpClient();
                    var json21 = new JavaScriptSerializer().Serialize(objInv);
                    Clsdata.LogFile("POSSalesExeserver", "Json Data:" + json21);
                    var content1 = new StringContent(json21, Encoding.UTF8, "application/json");
                    HttpResponseMessage response1 = Client1.PostAsync(ClientPOSTURL, content1).Result;
                    try
                    {
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST Before Response: " + response1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST response1:" + response1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST After Response: " + response1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST Before Result Response: " + response1.Content.ReadAsStringAsync().Result);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST Response:" + response1.Content.ReadAsStringAsync().Result);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST After Result Response: " + response1.Content.ReadAsStringAsync().Result);

                        ESPRoot objESPRootResponse = new ESPRoot();
                        var Result1 = response1.Content.ReadAsStringAsync().Result;
                        objESPRootResponse = JsonConvert.DeserializeObject<ESPRoot>(Result1);
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST DeserializeObject Response: " + objESPRootResponse);

                        ESPInvoiceNo = objESPRootResponse.InvoiceNumber;
                        Clsdata.LogFile("POSSalesExeserver", "ESP POST InvoiceNo : " + ESPInvoiceNo);

                        //if (response1.IsSuccessStatusCode)
                        //{
                        //    Console.WriteLine("Response from ESP POST");
                        //    Console.WriteLine("———————————————");
                        //    Console.WriteLine(response1.Content.ReadAsStringAsync().Result);
                        //    Clsdata.LogFile("POSSalesExeserver", "ESP POST Response:" + response1.Content.ReadAsStringAsync().Result);

                        //    //objESPRootResponse = new ESPRoot();
                        //    //Result1 = response1.Content.ReadAsStringAsync().Result;
                        //    //objESPRootResponse = JsonConvert.DeserializeObject<ESPRoot>(Result1);
                        //    //Clsdata.LogFile("POSSalesExeserver", "ESP POST DeserializeObject Response: " + objESPRootResponse);

                        //    //ESPInvoiceNo = objESPRootResponse.InvoiceNumber;
                        //    //Clsdata.LogFile("POSSalesExeserver", "ESP POST InvoiceNo : " + ESPInvoiceNo);
                        //}
                    }
                    catch (Exception ex)
                    {
                        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet POSSales exception:" + ex.Message);
                    }
                    #endregion
                }
                #endregion

                #region VoucherResave
                //string POSSales = "http://" + ServerAPIIP + "/Focus8API/ Transactions/Vouchers/POS Sales";
                //#region Header
                //Hashtable header = new Hashtable
                //{
                //    //mandatory
                //    { "HeaderId", HeaderId},
                //    { "VoucherNo", VoucherNo},
                //    { "Date",objResponse.data[0].Header.Date },
                //    { "SalesAC", objResponse.data[0].Header.SalesAC__Id },
                //    { "CustomerAC", objResponse.data[0].Header.CustomerAC__Id },
                //    { "Currency", objResponse.data[0].Header.Currency__Id},
                //    { "ExchangeRate", objResponse.data[0].Header.ExchangeRate},
                //    { "Outlet", objResponse.data[0].Header.Outlet__Id},
                //    {"Counter", objResponse.data[0].Header.Counter__Id},
                //    {"Member", objResponse.data[0].Header.Member__Id },
                //    {"Employee", objResponse.data[0].Header.Employee__Id },
                //    { "Table", objResponse.data[0].Header.Table__Id },
                //    {"Guest",objResponse.data[0].Header.Guest__Id },
                //    {"sPreOrderBillReferenceNo",objResponse.data[0].Header.sPreOrderBillReferenceNo},
                //    {"iShift", objResponse.data[0].Header.iShift},
                //    {"sNarration",objResponse.data[0].Header.sNarration},
                //    {"iTransactionType", objResponse.data[0].Header.iTransactionType},
                //    {"iSelectedEmployee",objResponse.data[0].Header.iSelectedEmployee__Id},
                //    {"iCreatedUser", objResponse.data[0].Header.iCreatedUser},
                //    {"sMessageGreeting", objResponse.data[0].Header.sMessageGreeting},
                //    {"sSpecialInstruction",objResponse.data[0].Header.sSpecialInstruction},
                //    {"sTokenNumber", objResponse.data[0].Header.sTokenNumber},
                //    {"iProductionOutlet", objResponse.data[0].Header.iProductionOutlet__Id},
                //    {"iDeliveryType",objResponse.data[0].Header.iDeliveryType},
                //    {"sPhoneNumber", objResponse.data[0].Header.sPhoneNumber},
                //    {"dTotalSaving", objResponse.data[0].Header.dTotalSaving},
                //    {"sPosOrderTypeName",objResponse.data[0].Header.sPosOrderTypeName},
                //    {"FBRInvNo",FBRInvoiceNo}
                //};
                //#endregion

                //List<Hashtable> body = new List<Hashtable>();
                //#region Body
                //Hashtable row1 = new Hashtable();
                //foreach (var item in objResponse.data[0].Body)
                //{
                //    row1 = new Hashtable
                //    {
                //        { "Item",item.Item__Id},
                //        { "iSetQty", item.iSetQty },
                //        { "Unit", item.Unit__Id },
                //        { "Quantity", item.Quantity },
                //        { "Rate", item.Rate },
                //        { "Gross", item.Gross},
                //        { "Discount", item.Discount},
                //        { "Disc%", item.DiscPerc},
                //        { "Total Discount", item.TotalDisc},
                //        { "FNet", item.FNet},
                //        { "GST%", item.GSTPerc},
                //        { "iProductType", item.iProductType},
                //        { "sLineNarration", item.sLineNarration},
                //        { "sComplimentaryRemarks", item.sComplimentaryRemarks},
                //        { "TransactionId", item.TransactionId},
                //    };
                //    body.Add(row1);
                //}
                //#endregion

                //var postingData = new PostingData();
                //postingData.data.Add(new Hashtable { { "Header", header }, { "Body", body } });
                //string sContent = JsonConvert.SerializeObject(postingData);
                //Clsdata.LogFile("POSSalesExeserver", "Voucher Update  sContent:" + sContent);
                //errors1 = "";
                //#region Response
                //var Vresponse = Focus8API.Post(POSSales, sContent, SessionId, ref errors1);
                //if (Vresponse != null)
                //{
                //    var responseData = JsonConvert.DeserializeObject<APIResponse.PostResponse>(Vresponse);
                //    if (responseData.result == -1)
                //    {
                //        Message = $"Posting Failed: {responseData.message } \n";
                //        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Posting Failed: " + Message);
                //    }
                //    else
                //    {
                //        Clsdata.LogFile("POSSalesExeserver", "POSSalesRet Voucher Updated Succesfully for Vocherno : " + VoucherNo);
                //        Message = "Updated Successfully";
                //    }
                //}
                //#endregion
                #endregion
                Clsdata.LogFile("POSSalesExeserver", "POSSalesRet After BillPrintData.HeaderExtraFieldValues Method");
                return new IdNamePair { ID = fieldId, Name = fieldName, Tag = TagValue };
            }
            catch (Exception ex)
            {
                Clsdata.LogFile("POSSalesExeserver", "POSSalesRet POSSales exception:" + ex.Message);

                return new IdNamePair();
            }
        }
    }
}
