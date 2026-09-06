namespace API_CON_DB.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }

        public int Cantidad { get; set; }

        public double Valor { get; set; }
    }
}
