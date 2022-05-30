using System.ComponentModel.DataAnnotations;

namespace Prode2022Server.Helpers
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UnlikeAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "El valor de {0} no puede ser igual al valor de {1}.";

        public string OtherProperty { get; private set; }

        public UnlikeAttribute(string otherProperty)
            : base(DefaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty))
            {
                throw new ArgumentNullException("otherProperty");
            }

            OtherProperty = otherProperty;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherProperty);
        }

        protected override ValidationResult? IsValid(object? value,
            ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectInstance.GetType()
                    .GetProperty(OtherProperty);
                if (otherProperty != null)
                {
                    var otherPropertyValue = otherProperty
                        .GetValue(validationContext.ObjectInstance, null);

                    if (value.Equals(otherPropertyValue))
                    {
                        return new ValidationResult(
                            FormatErrorMessage(validationContext.DisplayName));
                    }
                }
            }

            return ValidationResult.Success!;
        }
    }
}
