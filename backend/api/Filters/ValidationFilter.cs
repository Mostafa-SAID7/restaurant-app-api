using FakeRestuarantAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FakeRestuarantAPI.Filters;

/// <summary>
/// Action filter to validate model state and return standardized validation errors
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            context.Result = ResponseHelper.ValidationError(errors);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}

/// <summary>
/// Attribute to apply validation filter to controllers or actions
/// </summary>
public class ValidateModelAttribute : TypeFilterAttribute
{
    public ValidateModelAttribute() : base(typeof(ValidationFilter))
    {
    }
}