namespace Sahibinden
{
    class ScrapeExecutor
    {
        public static async Task Main(string[] args)
        {
            // Sum of price of every advert
            int priceSum = 0;

            // Get adverts
            List<Advert> adverts = await MainPageShelf.GetAdverts();

            // Line parses of adverts
            List<string> lines = new List<string>();

            foreach (Advert advert in adverts)
            {
                string line = advert.Title + new string(' ', 100 - advert.Title.Length) + advert.Price;
                Console.WriteLine(line);
                priceSum += advert.Price;
                lines.Add(line);
            }
            
            // Average price and number of adverts that were read
            string averagePriceInfo = "\n" +
                "There are " + adverts.Count + " adverts in total.\n" +
                "And their average price is: " + (float)priceSum / adverts.Count;
            Console.WriteLine(averagePriceInfo);
            lines.Add(averagePriceInfo);

            // They are not needed anymore
            adverts.Clear();

            // Save to txt file
            lines.Insert(0, "\n");
            lines.Insert(0, "ADVERT NAME" + new string(' ', 89) + "PRICE (TRY)\n");
            await File.WriteAllLinesAsync("adverts.txt", lines);

            Console.WriteLine("Saved adverts to 'adverts.txt'...");

            // Not needed anymore
            lines.Clear();

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}