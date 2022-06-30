using Microsoft.AspNetCore.Mvc;

namespace LanchoneteWeb.Controllers
{
    public class ContatoController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) // realiza uma restrição, somente quem está autenticado pode ver a view
            {
                return View();
            }
            return RedirectToAction("Login","Account");
        }
    }
}
