using System;
using System.Collections.Generic;
using System.Text;

namespace Kolibri.net.SilverScreen.Entities
{
    public class BrowseMoviesSearchOptions
    {

        public string SearchText { get; set; }

        public string? Genre { get; set; } = string.Empty;
        public bool Actor { get; set; } = false;
        public bool MovieTitle { get; set; } = false;
        public string? Year { get; set; }

        // Helper method to check if any search criteria has been entered
        public bool HasAnyCriteria()
        {
            return !string.IsNullOrWhiteSpace(SearchText) ||
                !string.IsNullOrWhiteSpace(Genre) ||
                   !string.IsNullOrWhiteSpace(Year);
        }
    }
}