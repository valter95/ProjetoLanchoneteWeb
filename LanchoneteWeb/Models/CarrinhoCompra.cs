﻿using LanchoneteWeb.Context;
using Microsoft.EntityFrameworkCore;

namespace LanchoneteWeb.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _context;

        public CarrinhoCompra(AppDbContext context)
        {
            _context = context;
        }

        public string CarrinhoCompraId { get; set; }
        public List<CarrinhoCompraItem> CarrinhoCompraItems { get; set; }

        public static CarrinhoCompra GetCarrinho(IServiceProvider services)
        {
            //define uma sessão 
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            //Obtem um serviço do tipo nosso contexto 
            var context = services.GetService<AppDbContext>();

            //obtem ou gera o id do carrinho caso seja nulo  ??(verifica se é null)
            string carrinhoId = session.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();

            //Atribui o id do carrinho na sessão 
            session.SetString("CarrinhoId", carrinhoId);

            return new CarrinhoCompra(context)
            {
                CarrinhoCompraId = carrinhoId
            };
        }

        public void AdicionarAoCarrinho(Lanche lanche)
        {
            var carrinhoCompraItem = _context.CarrinhoCompraItems.SingleOrDefault(s => s.Lanche.LancheId == lanche.LancheId
            && s.CarrinhoCompraId == CarrinhoCompraId);

            if (carrinhoCompraItem == null)
            {
                carrinhoCompraItem = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId,
                    Lanche = lanche,
                    Quantidade = 1
                };
                _context.CarrinhoCompraItems.Add(carrinhoCompraItem);
            }
            else
            {
                carrinhoCompraItem.Quantidade++;
            }
            //Persiste no banco de dados 
            _context.SaveChanges();
        }

        public void RemoveDoCarrinho(Lanche lanche)
        {
            var carrinhoCompraItem = _context.CarrinhoCompraItems.SingleOrDefault(s => s.Lanche.LancheId == lanche.LancheId
            && s.CarrinhoCompraId == CarrinhoCompraId);



            if (carrinhoCompraItem == null)
            {
                if (carrinhoCompraItem.Quantidade > 1)
                    carrinhoCompraItem.Quantidade--;

                else
                    _context.CarrinhoCompraItems.Remove(carrinhoCompraItem);
            }
            _context.SaveChanges();
        }

        public List<CarrinhoCompraItem> GetCarrinhoDeCompraItens()
        {
            return CarrinhoCompraItems ?? (CarrinhoCompraItems =
                   _context.CarrinhoCompraItems
                   .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                   .Include(s => s.Lanche).ToList());
        }

        public void LimparCarrinho()
        {
            var carrinhoItens = _context.CarrinhoCompraItems
                                .Where(carrinho => carrinho.CarrinhoCompraId == CarrinhoCompraId);
            _context.CarrinhoCompraItems.RemoveRange(carrinhoItens);

            _context.SaveChanges();
        }

        public decimal GetCarrinhoCompraTotal() 
        {
            var total = _context.CarrinhoCompraItems
                        .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                        .Select(c => c.Lanche.Preco * c.Quantidade).Sum();
            return total;
        }

    }
}
