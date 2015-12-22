using System.ComponentModel.DataAnnotations;

namespace SBSC.ViewModel.Emprestimos
{
    public class ItemEmprestimoViewModel
    {
        public int? Id { get; set; }

        public int? Position { get; set; }

        [Required]
        [Display(Name = "Livro")]
        public int LivroId { get; set; }

        [Display(Name = "Livro")]
        public string LivroText { get; set; }
    }
}
