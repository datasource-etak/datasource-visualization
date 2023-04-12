using BlazorDatasource.Server.Models.Dataset;
using System;

namespace BlazorDatasource.Server.Infrastructure
{
    public class StateContainer
    {
        protected DatasetModel? savedDatasetValue;

        public DatasetModel? Dataset
        {
            get => savedDatasetValue;
            set
            {
                savedDatasetValue = value;
                NotifyStateChanged();
            }
        }

        public event Action? OnChange;
        
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
