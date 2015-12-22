using SBSC.Business;
using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Lib.MVC.Search;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Emprestimos;
using SBSC.WebApp.Extensions;
using SBSC.WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBSC.WebApp.Controllers
{
    [Authorize]
    public class EmprestimosController : BaseController
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

        public EmprestimosController()
        {
        }

        public override string GetModuleName()
        {
            return EmprestimosModel.ConstantesInternas.ModuleNamePlural;
        }

        public override string GetModuleNameSingular()
        {
            return EmprestimosModel.ConstantesInternas.ModuleNameSingular;
        }

        public override string GetModuleNamePlural()
        {
            return EmprestimosModel.ConstantesInternas.ModuleNamePlural;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [ValidateInput(false)]
        public ActionResult Search(EmprestimoSearchModel searchModel)
        {
            if (searchModel == null)
                searchModel = new EmprestimoSearchModel();

            var query = EmprestimosModel.GetBaseQuery(db, searchModel).OrderByDescending(q => q.DtEmprestimo);

            var queryResult = SearchHelper.ApplyPaging(query, query.Count(), Constantes.GridPageSize, searchModel.Page);

            searchModel.Page = (searchModel.Page ?? 1);
            searchModel.CurrentPage = searchModel.Page.Value;
            searchModel.PageSize = Constantes.GridPageSize;
            searchModel.TotalPages = Math.Ceiling((double)query.Count() / Constantes.GridPageSize);

            var model = new EmprestimoSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in queryResult.ToList()
                           let pessoa = objeto.Pessoa
                           let livros = objeto.ItensEmprestimo.ToList()
                           let dtDevolucao = (objeto.Status == (int)Enumerations.Emprestimo.StatusEmprestimo.Devolvido ? objeto.ItensEmprestimo.Select(s => s.DtDevolucao).FirstOrDefault() : (DateTime?)null)
                           select new EmprestimoViewModel
                           {
                               Id = objeto.Id,
                               PessoaId = pessoa.Id,
                               PessoaText = pessoa.Nome,
                               LivrosText = string.Join("; ", livros.Select(x => x.Livro.Titulo).ToList()),
                               StatusId = (int)objeto.Status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Emprestimo.StatusEmprestimo), (int)objeto.Status),
                               DtEmprestimo = objeto.DtEmprestimo,
                               DtEmprestimoText = objeto.DtEmprestimo.ToShortDateString(),
                               DtPrevisaoDevolucao = objeto.DtPrevisaoDevolucao,
                               DtPrevisaoDevolucaoText = objeto.DtPrevisaoDevolucao.HasValue ?  objeto.DtPrevisaoDevolucao.Value.ToShortDateString() : string.Empty,
                               DtDevolucao = dtDevolucao,
                               DtDevolucaoText = dtDevolucao.HasValue ? dtDevolucao.Value.ToShortDateString() : string.Empty,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                           }).OrderByDescending(ob => ob.DtEmprestimo).ToList()
            };

            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código do empréstimo, nome da pessoa, código do livro, título, gênero, autores, editora, local";
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
        public ActionResult Create(EmprestimoViewModel formModel)
        {
            return Edit(formModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                var viewModel = id.HasValue ? EmprestimosModel.GetViewModel(db, id.Value) : new EmprestimoViewModel { StatusId = (int)Enumerations.Emprestimo.StatusEmprestimo.Emprestado };

                ViewBag.ListLivro1 = LivrosModel.GetListLivro(db, id.HasValue ? viewModel.Livro1Id : (int?)null, null);
                ViewBag.ListLivro2 = LivrosModel.GetListLivro(db, id.HasValue ? viewModel.Livro2Id : (int?)null, null);
                ViewBag.ListLivro3 = LivrosModel.GetListLivro(db, id.HasValue ? viewModel.Livro2Id : (int?)null, null);
                ViewBag.ListPessoa = PessoasModel.GetListPessoa(db, id.HasValue ? viewModel.PessoaId : (int?)null, null);
                ViewBag.Title = GetModuleNamePlural();
                ViewBag.Subtitle = string.Format("{0} {1}", (id.HasValue ? "Editar" : "Incluir"), GetModuleNameSingular());

                return View("Edit", viewModel);
            }
            catch (Exception e)
            {
                this.AddNotification(e.Message, NotificationType.WARNING);
                return RedirectToAction("Search");
            }
        }

        [HttpPost]
        public ActionResult Edit(EmprestimoViewModel formModel)
        {
            var viewModelError = new ViewModelErrors();

            if (ModelState.IsValid)
            {
                var objeto = !formModel.Id.HasValue ? new Emprestimo() : db.Emprestimo.FirstOrDefault(p => p.Id == formModel.Id);

                if (EmprestimosModel.ValidarObjeto(db, ref viewModelError, objeto, formModel))
                {
                    EmprestimosModel.Save(db, objeto, formModel);

                    this.AddNotification(string.Format(Constantes._SalvoComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);

                    return RedirectToAction("Search");
                }
            }

            ModelState.MergeErrors(viewModelError);

            this.AddNotification(Constantes.NaoFoiPossivelExecutarAOperacaoPorFavorVerifiqueAsMensagensDeValidacao, NotificationType.ERROR);

            ViewBag.ListLivro1 = LivrosModel.GetListLivro(db, formModel.Livro1Id, null);
            ViewBag.ListLivro2 = LivrosModel.GetListLivro(db, formModel.Livro1Id, null);
            ViewBag.ListLivro3 = LivrosModel.GetListLivro(db, formModel.Livro1Id, null);
            ViewBag.ListPessoa = PessoasModel.GetListPessoa(db, formModel.PessoaId, null);
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("{0} {1}", (formModel.Id.HasValue ? "Editar" : "Incluir"), GetModuleNameSingular());

            return View("Edit", formModel);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var viewModel = EmprestimosModel.GetViewModel(db, id);

            ViewBag.Title = string.Format("Visualizar {0}", GetModuleNameSingular());

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                var isDelete = EmprestimosModel.Delete(db, id);

                if (isDelete.Item1)
                    this.AddNotification(string.Format(Constantes._ExcluidoComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);
                else
                {
                    if (isDelete.Item2 && isDelete.Item3)
                        this.AddNotification(string.Format(Constantes.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("o {0}", GetModuleNameSingular().ToLower()), "empréstimos e reservas"), NotificationType.ERROR);
                    else if (isDelete.Item2 && !isDelete.Item3)
                        this.AddNotification(string.Format(Constantes.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("o {0}", GetModuleNameSingular().ToLower()), "empréstimos"), NotificationType.ERROR);
                    else
                        this.AddNotification(string.Format(Constantes.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("o {0}", GetModuleNameSingular().ToLower()), "reservas"), NotificationType.ERROR);
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