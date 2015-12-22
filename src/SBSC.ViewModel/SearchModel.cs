using System;
using System.ComponentModel.DataAnnotations;

namespace SBSC.ViewModel
{
    public class SearchModel
    {
        [Display(Name = "Texto")]
        public String Search { get; set; }

        public int? Page { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public double TotalPages { get; set; }
    }
}
