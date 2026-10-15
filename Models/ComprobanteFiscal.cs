using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGII.Api.Models;

//Creacion del modelo ComprobanteFiscal que representa un comprobante fiscal en la base de datos//
public class ComprobanteFiscal
{
    public const string EstadoActivo = "ACTIVO";
    public const string EstadoInactivo = "INACTIVO";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(11)]
    public string RncCedula { get; set; } = string.Empty;

    [Required]
    [MaxLength(19)]
    public string NCF { get; set; } = string.Empty;

    public DateTime FechaEmision { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Monto { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Itbis18 { get; set; }

    [Required]
    [MaxLength(10)]
    [RegularExpression($"^({EstadoActivo}|{EstadoInactivo})$")]
    public string Estado { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }

    public Contribuyente Contribuyente { get; set; } = null!;
}
