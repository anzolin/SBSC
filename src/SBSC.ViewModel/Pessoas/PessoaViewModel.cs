using SBSC.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace SBSC.ViewModel.Pessoas
{
    public class PessoaViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Tipo é um campo obrigatório.")]
        [Display(Name = "Tipo")]
        [EnumDataType(typeof(Enumerations.Pessoa.TipoPessoa))]
        public int? TipoId { get; set; }

        [Display(Name = "Tipo")]
        public string TipoText { get; set; }

        [Required(ErrorMessage = "Sexo é um campo obrigatório.")]
        [Display(Name = "Sexo")]
        [EnumDataType(typeof(Enumerations.Pessoa.Sexo))]
        public int? SexoId { get; set; }

        [Display(Name = "Sexo")]
        public string SexoText { get; set; }

        [Display(Name = "Código/Matrícula")]
        public string CodigoMatricula { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [MinLength(5)]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Nome é um campo obrigatório.")]
        [MinLength(3)]
        [MaxLength(250)]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [MinLength(3)]
        [MaxLength(250)]
        [Display(Name = "Endereço")]
        public string Endereco { get; set; }

        [MinLength(3)]
        [MaxLength(50)]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        [MinLength(8)]
        [MaxLength(9)]
        [Display(Name = "CEP")]
        public string CEP { get; set; }

        [MinLength(3)]
        [MaxLength(150)]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Display(Name = "UF")]
        [EnumDataType(typeof(Enumerations.Pessoa.UF))]
        public int? UFId { get; set; }

        [Display(Name = "UF")]
        public string UFText { get; set; }

        [MinLength(3)]
        [MaxLength(15)]
        [Display(Name = "Telefone")]
        public string Telefone { get; set; }

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
