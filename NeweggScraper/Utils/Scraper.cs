using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using HtmlAgilityPack;

namespace NeweggScraper.Utils
{
    public class Scraper
    {
        #region Props
        /// <summary>
        /// Data as HTML
        /// </summary>
        public static string SearchResults { get; set; }

        /// <summary>
        /// InStock Products Collection
        /// </summary>
        public ObservableCollection<EntryModel> InStockEntries
        {
            get { return _inStockEntries; }
            set { _inStockEntries = value; }
        }

        /// <summary>
        /// OutOfStock Products Collection
        /// </summary>
        public ObservableCollection<EntryModel> OutOfStockEntries
        {
            get { return _outOfStock; }
            set { _outOfStock = value; }
        }
        #endregion

        #region Members
        private bool classExist;
        private ObservableCollection<EntryModel> _outOfStock = new ObservableCollection<EntryModel>();
        private ObservableCollection<EntryModel> _inStockEntries = new ObservableCollection<EntryModel>();
        private static string _inStockCountForHtml = "";
        private bool isInStock = false;
        #endregion

        #region Consts
        public const string SOLD_OUT = "sold out";
        public const string OUT_OF_STOCK = "out of stock";
        public const string NOTIFY = "auto Notify ";
        public const string SEARCHING_IN = "Searching in:";

        // Building HTML structure for Email body

        private string HTML_OPEN1 = "<!DOCTYPE html><html><head><style>body{font-family: arial, sans-serif;}" +
                                   "table {font-family: arial, sans-serif;border-collapse: collapse; width: 100%;}th{background-color:#f5c449;} td, th {border: 1px solid #dddddd; text-align: left;padding: 8px;}tr:nth-child(even){background-color: #dddddd;}</style></head><body><h2> ";
        private const string HTML_OPEN2 = " RESULTS</h2>" +
                                         "<table><tr><th> BRAND </th><th> DESCRIPTION </th><th> PRICE </th><th> LINK </th><th> Add To Cart </th></tr> ";
        private string HTML_BODY = "";
        private const string HTML_CLOSE = "</table></body></html>";
        #endregion

        #region Public Funcs
        public string ScrapeData(string page)
        {
            SearchResults = "";
            HTML_BODY = "";
            var web = HtmlWebLoad(page, out var resultsHtml, out var doc);

            // number of pages in search result
            CheckHowManyPages(doc, out var pages);
            resultsHtml = GetItemResults(page, pages, web, resultsHtml);

            SearchResults += HTML_OPEN1 + _inStockCountForHtml + HTML_OPEN2;
            SearchResults += HTML_BODY;
            SearchResults += HTML_CLOSE;
            return resultsHtml;
        }
        #endregion

