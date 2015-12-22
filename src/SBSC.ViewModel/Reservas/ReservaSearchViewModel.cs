using System.Collections.Generic;

namespace SBSC.ViewModel.Reservas
{
    public class ReservaSearchViewModel : BaseSearchViewModel
    {
        public ReservaSearchModel SearchModel { get; set; }
        public List<ReservaViewModel> Objetos { get; set; }
    }
}
