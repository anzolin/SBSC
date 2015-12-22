using SBSC.Business;
using SBSC.Lib.MVC.ModelState;
using SBSC.Lib.MVC.Search;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Pessoas;
using SBSC.WebApp.Extensions;
using SBSC.WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBSC.WebApp.Controllers
{
    [Authorize]
    public class PessoasController : BaseController
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            var tipoPessoa = GetUserData().Item1;

            if (tipoPessoa == (int)Enumerations.Pessoa.TipoPessoa.Aluno)
            {
                requestContext.HttpContext.Response.RedirectToRoute(new RouteValueDictionary(new { action = "Index", controller = "Home" })); //.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "Index", controller = "Home" }));
            }
        }

        public PessoasController()
        {
        }

        public override string GetModuleName()
        {
            return PessoasModel.ConstantesInternas.ModuleNamePlural;
        }

        public override string GetModuleNameSingular()
        {
            return PessoasModel.ConstantesInternas.ModuleNameSingular;
        }

        public override string GetModuleNamePlural()
        {
            return PessoasModel.ConstantesInternas.ModuleNamePlural;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [ValidateInput(false)]
        public ActionResult Search(PessoaSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new PessoaSearchModel();

            var query = PessoasModel.GetBaseQuery(db, searchModel).OrderBy(q => q.Nome);

            var queryResult = SearchHelper.ApplyPaging(query, query.Count(), Constantes.GridPageSize, searchModel.Page);

            searchModel.Page = (searchModel.Page ?? 1);
            searchModel.CurrentPage = searchModel.Page.Value;
            searchModel.PageSize = Constantes.GridPageSize;
            searchModel.TotalPages = Math.Ceiling((double)query.Count() / Constantes.GridPageSize);

            var model = new PessoaSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in queryResult.ToList()
                           let totalVinculos = PessoasModel.GetTotalByItemLinkedRecords(db, objeto.Id)
                           select new PessoaViewModel
                           {
                               Id = objeto.Id,
                               Nome = objeto.Nome,
                               CodigoMatricula = objeto.CodigoMatricula,
                               Login = objeto.Login,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                               DtEdicao = objeto.DtHrEdicao,
                               DtEdicaoText = objeto.DtHrEdicao.HasValue ? objeto.DtHrEdicao.Value.ToShortDateString() : string.Empty,
                               TotalReservas = totalVinculos.Item1,
                               TotalEmprestimos = totalVinculos.Item2
                           }).OrderBy(ob => ob.Nome).ToList()
            };

            ViewBag.SearchWaterMark = "Pesquisar pelos campos: nome, login";
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("Pesquisa de {0}", GetModuleNamePlural());

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return Edit((int?)null);
        }

        [HttpPost]
        public ActionResult Create(PessoaViewModel formModel)
        {
            return Edit(formModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                var viewModel = id.HasValue ? PessoasModel.GetViewModel(db, id.Value) : new PessoaViewModel { };

                ViewBag.ListTipo = PessoasModel.GetListItemTipoPessoa(false, null);
                ViewBag.ListSexo = PessoasModel.GetListItemSexo(null);
                ViewBag.Title = GetModuleNamePlural();
                ViewBag.Subtitle = string.Format("{0} {1}", (id.HasValue ? "Editar" : "Cadastrar"), GetModuleNameSingular());

                return View("Edit", viewModel);
            }
            catch (Exception e)
            {
                this.AddNotification(e.Message, NotificationType.WARNING);
                return RedirectToAction("Search");
            }
        }

        [HttpPost]
        public ActionResult Edit(PessoaViewModel formModel)
        {
            var viewModelError = new ViewModelErrors();

            if (ModelState.IsValid)
            {
                var objeto = !formModel.Id.HasValue ? new Pessoa() : db.Pessoa.FirstOrDefault(p => p.Id == formModel.Id);

                if (PessoasModel.ValidarObjeto(db, ref viewModelError, objeto, formModel))
                {
                    PessoasModel.Save(db, objeto, formModel);

                    this.AddNotification(string.Format(Constantes._SalvaComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);

                    return RedirectToAction("Search");
                }
            }

            ModelState.MergeErrors(viewModelError);

            this.AddNotification(Constantes.NaoFoiPossivelExecutarAOperacaoPorFavorVerifiqueAsMensagensDeValidacao, NotificationType.ERROR);

            ViewBag.ListTipo = PessoasModel.GetListItemTipoPessoa(false, formModel.TipoId);
            ViewBag.ListSexo = PessoasModel.GetListItemSexo(formModel.SexoId);
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("{0} {1}", (formModel.Id.HasValue ? "Editar" : "Cadastrar"), GetModuleNameSingular());

            return View("Edit", formModel);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var viewModel = PessoasModel.GetViewModel(db, id);

            ViewBag.Title = string.Format("Visualizar {0}", GetModuleNameSingular());

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                var isDelete = PessoasModel.Delete(db, id);

                if (isDelete.Item1)
                    this.AddNotification(string.Format(Constantes._ExcluidaComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);
                else
                {
                    if (isDelete.Item2 && isDelete.Item3)
                        this.AddNotification(string.Format(Constantes.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("a {0}", GetModuleNameSingular().ToLower()), "empréstimos e reservas"), NotificationType.ERROR);
                    else if (isDelete.Item2 && !isDelete.Item3)
                        this.AddNotification(string.Format(Constantes.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("a {0}", GetModuleNameSingular().ToLower()), "empréstimos"), NotificationType.ERROR);
                    else
                        this.AddNotification(string.Format(Constantes.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("a {0}", GetModuleNameSingular().ToLower()), "reservas"), NotificationType.ERROR);
                }
            }
            catch (Exception e)
            {
                this.AddNotification(e.Message, NotificationType.WARNING);
            }

            return RedirectToAction("Search");
        }
    }
}