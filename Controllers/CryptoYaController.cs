using System.Globalization;
using CryptoTrade.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CryptoTrade.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CryptoYaController : ControllerBase
    {
        private readonly ILogger<CryptoYaController> _logger;

        public CryptoYaController(ILogger<CryptoYaController> logger)
        {
            _logger = logger;
        }


        [AllowAnonymous]
        [HttpGet("getByCrypto/{fiat}/{volumen}")]
        public async Task<IActionResult> GetByCrypto(string fiat, decimal volumen)
        {
            string baseUrl = "https://criptoya.com/api";
            var cryptos = "BTC,DAI,ETH,USDT,USDC,BNB,ADA,BUSD,DOGE,MATIC,SLP,XRP,AVAX,AXS,BAT,BCH,CAKE,CHZ,DOT,EOS,FTM,LINK,LTC,MANA,PAXG,SAND,SHIB,SOL,UNI,AAVE,ALGO,TRX,NUARS,XLM,USDP,UXD";
            var cryptoList = cryptos.Split(",");

            using var client = new HttpClient();
            try
            {
                foreach (var crypto in cryptoList)
                {
                    // Construye la URL completa con el endpoint y los parámetros necesarios
                    var url = $"{baseUrl}/{crypto}/{fiat}/{volumen.ToString(CultureInfo.InvariantCulture)}";

                    var response = await client.GetAsync(url);
                    decimal finalAsk = 0;
                    var exchangeNameAsk = string.Empty;
                    decimal finalBid = 0;
                    var exchangeNameBid = string.Empty;

                    if (response.IsSuccessStatusCode)
                    {
                        var quotations = JsonConvert.DeserializeObject<Dictionary<string, CryptoPriceModel>>(await response.Content.ReadAsStringAsync());

                        if (quotations != null)
                        {
                            // precio compra
                            foreach (var quotation in quotations)
                            {
                                if (finalAsk == 0)
                                    finalAsk = quotation.Value.TotalAsk;

                                if (quotation.Value.TotalAsk < finalAsk)
                                {
                                    finalAsk = quotation.Value.TotalAsk;
                                    exchangeNameAsk = quotation.Key;
                                }
                            }

                            // precio venta
                            foreach (var quotation in quotations)
                            {
                                if (finalBid == 0)
                                    finalBid = quotation.Value.TotalBid;

                                if (finalBid > quotation.Value.TotalBid && quotation.Key != exchangeNameAsk)
                                {
                                    finalBid = quotation.Value.TotalBid;
                                    exchangeNameBid = quotation.Key;
                                }
                            }

                            if (finalAsk < finalBid)
                                return Ok($"Flaa compra {crypto} en {exchangeNameAsk} a {finalAsk} y vende en {exchangeNameBid} a {finalBid} payaso");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        throw new Exception("Error en api");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(ex.Message);
            }

            return Ok("No compres nada");
        }
    }
}
