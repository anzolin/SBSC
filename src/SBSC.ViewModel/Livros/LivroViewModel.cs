using SBSC.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace SBSC.ViewModel.Livros
{
    public class LivroViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Status")]
        [EnumDataType(typeof(Enumerations.Livro.StatusLivro))]
        public int? StatusId { get; set; }

        [Display(Name = "Status")]
        public string StatusText { get; set; }

        [Required(ErrorMessage = "Código é um campo obrigatório. Minímo 5 caracters.")]
        [MinLength(5)]
        [MaxLength(50)]
        [Display(Name = "Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Título é um campo obrigatório. Minímo 3 caracters.")]
        [MinLength(3)]
        [MaxLength(250)]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Gênero é um campo obrigatório. Minímo 3 caracters.")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Gênero")]
        public string Genero { get; set; }

        [Required(ErrorMessage = "Autores é um campo obrigatório. Minímo 3 caracters.")]
        [MinLength(3)]
        [MaxLength(250)]
        [Display(Name = "Autores")]
        public string Autor { get; set; }

        [Required(ErrorMessage = "Editora é um campo obrigatório. Minímo 3 caracters.")]
        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Editora")]
        public string Editora { get; set; }

        [Required(ErrorMessage = "Ano é um campo obrigatório.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "O valor informado no campo Ano é inválido")]
        //[MinLength(4)]
        //[MaxLength(4)]
        [Display(Name = "Ano")]
        [DataType(DataType.Text)]
        public int? Ano { get; set; }

        [Required(ErrorMessage = "Estado de conservação é um campo obrigatório.")]
        [Display(Name = "Estado de conservação")]
        [EnumDataType(typeof(Enumerations.Livro.EstadoConservacaoLivro))]
        public int? EstadoConservacaoId { get; set; }

        [Display(Name = "Estado de conservação")]
        public string EstadoConservacaoText { get; set; }

        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Local")]
        public string Local { get; set; }

        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Resumo")]
        public string Resumo { get; set; }

        [Required(ErrorMessage = "Baixado é um campo obrigatório.")]
        [Display(Name = "Baixado")]
        [EnumDataType(typeof(Enumerations.Generico.SimOuNao))]
        public int? BaixadoId { get; set; }

        [Display(Name = "Baixado")]
        public string BaixadoText { get; set; }

        [Display(Name = "Baixa em")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DtBaixa { get; set; }

        [Display(Name = "Baixa em")]
        public string DtBaixaText { get; set; }

        [Display(Name = "Cadastrado em")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DtCadastro { get; set; }

        [Display(Name = "Cadastrado em")]
        public string DtCadastroText { get; set; }

        [Display(Name = "Editado em")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DtEdicao { get; set; }

        [Display(Name = "Editado em")]
        public string DtEdicaoText { get; set; }

        [Display(Name = "Total de reservas")]
        public int TotalReservas { get; set; }

        [Display(Name = "Total de empréstimos")]
        public int TotalEmprestimos { get; set; }
    }
}
