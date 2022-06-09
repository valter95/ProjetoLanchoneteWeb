using LanchoneteWeb.Models;
using LanchoneteWeb.Repositories.Interfaces;
using LanchoneteWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LanchoneteWeb.Controllers
{
    public class CarrinhoCompraController : Controller
    {
        private readonly ILancheRepository _ILancheRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraController(ILancheRepository iLancheRepository, CarrinhoCompra carrinhoCompra)
        {
            _ILancheRepository = iLancheRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        public IActionResult Index()
        {
            var itens = _carrinhoCompra.GetCarrinhoDeCompraItens();
            _carrinhoCompra.CarrinhoCompraItems = itens;
            var carrinhoCompraVm = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = _carrinhoCompra.GetCarrinhoCompraTotal()
            };
            return View(carrinhoCompraVm);
        }

        public IActionResult AdicionarItemNoCarrinhoCompra(int lancheId) 
        {
            var lancheSelecionado = _ILancheRepository.Lanches
                                    .FirstOrDefault(p => p.LancheId == lancheId);
            if (lancheSelecionado != null) 
            {
                _carrinhoCompra.AdicionarAoCarrinho(lancheSelecionado);
            }
            return RedirectToAction("Index");
        
        
        }
        public IActionResult RemoverItemNoCarrinhoCompra(int lancheId)
        {
            var lancheSelecionado = _ILancheRepository.Lanches
                                    .FirstOrDefault(p => p.LancheId == lancheId);
            if (lancheSelecionado != null)
            {
                _carrinhoCompra.RemoveDoCarrinho(lancheSelecionado);
            }
            return RedirectToAction("Index");


        }
    }
}
