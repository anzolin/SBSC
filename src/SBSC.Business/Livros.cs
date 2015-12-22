using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Livros;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace SBSC.Business
{
    public class Livros
    {
        public class ConstantesInternas
        {
            public const string ModuleNameSingular = "Livro";
            public const string ModuleNamePlural = "Livros";
        }

        public static IQueryable<Livro> GetBaseQuery(SBSCEntities db, LivroSearchModel searchModel)
        {
            IQueryable<Livro> query = db.Livro;

            if (!string.IsNullOrEmpty(searchModel.Search))
            {
                var term = searchModel.Search.Trim();

                query = from q in query
                        where q.Codigo.Contains(term)
                            || q.Titulo.Contains(term) 
                            || q.Genero.Contains(term)
                            || q.Autor.Contains(term)
                            || q.Editora.Contains(term)
                            || q.Local.Contains(term)
                        select q;
            }

            return query;
        }

        public static LivroViewModel GetViewModel(Livro livro)
        {
            var status = GetStatus(livro);

            var viewModel = new LivroViewModel
            {
                Id = livro.Id,
                Codigo = livro.Codigo,
                StatusId = (int)status,
                StatusText = EnumsHelper.GetText(typeof(Enumerations.Livro.StatusLivro), (int)status),
                Titulo = livro.Titulo.Trim(),
                Genero = !string.IsNullOrEmpty(livro.Genero) ? livro.Genero.Trim() : string.Empty,
                Autor = !string.IsNullOrEmpty(livro.Autor) ? livro.Autor.Trim() : string.Empty,
                Editora = !string.IsNullOrEmpty(livro.Editora) ? livro.Editora.Trim() : string.Empty,
                Ano = livro.Ano,
                EstadoConservacaoId = (int)livro.EstadoConservacao,
                EstadoConservacaoText = EnumsHelper.GetText(typeof(Enumerations.Livro.EstadoConservacaoLivro), (int)livro.EstadoConservacao),
                Local = !string.IsNullOrEmpty(livro.Local) ? livro.Local.Trim() : string.Empty,
                Resumo = livro.Resumo,
                BaixadoId = livro.IsBaixado ? (int)Enumerations.Generico.SimOuNao.Sim : (int)Enumerations.Generico.SimOuNao.Nao,
                BaixadoText = livro.IsBaixado ? EnumsHelper.GetText(Enumerations.Generico.SimOuNao.Sim) : EnumsHelper.GetText(Enumerations.Generico.SimOuNao.Nao),
                DtBaixa = livro.DtBaixa,
                DtBaixaText = livro.DtBaixa.HasValue ? livro.DtBaixa.Value.ToShortDateString() : string.Empty,
                DtCadastro = livro.DtHrCadastro,
                DtCadastroText = livro.DtHrCadastro.ToShortDateString(),
                DtEdicao = livro.DtHrEdicao,
                DtEdicaoText = livro.DtHrEdicao.HasValue ? livro.DtHrEdicao.Value.ToShortDateString() : string.Empty
            };

            return viewModel;
        }

        public static LivroViewModel GetViewModel(SBSCEntities db, int id)
        {
            var livro = db.Livro.FirstOrDefault(q => q.Id == id);

            if (livro == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            var viewModel = GetViewModel(livro);

            return viewModel;
        }

        public static int GetTotalLinkedRecords(SBSCEntities db, int id)
        {
            var values = GetTotalByItemLinkedRecords(db, id);

            return (values.Item1 + values.Item2);
        }

        public static Tuple<int, int> GetTotalByItemLinkedRecords(SBSCEntities db, int id)
        {
            var objeto = db.Livro.FirstOrDefault(q => q.Id == id);

            if (objeto == null) return Tuple.Create(0, 0);

            return Tuple.Create(objeto.Reservas.Count(), objeto.ItensEmprestimo.Count());
        }

        public static void Save(SBSCEntities db, Livro livro, LivroViewModel formModel)
        {
            if (livro == null) throw new ArgumentNullException("livro");
            if (formModel == null) throw new ArgumentNullException("formModel");

            livro.Codigo = formModel.Codigo;
            livro.Titulo = formModel.Titulo.Trim();
            livro.Genero = !string.IsNullOrEmpty(formModel.Genero) ? formModel.Genero.Trim() : string.Empty;
            livro.Autor = !string.IsNullOrEmpty(formModel.Autor) ? formModel.Autor.Trim() : string.Empty;
            livro.Editora = !string.IsNullOrEmpty(formModel.Editora) ? formModel.Editora.Trim() : string.Empty;
            livro.Ano = formModel.Ano;
            livro.EstadoConservacao = (byte)formModel.EstadoConservacaoId;
            livro.Local = !string.IsNullOrEmpty(formModel.Local) ? formModel.Local.Trim() : string.Empty;
            livro.Resumo = formModel.Resumo;

            if (formModel.BaixadoId.HasValue)
            {
                if (formModel.BaixadoId.Value == (int)Enumerations.Generico.SimOuNao.Sim)
                {
                    livro.IsBaixado = true;
                    livro.DtBaixa = Genericos.GetDateTimeFromBrazil();
                }
                else
                {
                    livro.IsBaixado = false;
                    livro.DtBaixa = (DateTime?)null;
                }
            }

            if (formModel.Id.HasValue)
            {
                livro.DtHrEdicao = Genericos.GetDateTimeFromBrazil();
            }
            else
            {
                livro.DtHrCadastro = Genericos.GetDateTimeFromBrazil();

                db.Livro.Add(livro);
            }

            db.SaveChanges();
        }

        public static Tuple<bool, bool, bool> Delete(SBSCEntities db, int id)
        {
            var livro = db.Livro.FirstOrDefault(q => q.Id == id);

            if (livro == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            if (livro.ItensEmprestimo.Any() || livro.Reservas.Any())
                return Tuple.Create(false, true, true);

            if (livro.ItensEmprestimo.Any())
                return Tuple.Create(false, true, false);

            if (livro.Reservas.Any())
                return Tuple.Create(false, false, true);

            db.Livro.Remove(livro);
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
        public static bool ValidarObjeto(SBSCEntities db, ref ViewModelErrors viewModelError, Livro objeto, LivroViewModel formModel)
        {
            if (formModel == null) throw new ArgumentNullException("formModel");

            if (objeto == null)
            {
                viewModelError.AddModelError(string.Empty, Constantes.ORegistroNaoFoiEncontrado);

                return false;
            }

            #region Valida se já existe registro com título igual

            //var tituloIgual = db.Livro.Where(p => p.Titulo.ToLower().Equals(formModel.Titulo.Trim().ToLower()));

            //if (formModel.Id.HasValue)
            //    tituloIgual = tituloIgual.Where(p => p.Id != formModel.Id.Value);

            //if (tituloIgual.FirstOrDefault() != null)
            //    viewModelError.AddModelError("Titulo", "Já existe um livro com o 'Título' informado.");

            #endregion

            #region Valida se já existe registro com código igual

            if (!string.IsNullOrEmpty(formModel.Codigo))
            {
                var codigoIgual = db.Livro.Where(p => p.Codigo.ToLower().Equals(formModel.Codigo.Trim().ToLower()));

                if (formModel.Id.HasValue)
                    codigoIgual = codigoIgual.Where(p => p.Id != formModel.Id.Value);

                if (codigoIgual.FirstOrDefault() != null)
                    viewModelError.AddModelError("Codigo", "Já existe um livro com o 'Código' informado.");
            }

            #endregion

            return viewModelError.IsValid;
        }

        /// <summary>
        /// Retorna uma enumeração de estado de conservação de livro
        /// </summary>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetListItemEstadoConservacaoLivro(int? selectValue)
        {
            return EnumsHelper.GetSelectListItems(typeof(Enumerations.Livro.EstadoConservacaoLivro), selectValue);
        }

        /// <summary>
        /// Retorna uma enumeração de livro
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectValue"></param>
        /// <param name="livroId"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetListLivro(SBSCEntities db, int? selectValue, int? livroId)
        {
            // Retorna somente livros que não foram baixados
            IQueryable<Livro> query = db.Livro.Where(q => !q.IsBaixado);

            if (livroId.HasValue)
                query = query.Where(q => q.Id == livroId.Value);

            var lista = (from q in query.ToList()
                         select new SelectListItem
                         {
                             Value = q.Id.ToString(CultureInfo.InvariantCulture),
                             Text = FormatLivro(q),
                             Selected = (selectValue.HasValue ? (selectValue.Value == q.Id ? true : false) : false)
                         }).OrderBy(w => w.Text).ToList();

            return lista;
        }

        public static string FormatLivro(Livro livro)
        {
            var status = "D";

            var ultimoEmprestimo = livro.ItensEmprestimo.OrderByDescending(ob => ob.DtDevolucao).FirstOrDefault();

            if (ultimoEmprestimo != null 
                && ultimoEmprestimo.Emprestimo.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado)
                    status = "ND";

            return string.Format("[{0}] {1}", status, livro.Titulo);
        }

        public static int GetStatus(SBSCEntities db, int id)
        {
            var livro = db.Livro.FirstOrDefault(q => q.Id == id);

            return GetStatus(livro);
        }

        public static int GetStatus(Livro livro)
        {
            if (livro.IsBaixado)
            {
                return (int)Enumerations.Livro.StatusLivro.Indisponivel;
            }
            else
            {
                var ultimoEmprestimo = livro.ItensEmprestimo.OrderByDescending(ob => ob.DtDevolucao).FirstOrDefault();

                if (ultimoEmprestimo != null
                    && ultimoEmprestimo.Emprestimo.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado)
                    return (int)Enumerations.Livro.StatusLivro.Indisponivel;
                else
                    return (int)Enumerations.Livro.StatusLivro.Disponivel;
            }
        }
    }
}
