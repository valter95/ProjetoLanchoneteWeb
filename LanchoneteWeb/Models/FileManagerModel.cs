namespace LanchoneteWeb.Models
{
    public class FileManagerModel
    {
        public FileInfo[] Files { get; set; }
        public IFormFile IformFile { get; set; }
        public List<IFormFile> IformFiles { get; set; }
        public string PathImagensProduto { get; set; }

    }
}
