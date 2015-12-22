using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Emprestimos;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;

namespace SBSC.Business
{
    public class Emprestimos
    {
        public class ConstantesInternas
        {
            public const string ModuleNameSingular = "Empréstimo";
            public const string ModuleNamePlural = "Empréstimos";
        }

        public static IQueryable<Emprestimo> GetBaseQuery(SBSCEntities db, EmprestimoSearchModel searchModel)
        {
            IQueryable<Emprestimo> query = db.Emprestimo;

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

        public static EmprestimoViewModel GetViewModel(Emprestimo emprestimo)
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
            
            var viewModel = new EmprestimoViewModel
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
                DtEmprestimo = emprestimo.DtEmprestimo,
                DtEmprestimoText = emprestimo.DtEmprestimo.ToShortDateString(),
                DtPrevisaoDevolucao = emprestimo.DtPrevisaoDevolucao,
                DtPrevisaoDevolucaoText = emprestimo.DtPrevisaoDevolucao.HasValue ? emprestimo.DtPrevisaoDevolucao.Value.ToShortDateString() : string.Empty,
                DtCadastro = emprestimo.DtHrCadastro,
                DtCadastroText = emprestimo.DtHrCadastro.ToShortDateString(),
            };

            return viewModel;
        }

        public static EmprestimoViewModel GetViewModel(SBSCEntities db, int id)
        {
            var emprestimo = db.Emprestimo.FirstOrDefault(q => q.Id == id);

            if (emprestimo == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            var viewModel = GetViewModel(emprestimo);

            return viewModel;
        }

        public static void Save(SBSCEntities db, Emprestimo emprestimo, EmprestimoViewModel formModel)
        {
            if (emprestimo == null) throw new ArgumentNullException("pessoa");
            if (formModel == null) throw new ArgumentNullException("formModel");

            emprestimo.Id_Pessoa = formModel.PessoaId;
            emprestimo.DtEmprestimo = formModel.DtEmprestimo.Value;
            emprestimo.DtPrevisaoDevolucao = formModel.DtPrevisaoDevolucao;
            emprestimo.Status = (byte)formModel.StatusId;

            var livros = new List<ItemEmprestimoViewModel>();

            // Livro 1
            var livro1 = new ItemEmprestimoViewModel()
            {
                LivroId = formModel.Livro1Id
            };

            livros.Add(livro1);

            // Livro 2
            if (formModel.Livro2Id.HasValue)
            {
                var livro2 = new ItemEmprestimoViewModel()
                {
                    LivroId = formModel.Livro2Id.Value
                };

                livros.Add(livro2);
            }

            // Livro 3
            if (formModel.Livro3Id.HasValue)
            {
                var livro3 = new ItemEmprestimoViewModel()
                {
                    LivroId = formModel.Livro3Id.Value
                };

                livros.Add(livro3);
            }

            if (!formModel.Id.HasValue)
            {
                emprestimo.DtHrCadastro = Genericos.GetDateTimeFromBrazil();

                db.Emprestimo.Add(emprestimo);
            }

            foreach(var l in livros)
            {
                var itemEmprestimo = new ItemEmprestimo()
                {
                    Id_Emprestimo = emprestimo.Id,
                    Id_Livro = l.LivroId,
                    DtHrCadastro = Genericos.GetDateTimeFromBrazil()
                };

                if (!formModel.Id.HasValue)
                    db.ItemEmprestimo.Add(itemEmprestimo);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Deleta o objeto
        /// </summary>
        /// <param name="db"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Tuple<bool, bool, bool> Delete(SBSCEntities db, int id)
        {
            var emprestimo = db.Emprestimo.FirstOrDefault(q => q.Id == id);

            if (emprestimo == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            if (emprestimo.ItensEmprestimo.Any())
                return Tuple.Create(false, true, true);

            db.Emprestimo.Remove(emprestimo);
            db.SaveChanges();

            return Tuple.Create(true, false, false);
        }

        /// <summary>
        /// Valida o objeto
        /// </summary>
        /// <param name="db"></param>
        /// <param name="viewModelError"></param>
        /// <param name="objeto"></param>
        /// <param name="formModel"></param>
        /// <returns></returns>
        public static bool ValidarObjeto(SBSCEntities db, ref ViewModelErrors viewModelError, Emprestimo objeto, EmprestimoViewModel formModel)
        {
            if (formModel == null) throw new ArgumentNullException("formModel");

            if (objeto == null)
            {
                viewModelError.AddModelError(string.Empty, Constantes.ORegistroNaoFoiEncontrado);

                return false;
            }

            #region Validações de livros

            var livroIndisponivel = string.Format("O livro está {0}.", EnumsHelper.GetText(Enumerations.Livro.StatusLivro.Indisponivel));
            var livroJaSelecionado = "O livro já foi selecionado.";

            // Livro indisponível
            if (Livros.GetStatus(db, formModel.Livro1Id) == (int)Enumerations.Livro.StatusLivro.Indisponivel)
            {
                viewModelError.AddModelError("Livro1Id", livroIndisponivel);
            }

            if (formModel.Livro2Id.HasValue)
            {
                // Livro já selecionado
                if (formModel.Livro1Id.Equals(formModel.Livro2Id.Value))
                    viewModelError.AddModelError("Livro2Id", livroJaSelecionado);

                // Livro indisponível
                if (Livros.GetStatus(db, formModel.Livro2Id.Value) == (int)Enumerations.Livro.StatusLivro.Indisponivel)
                    viewModelError.AddModelError("Livro2Id", livroIndisponivel);
            }

            if (formModel.Livro3Id.HasValue)
            {
                // Livro já selecionado
                if (formModel.Livro1Id.Equals(formModel.Livro3Id.Value))
                    viewModelError.AddModelError("Livro3Id", livroJaSelecionado);

                // Livro indisponível
                if (Livros.GetStatus(db, formModel.Livro3Id.Value) == (int)Enumerations.Livro.StatusLivro.Indisponivel)
                    viewModelError.AddModelError("Livro3Id", livroIndisponivel);
            }

            if (formModel.Livro2Id.HasValue && formModel.Livro3Id.HasValue)
            {
                // Livro já selecionado
                if (formModel.Livro2Id.Equals(formModel.Livro3Id.Value))
                    viewModelError.AddModelError("Livro2Id", livroJaSelecionado);

                // Livro indisponível
                if (Livros.GetStatus(db, formModel.Livro2Id.Value) == (int)Enumerations.Livro.StatusLivro.Indisponivel)
                    viewModelError.AddModelError("Livro2Id", livroIndisponivel);
            }

            #endregion

            var dateNow = Genericos.GetDateTimeFromBrazil();

            if (formModel.DtEmprestimo.HasValue && formModel.DtPrevisaoDevolucao.HasValue)
            {
                if (formModel.DtPrevisaoDevolucao.Value.Date < formModel.DtEmprestimo.Value.Date)
                    viewModelError.AddModelError("DtPrevisaoDevolucao", "A data de previsão de devolução não deve ser menor do que a data de empréstimo.");

                if (formModel.DtPrevisaoDevolucao.Value.Date == formModel.DtEmprestimo.Value.Date)
                    viewModelError.AddModelError("DtPrevisaoDevolucao", "A data de previsão de devolução não deve ser igual a data de empréstimo.");
            }

            return viewModelError.IsValid;
        }
    }
}
