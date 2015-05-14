using System.Collections.Generic;

namespace DomainsAPI
{
  public class DomainsResponseObject
  {
    public List<ProductList> products { get; set; }
    public ExactMatch exactMatch { get; set; }
  }

  public class ExactMatch
  {
    public string status { get; set; }
    public bool backorderable { get; set; }
    public string sld { get; set; }
    public string fqdn { get; set; }
    public string currentPrice { get; set; }
  }

  public class ProductList
  {
    public string tld { get; set; }
    public int phaseID { get; set; }
    public int tierID { get; set; }
    public int productID { get; set; }
    public bool hasIcannFee { get; set; }
    public PriceInfo priceInfo { get; set; }
  }

  public class PriceInfo
  {
    public int listPrice { get; set; }
    public string listPriceDisplay { get; set; }
    public int currentPrice { get; set; }
    public string currentPriceDisplay { get; set; }
  }
}