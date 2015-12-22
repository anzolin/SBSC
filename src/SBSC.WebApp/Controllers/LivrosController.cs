using SBSC.Business;
using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Lib.MVC.Search;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Livros;
using SBSC.ViewModel.Permissoes;
using SBSC.WebApp.Extensions;
using SBSC.WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBSC.WebApp.Controllers
{
    [Authorize]
    public class LivrosController : BaseController
    {
        public static PermissaoModuloViewModel permissoesAdministrador = new PermissaoModuloViewModel()
        {
            PodeConsultar = true,
            PodeCadastrar = true,
            PodeEditar = true,
            PodeExcluir = true
        };

        public static PermissaoModuloViewModel permissoesBibliotecario = permissoesAdministrador;

        public static PermissaoModuloViewModel permissoesAluno = new PermissaoModuloViewModel()
        {
            PodeConsultar = true,
            PodeCadastrar = false,
            PodeEditar = false,
            PodeExcluir = false
        };

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        public LivrosController()
        {

        }

        public override string GetModuleName()
        {
            return LivrosModel.ConstantesInternas.ModuleNamePlural;
        }

        public override string GetModuleNameSingular()
        {
            return LivrosModel.ConstantesInternas.ModuleNameSingular;
        }

        public override string GetModuleNamePlural()
        {
            return LivrosModel.ConstantesInternas.ModuleNamePlural;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [ValidateInput(false)]
        public ActionResult Search(LivroSearchModel searchModel)
        {
            if (!TemPermissao(Enumerations.Generico.TipoAcao.Consultar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                return RedirectToAction("Index", "Home");

            if (searchModel == null)
                searchModel = new LivroSearchModel();

            var query = LivrosModel.GetBaseQuery(db, searchModel).OrderBy(q => q.Titulo);

            var queryResult = SearchHelper.ApplyPaging(query, query.Count(), ConstantesModel.GridPageSize, searchModel.Page);

            searchModel.Page = (searchModel.Page ?? 1);
            searchModel.CurrentPage = searchModel.Page.Value;
            searchModel.PageSize = ConstantesModel.GridPageSize;
            searchModel.TotalPages = Math.Ceiling((double)query.Count() / ConstantesModel.GridPageSize);

            var model = new LivroSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in queryResult.ToList()
                           let status = LivrosModel.GetStatus(objeto)
                           let totalVinculos = LivrosModel.GetTotalByItemLinkedRecords(db, objeto.Id)
                           select new LivroViewModel
                           {
                               Id = objeto.Id,
                               Codigo = objeto.Codigo,
                               StatusId = (int)status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Livro.StatusLivro), (int)status),
                               Titulo = objeto.Titulo,
                               Autor = objeto.Autor,
                               Genero = objeto.Genero,
                               Editora = objeto.Editora,
                               EstadoConservacaoId = (int)objeto.EstadoConservacao,
                               EstadoConservacaoText = EnumsHelper.GetText(typeof(Enumerations.Livro.EstadoConservacaoLivro), (int)objeto.EstadoConservacao),
                               BaixadoId = objeto.IsBaixado ? (int)Enumerations.Generico.SimOuNao.Sim : (int)Enumerations.Generico.SimOuNao.Nao,
                               BaixadoText = objeto.IsBaixado ? EnumsHelper.GetText(Enumerations.Generico.SimOuNao.Sim) : EnumsHelper.GetText(Enumerations.Generico.SimOuNao.Nao),
                               DtBaixa = objeto.DtBaixa,
                               DtBaixaText = objeto.DtBaixa.HasValue ? objeto.DtBaixa.Value.ToShortDateString() : string.Empty,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                               DtEdicao = objeto.DtHrEdicao,
                               DtEdicaoText = objeto.DtHrEdicao.HasValue ? objeto.DtHrEdicao.Value.ToShortDateString() : string.Empty,
                               TotalReservas = totalVinculos.Item1,
                               TotalEmprestimos = totalVinculos.Item2
                           }).OrderBy(ob => ob.Titulo).ToList()
            };

            ViewBag.PodeCadastrar = TemPermissao(Enumerations.Generico.TipoAcao.Cadastrar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno);
            ViewBag.PodeEditar = TemPermissao(Enumerations.Generico.TipoAcao.Editar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno);
            ViewBag.PodeExcluir = TemPermissao(Enumerations.Generico.TipoAcao.Excluir, permissoesAdministrador, permissoesBibliotecario, permissoesAluno);

            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código, título, gênero, autores, editora, local";
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("Pesquisa de {0}", GetModuleNamePlural());

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!TemPermissao(Enumerations.Generico.TipoAcao.Cadastrar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                return RedirectToAction("Search");

            return Edit((int?)null);
        }

        [HttpPost]
        public ActionResult Create(LivroViewModel formModel)
        {
            if (!TemPermissao(Enumerations.Generico.TipoAcao.Cadastrar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                return RedirectToAction("Search");

            return Edit(formModel);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (!TemPermissao(id.HasValue ? Enumerations.Generico.TipoAcao.Editar : Enumerations.Generico.TipoAcao.Cadastrar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                    return RedirectToAction("Search");

                var viewModel = id.HasValue ? LivrosModel.GetViewModel(db, id.Value) : new LivroViewModel { };

                ViewBag.ListEstadoConservacao = LivrosModel.GetListItemEstadoConservacaoLivro(null);
                ViewBag.ListBaixado = GenericosModel.GetListItemSimOuNao(null);
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
        public ActionResult Edit(LivroViewModel formModel)
        {
            if (!TemPermissao(formModel.Id.HasValue ? Enumerations.Generico.TipoAcao.Editar : Enumerations.Generico.TipoAcao.Cadastrar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                return RedirectToAction("Search");

            var viewModelError = new ViewModelErrors();

            if (ModelState.IsValid)
            {
                var objeto = !formModel.Id.HasValue ? new Livro() : db.Livro.FirstOrDefault(p => p.Id == formModel.Id);

                if (LivrosModel.ValidarObjeto(db, ref viewModelError, objeto, formModel))
                {
                    LivrosModel.Save(db, objeto, formModel);

                    this.AddNotification(string.Format(Constantes._SalvoComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);

                    return RedirectToAction("Search");
                }
            }

            ModelState.MergeErrors(viewModelError);

            this.AddNotification(Constantes.NaoFoiPossivelExecutarAOperacaoPorFavorVerifiqueAsMensagensDeValidacao, NotificationType.ERROR);

            ViewBag.ListEstadoConservacao = LivrosModel.GetListItemEstadoConservacaoLivro(formModel.EstadoConservacaoId);
            ViewBag.ListBaixado = GenericosModel.GetListItemSimOuNao(formModel.BaixadoId);
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("{0} {1}", (formModel.Id.HasValue ? "Editar" : "Cadastrar"), GetModuleNameSingular());

            return View("Edit", formModel);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var viewModel = LivrosModel.GetViewModel(db, id);

            ViewBag.Title = string.Format("Visualizar {0}", GetModuleNameSingular());

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                if (!TemPermissao(Enumerations.Generico.TipoAcao.Excluir, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                    return RedirectToAction("Search");

                var isDelete = LivrosModel.Delete(db, id);

                if (isDelete.Item1)
                    this.AddNotification(string.Format(Constantes._ExcluidoComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);
                else
                {
                    if (isDelete.Item2 && isDelete.Item3)
                        this.AddNotification(string.Format(ConstantesModel.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("o {0}", GetModuleNameSingular().ToLower()), "empréstimos e reservas"), NotificationType.ERROR);
                    else if (isDelete.Item2 && !isDelete.Item3)
                        this.AddNotification(string.Format(ConstantesModel.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("o {0}", GetModuleNameSingular().ToLower()), "empréstimos"), NotificationType.ERROR);
                    else
                        this.AddNotification(string.Format(ConstantesModel.NaoFoiPossivelExcluir_PorquePossuiRegistrosDe_Vinculados, string.Format("o {0}", GetModuleNameSingular().ToLower()), "reservas"), NotificationType.ERROR);
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