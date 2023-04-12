using BlazorDatasource.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorDatasource.Server.Models.Extensions
{
    /// <summary>
    /// Represents model extensions
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// Convert list to paged list according to paging request
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="list">List of objects</param>
        /// <param name="pagingRequestModel">Paging request model</param>
        /// <returns>Paged list</returns>
        public static IPagedList<T> ToPagedList<T>(this IList<T> list, IPagingRequestModel pagingRequestModel)
        {
            return new PagedList<T>(list, pagingRequestModel.Page - 1, pagingRequestModel.PageSize);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static IPagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool getOnlyTotalCount = false)
        {
            if (source == null)
                return new PagedList<T>(new List<T>(), pageIndex, pageSize);

            //min allowed page size is 1
            pageSize = Math.Max(pageSize, 1);

            var count = source.Count();

            var data = new List<T>();

            if (!getOnlyTotalCount)
                data.AddRange(source.Skip(pageIndex * pageSize).Take(pageSize).ToList());

            return new PagedList<T>(data, pageIndex, pageSize, count);
        }

        /// <summary>
        /// Prepare passed list model to display in a grid
        /// </summary>
        /// <typeparam name="TListModel">List model type</typeparam>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <typeparam name="TObject">Object type</typeparam>
        /// <param name="listModel">List model</param>
        /// <param name="searchModel">Search model</param>
        /// <param name="objectList">Paged list of objects</param>
        /// <param name="dataFillFunction">Function to populate model data</param>
        /// <returns>List model</returns>
        public static TListModel PrepareToGrid<TListModel, TModel, TObject>(this TListModel listModel,
                                                                            IPagedList<TObject> objectList,
                                                                            Func<IEnumerable<TModel>> dataFillFunction)
            where TListModel : BasePagedListModel<TModel>
            where TModel : BaseModel
        {
            if (listModel == null)
                throw new ArgumentNullException(nameof(listModel));

            listModel.Data = dataFillFunction.Invoke();
            listModel.RecordsTotal = objectList?.TotalCount ?? 0;
            listModel.RecordsFiltered = objectList?.TotalCount ?? 0;

            return listModel;
        }
    }
}
