using System.Collections.Generic;

namespace SBSC.ViewModel.Devolucoes
{
    public class DevolucaoSearchViewModel : BaseSearchViewModel
    {
        public DevolucaoSearchModel SearchModel { get; set; }
        public List<DevolucaoViewModel> Objetos { get; set; }
    }
}
