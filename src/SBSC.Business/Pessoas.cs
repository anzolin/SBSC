using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Pessoas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace SBSC.Business
{
    public class Pessoas
    {
        public class ConstantesInternas
        {
            public const string ModuleNameSingular = "Pessoa";
            public const string ModuleNamePlural = "Pessoas";
        }

        public static IQueryable<Pessoa> GetBaseQuery(SBSCEntities db, PessoaSearchModel searchModel)
        {
            IQueryable<Pessoa> query = db.Pessoa.Where(q => !q.IsSistema);

            if (!string.IsNullOrEmpty(searchModel.Search))
            {
                var term = searchModel.Search.Trim();

                query = from q in query
                        where q.Nome.Contains(term)
                        || q.Login.Contains(term)
                        select q;
            }

            return query;
        }

        public static PessoaViewModel GetViewModel(Pessoa pessoa)
        {
            var viewModel = new PessoaViewModel
            {
                Id = pessoa.Id,
                TipoId = pessoa.Tipo,
                TipoText = EnumsHelper.GetText(typeof(Enumerations.Pessoa.TipoPessoa), (int)pessoa.Tipo),
                SexoId = pessoa.Sexo,
                SexoText = pessoa.Sexo.HasValue ? EnumsHelper.GetText(typeof(Enumerations.Pessoa.Sexo), (int)pessoa.Sexo) : string.Empty,
                CodigoMatricula = pessoa.CodigoMatricula,
                Login = pessoa.Login,
                Senha = pessoa.Senha,
                Nome = pessoa.Nome,
                Endereco = pessoa.Endereco,
                Bairro = pessoa.Bairro,
                CEP = pessoa.CEP,
                Cidade = pessoa.CEP,
                UFId = pessoa.UF,
                UFText = pessoa.UF.HasValue ? EnumsHelper.GetText(typeof(Enumerations.Pessoa.UF), (int)pessoa.UF) : string.Empty,
                Telefone = pessoa.Telefone,
                DtCadastro = pessoa.DtHrCadastro,
                DtCadastroText = pessoa.DtHrCadastro.ToShortDateString(),
                DtEdicao = pessoa.DtHrEdicao,
                DtEdicaoText = pessoa.DtHrEdicao.HasValue ? pessoa.DtHrEdicao.Value.ToShortDateString() : string.Empty
            };

            return viewModel;
        }

        public static PessoaViewModel GetViewModel(SBSCEntities db, int id)
        {
            var pessoa = db.Pessoa.FirstOrDefault(q => q.Id == id);

            if (pessoa == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            var viewModel = GetViewModel(pessoa);

            return viewModel;
        }

        public static int GetTotalLinkedRecords(SBSCEntities db, int id)
        {
            var values = GetTotalByItemLinkedRecords(db, id);

            return (values.Item1 + values.Item2);
        }

        public static Tuple<int, int> GetTotalByItemLinkedRecords(SBSCEntities db, int id)
        {
            var objeto = db.Pessoa.FirstOrDefault(q => q.Id == id);

            if (objeto == null) return Tuple.Create(0, 0);

            return Tuple.Create(objeto.Reservas.Count(), objeto.Emprestimos.Count());
        }

        public static void Save(SBSCEntities db, Pessoa pessoa, PessoaViewModel formModel)
        {
            if (pessoa == null) throw new ArgumentNullException("pessoa");
            if (formModel == null) throw new ArgumentNullException("formModel");

            pessoa.Tipo = (byte)formModel.TipoId;
            pessoa.Sexo = (byte?)formModel.SexoId;
            pessoa.CodigoMatricula = formModel.CodigoMatricula;
            pessoa.Login = !string.IsNullOrEmpty(formModel.Login) ? formModel.Login.Trim() : string.Empty;
            pessoa.Senha = !string.IsNullOrEmpty(formModel.Senha) ? formModel.Senha.Trim() : string.Empty;
            pessoa.Nome = formModel.Nome.Trim();
            pessoa.Endereco = formModel.Endereco;
            pessoa.Bairro = formModel.Bairro;
            pessoa.CEP = formModel.CEP;
            pessoa.Cidade = formModel.Cidade;
            pessoa.UF = (byte?)formModel.UFId;
            pessoa.Telefone = formModel.Telefone;
            
            if (formModel.Id.HasValue)
            {
                pessoa.DtHrEdicao = Genericos.GetDateTimeFromBrazil();
            }
            else
            {
                pessoa.DtHrCadastro = Genericos.GetDateTimeFromBrazil();

                db.Pessoa.Add(pessoa);
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
            var pessoa = db.Pessoa.FirstOrDefault(q => q.Id == id);

            if (pessoa == null)
                throw new ObjectNotFoundException("O registro não foi encontrado.");

            if (pessoa.Emprestimos.Any() || pessoa.Reservas.Any())
                return Tuple.Create(false, true, true);

            if (pessoa.Emprestimos.Any())
                return Tuple.Create(false, true, false);

            if (pessoa.Reservas.Any())
                return Tuple.Create(false, false, true);

            db.Pessoa.Remove(pessoa);
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
        public static bool ValidarObjeto(SBSCEntities db, ref ViewModelErrors viewModelError, Pessoa objeto, PessoaViewModel formModel)
        {
            if (formModel == null) throw new ArgumentNullException("formModel");

            if (objeto == null)
            {
                viewModelError.AddModelError(string.Empty, Constantes.ORegistroNaoFoiEncontrado);

                return false;
            }

            #region Valida se já existe registro com nome igual

            var nomeIgual = db.Pessoa.Where(p => p.Nome.ToLower().Equals(formModel.Nome.Trim().ToLower()));

            if (formModel.Id.HasValue)
                nomeIgual = nomeIgual.Where(p => p.Id != formModel.Id.Value);

            if (nomeIgual.FirstOrDefault() != null)
                viewModelError.AddModelError("Nome", "Já existe uma pessoa com o 'Nome' informado.");

            #endregion

            #region Valida se já existe registro com código/matrícula igual

            if (!string.IsNullOrEmpty(formModel.CodigoMatricula))
            {
                var codigoMatriculaIgual = db.Pessoa.Where(p => p.CodigoMatricula.ToLower().Equals(formModel.CodigoMatricula.Trim().ToLower()));

                if (formModel.Id.HasValue)
                    codigoMatriculaIgual = codigoMatriculaIgual.Where(p => p.Id != formModel.Id.Value);

                if (codigoMatriculaIgual.FirstOrDefault() != null)
                    viewModelError.AddModelError("CodigoMatricula", "Já existe uma pessoa com o 'Código/Matrícula' informado.");
            }

            #endregion

            #region Valida se já existe registro com login igual e se senha preenchida se login preenchido

            if (!string.IsNullOrEmpty(formModel.Login))
            {
                var loginIgual = db.Pessoa.Where(p => p.Login.ToLower().Equals(formModel.Login.Trim().ToLower()));

                if (formModel.Id.HasValue)
                    loginIgual = loginIgual.Where(p => p.Id != formModel.Id.Value);

                if (loginIgual.FirstOrDefault() != null)
                    viewModelError.AddModelError("Login", "Já existe uma pessoa com o 'Login' informado.");

                // Valida se a senha está preenchida quando o login é preenchido na tela de cadastro
                if (string.IsNullOrEmpty(formModel.Senha) && !string.IsNullOrEmpty(objeto.Login))
                    viewModelError.AddModelError("Senha", "O campo 'Senha' é obrigatório.");
            }

            #endregion

            return viewModelError.IsValid;
        }

        /// <summary>
        /// Valida o CPF informado
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public static bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf)) throw new ArgumentException("The string CPF cannot be null or empty", "cpf");

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCPF;
            string digito;
            int soma;
            int resto;

            cpf = cpf.Trim();

            // Se possuir máscara, removo-a
            cpf = StringHelper.RemoveMask(cpf);

            // Se tamanho for diferente de 11, é inválido
            if (cpf.Length != 11)
                return false;

            var cpfsInvalidos = new string[] { "00000000000", 
                                               "11111111111",
                                               "22222222222",
                                               "33333333333",
                                               "44444444444",
                                               "55555555555",
                                               "66666666666",
                                               "77777777777",
                                               "88888888888",
                                               "99999999999"};

            if (cpfsInvalidos.Contains(cpf))
                return false;

            tempCPF = cpf.Substring(0, 9);

            soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCPF[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCPF = tempCPF + digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCPF[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        /// <summary>
        /// Valida o CNPJ informado
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        public static bool ValidarCNPJ(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj)) throw new ArgumentException("The string CNPJ cannot be null or empty", "cnpj");

            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCNPJ;

            cnpj = cnpj.Trim();

            // Se possuir máscara, removo-a
            cnpj = StringHelper.RemoveMask(cnpj);

            // Se tamanho for diferente de 14, é inválido
            if (cnpj.Length != 14)
                return false;

            // Caso coloque todos os numeros iguais
            switch (cnpj)
            {
                case "00000000000000":
                    return false;

                case "11111111111111":
                    return false;

                case "22222222222222":
                    return false;

                case "33333333333333":
                    return false;

                case "44444444444444":
                    return false;

                case "55555555555555":
                    return false;

                case "66666666666666":
                    return false;

                case "77777777777777":
                    return false;

                case "88888888888888":
                    return false;

                case "99999999999999":
                    return false;
            }

            tempCNPJ = cnpj.Substring(0, 12);

            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCNPJ[i].ToString()) * multiplicador1[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();

            tempCNPJ = tempCNPJ + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCNPJ[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }

        /// <summary>
        /// Retorna uma enumeração de sexo
        /// </summary>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetListItemSexo(int? selectValue)
        {
            return EnumsHelper.GetSelectListItems(typeof(Enumerations.Pessoa.Sexo), selectValue);
        }

        /// <summary>
        /// Retorna uma enumeração de tipos de pessoas
        /// </summary>
        /// <param name="selectValue"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetListItemTipoPessoa(bool showAdministrador, int? selectValue)
        {
            var list = new List<SelectListItem>();

            var administador = new SelectListItem()
            {
                Value = ((int)Enumerations.Pessoa.TipoPessoa.Administrador).ToString(),
                Text = EnumsHelper.GetText(Enumerations.Pessoa.TipoPessoa.Administrador),
                Selected = false
            };

            var bibliotecario = new SelectListItem()
            {
                Value = ((int)Enumerations.Pessoa.TipoPessoa.Bibliotecario).ToString(),
                Text = EnumsHelper.GetText(Enumerations.Pessoa.TipoPessoa.Bibliotecario),
                Selected = false
            };

            var aluno = new SelectListItem()
            {
                Value = ((int)Enumerations.Pessoa.TipoPessoa.Aluno).ToString(),
                Text = EnumsHelper.GetText(Enumerations.Pessoa.TipoPessoa.Aluno),
                Selected = false
            };

            if (showAdministrador)
                list.Add(administador);

            list.Add(bibliotecario);

            list.Add(aluno);

            foreach (var item in list)
            {
                if (selectValue.HasValue)
                    if (item.Value == selectValue.ToString())
                        item.Selected = true;
            }

            return list;
        }

        /// <summary>
        /// Retorna uma enumeração de pessoa
        /// </summary>
        /// <param name="db"></param>
        /// <param name="selectValue"></param>
        /// <param name="pessoaId"></param>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetListPessoa(SBSCEntities db, int? selectValue, int? pessoaId)
        {
            IQueryable<Pessoa> query = db.Pessoa.Where(q => !q.IsSistema);

            if (pessoaId.HasValue)
                query = query.Where(q => q.Id == pessoaId.Value);

            var lista = (from q in query.ToList()
                         select new SelectListItem
                         {
                             Value = q.Id.ToString(CultureInfo.InvariantCulture),
                             Text = q.Nome,
                             Selected = (selectValue.HasValue ? (selectValue.Value == q.Id ? true : false) : false)
                         }).OrderBy(w => w.Text).ToList();

            return lista;
        }
    }
}
