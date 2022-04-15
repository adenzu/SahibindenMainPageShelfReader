using System.Text.RegularExpressions;

namespace Sahibinden
{
    /**
     * Summary:
     *  Advert data container class
     */
    internal class Advert
    {
        // Title of the advert
        public string Title { get; }

        // Price of the advert
        public int Price { get; }
        
        /**
         * Summary:
         *  Constructs an advert object with given title and price.
         * @param   title   Title of the advert.
         * @param   price   Price of the advert.
         */
        public Advert(string title, string price)
        {
            Title = title;
            Price = ConvertPrice(price);
        }

        /**
         * Summary:
         *  Converts read price string value to integer value.
         * @param   price   The price that is read.
         * @return  int     The integer equivalent of read price.
         */
        private static int ConvertPrice(string price)
        {
            price = Regex.Replace(price, "[^0-9]", "");
            return int.Parse(price);
        }
    }
}
