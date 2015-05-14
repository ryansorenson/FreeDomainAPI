using System;
using Automation.Framework.TestSetUpAndSettings;
using FreeDomainAPI.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AssertHelper = Automation.Framework.TestSetUpAndSettings.AssertHelper;

namespace DomainsAPI
{
  [TestClass]
  public class DomainsAPITests : TestBase
  {
    public DomainsAPITests()
      : base(TestSetUp.GetEnvironment(Settings.Default.Environment))
    {
    }

    [TestMethod]
    public void IsAvailable()
    {
      string isAvail = DomainsAPI.GetAvailabilityStatus("thisrandomdomainshouldbeavail.com");
      AssertHelper.AddResults(isAvail.Equals("1000"), "Domain name is not available");
      AssertHelper.AddResults(DomainsAPI.exactMatch.status.Equals("1000"), "Domain name is not available");
    }

    [TestMethod]
    public void SearchedDomainEqualsExactMatch()
    {
      string fqdn = DomainsAPI.GetFQDN("target.com");
      AssertHelper.AddResults(fqdn.Equals("target.com"), "Searched domain is not the same as the exact match domain");
    }

    [TestMethod]
    public void CurrentPriceMatchesCurrencyProvider()
    {
      string cPrice = DomainsAPI.GetCurrentPrice("thisrandomdomainshouldbeavail.com").ToString();
      cPrice = cPrice.Insert(cPrice.Length - 2, ".");
      string providerPrice = String.Format(Pricing.Pricing.GetCurrentPriceForProduct(101, Automation.Framework.Enums.Environment.Prod));
      AssertHelper.AddResults(cPrice == providerPrice, "Pricing is not correct");
    }
  }
}