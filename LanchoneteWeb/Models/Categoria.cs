namespace LanchoneteWeb.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; }
        public string Descricao { get; set; }

        //TODO: Criar o relacionamento com a classe lanches 
        //1 categoria possui muitos(*) lanches
        public List<Lanche> Lanches { get; set; }   
    }
}
