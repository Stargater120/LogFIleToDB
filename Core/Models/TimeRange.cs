using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    internal class TimeRange : IValidatableObject
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Begin <= End)
            {
                yield return new ValidationResult("End must be after Begin");
            }
        }
    }
}
