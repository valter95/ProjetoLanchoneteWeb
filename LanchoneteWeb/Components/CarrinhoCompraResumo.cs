using LanchoneteWeb.Models;
using LanchoneteWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LanchoneteWeb.Components
{
    public class CarrinhoCompraResumo : ViewComponent
    {
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraResumo(CarrinhoCompra carrinhoCompra)
        {
            _carrinhoCompra = carrinhoCompra;
        }

        public IViewComponentResult Invoke() 
        {
            var itens = _carrinhoCompra.GetCarrinhoDeCompraItens();
           // Teste => var itens = new List<CarrinhoCompraItem>() { new CarrinhoCompraItem(), new CarrinhoCompraItem() };
            _carrinhoCompra.CarrinhoCompraItems = itens;
            var carrinhoCompraVm = new CarrinhoCompraViewModel
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = _carrinhoCompra.GetCarrinhoCompraTotal()
            };
            return View(carrinhoCompraVm);
        }
    }
}
