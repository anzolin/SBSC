using SBSC.Business;
using SBSC.Lib.Helpers;
using SBSC.Lib.MVC.ModelState;
using SBSC.Lib.MVC.Search;
using SBSC.Model;
using SBSC.Model.Models;
using SBSC.ViewModel.Permissoes;
using SBSC.ViewModel.Reservas;
using SBSC.WebApp.Extensions;
using SBSC.WebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBSC.WebApp.Controllers
{
    [Authorize]
    public class ReservasController : BaseController
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
            PodeCadastrar = true,
            PodeEditar = false,
            PodeExcluir = false
        };

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        public ReservasController()
        {
        }

        public override string GetModuleName()
        {
            return ReservasModel.ConstantesInternas.ModuleNamePlural;
        }

        public override string GetModuleNameSingular()
        {
            return ReservasModel.ConstantesInternas.ModuleNameSingular;
        }

        public override string GetModuleNamePlural()
        {
            return ReservasModel.ConstantesInternas.ModuleNamePlural;
        }

        public ActionResult Index()
        {
            return RedirectToAction("Search");
        }

        [ValidateInput(false)]
        public ActionResult Search(ReservaSearchModel searchModel)
        {
            if (!TemPermissao(Enumerations.Generico.TipoAcao.Consultar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                return RedirectToAction("Index", "Home");

            if (searchModel == null)
                searchModel = new ReservaSearchModel();

            var userData = GetUserData();

            var query = ReservasModel.GetBaseQuery(db, searchModel, userData).OrderByDescending(q => q.DtPrevisaoEmprestimo);

            var queryResult = SearchHelper.ApplyPaging(query, query.Count(), Constantes.GridPageSize, searchModel.Page);

            searchModel.Page = (searchModel.Page ?? 1);
            searchModel.CurrentPage = searchModel.Page.Value;
            searchModel.PageSize = Constantes.GridPageSize;
            searchModel.TotalPages = Math.Ceiling((double)query.Count() / Constantes.GridPageSize);

            var model = new ReservaSearchViewModel
            {
                SearchModel = searchModel,
                Termo = searchModel.Search,
                FoundObjects = query.Count(),
                Objetos = (from objeto in queryResult.ToList()
                           let pessoa = objeto.Pessoa
                           let livro = objeto.Livro
                           select new ReservaViewModel
                           {
                               Id = objeto.Id,
                               PessoaId = pessoa.Id,
                               PessoaText = pessoa.Nome,
                               LivroId = livro.Id,
                               LivroText = livro.Titulo,
                               StatusId = (int)objeto.Status,
                               StatusText = EnumsHelper.GetText(typeof(Enumerations.Reserva.StatusReserva), (int)objeto.Status),
                               DtPrevisaoEmprestimo = objeto.DtPrevisaoEmprestimo,
                               DtPrevisaoEmprestimoText = objeto.DtPrevisaoEmprestimo.HasValue ? objeto.DtPrevisaoEmprestimo.Value.ToShortDateString() : string.Empty,
                               DtCadastro = objeto.DtHrCadastro,
                               DtCadastroText = objeto.DtHrCadastro.ToShortDateString(),
                           }).OrderBy(ob => ob.StatusId).ThenByDescending(ob => ob.DtPrevisaoEmprestimo).ToList()
            };

            ViewBag.PodeCadastrar = TemPermissao(Enumerations.Generico.TipoAcao.Cadastrar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno);
            ViewBag.PodeEditar = TemPermissao(Enumerations.Generico.TipoAcao.Editar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno);
            ViewBag.PodeExcluir = TemPermissao(Enumerations.Generico.TipoAcao.Excluir, permissoesAdministrador, permissoesBibliotecario, permissoesAluno);

            ViewBag.SearchWaterMark = "Pesquisar pelos campos: código da reserva, nome da pessoa, código do livro, título, gênero, autores, editora, local";
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
        public ActionResult Create(ReservaViewModel formModel)
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

                var data = GetUserData();

                var viewModel = id.HasValue ? ReservasModel.GetViewModel(db, id.Value) : new ReservaViewModel { StatusId = (int)Enumerations.Reserva.StatusReserva.Pendente };

                ViewBag.ListLivro = LivrosModel.GetListLivro(db, id.HasValue ? viewModel.LivroId : (int?)null, null);
                if (data.Item1 == (int)Enumerations.Pessoa.TipoPessoa.Aluno)
                    ViewBag.ListPessoa = PessoasModel.GetListPessoa(db, id.HasValue ? viewModel.PessoaId : data.Item2, data.Item2);
                else
                    ViewBag.ListPessoa = PessoasModel.GetListPessoa(db, id.HasValue ? viewModel.PessoaId : (int?)null, null);
                ViewBag.ListStatus = ReservasModel.GetListItemStatusReserva(null);
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
        public ActionResult Edit(ReservaViewModel formModel)
        {
            if (!TemPermissao(formModel.Id.HasValue ? Enumerations.Generico.TipoAcao.Editar : Enumerations.Generico.TipoAcao.Cadastrar, permissoesAdministrador, permissoesBibliotecario, permissoesAluno))
                return RedirectToAction("Search");

            var viewModelError = new ViewModelErrors();

            if (ModelState.IsValid)
            {
                var objeto = !formModel.Id.HasValue ? new Reserva() : db.Reserva.FirstOrDefault(p => p.Id == formModel.Id);

                if (ReservasModel.ValidarObjeto(db, ref viewModelError, objeto, formModel))
                {
                    ReservasModel.Save(db, objeto, formModel);

                    this.AddNotification(string.Format(Constantes._SalvaComSucesso, GetModuleNameSingular()), NotificationType.SUCCESS);

                    return RedirectToAction("Search");
                }
            }

            ModelState.MergeErrors(viewModelError);

            this.AddNotification(Constantes.NaoFoiPossivelExecutarAOperacaoPorFavorVerifiqueAsMensagensDeValidacao, NotificationType.ERROR);

            ViewBag.ListLivro = LivrosModel.GetListLivro(db, formModel.LivroId, null);
            ViewBag.ListPessoa = PessoasModel.GetListPessoa(db, formModel.PessoaId, null);
            ViewBag.ListStatus = ReservasModel.GetListItemStatusReserva(formModel.StatusId);
            ViewBag.Title = GetModuleNamePlural();
            ViewBag.Subtitle = string.Format("{0} {1}", (formModel.Id.HasValue ? "Editar" : "Incluir"), GetModuleNameSingular());

            return View("Edit", formModel);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var viewModel = ReservasModel.GetViewModel(db, id);

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

                var isDelete = ReservasModel.Delete(db, id);

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