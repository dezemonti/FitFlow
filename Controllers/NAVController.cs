using FitFlow.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Serializers.Xml;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using System.Drawing.Text;
using System.Configuration;

namespace FitFlow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NAVController : Controller
    {
        private IConfiguration configuration;

        public NAVController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }

        // POST: NAV/GetProducts
        [HttpPost]
        [Route("GetProducts")]
        public IActionResult GetProducts(string productNumber)
        {
            RestClient clientProduct = new RestClient("https://navdev.fitflow.com:7047/");
            RestRequest requestProduct = new RestRequest("DynamicsNAV71/WS/Fitflow/Codeunit/CRMIntegrations", Method.Post);
            string key = configuration.GetSection("AppSettings").GetSection("NAV").Value;

            string bodyRequestProduct =
            $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:crm=\"urn:microsoft-dynamics-schemas/codeunit/CRMIntegrations\" xmlns:x50=\"urn:microsoft-dynamics-nav/xmlports/x50015\" xmlns:x501=\"urn:microsoft-dynamics-nav/xmlports/x50016\">\r\n" +
            $"   <soapenv:Header />\r\n" +
            $"   <soapenv:Body>\r\n" +
            $"      <crm:GetStock>\r\n" +
            $"         <crm:itemNo>{productNumber}</crm:itemNo>\r\n" +
            $"         <crm:cRMResponse>\r\n" +
            $"            e\r\n" +
            $"            <!--1 or more repetitions:-->\r\n" +
            $"            <x50:Item>\r\n" +
            $"               <x50:ItemNo>?</x50:ItemNo>\r\n" +
            $"               <x50:Description>?</x50:Description>\r\n" +
            $"               <x50:Stock>?</x50:Stock>\r\n" +
            $"            </x50:Item>\r\n" +
            $"            gero\r\n" +
            $"         </crm:cRMResponse>\r\n" +
            $"      </crm:GetStock>\r\n" +
            $"   </soapenv:Body>\r\n" +
            $"</soapenv:Envelope>";

            requestProduct.Timeout = -1;
            requestProduct.AddXmlBody(bodyRequestProduct);
            requestProduct.AddHeader("Authorization", $"Basic {key}");
            requestProduct.AddHeader("SOAPAction", "");
            requestProduct.AddHeader("Content-Type", "application/xml");

            RestResponse productStock = clientProduct.Execute(requestProduct);

            Regex regexItem = new Regex("<ItemNo>(.*)</ItemNo>");
            Regex regexDescription = new Regex("<Description>(.*)</Description>");
            Regex regexStock = new Regex("<Stock>(.*)</Stock>");
            var item = regexItem.Match(productStock.Content);
            var description = regexDescription.Match(productStock.Content);
            var stock = regexStock.Match(productStock.Content);

            Product product = new Product();
            product.ItemNumber = item.Groups[1].ToString();
            product.Description = description.Groups[1].ToString();
            product.Stock = Convert.ToInt32(stock.Groups[1].ToString());

            string json = JsonConvert.SerializeObject(product);

            return Ok(json);
        }

        // POST: NAV/GetProducts
        [HttpPost]
        [Route("GetCreditLimit")]
        public IActionResult GetCreditLimit(string clientCode)
        {
            var clientCreditLimit = new RestClient("https://navdev.fitflow.com:7047/");
            var requestCreditLimit = new RestRequest("DynamicsNAV71/WS/Fitflow/Codeunit/CRMIntegrations", Method.Post);
            string key = configuration.GetSection("AppSettings").GetSection("NAV").Value;

            string bodyRequestCreditLimit =
                $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:crm=\"urn:microsoft-dynamics-schemas/codeunit/CRMIntegrations\" xmlns:x50=\"urn:microsoft-dynamics-nav/xmlports/x50015\" xmlns:x501=\"urn:microsoft-dynamics-nav/xmlports/x50016\">\r\n" +
                $"   <soapenv:Header />\r\n" +
                $"   <soapenv:Body>\r\n" +
                $"      <crm:GetCreditLimit>\r\n" +
                $"         <crm:customerNo>{clientCode}</crm:customerNo>\r\n" +
                $"         <crm:cRMResponse>\r\n" +
                $"            e\r\n" +
                $"            <!--1 or more repetitions:-->\r\n" +
                $"            <x50:Cust>\r\n" +
                $"               <x50:CustNo>?</x50:CustNo>\r\n" +
                $"               <x50:Name>?</x50:Name>\r\n" +
                $"               <x50:CreditLimit>?</x50:CreditLimit>\r\n" +
                $"            </x50:Cust>\r\n" +
                $"            gero\r\n" +
                $"         </crm:cRMResponse>\r\n" +
                $"      </crm:GetCreditLimit>\r\n" +
                $"   </soapenv:Body>\r\n" +
                $"</soapenv:Envelope>";

            requestCreditLimit.Timeout = -1;
            requestCreditLimit.AddXmlBody(bodyRequestCreditLimit);
            requestCreditLimit.AddHeader("Authorization", $"Basic {key}");
            requestCreditLimit.AddHeader("SOAPAction", "");
            requestCreditLimit.AddHeader("Content-Type", "application/xml");

            RestResponse creditLimit = clientCreditLimit.Execute(requestCreditLimit);

            Regex regexCustNo = new Regex("<CustNo>(.*)</CustNo>");
            Regex regexName = new Regex("<Name>(.*)</Name>");
            Regex regexCreditLimit = new Regex("<CreditLimit>(.*)</CreditLimit>");
            var custno = regexCustNo.Match(creditLimit.Content);
            var name = regexName.Match(creditLimit.Content);
            var limit = regexCreditLimit.Match(creditLimit.Content);

            Customer customer = new Customer();
            customer.CustNo = custno.Groups[1].ToString();
            customer.CustName = name.Groups[1].ToString();
            customer.CreditLimit = limit.Groups[1].ToString();

            string json = JsonConvert.SerializeObject(customer);

            return Ok(json);
        }

        // POST: NAV/GetProducts
        [HttpPost]
        [Route("CreateSalesOrderCRM")]
        public IActionResult CreateSalesOrderCRM(string orderNumber)
        {
            var clientSalesOrderCRM = new RestClient("http://20.25.122.85:14057/");
            var requestSalesOrderCRM = new RestRequest("FitflowWS/WS/Fitflow/Codeunit/CRMIntegrations/CreateSalesOrderCRM", Method.Post);

            string bodyRequestSalesOrderCRM =
                $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:crm=\"urn:microsoft-dynamics-schemas/codeunit/CRMIntegrations\" xmlns:x50=\"urn:microsoft-dynamics-nav/xmlports/x50015\" xmlns:x501=\"urn:microsoft-dynamics-nav/xmlports/x50016\">\r\n" +
                $"   <soapenv:Header />\r\n" +
                $"   <soapenv:Body>\r\n" +
                $"      <crm:CreateSalesOrderCRM>\r\n" +
                $"         <crm:cRMMessage>\r\n" +
                $"         e" +
                $"         <x50:Header>" +
                $"               <x50:NoCRM>{orderNumber}</x50:NoCRM>" +
                $"               <x50:SellToCustomerNo>{"aw_navcode"}</x50:SellToCustomerNo>" +
                //$"               <x50:SellToCustomerName>ABELIN SA</x50:SellToCustomerName>" +
                //$"               <x50:SelltoAddress>CAL.SUPE NRO. 470 URB. SANTA MARINA SUR</x50:SelltoAddress>" +
                $"               <x50:CurrencyCode>USD</x50:CurrencyCode>" +
                $"               <x50:LocationCode>CENTRAL</x50:LocationCode>" +
                $"               <x50:PostingDate>06/10/22</x50:PostingDate>" +
                $"               <x50:OrderDate>06/10/22</x50:OrderDate>" +
                $"               <x50:ShortCutDim1>MERCAD</x50:ShortCutDim1>" +
                $"               <x50:CustomerPostGr>NAC-ME</x50:CustomerPostGr>" +
                $"               <x50:DocumentDate>06/10/22</x50:DocumentDate>" +
                //$"               <x50:DeliveryDate>08/10/22</x50:DeliveryDate>" +
                $"               <x50:ExtermalDoc>900-0013</x50:ExtermalDoc>" +
                $"               <x50:SalespersonCode>co</x50:SalespersonCode>" +
                $"               <x50:Status>{"Abierto"}</x50:Status>" +
                //$"               <x50:ShippingAgentCode>AAFITFLOW</x50:ShippingAgentCode>" +
                //$"               <x50:PurchaseDocType>09</x50:PurchaseDocType>" +
                //$"               <x50:ShippingNoSeries>GUIA</x50:ShippingNoSeries>" +
                //$"               <x50:PostingNoSeries>V-FAC+</x50:PostingNoSeries>" +
                //$"               <x50:UserID>uln</x50:UserID>" +
                $"               <!--1 or more repetitions:-->" +
                $"               <x50:Lines>" +
                $"                  <x50:NoCRML>900003</x50:NoCRML>" +
                $"                  <x50:Type>2</x50:Type>" +
                $"                  <x50:No>4-400-G</x50:No>" +
                $"                  <x50:LocationCode>CENTRAL</x50:LocationCode>" +
                $"                  <x50:Quantity>2</x50:Quantity>" +
                $"                  <x50:UnitOfMeasureCode>UND</x50:UnitOfMeasureCode>" +
                $"                  <x50:LineDsctoPer>0</x50:LineDsctoPer>" +
                $"                  <x50:UnitPrice>320</x50:UnitPrice>" +
                //$"                  <x50:LineAmount>0</x50:LineAmount>" +
                $"                  <x50:LineDiscountAmt>0</x50:LineDiscountAmt>" +
                //$"                  <x50:PlannedDelDate>06/10/22</x50:PlannedDelDate>" +
                //$"                  <x50:PlannedShipDate>06/10/22</x50:PlannedShipDate>" +
                //$"                  <x50:ShipmentDate>06/10/22</x50:ShipmentDate>" +
                //$"                  <x50:PurchaseDocTypeL>0</x50:PurchaseDocTypeL>" +
                //$"                  <x50:ShortDimCode2>EXTINCION</x50:ShortDimCode2>" +
                //$"                  <x50:SubLineCode></x50:SubLineCode>" +
                //$"                  <x50:SubLineLineCode></x50:SubLineLineCode>" +
                $"                  <x50:BinCode></x50:BinCode>" +
                //$"                  <x50:ShippingAgentCode>AAFITFLOW</x50:ShippingAgentCode>" +
                //$"                  <x50:ShortDimCode1L>ADM010</x50:ShortDimCode1L>" +
                //$"                  <x50:ExistPrice>0</x50:ExistPrice>" +
                //$"                  <x50:VATBusPostingGr></x50:VATBusPostingGr>" +
                //$"                  <x50:CustomerPriceGr></x50:CustomerPriceGr>" +
                $"               </x50:Lines>" +
                $"         </x50:Header>" +
                $"         </crm:cRMMessage>\r\n" +
                $"         <crm:cRMResponse>\r\n" +
                $"            <!--1 or more repetitions:-->\r\n" +
                $"            <x501:ResponseList>\r\n" +
                $"               <x501:No>?</x501:No>\r\n" +
                $"               <x501:CodRespuesta>?</x501:CodRespuesta>\r\n" +
                $"               <x501:DescRespuesta>?</x501:DescRespuesta>\r\n" +
                $"            </x501:ResponseList>\r\n" +
                $"         </crm:cRMResponse>\r\n" +
                $"      </crm:CreateSalesOrderCRM>\r\n" +
                $"   </soapenv:Body>\r\n" +
                $"</soapenv:Envelope>";

            requestSalesOrderCRM.AddXmlBody(bodyRequestSalesOrderCRM);
            var salesOrdenCRM = clientSalesOrderCRM.Execute(requestSalesOrderCRM);

            return Ok(salesOrdenCRM);
        }

        /*
        // POST: NAV/GetProducts
        [HttpPost]
        [Route("CreateSalesOrder")]
        public IActionResult CreateSalesOrder(int orderNumber)
        {
            var clientProduct = new RestClient("http://20.25.122.85:14057/");
            var requestProduct = new RestRequest("FitflowWS/WS/Fitflow/Codeunit/CRMIntegrations/CreateSO", Method.Post);

            string bodyRequest =
                $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:crm=\"urn:microsoft-dynamics-schemas/codeunit/CRMIntegrations\" xmlns:x50=\"urn:microsoft-dynamics-nav/xmlports/x50015\" xmlns:x501=\"urn:microsoft-dynamics-nav/xmlports/x50016\">\r\n" +
                $"   <soapenv:Header />\r\n" +
                $"   <soapenv:Body>\r\n" +
                $"      <crm:GetStock>\r\n" +
                $"         <crm:itemNo>{0}</crm:itemNo>\r\n" +
                $"      </crm:GetStock>\r\n" +
                $"   </soapenv:Body>\r\n" +
                $"</soapenv:Envelope>";

            return Ok();
        }
        */
    }
}
