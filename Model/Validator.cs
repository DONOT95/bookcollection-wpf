using System;
using System.Collections.Generic;
using System.Linq;

namespace BookCollection.Model
{
    public class Validator
    {
        // Validate -> Integer, min: positive, max: current year
        public static bool ValidateYear(string input) 
        {
            input = input.Trim();

            // Check if input integer
            bool isInt = int.TryParse(input, out var year);

            // Guard for not int input
            if (!isInt)
            {
                return false;
            }

            // Check if publication year valid (No negative or null)
            if(year <= 0)
            {
                return false;
            }

            // Check if year in the future
            DateTime currentYear = DateTime.Today;
            if (currentYear.Year < year) 
            {
                return false;
            }
            // Passed all validation
            return true;
        }

        // Check for empty or space
        public static bool ValidateEmpty(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        // Check unique Autor+Titel with the same publication year in collection
        public static bool IsBookUnique(Book book, IEnumerable<Book> books)
        {
            return !books.Any(b =>
                string.Equals(b.Title, book.Title, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(b.Author, book.Author, StringComparison.OrdinalIgnoreCase) &&
                b.PublicationYear == book.PublicationYear
            );
        }
    }
}
