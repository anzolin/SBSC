using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Devolucoes;
using SBSC.ViewModel.Emprestimos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;

namespace SBSC.Business
{
    public class Devolucoes
    {
        public class ConstantesInternas
        {
            public const string ModuleNameSingular = "Devolução";
            public const string ModuleNamePlural = "Devoluções";
        }

        public static IQueryable<Emprestimo> GetBaseQuery(SBSCEntities db, DevolucaoSearchModel searchModel)
        {
            IQueryable<Emprestimo> query = db.Emprestimo.Where(q => q.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado);

            if (!string.IsNullOrEmpty(searchModel.Search))
            {
                var term = searchModel.Search.Trim();

                query = from q in query
                        where term.Contains(q.Id.ToString())
                            // Itens do emprestimo
                            || q.ItensEmprestimo.Any(ie => ie.Livro.Titulo.Contains(term))
                            || q.ItensEmprestimo.Any(ie => ie.Livro.Genero.Contains(term))
                            || q.ItensEmprestimo.Any(ie => ie.Livro.Autor.Contains(term))
                            || q.ItensEmprestimo.Any(ie => ie.Livro.Editora.Contains(term))
                            || q.ItensEmprestimo.Any(ie => ie.Livro.Local.Contains(term))
                            // Pessoa
                            || q.Pessoa.Nome.Contains(term)
                            || q.Pessoa.Login.Contains(term)
                        select q;
            }

            return query;
        }

        public static DevolucaoViewModel GetViewModel(Emprestimo emprestimo)
        {
            var pessoa = emprestimo.Pessoa;

            var livros = new List<ItemEmprestimoViewModel>();

            var i = 1;

            foreach (var livro in emprestimo.ItensEmprestimo)
            {
                livros.Add(new ItemEmprestimoViewModel()
                {
                    Position = i,
                    LivroId = livro.Id_Livro,
                    LivroText = livro.Livro.Titulo
                });

                i++;
            }

            var livro1 = livros.FirstOrDefault(x => x.Position == 1);
            var livro2 = livros.FirstOrDefault(x => x.Position == 2);
            var livro3 = livros.FirstOrDefault(x => x.Position == 3);

            var currentDate = Genericos.GetDateTimeFromBrazil().Date;

            var situacao = (emprestimo.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado
                           && emprestimo.DtPrevisaoDevolucao < currentDate ? (int)Enumerations.Devolucao.SituacaoDevolucao.Atrasado :
                           (int)Enumerations.Devolucao.SituacaoDevolucao.NoPrazo);

            var viewModel = new DevolucaoViewModel
            {
                Id = emprestimo.Id,
                PessoaId = pessoa.Id,
                PessoaText = pessoa.Nome,
                Livro1Id = livro1.LivroId,
                Livro1Text = livro1.LivroText,
                Livro2Id = livro2 != null ? livro2.LivroId : (int?)null,
                Livro2Text = livro2 != null ? livro2.LivroText : string.Empty,
                Livro3Id = livro3 != null ? livro3.LivroId : (int?)null,
                Livro3Text = livro3 != null ? livro3.LivroText : string.Empty,
                StatusId = (int)emprestimo.Status,
                StatusText = EnumsHelper.GetText(typeof(Enumerations.Emprestimo.StatusEmprestimo), (int)emprestimo.Status),
                SituacaoId = situacao,
                SituacaoText = EnumsHelper.GetText(typeof(Enumerations.Devolucao.SituacaoDevolucao), situacao),
                DtEmprestimo = emprestimo.DtEmprestimo,
                DtEmprestimoText = emprestimo.DtEmprestimo.ToShortDateString(),
                DtPrevisaoDevolucao = emprestimo.DtPrevisaoDevolucao,
                DtPrevisaoDevolucaoText = emprestimo.DtPrevisaoDevolucao.HasValue ? emprestimo.DtPrevisaoDevolucao.Value.ToShortDateString() : string.Empty,
                DtCadastro = emprestimo.DtHrCadastro,
                DtCadastroText = emprestimo.DtHrCadastro.ToShortDateString(),
            };

            return viewModel;
        }

        public static DevolucaoViewModel GetViewModel(SBSCEntities db, int id)
        {
            var emprestimo = db.Emprestimo.FirstOrDefault(q => q.Id == id);

            if (emprestimo == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            var viewModel = GetViewModel(emprestimo);

            return viewModel;
        }

        public static void Save(SBSCEntities db, Emprestimo emprestimo, DevolucaoViewModel formModel)
        {
            if (emprestimo == null) throw new ArgumentNullException("pessoa");
            if (formModel == null) throw new ArgumentNullException("formModel");

            if (!formModel.DtDevolucao.HasValue)
                formModel.DtDevolucao = Genericos.GetDateTimeFromBrazil();

            emprestimo.Status = (byte)Enumerations.Emprestimo.StatusEmprestimo.Devolvido;

            foreach (var item in emprestimo.ItensEmprestimo)
            {
                item.DtDevolucao = formModel.DtDevolucao.Value;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Valida o objeto
        /// </summary>
        /// <param name="db"></param>
        /// <param name="viewModelError"></param>
        /// <param name="objeto"></param>
        /// <param name="formModel"></param>
        /// <returns></returns>
        public static bool ValidarObjeto(SBSCEntities db, ref ViewModelErrors viewModelError, Emprestimo objeto, DevolucaoViewModel formModel)
        {
            if (formModel == null) throw new ArgumentNullException("formModel");

            if (objeto == null)
            {
                viewModelError.AddModelError(string.Empty, Constantes.ORegistroNaoFoiEncontrado);

                return false;
            }

            if (formModel.DtDevolucao.HasValue)
            {
                if (formModel.DtDevolucao.Value.Date < objeto.DtEmprestimo.Date)
                    viewModelError.AddModelError("DtDevolucao", "A data de devolução não deve ser menor do que a data de empréstimo.");

                if (formModel.DtDevolucao.Value.Date == objeto.DtEmprestimo.Date)
                    viewModelError.AddModelError("DtDevolucao", "A data de devolução não deve ser igual a data de empréstimo.");
            }

            return viewModelError.IsValid;
        }
    }
}
