using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RunGroopWebApp.Data.Enum;
using RunGroopWebApp.Extensions;
using RunGroopWebApp.Models;
using RunGroopWebApp.Scraper.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGroopWebApp.Scraper.Services
{
    public class MeetupScraper
    {
        private IWebDriver _driver;
        public MeetupScraper(bool isHideChrome, bool isHideImage, bool isDisableSound, string UserAgent, string Proxy, int TimeWaitForSearchingElement = 3, int TimeWaitForLoadingPage = 5)
        {
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments(new string[]
            {
                "--disable-notifications",
                //"--window-size=" + Size.X.ToString() + "," + Size.Y.ToString(),
                "--no-sandbox",
                "--disable-gpu",
                "--app=https://bitly.com/",
                "--disable-dev-shm-usage",
                "--disable-web-security",
                "--disable-rtc-smoothness-algorithm",
                "--disable-webrtc-hw-decoding",
                "--disable-webrtc-hw-encoding",
                "--disable-webrtc-multiple-routes",
                "--disable-webrtc-hw-vp8-encoding",
                "--enforce-webrtc-ip-permission-check",
                "--force-webrtc-ip-handling-policy",
                "--ignore-certificate-errors",
                "--disable-infobars",
                "--disable-popup-blocking"
            });
            options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.plugins", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.popups", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.auto_select_certificate", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.mixed_script", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.media_stream", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.media_stream_mic", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.media_stream_camera", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.protocol_handlers", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.midi_sysex", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.push_messaging", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.ssl_cert_decisions", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.metro_switch_to_desktop", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.protected_media_identifier", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.site_engagement", 1);
            options.AddUserProfilePreference("profile.default_content_setting_values.durable_storage", 1);
            options.AddUserProfilePreference("useAutomationExtension", true);

            if (isDisableSound)
            {
                options.AddArgument("--mute-audio");
            }
            bool flag = !isHideChrome;
            if (flag)
            {
                if (isHideImage)
                {
                    options.AddArgument("--blink-settings=imagesEnabled=false");
                }
            }
            else
            {
                options.AddArgument("--blink-settings=imagesEnabled=false");
                options.AddArgument("--headless");
            }
            bool flag3 = !string.IsNullOrEmpty(Proxy);
            if (flag3)
            {
                bool flag4 = Proxy.Contains(":");
                if (flag4)
                {
                    options.AddArgument("--proxy-server= " + Proxy);
                }
                else
                {
                    options.AddArgument("--proxy-server= socks5://127.0.0.1:" + Proxy);
                }
            }
            bool flag5 = !string.IsNullOrEmpty(UserAgent);
            if (flag5)
            {
                options.AddArgument("--user-agent=" + UserAgent);
            }
            bool flag6 = File.Exists("chrome\\chrome.exe");
            if (flag6)
            {
                options.BinaryLocation = "chrome\\chrome.exe";
            }
            _driver = new ChromeDriver(service, options);

            _driver.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, TimeWaitForSearchingElement);
            _driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes((double)TimeWaitForLoadingPage);
        }
        public void Run()
        {
            GetListOfCityAndState();
        }

        public void GetListOfCityAndState()
        {
            int batchSize = 100;
            int currentBatch = 0;
            bool done = false;
            while(!done)
            using (var context = new ScraperDBContext())
            {
               var cities = context.Cities.OrderBy(x => x.Id).Skip(currentBatch++ * batchSize).Take(batchSize).ToList();
                foreach(var city in cities)
                {
                    IterateOverRunningClubs(city.StateCode.ToLower(), city.CityName.ToLower());
                    if(city.Id == 40000)
                    {
                       done = true;
                    }
                }
            }
        }


        public void IterateOverRunningClubs(string state, string city)
        {
            try
            {
                _driver.Navigate().GoToUrl($"https://www.meetup.com/find/?suggested=true&source=GROUPS&keywords=running%20club&location=us--{state}--{city}");
                //System.Threading.Thread.Sleep(1000);
                var pageElements = _driver.FindElements(By.CssSelector("h3[data-testid='group-card-title']"));
                for (int i = 0; i < pageElements.Count; i++)
                {
                    try
                    {
                        pageElements = _driver.FindElements(By.CssSelector("h3[data-testid='group-card-title']"));
                        var element = pageElements.ElementAt(i);
                        var placeholder = element.Text;
                        var placeholder2 = element.Text.Contains("run", System.StringComparison.CurrentCultureIgnoreCase);
                        if (element.Text.Contains("run", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            element.Click();
                            var club = new Club()
                            {
                                Title = _driver.FindElement(By.CssSelector("a[class='groupHomeHeader-groupNameLink']")).Text ?? "",
                                Description = _driver.FindElement(By.CssSelector("p[class='group-description _groupDescription-module_description__3qvYh margin--bottom']")).Text ?? "",
                                Address = new Address()
                                {
                                    State = state.ToUpper(),
                                    City = city.FirstCharToUpper()
                                },
                                ClubCategory = ClubCategory.City
                            };
                            using (var context = new ScraperDBContext())
                            {
                                if (!context.Clubs.Any(c => c.Title == club.Title))
                                {
                                    context.Clubs.Add(club);
                                    context.SaveChanges();
                                }
                            }
                            _driver.Navigate().Back();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                        _driver.Navigate().Back();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

    }
}
