using System.Collections.Generic;

namespace SBSC.ViewModel.Emprestimos
{
    public class EmprestimoSearchViewModel : BaseSearchViewModel
    {
        public EmprestimoSearchModel SearchModel { get; set; }
        public List<EmprestimoViewModel> Objetos { get; set; }
    }
}
