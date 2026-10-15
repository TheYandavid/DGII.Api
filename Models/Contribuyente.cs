using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DGII.Api.Models;

public class Contribuyente
{
    public const string TipoPersonaFisica = "PERSONA FISICA";
    public const string TipoPersonaJuridica = "PERSONA JURIDICA";
    public const string EstatusActivo = "ACTIVO";
    public const string EstatusInactivo = "INACTIVO";

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(11)]
    public string RncCedula { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [RegularExpression($"^({TipoPersonaFisica}|{TipoPersonaJuridica})$")]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    [RegularExpression($"^({EstatusActivo}|{EstatusInactivo})$")]
    public string Estatus { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; }

    public ICollection<ComprobanteFiscal> ComprobantesFiscales { get; set; } = new List<ComprobanteFiscal>();
}
