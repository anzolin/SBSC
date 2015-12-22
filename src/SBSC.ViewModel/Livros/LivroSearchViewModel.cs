using System.Collections.Generic;

namespace SBSC.ViewModel.Livros
{
    public class LivroSearchViewModel : BaseSearchViewModel
    {
        public LivroSearchModel SearchModel { get; set; }
        public List<LivroViewModel> Objetos { get; set; }
    }
}
