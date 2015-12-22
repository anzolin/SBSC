using System.ComponentModel.DataAnnotations;

namespace SBSC.Model
{
    public class Enumerations
    {
        #region Genérico

        public class Generico
        {
            public enum SimOuNao
            {
                [Display(Name = "Sim")]
                Sim = 2,

                [Display(Name = "Não")]
                Nao = 1
            }

            public enum Frequencia
            {
                [Display(Name = "Semanal")]
                Semanal = 1,

                [Display(Name = "Quinzenal")]
                Quinzenal = 2,

                [Display(Name = "Mensal")]
                Mensal = 3,

                [Display(Name = "Bimestral")]
                Bimestral = 4,

                [Display(Name = "Trimestral")]
                Trimestral = 5,

                [Display(Name = "Semestral")]
                Semestral = 6,

                [Display(Name = "Anual")]
                Anual = 7
            }

            public enum DiasDaSemana
            {
                [Display(Name = "domingo")]
                Domingo = 1,

                [Display(Name = "segunda-feira")]
                Segunda = 2,

                [Display(Name = "terça-feira")]
                Terca = 3,

                [Display(Name = "quarta-feira")]
                Quarta = 4,

                [Display(Name = "quinta-feira")]
                Quinta = 5,

                [Display(Name = "sexta-feira")]
                Sexta = 6,

                [Display(Name = "sábado")]
                Sabado = 7
            }

            public enum Mes
            {
                [Display(Name = "Janeiro")]
                Janeiro = 1,

                [Display(Name = "Fevereiro")]
                Fevereiro = 2,

                [Display(Name = "Março")]
                Marco = 3,

                [Display(Name = "Abril")]
                Abril = 4,

                [Display(Name = "Maio")]
                Maio = 5,

                [Display(Name = "Junho")]
                Junho = 6,

                [Display(Name = "Julho")]
                Julho = 7,

                [Display(Name = "Agosto")]
                Agosto = 8,

                [Display(Name = "Setembro")]
                Setembro = 9,

                [Display(Name = "Outubro")]
                Outubro = 10,

                [Display(Name = "Novembro")]
                Novembro = 11,

                [Display(Name = "Dezembro")]
                Dezembro = 12
            }

            public enum TipoAcao
            {
                [Display(Name = "Consultar")]
                Consultar = 1,

                [Display(Name = "Cadastrar")]
                Cadastrar = 2,

                [Display(Name = "Editar")]
                Editar = 3,

                [Display(Name = "Excluir")]
                Excluir = 4
            }
        }

        #endregion

        #region Pessoa

        public class Pessoa
        {
            public enum TipoPessoa
            {
                [Display(Name = "Administrador")]
                Administrador = 1,

                [Display(Name = "Bibliotecário")]
                Bibliotecario = 2,

                [Display(Name = "Aluno")]
                Aluno = 3
            }

            public enum Sexo
            {
                [Display(Name = "Feminino")]
                Feminino = 1,

                [Display(Name = "Masculino")]
                Masculino = 2
            }

            public enum UF
            {
                [Display(Name = "Pessoa física")]
                Fisica = 1,
            }
        }

        #endregion

        #region Livro

        public class Livro
        {
            public enum EstadoConservacaoLivro
            {
                [Display(Name = "Novo")]
                Novo = 1,

                [Display(Name = "Estado de novo")]
                EstadoDeNovo = 2,

                [Display(Name = "Muito bom")]
                MuitoBom = 3,

                [Display(Name = "Bom")]
                Bom = 4,

                [Display(Name = "Regular")]
                Regular = 5,

                [Display(Name = "Ruim")]
                Ruim = 6
            }

            public enum StatusLivro
            {
                [Display(Name = "Disponível")]
                Disponivel = 1,

                [Display(Name = "Indisponível")]
                Indisponivel = 2
            }
        }

        #endregion

        #region Reserva

        public class Reserva
        {
            public enum StatusReserva
            {
                [Display(Name = "Pendente")]
                Pendente = 1,

                [Display(Name = "Atendido")]
                Atendido = 2,

                //[Display(Name = "Cancelada")]
                //Cancelada = 3
            }
        }

        #endregion

        #region Empréstimo

        public class Emprestimo
        {
            public enum StatusEmprestimo
            {
                [Display(Name = "Emprestado")]
                Emprestado = 1,

                [Display(Name = "Devolvido")]
                Devolvido = 2,

                //[Display(Name = "Cancelado")]
                //Cancelado = 3
            }
        }

        #endregion

        #region Devolução

        public class Devolucao
        {
            public enum SituacaoDevolucao
            {
                [Display(Name = "No prazo")]
                NoPrazo = 1,

                [Display(Name = "Atrasado")]
                Atrasado = 2
            }
        }

        #endregion
    }
}
