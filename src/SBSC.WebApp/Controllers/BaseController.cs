using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Permissoes;
using System;
using System.Data.Entity.Core;
using System.Web.Mvc;

namespace SBSC.WebApp.Controllers
{
    public class BaseController : Controller
    {
        public SBSCEntities db = new SBSCEntities();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }

        public string GetNomeSistema()
        {
            return "SBSC";
        }

        public string GetNomeSistemaCompleto()
        {
            return "Sistema Bibliotecário Santa Cândida";
        }

        public virtual string GetModuleName()
        {
            return string.Empty;
        }

        public virtual string GetModuleNameSingular()
        {
            return string.Empty;
        }

        public virtual string GetModuleNamePlural()
        {
            return string.Empty;
        }

        public Tuple<int, int, string> GetUserData()
        {
            var data = User.Identity.Name;

            if (string.IsNullOrEmpty(data))
                throw new ObjectNotFoundException("O login não foi efetuado.");

            var arr = data.Split('-');

            int tipoUsuarioId;
            int usuarioId;
            string login = arr[3];

            Int32.TryParse(arr[1], out tipoUsuarioId);
            Int32.TryParse(arr[2], out usuarioId);

            return Tuple.Create(tipoUsuarioId, usuarioId, login);
        }

        public bool TemPermissao(Enumerations.Generico.TipoAcao tipoAcao,
            PermissaoModuloViewModel permissaoAdministrador,
            PermissaoModuloViewModel permissaoBibliotecario,
            PermissaoModuloViewModel permissaoAluno)
        {
            var tipoPessoa = GetUserData().Item1;

            switch (tipoPessoa)
            {
                case (int)Enumerations.Pessoa.TipoPessoa.Administrador:
                    if (tipoAcao == Enumerations.Generico.TipoAcao.Consultar)
                        return permissaoAdministrador.PodeConsultar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Cadastrar)
                        return permissaoAdministrador.PodeCadastrar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Editar)
                        return permissaoAdministrador.PodeEditar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Excluir)
                        return permissaoAdministrador.PodeExcluir;
                    break;

                case (int)Enumerations.Pessoa.TipoPessoa.Bibliotecario:
                    if (tipoAcao == Enumerations.Generico.TipoAcao.Consultar)
                        return permissaoBibliotecario.PodeConsultar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Cadastrar)
                        return permissaoBibliotecario.PodeCadastrar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Editar)
                        return permissaoBibliotecario.PodeEditar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Excluir)
                        return permissaoBibliotecario.PodeExcluir;
                    break;

                case (int)Enumerations.Pessoa.TipoPessoa.Aluno:
                    if (tipoAcao == Enumerations.Generico.TipoAcao.Consultar)
                        return permissaoAluno.PodeConsultar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Cadastrar)
                        return permissaoAluno.PodeCadastrar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Editar)
                        return permissaoAluno.PodeEditar;
                    else if (tipoAcao == Enumerations.Generico.TipoAcao.Excluir)
                        return permissaoAluno.PodeExcluir;
                    break;
            }

            return false;
        }

        public ActionResult UserData()
        {
            var data = GetUserData();

            ViewBag.Login = data.Item3;

            return View();
        }

        public ActionResult Menu()
        {
            var data = GetUserData();

            var tipoPessoa = data.Item1;

            switch (tipoPessoa)
            {
                case (int)Enumerations.Pessoa.TipoPessoa.Administrador:
                case (int)Enumerations.Pessoa.TipoPessoa.Bibliotecario:
                    return View("_MenuBibliotecario");

                case (int)Enumerations.Pessoa.TipoPessoa.Aluno:
                    return View("_MenuAluno");
            }

            return View("_MenuAluno");
        }
    }
}