using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BlazorDatasource.Server.Validators
{
    public static class ValidationExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
        }

        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState, string prefix)
        {
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    string key = string.IsNullOrEmpty(prefix)
                        ? error.PropertyName
                        : string.IsNullOrEmpty(error.PropertyName)
                            ? prefix
                            : prefix + "." + error.PropertyName;
                    modelState.AddModelError(key, error.ErrorMessage);
                }
            }
        }
    }
}
