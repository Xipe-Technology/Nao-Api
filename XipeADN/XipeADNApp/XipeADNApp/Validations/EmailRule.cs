using System.ComponentModel.DataAnnotations;

namespace XipeADNApp.Validations
{
    public class EmailRule : IValidationRule<string>
    {
        public EmailRule() => ValidationMessage = "Este campo debe ser un correo";

        public string ValidationMessage { get; set; }

        public bool Check(string value) => new EmailAddressAttribute().IsValid(value);
    }
}
