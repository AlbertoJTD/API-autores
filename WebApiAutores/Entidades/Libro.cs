namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set; } // Llave foranea
        public Autor Autor { get; set; } // Propiedad de navegacion
    }
}
