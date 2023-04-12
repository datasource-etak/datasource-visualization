using BlazorDatasource.Shared.Domain;
using BlazorDatasource.Shared.Domain.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace BlazorDatasource.Shared.Services.Localization
{
    /// <summary>
    /// Localization manager
    /// </summary>
    public partial class LocalizationService : ILocalizationService
    {
        #region Fields

        protected readonly ILanguageService _languageService;
        protected readonly ILocalizedEntityService _localizedEntityService;
        protected readonly IRepository<LocaleStringResource> _lsrRepository;
        protected readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public LocalizationService(ILanguageService languageService,
                                   ILocalizedEntityService localizedEntityService,
                                   IRepository<LocaleStringResource> lsrRepository,
                                   IWorkContext workContext)
        {
            _languageService = languageService;
            _localizedEntityService = localizedEntityService;
            _lsrRepository = lsrRepository;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resources
        /// </returns>
        protected virtual async Task<IList<LocaleStringResource>> GetAllResourcesAsync(int languageId)
        {
            var locales = await _lsrRepository.GetAllAsync(query =>
            {
                return from l in query
                       orderby l.ResourceName
                       where l.LanguageId == languageId
                       select l;
            });

            return locales;
        }

        protected virtual HashSet<(string name, string value)> LoadLocaleResourcesFromStream(StreamReader xmlStreamReader, string language)
        {
            var result = new HashSet<(string name, string value)>();

            using (var xmlReader = XmlReader.Create(xmlStreamReader))
                while (xmlReader.ReadToFollowing("Language"))
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    using var languageReader = xmlReader.ReadSubtree();
                    while (languageReader.ReadToFollowing("LocaleResource"))
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.GetAttribute("Name") is string name)
                        {
                            using var lrReader = languageReader.ReadSubtree();
                            if (lrReader.ReadToFollowing("Value") && lrReader.NodeType == XmlNodeType.Element)
                                result.Add((name.ToLowerInvariant(), lrReader.ReadString()));
                        }

                    break;
                }

            return result;
        }

        protected virtual Dictionary<string, KeyValuePair<int, string>> ResourceValuesToDictionary(IEnumerable<LocaleStringResource> locales)
        {
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var locale in locales)
            {
                var resourceName = locale.ResourceName.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceValue));
            }

            return dictionary;
        }

        protected virtual async Task<IDictionary<string, string>> UpdateLocaleResourceAsync(IDictionary<string, string> resources, int? languageId = null, bool clearCache = true)
        {
            var localResources = new Dictionary<string, string>(resources, StringComparer.InvariantCultureIgnoreCase);
            var keys = localResources.Keys.Select(key => key.ToLowerInvariant()).ToArray();
            var resourcesToUpdate = await _lsrRepository.GetAllAsync(query =>
            {
                var rez = query.Where(p => !languageId.HasValue || p.LanguageId == languageId)
                    .Where(p => keys.Contains(p.ResourceName.ToLower()));

                return rez;
            });

            var existsResources = new List<string>();

            foreach (var localeStringResource in resourcesToUpdate.ToList())
            {
                var newValue = localResources[localeStringResource.ResourceName];

                if (localeStringResource.ResourceValue.Equals(newValue))
                    resourcesToUpdate.Remove(localeStringResource);

                localeStringResource.ResourceValue = newValue;
                existsResources.Add(localeStringResource.ResourceName);
            }

            await _lsrRepository.UpdateAsync(resourcesToUpdate);

            return localResources.Where(item => !existsResources.Contains(item.Key, StringComparer.InvariantCultureIgnoreCase))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        protected virtual IDictionary<string, string> UpdateLocaleResource(IDictionary<string, string> resources, int? languageId = null, bool clearCache = true)
        {
            var localResources = new Dictionary<string, string>(resources, StringComparer.InvariantCultureIgnoreCase);
            var keys = localResources.Keys.Select(key => key.ToLowerInvariant()).ToArray();
            var resourcesToUpdate = _lsrRepository.GetAll(query =>
            {
                var rez = query.Where(p => !languageId.HasValue || p.LanguageId == languageId)
                    .Where(p => keys.Contains(p.ResourceName.ToLower()));

                return rez;
            });

            var existsResources = new List<string>();

            foreach (var localeStringResource in resourcesToUpdate.ToList())
            {
                var newValue = localResources[localeStringResource.ResourceName];

                if (localeStringResource.ResourceValue.Equals(newValue))
                    resourcesToUpdate.Remove(localeStringResource);

                localeStringResource.ResourceValue = newValue;
                existsResources.Add(localeStringResource.ResourceName);
            }

            _lsrRepository.Update(resourcesToUpdate);

            return localResources.Where(item => !existsResources.Contains(item.Key, StringComparer.InvariantCultureIgnoreCase))
                .ToDictionary(p => p.Key, p => p.Value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.DeleteAsync(localeStringResource);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="localeStringResourceId">Locale string resource identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resource
        /// </returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByIdAsync(int localeStringResourceId)
        {
            return await _lsrRepository.GetByIdAsync(localeStringResourceId);
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resource
        /// </returns>
        public virtual async Task<LocaleStringResource> GetLocaleStringResourceByNameAsync(string resourceName,
                                                                                           int languageId)
        {
            var localeStringResources = await _lsrRepository.GetAllAsync(query =>
            {
                query = query.Where(lsr => lsr.LanguageId == languageId && lsr.ResourceName == resourceName.ToLowerInvariant());
                query = query.OrderBy(lsr => lsr.ResourceName);

                return query;
            });

            var localeStringResource = localeStringResources.FirstOrDefault();

            if (localeStringResource == null && LocalizationDefaults.CreateLocaleStringResourceIfNotFound)
            {
                //save at the database with try catch
                try
                {
                    resourceName = resourceName.Trim().ToLowerInvariant();

                    var autoCreateLocaleStringResource = new LocaleStringResource()
                    {
                        LanguageId = languageId,
                        ResourceName = resourceName,
                        ResourceValue = resourceName[(resourceName.LastIndexOf('.') + 1)..]
                    };

                    await InsertLocaleStringResourceAsync(autoCreateLocaleStringResource);

                    localeStringResource = autoCreateLocaleStringResource;
                }
                catch (Exception)
                {
                }
            }

            return localeStringResource;
        }

        /// <summary>
        /// Gets a locale string resource
        /// </summary>
        /// <param name="resourceName">A string representing a resource name</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// The locale string resource
        /// </returns>
        public virtual LocaleStringResource GetLocaleStringResourceByName(string resourceName,
                                                                          int languageId)
        {
            var localeStringResources = _lsrRepository.GetAll(query =>
            {
                query = query.Where(lsr => lsr.LanguageId == languageId && lsr.ResourceName == resourceName.ToLowerInvariant());
                query = query.OrderBy(lsr => lsr.ResourceName);

                return query;
            });

            var localeStringResource = localeStringResources.FirstOrDefault();

            if (localeStringResource == null && LocalizationDefaults.CreateLocaleStringResourceIfNotFound)
            {
                //save at the database with try catch
                try
                {
                    resourceName = resourceName.Trim().ToLowerInvariant();

                    var autoCreateLocaleStringResource = new LocaleStringResource()
                    {
                        LanguageId = languageId,
                        ResourceName = resourceName,
                        ResourceValue = resourceName[(resourceName.LastIndexOf('.') + 1)..]
                    };

                    _lsrRepository.Insert(autoCreateLocaleStringResource);

                    localeStringResource = autoCreateLocaleStringResource;
                }
                catch (Exception)
                {
                }
            }

            return localeStringResource;
        }

        /// <summary>
        /// Inserts a locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            if (!string.IsNullOrEmpty(localeStringResource?.ResourceName))
                localeStringResource.ResourceName = localeStringResource.ResourceName.Trim().ToLowerInvariant();

            await _lsrRepository.InsertAsync(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateLocaleStringResourceAsync(LocaleStringResource localeStringResource)
        {
            await _lsrRepository.UpdateAsync(localeStringResource);
        }

        /// <summary>
        /// Updates the locale string resource
        /// </summary>
        /// <param name="localeStringResource">Locale string resource</param>
        public virtual void UpdateLocaleStringResource(LocaleStringResource localeStringResource)
        {
            _lsrRepository.Update(localeStringResource);
        }

        /// <summary>
        /// Gets all locale string resources by language identifier
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the locale string resources
        /// </returns>
        public virtual async Task<Dictionary<string, KeyValuePair<int, string>>> GetAllResourceValuesAsync(int languageId)
        {
            //we use no tracking here for performance optimization
            //anyway records are loaded only for read-only operations
            var query = await _lsrRepository.GetAllAsync(query =>
            {
                query = query.Where(lsr => lsr.LanguageId == languageId);
                query = query.OrderBy(lsr => lsr.ResourceName);
                return query;
            });

            return ResourceValuesToDictionary(query);
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a string representing the requested resource string.
        /// </returns>
        public virtual async Task<string> GetResourceAsync(string resourceKey)
        {
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

            if (workingLanguage != null)
                return await GetResourceAsync(resourceKey, workingLanguage.Id);

            return string.Empty;
        }

        /// <summary>
        /// Gets a resource string based on the specified ResourceKey property.
        /// </summary>
        /// <param name="resourceKey">A string representing a ResourceKey.</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="returnEmptyIfNotFound">A value indicating whether an empty string will be returned if a resource is not found and default value is set to empty string</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains a string representing the requested resource string.
        /// </returns>
        public virtual async Task<string> GetResourceAsync(string resourceKey, int languageId, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            var result = string.Empty;
            resourceKey ??= string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();

            //gradual loading
            var query = await _lsrRepository.GetAllAsync(query =>
            {
                query = query.Where(lsr => lsr.ResourceName == resourceKey && lsr.LanguageId == languageId);
                return query;
            });

            var lsr = query.FirstOrDefault();

            if (lsr != null)
                result = lsr.ResourceValue;

            if (!string.IsNullOrEmpty(result))
                return result;

            if (LocalizationDefaults.CreateLocaleStringResourceIfNotFound)
            {
                //save at the database with try catch
                try
                {
                    var autoCreateLocaleStringResource = new LocaleStringResource()
                    {
                        LanguageId = languageId,
                        ResourceName = resourceKey,
                        ResourceValue = resourceKey[(resourceKey.LastIndexOf('.') + 1)..]
                    };

                    _lsrRepository.Insert(autoCreateLocaleStringResource);

                    result = autoCreateLocaleStringResource.ResourceValue;
                }
                catch (Exception)
                {
                }
            }

            if (!string.IsNullOrEmpty(defaultValue))
            {
                result = defaultValue;
            }
            else
            {
                if (!returnEmptyIfNotFound)
                    result = resourceKey;
            }

            return result;
        }

        /// <summary>
        /// Export language resources to XML
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result in XML format
        /// </returns>
        public virtual async Task<string> ExportResourcesToXmlAsync(Language language)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            var settings = new XmlWriterSettings
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                ConformanceLevel = ConformanceLevel.Auto
            };

            await using var stream = new MemoryStream();
            await using var xmlWriter = XmlWriter.Create(stream, settings);

            await xmlWriter.WriteStartDocumentAsync();
            await xmlWriter.WriteStartElementAsync(prefix: null, localName: "Language", ns: null);
            await xmlWriter.WriteAttributeStringAsync(prefix: null, localName: "Name", ns: null, value: language.Name);

            var resources = await GetAllResourcesAsync(language.Id);
            foreach (var resource in resources)
            {
                await xmlWriter.WriteStartElementAsync(prefix: null, localName: "LocaleResource", ns: null);
                await xmlWriter.WriteAttributeStringAsync(prefix: null, localName: "Name", ns: null, value: resource.ResourceName);
                await xmlWriter.WriteElementStringAsync(prefix:null,localName:"Value",ns:null, resource.ResourceValue);
                await xmlWriter.WriteEndElementAsync();
            }

            await xmlWriter.WriteEndElementAsync();
            await xmlWriter.WriteEndDocumentAsync();
            await xmlWriter.FlushAsync();

            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>
        /// Import language resources from XML file
        /// </summary>
        /// <param name="language">Language</param>
        /// <param name="xmlStreamReader">Stream reader of XML file</param>
        /// <param name="updateExistingResources">A value indicating whether to update existing resources</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ImportResourcesFromXmlAsync(Language language, StreamReader xmlStreamReader, bool updateExistingResources = true)
        {
            if (language == null)
                throw new ArgumentNullException(nameof(language));

            if (xmlStreamReader.EndOfStream)
                return;

            var lsNamesList = new Dictionary<string, LocaleStringResource>();

            foreach (var localeStringResource in _lsrRepository.GetAll().Where(lsr => lsr.LanguageId == language.Id)
                .OrderBy(lsr => lsr.Id))
                lsNamesList[localeStringResource.ResourceName.ToLowerInvariant()] = localeStringResource;

            var lsrToUpdateList = new List<LocaleStringResource>();
            var lsrToInsertList = new Dictionary<string, LocaleStringResource>();

            foreach (var (name, value) in LoadLocaleResourcesFromStream(xmlStreamReader, language.Name))
            {
                if (lsNamesList.ContainsKey(name))
                {
                    if (!updateExistingResources)
                        continue;

                    var lsr = lsNamesList[name];
                    lsr.ResourceValue = value;
                    lsrToUpdateList.Add(lsr);
                }
                else
                {
                    var lsr = new LocaleStringResource { LanguageId = language.Id, ResourceName = name, ResourceValue = value };
                    lsrToInsertList[name] = lsr;
                }
            }

            await _lsrRepository.UpdateAsync(lsrToUpdateList);
            await _lsrRepository.InsertAsync(lsrToInsertList.Values.ToList());
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language; pass 0 to get standard language value</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized property
        /// </returns>
        public virtual async Task<TPropType> GetLocalizedAsync<TEntity, TPropType>(TEntity entity,
                                                                                   Expression<Func<TEntity, TPropType>> keySelector,
                                                                                   int? languageId = null,
                                                                                   bool returnDefaultValue = true,
                                                                                   bool ensureTwoPublishedLanguages = true)
            where TEntity : BaseEntity, ILocalizedEntity
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (keySelector.Body is not MemberExpression member)
                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

            if (member.Member is not PropertyInfo propInfo)
                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

            var result = default(TPropType);
            var resultStr = string.Empty;

            var localeKeyGroup = entity.GetType().Name;
            var localeKey = propInfo.Name;

            var workingLanguage = await _workContext.GetWorkingLanguageAsync();

            if (!languageId.HasValue)
                languageId = workingLanguage.Id;

            if (languageId > 0)
            {
                //ensure that we have at least two published languages
                var loadLocalizedValue = true;
                if (ensureTwoPublishedLanguages)
                {
                    var totalPublishedLanguages = (await _languageService.GetAllLanguagesAsync()).Count;
                    loadLocalizedValue = totalPublishedLanguages >= 2;
                }

                //localized value
                if (loadLocalizedValue)
                {
                    resultStr = await _localizedEntityService
                        .GetLocalizedValueAsync(languageId.Value, entity.Id, localeKeyGroup, localeKey);
                    if (!string.IsNullOrEmpty(resultStr))
                        result = CommonHelper.To<TPropType>(resultStr);
                }
            }

            //set default value if required
            if (!string.IsNullOrEmpty(resultStr) || !returnDefaultValue)
                return result;
            var localizer = keySelector.Compile();
            result = localizer(entity);

            return result;
        }

        /// <summary>
        /// Get localized value of enum
        /// </summary>
        /// <typeparam name="TEnum">Enum type</typeparam>
        /// <param name="enumValue">Enum value</param>
        /// <param name="languageId">Language identifier; pass null to use the current working language</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the localized value
        /// </returns>
        public virtual async Task<string> GetLocalizedEnumAsync<TEnum>(TEnum enumValue, int? languageId = null) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("T must be an enumerated type");

            //localized value
            var workingLanguage = await _workContext.GetWorkingLanguageAsync();
            var resourceName = $"{LocalizationDefaults.EnumLocaleStringResourcesPrefix}{typeof(TEnum)}.{enumValue}";
            var result = await GetResourceAsync(resourceName, languageId ?? workingLanguage.Id, string.Empty, true);

            //set default value if required
            if (string.IsNullOrEmpty(result))
                result = CommonHelper.ConvertEnum(enumValue.ToString());

            return result;
        }

        /// <summary>
        /// Add a locale resource (if new) or update an existing one
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <param name="resourceValue">Resource value</param>
        /// <param name="languageCulture">Language culture code. If null or empty, then a resource will be added for all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrUpdateLocaleResourceAsync(string resourceName, string resourceValue, string languageCulture = null)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                if (!string.IsNullOrEmpty(languageCulture) && !languageCulture.Equals(lang.LanguageCulture))
                    continue;

                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id);
                if (lsr == null)
                {
                    lsr = new LocaleStringResource
                    {
                        LanguageId = lang.Id,
                        ResourceName = resourceName,
                        ResourceValue = resourceValue
                    };
                    await InsertLocaleStringResourceAsync(lsr);
                }
                else
                {
                    lsr.ResourceValue = resourceValue;
                    await UpdateLocaleStringResourceAsync(lsr);
                }
            }
        }

        /// <summary>
        /// Add locale resources
        /// </summary>
        /// <param name="resources">Resource name-value pairs</param>
        /// <param name="languageId">Language identifier; pass null to add the passed resources for all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task AddOrUpdateLocaleResourceAsync(IDictionary<string, string> resources, int? languageId = null)
        {
            //first update all previous locales with the passed names if they exist
            var resourcesToInsert = await UpdateLocaleResourceAsync(resources, languageId, false);

            if (resourcesToInsert.Any())
            {
                //insert new locale resources
                var locales = (await _languageService.GetAllLanguagesAsync(true))
                    .Where(language => !languageId.HasValue || language.Id == languageId.Value)
                    .SelectMany(language => resourcesToInsert.Select(resource => new LocaleStringResource
                    {
                        LanguageId = language.Id,
                        ResourceName = resource.Key.Trim().ToLowerInvariant(),
                        ResourceValue = resource.Value
                    }))
                    .ToList();

                await _lsrRepository.InsertAsync(locales);
            }
        }

        /// <summary>
        /// Add locale resources
        /// </summary>
        /// <param name="resources">Resource name-value pairs</param>
        /// <param name="languageId">Language identifier; pass null to add the passed resources for all languages</param>
        public virtual void AddOrUpdateLocaleResource(IDictionary<string, string> resources, int? languageId = null)
        {
            //first update all previous locales with the passed names if they exist
            var resourcesToInsert = UpdateLocaleResource(resources, languageId, false);

            if (resourcesToInsert.Any())
            {
                //insert new locale resources
                var locales = _languageService.GetAllLanguages(true)
                    .Where(language => !languageId.HasValue || language.Id == languageId.Value)
                    .SelectMany(language => resourcesToInsert.Select(resource => new LocaleStringResource
                    {
                        LanguageId = language.Id,
                        ResourceName = resource.Key.Trim().ToLowerInvariant(),
                        ResourceValue = resource.Value
                    }))
                    .ToList();

                _lsrRepository.Insert(locales);
            }

        }

        /// <summary>
        /// Delete a locale resource
        /// </summary>
        /// <param name="resourceName">Resource name</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourceAsync(string resourceName)
        {
            foreach (var lang in await _languageService.GetAllLanguagesAsync(true))
            {
                var lsr = await GetLocaleStringResourceByNameAsync(resourceName, lang.Id);
                if (lsr != null)
                    await DeleteLocaleStringResourceAsync(lsr);
            }
        }

        /// <summary>
        /// Delete locale resources
        /// </summary>
        /// <param name="resourceNames">Resource names</param>
        /// <param name="languageId">Language identifier; pass null to delete the passed resources from all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourcesAsync(IList<string> resourceNames, int? languageId = null)
        {
            var lsrsToDelete = await _lsrRepository.GetAllAsync(query =>
            {
                query = query.Where(locale => (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
                                    resourceNames.Contains(locale.ResourceName, StringComparer.InvariantCultureIgnoreCase));
                return query;
            });

            await _lsrRepository.DeleteAsync(lsrsToDelete);
        }

        /// <summary>
        /// Delete locale resources
        /// </summary>
        /// <param name="resourceNames">Resource names</param>
        /// <param name="languageId">Language identifier; pass null to delete the passed resources from all languages</param>
        public virtual void DeleteLocaleResources(IList<string> resourceNames, int? languageId = null)
        {
            var lsrsToDelete = _lsrRepository.GetAll(query =>
            {
                query = query.Where(locale => (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
                                              resourceNames.Contains(locale.ResourceName, StringComparer.InvariantCultureIgnoreCase));
                return query;
            });

            _lsrRepository.Delete(lsrsToDelete);
        }

        /// <summary>
        /// Delete locale resources by the passed name prefix
        /// </summary>
        /// <param name="resourceNamePrefix">Resource name prefix</param>
        /// <param name="languageId">Language identifier; pass null to delete resources by prefix from all languages</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteLocaleResourcesAsync(string resourceNamePrefix, int? languageId = null)
        {
            var lsrsToDelete = await _lsrRepository.GetAllAsync(query =>
            {
                query = query.Where(locale => (!languageId.HasValue || locale.LanguageId == languageId.Value) &&
                                              !string.IsNullOrEmpty(locale.ResourceName) &&
                                              locale.ResourceName.StartsWith(resourceNamePrefix, StringComparison.InvariantCultureIgnoreCase));
                return query;
            });

            await _lsrRepository.DeleteAsync(lsrsToDelete);
        }

        #endregion
    }
}
