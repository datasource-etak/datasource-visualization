using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;

namespace BlazorDatasource.Server.Shared
{
    public partial class FluentValidationSummary : ComponentBase
    {
        /// <summary>
        /// Validation message store
        /// </summary>
        protected ValidationMessageStore? messageStore;

        [CascadingParameter]
        private EditContext? CurrentEditContext { get; set; }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected override void OnInitialized()
        {
            if (CurrentEditContext is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(FluentValidationSummary)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(FluentValidationSummary)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            messageStore = new(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
                messageStore?.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) =>
                messageStore?.Clear(e.FieldIdentifier);
        }

        /// <summary>
        /// Display errors
        /// </summary>
        /// <param name="errors">Errors to display</param>
        public void DisplayErrors(Dictionary<string, string> errors)
        {
            if (CurrentEditContext is not null)
            {
                foreach (var err in errors)
                {
                    messageStore?.Add(CurrentEditContext.Field(err.Key), err.Value);
                }

                CurrentEditContext.NotifyValidationStateChanged();
            }
        }

        /// <summary>
        /// Clear all errors
        /// </summary>
        public void ClearErrors()
        {
            messageStore?.Clear();
            CurrentEditContext?.NotifyValidationStateChanged();
        }
    }
}
