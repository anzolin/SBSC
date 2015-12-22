using SBSC.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace SBSC.ViewModel.Reservas
{
    public class ReservaViewModel
    {
        [Display(Name = "Código")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Pessoa é um campo obrigatório.")]
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }

        [Display(Name = "Pessoa")]
        public string PessoaText { get; set; }

        [Required(ErrorMessage = "Livro é um campo obrigatório.")]
        [Display(Name = "Livro")]
        public int LivroId { get; set; }

        [Display(Name = "Livro")]
        public string LivroText { get; set; }

        [Required(ErrorMessage = "Previsão do empréstimo é um campo obrigatório.")]
        [Display(Name = "Previsão do empréstimo")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DtPrevisaoEmprestimo { get; set; }

        [Display(Name = "Previsão do empréstimo")]
        public string DtPrevisaoEmprestimoText { get; set; }

        [Required]
        [Display(Name = "Status")]
        [EnumDataType(typeof(Enumerations.Reserva.StatusReserva))]
        public int StatusId { get; set; }

        [Display(Name = "Status")]
        public string StatusText { get; set; }

        [Display(Name = "Cadastrado em")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Cadastrado em")]
        public string DtCadastroText { get; set; }
    }
}
