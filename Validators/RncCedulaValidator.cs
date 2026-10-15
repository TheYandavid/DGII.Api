namespace DGII.Api.Validators;

public static class RncCedulaValidator
{
    public const int MaxLength = 11;

    public static string Normalize(string? rncCedula)
    {
        if (string.IsNullOrWhiteSpace(rncCedula))
        {
            throw new ArgumentException("RNC/Cédula is required.", nameof(rncCedula));
        }

        var normalized = rncCedula.Trim();

        if (normalized.Length > MaxLength)
        {
            throw new ArgumentException($"RNC/Cédula cannot exceed {MaxLength} characters.", nameof(rncCedula));
        }

        if (!normalized.All(char.IsDigit))
        {
            throw new ArgumentException("RNC/Cédula must contain only digits.", nameof(rncCedula));
        }

        return normalized;
    }
}
