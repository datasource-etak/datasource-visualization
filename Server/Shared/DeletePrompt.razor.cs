using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Shared
{
    public partial class DeletePrompt : ComponentBase
    {
        /// <summary>
        /// Delegate confirmation to parent.
        /// </summary>
        [Parameter]
        public EventCallback<bool> Confirmation { get; set; }

        /// <summary>
        /// Confirmation.
        /// </summary>
        /// <param name="confirmed"><c>True</c> when confirmed.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ConfirmAsync(bool confirmed)
        {
            await Confirmation.InvokeAsync(confirmed);
        }
    }
}
