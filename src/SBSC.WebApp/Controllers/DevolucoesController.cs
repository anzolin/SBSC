using SBSC.Business;
using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Lib.MVC.Search;
using SBSC.Model;
using SBSC.ViewModel.Devolucoes;
using SBSC.WebApp.Extensions;
using SBSC.WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBSC.WebApp.Controllers
{
    [Authorize]
    public class DevolucoesController : BaseController
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

        public DevolucoesController()
        {

        }

        public ActionResult DoIt()
        {
            return RedirectToAction("Index", "Home");
        }

        public override string GetModuleName()
        {
            return DevolucoesModel.ConstantesInternas.ModuleNamePlural;
        }

        public override string GetModuleNameSingular()
        {
            return DevolucoesModel.ConstantesInternas.ModuleNameSingular;
        }

        public override string GetModuleNamePlural()
        {
            return DevolucoesModel.ConstantesInternas.ModuleNamePlural;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [ValidateInput(false)]
        public ActionResult Search(DevolucaoSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new DevolucaoSearchModel();

            var query = DevolucoesModel.GetBaseQuery(db, searchModel).OrderByDescending(q => q.DtEmprestimo);

            var queryResult = SearchHelper.ApplyPaging(query, query.Count(), Constantes.GridPageSize, searchModel.Page);

            searchModel.Page = (searchModel.Page ?? 1);
            searchModel.CurrentPage = searchModel.Page.Value;
            searchModel.PageSize = Constantes.GridPageSize;
            searchModel.TotalPages = Math.Ceiling((double)query.Count() / Constantes.GridPageSize);

            var currentDate = Genericos.GetDateTimeFromBrazil().Date;

            var model = new DevolucaoSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in queryResult.ToList()
                           let pessoa = objeto.Pessoa
                           let livros = objeto.ItensEmprestimo.ToList()
                           let dtDevolucao = objeto.ItensEmprestimo.Select(s => s.DtDevolucao).FirstOrDefault()
                           let situacao = (objeto.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado
                           && objeto.DtPrevisaoDevolucao < currentDate ? (int)Enumerations.Devolucao.SituacaoDevolucao.Atrasado :
                           (int)Enumerations.Devolucao.SituacaoDevolucao.NoPrazo)
                           select new DevolucaoViewModel
                           {
                               Id = objeto.Id,
                               PessoaId = pessoa.Id,
                               PessoaText = pessoa.Nome,
                               LivrosText = string.Join("; ", livros.Select(x => x.Livro.Titulo).ToList()),
                               StatusId = (int)objeto.Status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Emprestimo.StatusEmprestimo), (int)objeto.Status),
                               SituacaoId = situacao,
                               SituacaoText = EnumsHelper.GetText(typeof(Enumerations.Devolucao.SituacaoDevolucao), situacao),
                               DtEmprestimo = objeto.DtEmprestimo,
                               DtEmprestimoText = objeto.DtEmprestimo.ToShortDateString(),
                               DtPrevisaoDevolucao = objeto.DtPrevisaoDevolucao,
                               DtPrevisaoDevolucaoText = objeto.DtPrevisaoDevolucao.HasValue ? objeto.DtPrevisaoDevolucao.Value.ToShortDateString() : string.Empty,
                               DtDevolucao = dtDevolucao,
                               DtDevolucaoText = dtDevolucao.ToShortDateString(),
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                           }).OrderByDescending(ob => ob.SituacaoId).ThenBy(ob => ob.DtPrevisaoDevolucao).ToList()
            };

            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código do empréstimo, nome da pessoa, código do livro, título, gênero, autores, editora, local";
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("Pesquisa de {0}", GetModuleNamePlural());

            return View(model);
        }

        [HttpGet]
        public ActionResult Devolver(int id)
        {
            try
            {
                var viewModel = DevolucoesModel.GetViewModel(db, id);

                viewModel.DtDevolucao = Genericos.GetDateTimeFromBrazil();

                ViewBag.Title = GetModuleNamePlural();
                ViewBag.Subtitle = string.Format("Incluir {0}", GetModuleNameSingular());

                return View("Devolver", viewModel);
            }
            catch (Exception e)
            {
                this.AddNotification(e.Message, NotificationType.WARNING);
                return RedirectToAction("Search");
            }
        }

        [HttpPost]
        public ActionResult Devolver(DevolucaoViewModel formModel)
        {
            var viewModelError = new ViewModelErrors();

            if (ModelState.IsValid)
            {
                var objeto = db.Emprestimo.FirstOrDefault(p => p.Id == formModel.Id);

                if (DevolucoesModel.ValidarObjeto(db, ref viewModelError, objeto, formModel))
                {
                    DevolucoesModel.Save(db, objeto, formModel);

                    this.AddNotification(string.Format(Constantes._SalvaComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);

                    return RedirectToAction("Search");
                }
            }

            ModelState.MergeErrors(viewModelError);

            this.AddNotification(Constantes.NaoFoiPossivelExecutarAOperacaoPorFavorVerifiqueAsMensagensDeValidacao, NotificationType.ERROR);

            formModel = DevolucoesModel.GetViewModel(db, formModel.Id.Value);

            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("Incluir {0}", GetModuleNameSingular());

            return View("Devolver", formModel);
        }
    }
}