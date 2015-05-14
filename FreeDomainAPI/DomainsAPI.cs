using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Automation.Framework.Data.DatabaseModels;
using FreeDomainAPI.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pricing;

namespace DomainsAPI
{
  public static class DomainsAPI
  {
    private const string endpoint = "/domainsapi/v1/search/free?q=";
    public static ExactMatch exactMatch;
    public static ProductList productList;
    public static DomainsResponseObject domainsResponse;

    private static string GetURL()
    {
      string url = "http://www.godaddy.com" + endpoint;
      if (Settings.Default.Environment == "Test")
      {
        url = "http://www.test-godaddy.com" + endpoint;
      }
      if (Settings.Default.Environment == "Dev")
      {
        url = "http://www.dev-godaddy.com" + endpoint;
      }
      return url;
    }

    public static string GetAvailabilityStatus(string domainName)
    {
      GetResponse(domainName);
      return domainsResponse.exactMatch.status;
    }

    public static string GetFQDN(string domainName)
    {
      GetResponse(domainName);
      return exactMatch.fqdn;
    }

    public static int GetCurrentPrice(string domainName)
    {
      GetResponse(domainName);
      int cPrice = 0;
      string tld = domainName.Substring(domainName.IndexOf(".") + 1);
      foreach (var product in domainsResponse.products)
      {
        if (tld == product.tld)
        {
          cPrice = product.priceInfo.currentPrice;
        }
      }
      return cPrice;
    }

    private static void GetResponse(string domainName)
    {
      var client = new HttpClient();
      string uri = GetURL() + domainName;
      var response = client.GetAsync(uri);
      var jsonResponse = response.Result.Content.ReadAsStringAsync().Result;

      PullResultsfromJson(jsonResponse);
    }

    private static void PullResultsfromJson(string jsonResponse)
    {
      var domainsObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);

      string status = domainsObject.SelectToken("ExactMatchDomain.AvailabilityStatus").Value<string>();
      string sld = domainsObject.SelectToken("ExactMatchDomain.NameWithoutExtension").Value<string>();
      string fqdn = domainsObject.SelectToken("ExactMatchDomain.Fqdn").Value<string>();

      exactMatch = new ExactMatch();
      exactMatch.status = status;
      exactMatch.sld = sld;
      exactMatch.fqdn = fqdn;

      domainsResponse = new DomainsResponseObject();
      domainsResponse.exactMatch = exactMatch;

      List<ProductList> prodList = new List<ProductList>();
      PriceInfo priceInfo;
      ProductList product;
      int i = domainsObject.SelectTokens("Products").Children().Count();
      foreach (var token in domainsObject.SelectTokens("Products").Children())
      {
        product = new ProductList();
        priceInfo = new PriceInfo();
        product.tld = token.SelectToken("Tld").Value<string>();
        product.phaseID = token.SelectToken("PhaseId").Value<int>();
        product.tierID = token.SelectToken("TierId").Value<int>();
        product.productID = token.SelectToken("ProductId").Value<int>();
        product.hasIcannFee = token.SelectToken("HasIcannFee").Value<bool>();

        priceInfo.listPrice = token.SelectToken("PriceInfo.ListPrice").Value<int>();
        priceInfo.currentPrice = token.SelectToken("PriceInfo.CurrentPrice").Value<int>();
        product.priceInfo = priceInfo;
        prodList.Add(product);
      }
      domainsResponse.products = prodList;
    }
  }
}