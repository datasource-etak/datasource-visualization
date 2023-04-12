using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlazorDatasource.Server.Shared
{
    public partial class FileUploadPrompt : ComponentBase
    {
        [Inject]
        protected IWebHostEnvironment Environment { get; set; } = default!;

        [Inject]
        protected ILogger<FileUploadPrompt> Logger { get; set; } = default!;

        /// <summary>
        /// Max allowed number of files
        /// </summary>
        [Parameter]
        public int MaxAllowedFiles { get; set; }

        /// <summary>
        /// Max file size
        /// </summary>
        [Parameter]
        public long MaxFileSize { get; set; } = 1024 * 1024 * 15;

        /// <summary>
        /// Delegate confirmation to parent.
        /// </summary>
        [Parameter]
        public EventCallback<(bool, List<UploadResult>)> Confirmation { get; set; }

        /// <summary>
        /// Confirmation.
        /// </summary>
        /// <param name="confirmed"><c>True</c> when confirmed.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task ConfirmAsync(bool confirmed)
        {
            await Confirmation.InvokeAsync((confirmed, UploadedFiles));
        }

        /// <summary>
        /// Prevent concurrent requests
        /// </summary>
        protected bool IsLoading;

        /// <summary>
        /// Progress
        /// </summary>
        protected decimal ProgressPercent;

        /// <summary>
        /// List of uploaded files
        /// </summary>
        protected List<UploadResult> UploadedFiles = new();

        /// <summary>
        /// Upload
        /// </summary>
        /// <param name="e">InputFileChangeEventArgs</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        protected async Task UploadFiles(InputFileChangeEventArgs e)
        {
            IsLoading = true;
            UploadedFiles.Clear();
            ProgressPercent = 0;

            foreach (var file in e.GetMultipleFiles(MaxAllowedFiles))
            {
                try
                {
                    var trustedFileName = Path.GetRandomFileName();
                    var path = Path.Combine(Environment.ContentRootPath,
                        Environment.EnvironmentName, "unsafe_uploads", trustedFileName);

                    using var readStream = file.OpenReadStream(MaxFileSize);                    
                    await using FileStream writeStream = new(path, FileMode.Create);
                    var bytesRead = 0;
                    var totalRead = 0;
                    var buffer = new byte[1024 * 1];

                    while ((bytesRead = await readStream.ReadAsync(buffer)) != 0)
                    {
                        totalRead += bytesRead;

                        await writeStream.WriteAsync(buffer.AsMemory(0,bytesRead));

                        ProgressPercent = decimal.Divide(totalRead, file.Size);

                        StateHasChanged();
                    }

                    var result = new UploadResult()
                    {                        
                        Uploaded = true,
                        FileName = file.Name,
                        StoredFileName = path,
                    };
                    UploadedFiles.Add(result);
                }
                catch (Exception ex)
                {
                    Logger.LogError("File: {Filename} Error: {Error}", file.Name, ex.Message);
                    var result = new UploadResult()
                    {
                        Uploaded = false,
                        FileName = file.Name,
                        ErrorCode = 6,
                        ErrorMessage = $"File: {file.Name} Error: {ex.Message}"
                    };
                    UploadedFiles.Add(result);
                }
            }

            IsLoading = false;
        }
    }

    public record UploadResult
    {
        /// <summary>
        /// True if success
        /// </summary>
        public bool Uploaded { get; set; }

        /// <summary>
        /// Original filename
        /// </summary>
        public string? FileName { get; set; }
        
        /// <summary>
        /// Stored filename
        /// </summary>
        public string? StoredFileName { get; set; }
        
        /// <summary>
        /// Errorcode
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
