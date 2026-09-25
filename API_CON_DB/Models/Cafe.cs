using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_CON_DB.Models;

[Table("cafe")]
public class Cafe
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, StringLength(120)]
    [Column("cafe")]
    [JsonPropertyName("cafe")]
    public string Nombre { get; set; } = string.Empty;

    [Required, StringLength(120)]
    [Column("especialidad")]
    public string Especialidad { get; set; } = string.Empty;

    [Required, StringLength(80)]
    [Column("presentacion")]
    public string Presentacion { get; set; } = string.Empty;

    [Required, StringLength(120)]
    [Column("origen")]
    public string Origen { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    [Column("cantidad")]
    public int Cantidad { get; set; }

    [Range(0.0, 99999999.99)]
    [Column("valor", TypeName = "decimal(10,2)")]
    public decimal Valor { get; set; }

    [Column("descripcion", TypeName = "text")]
    public string? Descripcion { get; set; }
}
