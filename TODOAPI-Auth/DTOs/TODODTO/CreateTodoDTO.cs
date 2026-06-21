using System.ComponentModel.DataAnnotations;

namespace TODOAPI_Auth.DTOs.TODODTO
{

    public class CreateTodoDTO : IValidatableObject
    {



        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = "Medium";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (string.IsNullOrEmpty(Title))
            {
                yield return new ValidationResult(
                    "Title is required",
                    new[] { nameof(Title) }
                );

            }

            if (string.IsNullOrWhiteSpace(Title))
            {
                yield return new ValidationResult(
                    "Title cannot contain only whitespace",
                    new[] { nameof(Title) }
                );

            }

            if (!string.IsNullOrWhiteSpace(Title) &&
                Title.Trim().Length < 3)
            {
                yield return new ValidationResult(
                    "Title must be at least 3 characters",
                    new[] { nameof(Title) }
                );
            }

            if (
                string.IsNullOrWhiteSpace(Description))
            {
                yield return new ValidationResult(
                    "Description cannot contain only whitespace",
                    new[] { nameof(Description) }
                );
            }

            var validPriorities = new[] { "Low", "Medium", "High" };
            if (!validPriorities.Contains(Priority))
            {
                yield return new ValidationResult(
                    "Priority must be Low, Medium, or High",
                    new[] { nameof(Priority) }
                );
            }
            if (DueDate.HasValue &&
                DueDate.Value.Date < DateTime.Today)
            {
                yield return new ValidationResult(
                    "DueDate cannot be in the past",
                    new[] { nameof(DueDate) }
                );
            }
        }




    }


}
