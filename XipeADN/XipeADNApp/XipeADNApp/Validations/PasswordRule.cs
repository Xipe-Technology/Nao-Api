using System.ComponentModel.DataAnnotations;

namespace XipeADNApp.Validations
{
    public class PasswordRule : IValidationRule<string>
    {
        public PasswordRule() => ValidationMessage = "La contraseña debe de contener por lo menos 6 digitos alfanuméricos";

        public string ValidationMessage { get; set; }

        public bool Check(string value) => new DataTypeAttribute(DataType.Password).IsValid(value);
    }
}
