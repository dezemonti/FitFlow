using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace FitFlow.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SoftlandController : Controller
    {
        // POST: Softland/GetProducts
        [HttpPost]
        [Route("GetProducts")]
        public IActionResult GetProducts()
        {
            var clientToken = new RestClient("http://186.67.224.61:8065/");
            var requestToken = new RestRequest("login/obtener-token", Method.Post);
            requestToken.AddJsonBody(new { username = "admin", password = "1234" });
            var tokenResult = clientToken.Execute(requestToken);

            var token = tokenResult?.Content;
            var clientProducts = new RestClient("http://186.67.224.61:8065/");
            var requestProducts = new RestRequest("softland/lista-productos", Method.Post);

            requestProducts.AddJsonBody(new {
                IdEmpresa = 1,
                CodigoProducto = ""
            });

            requestProducts.AddHeader("Authorization", $"Bearer {token?.Substring(1, token.Length - 2)}");
            var productsStock = clientProducts.Execute(requestProducts).Content;
            return Ok(productsStock);
        }

        // POST: Softland/GetProducts/5
        [HttpPost]
        [Route("GetProducts/{code}")]
        public IActionResult GetProducts(string code)
        {
            var clientToken = new RestClient("http://186.67.224.61:8065/");
            var requestToken = new RestRequest("login/obtener-token", Method.Post);
            requestToken.AddJsonBody(new { username = "admin", password = "1234" });
            var tokenResult = clientToken.Execute(requestToken);

            var token = tokenResult?.Content;
            var clientProducts = new RestClient("http://186.67.224.61:8065/");
            var requestProducts = new RestRequest("softland/lista-productos", Method.Post);

            requestProducts.AddJsonBody(new { 
                IdEmpresa = 1,
                CodigoProducto = code
            });

            requestProducts.AddHeader("Authorization", $"Bearer {token?.Substring(1, token.Length - 2)}");
            requestProducts.AddHeader("Access-Control-Allow-Origin", "*");
            var productsStock = clientProducts.Execute(requestProducts).Content;
            return Ok(productsStock);
        }

        
        // Función auxiliar para obtener Contacto de CRM
        public static Entity RetrieveEntityById(IOrganizationService service, string entityName, Guid entityGuid)
        {
            ColumnSet columnSet = new ColumnSet(true);
            Entity RetrievedEntity = service.Retrieve(entityName, entityGuid, columnSet);
            return RetrievedEntity;
        }

        // POST: Softland/SendSaleOrder/1
        [HttpPost]
        [Route("SendSaleOrder/{code}")]
        public IActionResult SendSaleOrder(string code)
        {
            var clientToken = new RestClient("http://186.67.224.61:8065/");
            var requestToken = new RestRequest("login/obtener-token", Method.Post);
            requestToken.AddJsonBody(new { username = "admin", password = "1234" });
            var tokenResult = clientToken.Execute(requestToken);

            var token = tokenResult?.Content;
            var clientProducts = new RestClient("http://186.67.224.61:8065/");
            var requestProducts = new RestRequest("softland/inserta-nota-venta", Method.Post);

            requestProducts.AddJsonBody(new { 
                CabeceraNV = (new {
                    IdEmpresa = code,
                    NumeroCotizacion = 0,
                    CodigoAuxiliar = "76029899",
                    FechaNota = "20221026",
                    FechaEntrega = "20221103",
                    CodigoVendedor = "65",
                    CodigoMoneda = "02",
                    CodigoListaPrecio = "072",
                    Observacion = "nota de venta de prueba",
                    CondicionVenta = "52",
                    NombreContacto = "Danny Garrido",
                    CodigoCentroCosto = "80-006",
                    SubTotal = "3128.84"
                }),
                DetalleNV = (new {
                    Linea = 1,
                    FechaEntrega = "20221103",
                    CodigoProducto = "C02-428949",
                    Cantidad = 1,
                    Valor = 421.58,
                    DescripcionProducto = "BOOSTER ACTUATOR SHIP ASM"
                }, new {
                    Linea = 2,
                    FechaEntrega = "20221103",
                    CodigoProducto = "C02-451500",
                    Cantidad = 1,
                    Valor = 685.44,
                    DescripcionProducto = "Cyl 67L 100 lbs CV-98 C02 ANS"
                }),
            });
            requestProducts.AddHeader("Authorization", $"Bearer {token?.Substring(1, token.Length - 2)}");
            var productsStock = clientProducts.Execute(requestProducts).Content;

            return Ok();
        }

        // POST: Softland/GetOrderStatus
        [HttpPost]
        [Route("GetOrderStatus/{orderNumber}")]
        public IActionResult GetOrderStatus(string orderNumber)
        {
            var clientToken = new RestClient("http://186.67.224.61:8065/");
            var requestToken = new RestRequest("login/obtener-token", Method.Post);
            requestToken.AddJsonBody(new { username = "admin", password = "1234" });
            var tokenResult = clientToken.Execute(requestToken);

            var token = tokenResult?.Content;
            var clientOrderStatus = new RestClient("http://186.67.224.61:8065/");
            var requestOrderStatus = new RestRequest("softland/estado-nota-pedido", Method.Post);

            requestOrderStatus.AddJsonBody(new {
                IdEmpresa = 1,
                NVNumero = Int32.Parse(orderNumber)
            });

            requestOrderStatus.AddHeader("Authorization", $"Bearer {token?.Substring(1, token.Length - 2)}");
            var orderStatusResponse = clientOrderStatus.Execute(requestOrderStatus).Content;

            return Ok(orderStatusResponse);
        }

        // POST: Softland/GetCreditStatus
        [HttpPost]
        [Route("GetCreditStatus/{rut}")]
        public IActionResult GetCreditStatus(string rut)
        {
            var clientToken = new RestClient("http://186.67.224.61:8065/");
            var requestToken = new RestRequest("login/obtener-token", Method.Post);
            requestToken.AddJsonBody(new { username = "admin", password = "1234" });
            var tokenResult = clientToken.Execute(requestToken);

            var token = tokenResult?.Content;
            var clientCreditStatus = new RestClient("http://186.67.224.61:8065/");
            var requestCreditStatus = new RestRequest("softland/cuenta-corriente", Method.Post);

            requestCreditStatus.AddJsonBody(new
            {
                IdEmpresa = 1,
                Rut = rut
            });

            requestCreditStatus.AddHeader("Authorization", $"Bearer {token?.Substring(1, token.Length - 2)}");
            var responseCreditStatus = clientCreditStatus.Execute(requestCreditStatus).Content;

            return Ok(responseCreditStatus);
        }
    }
}
