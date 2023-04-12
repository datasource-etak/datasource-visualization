using BlazorDatasource.Shared.Domain.Localization;
using System.Threading.Tasks;

namespace BlazorDatasource.Shared
{
    /// <summary>
    /// Represents work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<Language> GetWorkingLanguageAsync();

        /// <summary>
        /// Gets current user working language
        /// </summary>
        /// <returns>Language</returns>
        Language GetWorkingLanguage();
    }
}
