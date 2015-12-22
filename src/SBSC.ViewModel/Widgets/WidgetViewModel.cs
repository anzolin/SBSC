using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSC.ViewModel.Widgets
{
    public class WidgetViewModel
    {
        public string PanelStyle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ColumnTitle { get; set; }
        public string MessageNoItens { get; set; }
        public List<WidgetListaViewModel> Itens { get; set; }
    }
}
