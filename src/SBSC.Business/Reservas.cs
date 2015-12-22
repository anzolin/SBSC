using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Reservas;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Web.Mvc;

namespace SBSC.Business
{
    public class Reservas
    {
        public class ConstantesInternas
        {
            public const string ModuleNameSingular = "Reserva";
            public const string ModuleNamePlural = "Reservas";
        }

        public static IQueryable<Reserva> GetBaseQuery(SBSCEntities db, ReservaSearchModel searchModel, Tuple<int, int, string> userData)
        {
            IQueryable<Reserva> query = db.Reserva;

            // Se for o aluno que está consultando, só retorno os registros dele
            if (userData.Item1 == (int)Enumerations.Pessoa.TipoPessoa.Aluno)
                query = query.Where(q => q.Id_Pessoa == userData.Item2);

            if (!string.IsNullOrEmpty(searchModel.Search))
            {
                var term = searchModel.Search.Trim();

                query = from q in query
                        where term.Contains(q.Id.ToString())
                            // Livro
                            || q.Livro.Titulo.Contains(term)
                            || q.Livro.Genero.Contains(term)
                            || q.Livro.Autor.Contains(term)
                            || q.Livro.Editora.Contains(term)
                            || q.Livro.Local.Contains(term)
                            // Pessoa
                            || q.Pessoa.Nome.Contains(term)
                            || q.Pessoa.Login.Contains(term)
                        select q;
            }

            return query;
        }

        public static ReservaViewModel GetViewModel(Reserva reserva)
        {
            var viewModel = new ReservaViewModel
            {
                Id = reserva.Id,
                PessoaId = reserva.Pessoa.Id,
                PessoaText = reserva.Pessoa.Nome,
                LivroId = reserva.Livro.Id,
                LivroText = reserva.Livro.Titulo,
                StatusId = (int)reserva.Status,
                StatusText = EnumsHelper.GetText(typeof(Enumerations.Reserva.StatusReserva), (int)reserva.Status),
                DtPrevisaoEmprestimo = reserva.DtPrevisaoEmprestimo,
                DtPrevisaoEmprestimoText = reserva.DtPrevisaoEmprestimo.HasValue ? reserva.DtPrevisaoEmprestimo.Value.ToShortDateString() : string.Empty,
                DtCadastro = reserva.DtHrCadastro,
                DtCadastroText = reserva.DtHrCadastro.ToShortDateString(),
            };

            return viewModel;
        }

        public static ReservaViewModel GetViewModel(SBSCEntities db, int id)
        {
            var reserva = db.Reserva.FirstOrDefault(q => q.Id == id);

            if (reserva == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            var viewModel = GetViewModel(reserva);

            return viewModel;
        }

        public static void Save(SBSCEntities db, Reserva reserva, ReservaViewModel formModel)
        {
            if (reserva == null) throw new ArgumentNullException("pessoa");
            if (formModel == null) throw new ArgumentNullException("formModel");

            reserva.Id_Livro = formModel.LivroId;
            reserva.Id_Pessoa = formModel.PessoaId;
            reserva.Status = (byte)formModel.StatusId;
            reserva.DtPrevisaoEmprestimo = formModel.DtPrevisaoEmprestimo;

            if (!formModel.Id.HasValue)
            {
                reserva.DtHrCadastro = Genericos.GetDateTimeFromBrazil();

                db.Reserva.Add(reserva);
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
            var reserva = db.Reserva.FirstOrDefault(q => q.Id == id);

            if (reserva == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            //if (reserva.Emprestimos.Any() || reserva.Reservas.Any())
            //    return Tuple.Create(false, true, true);

            //if (reserva.Emprestimos.Any())
            //    return Tuple.Create(false, true, false);

            //if (reserva.Reservas.Any())
            //    return Tuple.Create(false, false, true);

            db.Reserva.Remove(reserva);
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
        public static bool ValidarObjeto(SBSCEntities db, ref ViewModelErrors viewModelError, Reserva objeto, ReservaViewModel formModel)
        {
            if (formModel == null) throw new ArgumentNullException("formModel");

            if (objeto == null)
            {
                viewModelError.AddModelError(string.Empty, Constantes.ORegistroNaoFoiEncontrado);

                return false;
            }

            var dateNow = Genericos.GetDateTimeFromBrazil();

            if (formModel.DtPrevisaoEmprestimo.HasValue)
            {
                if (formModel.DtPrevisaoEmprestimo.Value.Date < dateNow.Date)
                    viewModelError.AddModelError("DtPrevisaoEmprestimo", "A data de previsão do empréstimo não deve ser menor do que hoje.");
            }

            return viewModelError.IsValid;
        }

        /// <summary>
        /// Retorna uma enumeração de status
        /// </summary>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetListItemStatusReserva(int? selectValue)
        {
            return EnumsHelper.GetSelectListItems(typeof(Enumerations.Reserva.StatusReserva), selectValue);
        }
    }
}
