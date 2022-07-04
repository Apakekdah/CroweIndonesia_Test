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
        private readonly bool lowestValidation;

        public DateRangeAttribute(string dependencyProperty)
        : this(dependencyProperty, true) { }

        public DateRangeAttribute(string dependencyProperty, bool lowestValidation)
        {
            if (dependencyProperty.IsNullOrEmptyOrWhitespace())
            {
                dependencyProperty.ThrowIfNull("dependencyProperty");
            }

            this.dependencyProperty = dependencyProperty;
            this.lowestValidation = lowestValidation;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult vr = ValidationResult.Success;
            if (dependencyProperty.IsNullOrEmptyOrWhitespace())
                return vr;

            if (value.IsNull())
            {
                vr = new ValidationResult($"Property {validationContext.DisplayName} value cannot be null.", new[] { validationContext.MemberName });
                return vr;
            }

            var dt = typeof(DateTime);
            var dateFrom = (DateTime)value;

            var prop = validationContext.ObjectType.GetProperty(validationContext.DisplayName);
            if (prop.IsNull())
            {
                vr = new ValidationResult($"Property {validationContext.DisplayName} not found in {validationContext.ObjectType.Name}", new[] { validationContext.MemberName });
            }
            else if (value.IsNull())
            {
                vr = new ValidationResult($"The {validationContext.MemberName} is Required", new[] { validationContext.MemberName });
            }
            else if (prop.PropertyType != dt)
            {
                vr = new ValidationResult($"Member of {validationContext.MemberName} is not valid type of DateTime");
            }
            else
            {
                prop = validationContext.ObjectType.GetProperty(dependencyProperty, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty);
                if (prop.IsNull())
                {
                    vr = new ValidationResult($"Failed to get relation property '{dependencyProperty}' from object '{validationContext.ObjectType.Name}'");
                }
                else if (prop.PropertyType != dt)
                {
                    vr = new ValidationResult($"Member of relation {dependencyProperty} is not valid type of DateTime");
                }
                else
                {
                    var dateTo = (DateTime)prop.GetValue(validationContext.ObjectInstance);
                    if (lowestValidation)
                    {
                        if (dateFrom > dateTo)
                        {
                            vr = new ValidationResult(string.Format(DefaultMessageLowest, validationContext.MemberName, dependencyProperty), new[] { validationContext.MemberName, dependencyProperty });
                        }
                    }
                    else if (dateFrom < dateTo)
                    {
                        vr = new ValidationResult(string.Format(DefaultMessageGreater, validationContext.MemberName, dependencyProperty), new[] { validationContext.MemberName, dependencyProperty });
                    }
                }
            }
            return vr;
        }
    }
}