using SBSC.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace SBSC.ViewModel.Emprestimos
{
    public class EmprestimoViewModel
    {
        [Display(Name = "Código")]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Pessoa é um campo obrigatório.")]
        [Display(Name = "Pessoa")]
        public int PessoaId { get; set; }

        [Display(Name = "Pessoa")]
        public string PessoaText { get; set; }

        [Required(ErrorMessage = "Livro (1) é um campo obrigatório.")]
        [Display(Name = "Livro (1)")]
        public int Livro1Id { get; set; }

        [Display(Name = "Livro (1)")]
        public string Livro1Text { get; set; }

        [Display(Name = "Livro (2)")]
        public int? Livro2Id { get; set; }

        [Display(Name = "Livro (2)")]
        public string Livro2Text { get; set; }

        [Display(Name = "Livro (3)")]
        public int? Livro3Id { get; set; }

        [Display(Name = "Livro (3)")]
        public string Livro3Text { get; set; }

        [Display(Name = "Livros")]
        public string LivrosText { get; set; }

        [Required(ErrorMessage = "Empréstimo em é um campo obrigatório.")]
        [Display(Name = "Empréstimo em")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DtEmprestimo { get; set; }

        [Display(Name = "Empréstimo em")]
        public string DtEmprestimoText { get; set; }

        [Required(ErrorMessage = "Previsão devolução é um campo obrigatório.")]
        [Display(Name = "Previsão devolução")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DtPrevisaoDevolucao { get; set; }

        [Display(Name = "Previsão devolução")]
        public string DtPrevisaoDevolucaoText { get; set; }

        [Display(Name = "Devolução em")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DtDevolucao { get; set; }

        [Display(Name = "Devolução em")]
        public string DtDevolucaoText { get; set; }

        [Display(Name = "Status")]
        [EnumDataType(typeof(Enumerations.Emprestimo.StatusEmprestimo))]
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
