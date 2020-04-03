using System.Collections.Generic;

namespace Siteminder.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetCompanyPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetSiteTypePropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetSitePropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetTerminalPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetTerminalSettingsPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetContactPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetDevicePropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetFuelTypePropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetFuelPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetDispenserPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetCardTypePropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetUserAccountPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetCustomerAccountPropertyMapping<TSource, TDestination>();
        Dictionary<string, PropertyMappingValue> GetTankPropertyMapping<TSource, TDestination>();

        bool ValidCompanyMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidSiteTypeMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidSiteMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidTerminalMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidTerminalSettingsMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidContactMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidDeviceMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidFuelTypeMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidFuelMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidDispenserMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidCardTypeMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidUserAccountMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidCustomerAccountMappingExistsFor<TSource, TDestination>(string fields);
        bool ValidTankMappingExistsFor<TSource, TDestination>(string fields);
    }
}

