using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTestMusicLibrary
{
    public class UnitTest1
    {
        private static readonly string DriverDirectory = "C:\\webDrivers";
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public UnitTest1()
        {
            _driver = new ChromeDriver(DriverDirectory);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        } 
        [Fact]
        public void Getall_Music_Records_ShouldDisplayRecords()
        {
            // 1. Arrange & Naviger til siden
            string url = "http://127.0.0.1:5500/index.html";
            _driver.Navigate().GoToUrl(url);

            // 2. Verificer side-titel og overskrift
            Assert.True(string.Equals("Music Library", _driver.Title, StringComparison.OrdinalIgnoreCase));
            var titleHeader = _driver.FindElement(By.Id("title-header"));
            Assert.True(string.Equals("title-header", titleHeader.GetAttribute("id"), StringComparison.OrdinalIgnoreCase));
            Assert.True(string.Equals("Title", titleHeader.Text, StringComparison.OrdinalIgnoreCase));

            // 3. Act: Vent på at "Loading..." forsvinder og at rækker med data indlæses
            // Vi venter på, at der er mindst én datarække (eller at "No music records found" / tabellen er klar)
            _wait.Until(d => d.FindElements(By.CssSelector("#music-records-table tbody tr")).Count > 0);

            // 4. Assert: Hent alle rækker i tabellen (undgå loading/error rækker hvis de er der)
            var recordRows = _driver.FindElements(By.CssSelector("#music-records-table tbody tr:not(:has(.table-message))"));

            // Verificer at der rent faktisk er hentet plader frem (tilpas alt efter dit test-setup)
            Assert.True(recordRows.Count > 0, "Der blev ikke fundet nogen musikplader i tabellen.");

            // Eksempel på at tjekke indholdet af den første række
            var firstRowTitle = recordRows[0].FindElement(By.ClassName("record-title")).Text;
            Assert.True(!string.IsNullOrEmpty(firstRowTitle), "Den første musikplade mangler en titel.");

        }
    }
}

