using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;

namespace NUnitTestProject1
{
    class BuyEnergy
    {
		IWebDriver driver;

		[SetUp]
		public void Setup()
		{
			driver = new ChromeDriver();
			driver.Manage().Window.Maximize();
			driver.Navigate().GoToUrl("https://ensekautomationcandidatetest.azurewebsites.net/Energy/Buy");

		}

		[TearDown]
		public void CloseDriver()
		{
			driver.Close();
			driver.Quit();
		}

		[Test]
		public void TestBuyProcess()
        {
			IWebElement buyTable = driver.FindElement(By.XPath("//table[@class='table']"));

			IList<IWebElement> rows = buyTable.FindElements(By.XPath("//table[@class='table']/tbody[1]/tr"));

			int inputQty = 10;

			for (int i = 1; i <= rows.Count; i++)
			{
				string energyType = driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[1]")).Text;
				int originalQty = Convert.ToInt32(driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[3]")).Text);

				if (originalQty != 0)
				{
					driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[4]/input[1]")).Clear();
					driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[4]/input[1]")).SendKeys(inputQty.ToString());
					driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[5]")).Click();


					WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains("https://ensekautomationcandidatetest.azurewebsites.net/Energy/SaleConfirmed?"));

					string salesText = driver.FindElement(By.TagName("h2")).Text;

					Assert.AreEqual(salesText, "Sale Confirmed!");

					driver.FindElement(By.LinkText("Buy more »")).Click();

					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains("https://ensekautomationcandidatetest.azurewebsites.net/Energy/Buy"));
				
				}

			}
      
        }

		[Test]
		public void TestQuantityIsReducedAfterSuccessfulBuy()
        {
			IWebElement buyTable = driver.FindElement(By.XPath("//table[@class='table']"));

			IList<IWebElement> rows = buyTable.FindElements(By.XPath("//table[@class='table']/tbody[1]/tr"));

			int inputQty = 10;

			for (int i = 1; i <= rows.Count; i++)
			{
				string energyType = driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[1]")).Text;
				int originalQty = Convert.ToInt32(driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[3]")).Text);
				
				if (originalQty != 0)
				{
					driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[4]/input[1]")).Clear();
					driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[4]/input[1]")).SendKeys(inputQty.ToString());
					driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[5]")).Click();


					WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains("https://ensekautomationcandidatetest.azurewebsites.net/Energy/SaleConfirmed?"));

					string salesText = driver.FindElement(By.TagName("h2")).Text;

					Assert.AreEqual(salesText, "Sale Confirmed!");

					driver.FindElement(By.LinkText("Buy more »")).Click();

					wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.UrlContains("https://ensekautomationcandidatetest.azurewebsites.net/Energy/Buy"));

					int updatedQty = Convert.ToInt32(driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[3]")).Text);
					
					int diffInQty = originalQty - updatedQty;
					
					Assert.AreEqual(diffInQty, inputQty);
				}

			}

		}

		[Test]
		public void TestEnergyTypeValues()
		{
			IWebElement buyTable = driver.FindElement(By.XPath("//table[@class='table']"));

			IList<IWebElement> rows = buyTable.FindElements(By.XPath("//table[@class='table']/tbody[1]/tr"));

			List<string> energyTypeList = new List<string>{"Gas", "Nuclear", "Electricity", "Oil"};

			for (int i = 0; i < rows.Count; i++)
            {
				string energyType = driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + (i+1) + "]/td[1]")).Text;

				Assert.AreEqual(energyType, energyTypeList[i]);

			}
		}

		[Test]
		public void TestNoBuyButtonDisplayedForZeroQuantityItems()
		{
			IWebElement buyTable = driver.FindElement(By.XPath("//table[@class='table']"));

			IList<IWebElement> rows = buyTable.FindElements(By.XPath("//table[@class='table']/tbody[1]/tr"));

			for (int i = 1; i <= rows.Count; i++)
			{
				string energyType = driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[1]")).Text;
				int originalQty = Convert.ToInt32(driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[3]")).Text);

				if (originalQty == 0)
				{
					IWebElement btnElement = driver.FindElement(By.XPath("//table[@class='table']/tbody[1]/tr[" + i + "]/td[5]"));

					Assert.IsTrue(btnElement.Displayed);
				}

			}

		}

		[Test]
		public void TestAllLinksOnBuyPage()
		{
			IList<IWebElement> links = driver.FindElements(By.TagName("a"));

			int count = links.Count;

			for (int i = 0; i < count; i++)
			{
				var link = links[i];
				var href = link.GetAttribute("href");

				//ignore the anchor links without href attribute
				if (string.IsNullOrEmpty(href))
					continue;

				using (var webclient = new HttpClient())
				{
					var response = webclient.GetAsync(href).Result;
					Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
				}

			}
		}


	}
}
