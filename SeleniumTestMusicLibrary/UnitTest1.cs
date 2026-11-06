using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTestMusicLibrary
{
    public class UnitTest1 : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public UnitTest1()
        {
            _driver = new ChromeDriver();
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
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
        [Fact]
        public void Add_Music_Record_ShouldDisplayNewRecord()
        {
            string url = "http://127.0.0.1:5500/index.html";
            _driver.Navigate().GoToUrl(url);

            IWebElement addButton = _driver.FindElement(By.Id("add-button"));
            addButton.Click();

            IWebElement titleElement = _driver.FindElement(By.Id("add-record-title"));
            titleElement.SendKeys("Test Title");

            IWebElement artistElement = _driver.FindElement(By.Id("add-record-artist"));
            artistElement.SendKeys("Test Artist");

            IWebElement durationElement = _driver.FindElement(By.Id("add-record-duration"));
            durationElement.SendKeys("130");

            IWebElement yearElement = _driver.FindElement(By.Id("add-record-year"));
            yearElement.SendKeys("2023");

            IWebElement submitButton = _driver.FindElement(By.Id("add-record-submit"));
            submitButton.Click();



        }


        [Fact]
        public void SearchByTitle_ShouldDisplayOneRecord()
        {
            // Arrange - Åbn hjemmesiden
            _driver.Navigate().GoToUrl(
                "http://127.0.0.1:5500/index.html");

            // forvent at der er musikplader i tabellen
            _wait.Until(d =>
                  d.FindElements(By.CssSelector("tbody .record-title")).Count > 0);

            // Find søgefeltet
            var searchInput = _driver.FindElement(By.Id("search-title"));

            // Skriv Song 1
            searchInput.Clear();
            searchInput.SendKeys("Song 1");

            // Act - Klik på Search
            _driver.FindElement(
                By.CssSelector(".search-container button")).Click();

            // Vent på søgeresultatet
            _wait.Until(d =>
            {
                var records = d.FindElements(By.CssSelector("tbody .record-title"));

                return records.Count == 1 && records[0].Text == "Song 1";
            });

            // Assert
            var result = _driver.FindElements(
                By.CssSelector("tbody .record-title"));

            Assert.Single(result);
            Assert.Equal("Song 1", result[0].Text);
        }

        [Fact]
        public void DeleteMusicRecord_ShouldRemoveRecord()
        {
            // Arrange - Åbn hjemmesiden
            _driver.Navigate().GoToUrl(
                "http://127.0.0.1:5500/index.html");

            // Vent på musikpladerne
            _wait.Until(d =>
                d.FindElements(By.CssSelector("tbody .record-title")).Count > 0);

            // Find alle musikplader
            var records = _driver.FindElements(
                By.CssSelector("tbody tr:has(.record-title)"));

            int countBefore = records.Count;


            // Slet den sidste tilgængelige plade. Testen må ikke afhænge af,
            // at testdata altid indeholder en bestemt titel.
            var recordToDelete = records[^1];
            var deletedTitle = recordToDelete
                .FindElement(By.ClassName("record-title")).Text;

            recordToDelete.FindElement(By.ClassName("delete-btn")).Click();

            // Bekræft sletning
            _driver.SwitchTo().Alert().Accept();

            // Vent på at listen bliver opdateret
            _wait.Until(d =>
                d.FindElements(
                    By.CssSelector("tbody tr:has(.record-title)")
                ).Count == countBefore - 1);

            // Assert - Kontrollér at én musikplade er slettet
            var remainingRecords = _driver.FindElements(
                By.CssSelector("tbody tr:has(.record-title)"));

            Assert.Equal(countBefore - 1, remainingRecords.Count);
            Assert.DoesNotContain(
                remainingRecords,
                record => record.FindElement(By.ClassName("record-title")).Text == deletedTitle);
        }

        [Fact]

        public void UpdateMusicRecord_ShouldChangeRecordDetails()
        {
            string url = "http://127.0.0.1:5500/index.html";
            _driver.Navigate().GoToUrl(url);

            IWebElement updateButton = _driver.FindElement(By.Id("update-record-button"));
            updateButton.Click();

            IWebElement titleElement = _driver.FindElement(By.Id("edit-record-title"));
            titleElement.SendKeys("Test Title");

            IWebElement artistElement = _driver.FindElement(By.Id("edit-record-artist"));
            artistElement.SendKeys("Test Artist");

            IWebElement durationElement = _driver.FindElement(By.Id("edit-record-duration"));
            durationElement.SendKeys("130");

            IWebElement yearElement = _driver.FindElement(By.Id("edit-record-year"));
            yearElement.SendKeys("2023");

            IWebElement submitButton = _driver.FindElement(By.Id("edit-record-submit"));
            submitButton.Click();

        }
    }
}

