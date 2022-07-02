using Hero;
using System;
using System.ComponentModel.DataAnnotations;

namespace CI.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateRangeAttribute : ValidationAttribute
    {
        private const string DefaultMessageGreater = "Property '{0}' cannot be greater than property '{1}'";
        private const string DefaultMessageLowest = "Property '{0}' cannot be lowest than property '{1}'";

        private readonly string dependencyProperty;
        private readonly bool allowNull;
        private readonly bool lowestValidation;

        public DateRangeAttribute(string dependencyProperty, bool allowNull)
        : this(dependencyProperty, allowNull, false) { }

        public DateRangeAttribute(string dependencyProperty, bool allowNull, bool lowestValidation)
        {
            if (dependencyProperty.IsNullOrEmptyOrWhitespace())
            {
                dependencyProperty.ThrowIfNull("dependencyProperty");
            }

            this.dependencyProperty = dependencyProperty;
            this.allowNull = allowNull;
            this.lowestValidation = lowestValidation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult vr = ValidationResult.Success;
            if (dependencyProperty.IsNullOrEmptyOrWhitespace())
                return vr;

            Type dt = typeof(DateTime);

            if (value == null)
            {
                vr = new ValidationResult($"The {validationContext.MemberName} is Required", new[] { validationContext.MemberName });
            }
            else if (validationContext.ObjectType != dt)
            {
                vr = new ValidationResult($"Member of {validationContext.MemberName} is not valid type of DateTime");
            }
            else
            {
                var prop = validationContext.ObjectType.GetProperty(dependencyProperty, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
                if (prop == null)
                {
                    vr = new ValidationResult($"Failed to get relation property '{dependencyProperty}' from object '{validationContext.MemberName}'");
                }
                else
                {
                    var obj = prop.GetValue(validationContext.ObjectInstance);
                    if (obj == null)
                    {

                    }
                    //if (typeof(obj) != typeof(DateTime))
                    //    DateTime dt = ((DateTime)value);
                    //if (dt < minDate)
                    //{
                    //    if (errMsg.IsNullOrEmptyOrWhitespace())
                    //    {
                    //        vr = new ValidationResult(string.Format(DefaultMessage, validationContext.MemberName, dt, minDate), new[] { validationContext.MemberName });
                    //    }
                    //    else
                    //    {
                    //        vr = new ValidationResult(errMsg, new[] { validationContext.MemberName });
                    //    }
                    //}
                }
            }
            return vr;
        }
    }
}