using System.Collections.Generic;

namespace SBSC.ViewModel.Pessoas
{
    public class PessoaSearchViewModel : BaseSearchViewModel
    {
        public PessoaSearchModel SearchModel { get; set; }
        public List<PessoaViewModel> Objetos { get; set; }
    }
}