        #region Private Funcs
        private static HtmlWeb HtmlWebLoad(string page, out string resultsHtml, out HtmlDocument doc)
        {
            var web = new HtmlWeb();
            Uri uri = new Uri(page);
            resultsHtml = "";

            web.PreRequest = request =>
            {
                try
                {
                    CookieContainer cookieContainer = new CookieContainer();
                    // set cookies according location
                    SetCookieByLocation(page, cookieContainer, uri);
                    request.CookieContainer = cookieContainer;
                    return true;
                }
                catch (Win32Exception e)
                {
                    Debug.Print(e.Message);
                    Debug.Print(e.Source);
                    Debug.Print(e.StackTrace);
                    Debug.Print(e.NativeErrorCode.ToString());
                    Debug.Print(e.ErrorCode.ToString());
                }
                return true;
            };

            doc = web.Load(page);
            return web;
        }
        private static void SetCookieByLocation(string page, CookieContainer cookieContainer, Uri uri)
        {
            switch (page)
            {
                case var x when x.Contains("global/il-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "ISR"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Israeli Store";
                    break;
                case var x when x.Contains("global/uk-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "GBR"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} UK Store";
                    break;
                case var x when x.Contains("global/tr-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "TUR"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Turkish Store";
                    break;
                case var x when x.Contains("www.newegg.ca"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "CAN"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Canada Store";
                    break;
                case var x when x.Contains("global/mx-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "MEX"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Mexico Store";
                    break;
                case var x when x.Contains("global/au-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "AUS"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} AUS Store";
                    break;
                case var x when x.Contains("global/nz-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "NZL"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} New-Z Store";
                    break;
                case var x when x.Contains("global/ar-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "ARG"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Argentina Store";
                    break;
                case var x when x.Contains("global/bh-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "BHR"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Bahrain Store";
                    break;
                case var x when x.Contains("global/kw-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "KWT"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Kuwait Store";
                    break;
                case var x when x.Contains("global/om-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "OMN"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Oman Store";
                    break;
                case var x when x.Contains("global/qa-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "QAT"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Qatar Store";
                    break;
                case var x when x.Contains("global/sa-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "SAU"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Saudi-Arab Store";
                    break;
                case var x when x.Contains("global/ae-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "ARE"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} UAE Store";
                    break;
                case var x when x.Contains("global/hk-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "HKG"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Hong-Kong Store";
                    break;
                case var x when x.Contains("global/jp-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "JPN"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Japan Store";
                    break;
                case var x when x.Contains("global/ph-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "PHL"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Pillipins Store";
                    break;
                case var x when x.Contains("global/sg-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "SGP"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Singapore Store";
                    break;
                case var x when x.Contains("global/kr-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "KOR"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Korea Store";
                    break;
                case var x when x.Contains("global/th-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "THA"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Thai Store";
                    break;
                case var x when x.Contains("global/in-en"):
                    cookieContainer.Add(uri, new Cookie("NV%5FW57", "IND"));
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} Indian Store";
                    break;
                default:
                    MainWindow.SearchingIn1 = $"{SEARCHING_IN} USA Store";
                    break;
            }
        }
        private static bool IsClassExist(HtmlNode item, string htmlTag, string attribute, string attributeValue)
        {
            var isclassExist = item.ChildNodes
                .Descendants(htmlTag)
                .Any(d => d.GetAttributeValue(attribute, "") == attributeValue);
            return isclassExist;
        }
        private static bool IsClassExist(HtmlDocument doc, string htmlTag, string attribute, string attributeValue)
        {
            var isclassExist = doc.DocumentNode
                .Descendants(htmlTag)
                .Any(d => d.GetAttributeValue(attribute, "") == attributeValue);
            return isclassExist;
        }

        private void CheckHowManyPages(HtmlDocument doc, out string pages)
        {
            pages = "";
            classExist = IsClassExist(doc, "span", "class", "list-tool-pagination-text");
            if (classExist)
            {
                pages = doc.DocumentNode.SelectSingleNode("//*[@class = 'list-tool-pagination-text']").InnerText;
            }
            //else
            //{
            //    MessageBox.Show("Check your search link and try again");
            //}

        }
        private string GetItemResults(string page, string pages, HtmlWeb web, string resultsHtml)
        {
            HtmlDocument doc = null;
            var numOfPages = (pages.Contains("1/1")) ? new string[1] : pages.Split('/');
            // iterates number of pages returned in search
            for (int i = 1; i <= numOfPages.Length; i++)
            {
                if (!page.Contains("&page="))
                    page += $"&page={i}";
                else
                {
                    page = page.Remove(page.Length - 1);
                    page += i;
                }
                doc = web.Load(page);
                web.UseCookies = true;

                var Items = doc.DocumentNode.SelectNodes("//*[@class = 'item-cell']");
                // iterates all items in the current page
                resultsHtml = GetResults(Items, resultsHtml, doc);
            }
            AddSearchFiltersDynamically(doc);
            return resultsHtml;
        }
        private string GetResults(HtmlNodeCollection Items, string resultsHtml, HtmlDocument doc)
        {
            try
            {
                foreach (var item in Items)
                {
                    var itemTitle = "";
                    var itemLink = "";
                    try
                    {
                        classExist = IsClassExist(item, "a", "class", "item-brand");
                        if (classExist)
                        {
                            itemTitle = HttpUtility.HtmlDecode(item.SelectSingleNode(".//a[@class = 'item-brand']").FirstChild
                                .Attributes["title"].Value);
                        }

                        classExist = IsClassExist(item, "a", "class", "item-img combo-img-0");
                        if (classExist)
                        {
                            itemTitle = "COMBO DEAL";
                        }

                        classExist = IsClassExist(item, "div", "class", "txt-ads-title");
                        if (classExist)
                            continue;

                    }
                    catch (Exception e)
                    {

                        itemTitle = "NO BRAND ASSIGNED";
                    }

                    var itemDesc = HttpUtility.HtmlDecode(item.SelectSingleNode(".//a[@class = 'item-title']").InnerText);
                    if (itemTitle.Contains("COMBO DEAL"))
                    {
                        itemLink =
                           HttpUtility.HtmlDecode(item.SelectSingleNode(".//a[@class = 'item-img combo-img-0']").Attributes["href"].Value);
                    }
                    else
                    {
                        itemLink =
                           HttpUtility.HtmlDecode(item.SelectSingleNode(".//a[@class = 'item-img']").Attributes["href"].Value);
                    }

                    var price = item.SelectSingleNode(".//div[@class = 'item-action']").FirstChild.InnerText;

                    price = FixPriceString(ref resultsHtml, price, item);
                    price = CheckIfItemIsInStock(item, price);
                    AddItemToRelevantCollection(itemTitle, itemDesc, itemLink, price, item);

                }
                return resultsHtml;
            }
            catch (Exception e)
            {
                return "Couldn't get any items";
            }
        }
        private string CheckIfItemIsInStock(HtmlNode item, string price)
        {
            var itemPromo = "";
            var node = item.ChildNodes.Descendants("p");
            classExist = IsClassExist(item, "p", "class", "item-promo");
            if (classExist)
            {
                try
                {
                    itemPromo = HttpUtility.HtmlDecode(item.SelectSingleNode(".//p[@class = 'item-promo']").InnerText);
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                    Debug.Print(e.StackTrace);
                }
            }

            var itemButton =
                HttpUtility.HtmlDecode(item.SelectSingleNode(".//div[@class = 'item-button-area']").FirstChild.InnerText);
            if (itemButton.ToLower() == SOLD_OUT || itemPromo.ToLower() == OUT_OF_STOCK)
            {
                price = (itemButton.ToLower() == NOTIFY) ? itemPromo : $"{itemPromo} \n {itemButton}";
                isInStock = false;
            }
            else
            {
                isInStock = true;
            }
            return price;
        }
        private static string FixPriceString(ref string resultsHtml, string price, HtmlNode item)
        {
            //remove unwanted chars from price string
            if (!String.IsNullOrEmpty(price))
            {
                price = price.Remove(price.Length - 2, 2);
                price = price.Remove(0, 1);
                resultsHtml += item.OuterHtml;
            }
            return price;
        }
        private void AddItemToRelevantCollection(string itemTitle, string itemDesc, string itemLink, string price, HtmlNode item)
        {
            //Debug.Print($"Brand: {itemTitle} \nDescription {itemDesc} \nPrice: {price} \nProduct Link: {itemLink}");
            var addToCartLink = "";
            if (isInStock)
            {
                var link = new string[1];

                if (itemTitle != "COMBO DEAL")
                {
                    link = itemLink.Split('?');
                    itemLink = link[0];
                }

                addToCartLink = AddToCartLink(itemLink, item);
                HTML_BODY +=
                   $"<tr><td>{itemTitle}</td><td>{itemDesc}</td><td>{price}</td><td><a href=\"{itemLink}\">Go To Product</a></td><td><a href=\"{addToCartLink}\">Add To Cart</a></td></tr>";
                _inStockCountForHtml = InStockEntries.Count.ToString();
                InStockEntries.Add(new EntryModel
                {
                    Brand = itemTitle,
                    Description = itemDesc,
                    Link = itemLink,
                    Price = price,
                    AddToCart = addToCartLink
                });
            }
            else
            {
                OutOfStockEntries.Add(new EntryModel
                {
                    Brand = itemTitle,
                    Description = itemDesc,
                    Link = itemLink,
                    Price = price,
                });
            }
        }

        private string AddToCartLink(string itemLink, HtmlNode item)
        {
            string[] link = null;
            var cartLink = "";
            var itemNumber = "";
            var setLocation = "";
            try
            {
                classExist = IsClassExist(item, "div", "class", "item-stock");
                if (classExist)
                {
                    itemNumber = HttpUtility.HtmlDecode(item.SelectSingleNode(".//div[@class = 'item-stock']").Attributes["id"].Value);
                    var fixNumber = itemNumber.Split('_');
                    if (fixNumber[1].Contains("-"))
                    {
                        string result = string.Concat(fixNumber[1].Where(c => !char.IsPunctuation(c)));
                        itemNumber = result;
                    }
                    else
                    {
                        itemNumber = fixNumber[1];
                    }

                }

                if (!itemLink.ToLower().Contains("combodeal"))
                {
                    link = itemLink.Split('?');
                    var cart = link[0].Split('/');
                    for (int i = 0; i < cart.Length; i++)
                    {
                        if (cart[i] == "p")
                        {
                            if (!itemNumber.Contains("9SIA"))
                                cartLink = $"N82E168{itemNumber}";
                            else
                            {
                                cartLink = itemNumber;
                            }
                        }
                    }
                }
                else
                {
                    link = itemLink.Split('?');
                    var cart = itemLink.Split('=');
                    cartLink = cart[1];
                }


            }
            catch (Exception exception)
            {

            }
            var location = itemLink.Split('/');

            for (int i = 0; i < location.Length; i++)
            {
                if (location[i].Contains("global"))
                {
                    setLocation = $"global/{location[i + 1]}/";
                }
            }
            var addToCart =
                $"https://secure.newegg.com/{setLocation}Shopping/AddtoCart.aspx?Submit=ADD&ItemList={cartLink}";

            return addToCart;
        }
        private void AddSearchFiltersDynamically(HtmlDocument doc)
        {
            HtmlNodeCollection filters = null;
            classExist = IsClassExist(doc, "ul", "class", "filter-choices");
            if (classExist)
            {
                filters = doc.DocumentNode.SelectNodes("//*[@class = 'filter-choices']");
            }

            classExist = IsClassExist(doc, "li", "class", "is-current");
            if (classExist)
            {

                /*MainWindow.Instance.CategoryValue.Text*/

                MainWindow.Instance.CategoryValue.Text = doc.DocumentNode.SelectSingleNode("//*[@class = 'is-current']").InnerText;

            }

            try
            {
                int firstBlock = 0;
                int secondBlock = 0;
                string sp = "sp";

                if (filters != null)
                {
                    foreach (var filter in filters)
                    {
                        var list = filter.SelectNodes(".//li");
                        foreach (var li in list)
                        {

                            var header = HttpUtility.HtmlDecode(li.SelectSingleNode(".//label[@class = 'filter-choice-title']").InnerText);
                            TextBlock block = new TextBlock
                            {
                                Text = header,
                                Margin = new Thickness(5, 5, 0, 5)
                            };

                            StackPanel panel = new StackPanel
                            {
                                Name = sp + firstBlock,
                                Orientation = Orientation.Horizontal
                            };

                            if (firstBlock <= 0)
                            {
                                MainWindow.Instance.SearchFilters.Children.Insert(firstBlock, block);
                                MainWindow.Instance.SearchFilters.Children.Insert(firstBlock + 1, panel);
                            }
                            else
                            {
                                MainWindow.Instance.SearchFilters.Children.Insert(MainWindow.Instance.SearchFilters.Children.Count, block);
                                MainWindow.Instance.SearchFilters.Children.Insert(MainWindow.Instance.SearchFilters.Children.Count, panel);
                            }
                            firstBlock++;

                            var content = (li.SelectNodes(".//span[@class = 'filter-choice']"));
                            foreach (var cont in content)
                            {
                                var text = HttpUtility.HtmlDecode(cont.InnerText);
                                TextBlock innerBlock = new TextBlock
                                {
                                    Text = text,
                                    Margin = new Thickness(5, 0, 0, 0),
                                    Background = Application.Current.Resources["darkGreyBrush"] as SolidColorBrush,
                                    Foreground = Brushes.White,
                                    Padding = new Thickness(5)


                                };

                                //MainWindow.Instance.SearchFiltersChild.Children.Add(innerBlock);
                                panel.Children.Insert(secondBlock, innerBlock);
                                secondBlock++;
                            }
                            secondBlock = 0;
                        }
                    }
                }

            }
            catch (Exception e)
            {

            }


        }
        #endregion

    }
}
