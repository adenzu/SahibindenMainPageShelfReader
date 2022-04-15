using HtmlAgilityPack;


namespace Sahibinden
{
    /**
     * Summary:
     *  Advert data collection class for main page shelf section
     */
    static internal class MainPageShelf
    {
        // Link of the main page shelf of Sahibinden
        public static string Link => @"https://www.sahibinden.com/anasayfa-vitrin";

        /**
         * Summary:
         *  Returns every advert in the first 50 page.
         * @return  Task<List<Advert>>  Async list of adverts.
         */
        public static Task<List<Advert>> GetAdverts()
        {
            return GetAdverts(0, 50);
        }

        /**
         * Summary:
         *  Returns the adverts at given page.
         * @param   pageIndex           Index of the page that whose adverts will be read.
         * @return  Task<List<Advert>>  Async list of adverts.
         */
        public static Task<List<Advert>> GetAdverts(int pageIndex)
        {
            return GetAdverts(pageIndex, 1);
        }

        /**
         * Summary:
         *  Returns the adverts between given pages.
         * @param   startingPage        At which page adverts will be read starting from.
         * @param   pageCount           Including startingPage how many pages will be read.
         * @return  Task<List<Advert>>  Async list of adverts.
         */
        public static async Task<List<Advert>> GetAdverts(int startingPage, int pageCount = 50)
        {
            // List of read adverts
            List<Advert> advertList = new List<Advert>();

            // Http client to fetch html contents of pages
            HttpClient client = new HttpClient();

            // List of page content read tasks that return html documents created by them
            List<Task<HtmlDocument>> htmlDocumentReadTasks = new List<Task<HtmlDocument>>();

            // Read given number of pages starting from given index
            for (int i = startingPage; i < startingPage + pageCount; ++i)
            {
                // Add the asynch task to the list
                htmlDocumentReadTasks.Add(Task.Run(async () =>
                {
                    var response = await client.GetAsync(Link + "?pagingOffset=" + i + "&pagingSize=50");   // Get response
                    var content  = await response.Content.ReadAsStringAsync();                              // Get content
                    HtmlDocument htmlDocument = new HtmlDocument();                                         // Create doc
                    htmlDocument.LoadHtml(content);                                                         // Load content to doc
                    return htmlDocument;                                                                    // Return new doc
                }));
            }

            // List of html documents that resulted from executing reading task of every page
            // parallel. This is done this way to prevent Sahibinden interrupting the process
            HtmlDocument[] htmlDocuments  = await Task.WhenAll(htmlDocumentReadTasks);

            // Sahibinden doesn't show a special page upon trying to reach to adverts with index
            // that is outside of current number of adverts, instead it shows last valid page.
            // So this is used for storing last page and checking if every advert is read.
            HtmlDocument prevHtmlDocument = new HtmlDocument();

            foreach (HtmlDocument htmlDocument in htmlDocuments)
            {
                // If previous html document is the same as current one
                // (last page is reached)
                if (prevHtmlDocument == htmlDocument) break;

                // If not update the variable
                else prevHtmlDocument = htmlDocument;

                // Get advert elements
                HtmlNodeCollection advertNodes = htmlDocument.DocumentNode.SelectNodes("//td[contains(@class, 'searchResultsGalleryItem')]");

                // If there are any adverts
                if (advertNodes != null)
                {
                    foreach (var advertNode in advertNodes)
                    {
                        string title = advertNode.SelectSingleNode(".//a[contains(@class, 'classifiedTitle')]").GetAttributeValue("title", "None"); // Get title
                        string price = advertNode.SelectSingleNode(".//div[contains(@class, 'searchResultsPriceValue')]/div").InnerText;            // Get price
                        advertList.Add(new Advert(title, price));                                                                                   // Add to list
                    }
                }
            }

            // Return the resulting list
            return advertList;
        }
    }
}
