using Siteminder.API.Entities;
using Siteminder.API.Models;
using Siteminder.API.Models.CustomerAccounts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Siteminder.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _companyPropertyMapping =
          new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
          {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Name" } )},
               { "Type", new PropertyMappingValue(new List<string>() { "Type" } )},
               { "Address1", new PropertyMappingValue(new List<string>() { "Address1" })},
               { "Address2", new PropertyMappingValue(new List<string>() { "Address2" })},
               { "City", new PropertyMappingValue(new List<string>() { "City" })},
               { "State", new PropertyMappingValue(new List<string>() { "State" })},
               { "ZipCode", new PropertyMappingValue(new List<string>() { "ZipCode" })}
          };

        private Dictionary<string, PropertyMappingValue> _siteTypePropertyMapping =
         new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
         {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Type", new PropertyMappingValue(new List<string>() { "Type" } )},
               { "Description", new PropertyMappingValue(new List<string>() { "Description" })}
         };

        private Dictionary<string, PropertyMappingValue> _sitePropertyMapping =
         new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
         {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "CompanyId", new PropertyMappingValue(new List<string>() { "CompanyId" } )},
               { "SiteTypeId", new PropertyMappingValue(new List<string>() { "SiteTypeId" } )},
               { "SiteName", new PropertyMappingValue(new List<string>() { "SiteName" } )},
               { "Address1", new PropertyMappingValue(new List<string>() { "Address1" })},
               { "Address2", new PropertyMappingValue(new List<string>() { "Address2" })},
               { "City", new PropertyMappingValue(new List<string>() { "City" })},
               { "State", new PropertyMappingValue(new List<string>() { "State" })},
               { "ZipCode", new PropertyMappingValue(new List<string>() { "ZipCode" })}
         };

        private Dictionary<string, PropertyMappingValue> _terminalPropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "TerminalName", new PropertyMappingValue(new List<string>() { "TerminalName" } )},
               { "SiteId", new PropertyMappingValue(new List<string>() { "SiteId" } )}
        };

        private Dictionary<string, PropertyMappingValue> _terminalSettingsPropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "TerminalId", new PropertyMappingValue(new List<string>() { "TerminalId" } )},
               { "ScheduleId", new PropertyMappingValue(new List<string>() { "ScheduleId" } )},
               { "PrintPvtAccountBalanceOnReceipt", new PropertyMappingValue(new List<string>() { "PrintPvtAccountBalanceOnReceipt" } )},
               { "BatchCloseTime ", new PropertyMappingValue(new List<string>() { "BatchCloseTime" } )},
               { "TimeZoneId", new PropertyMappingValue(new List<string>() { "TimeZoneId" } )},
               { "CCPlatform", new PropertyMappingValue(new List<string>() { "CCPlatform" } )},
               { "RejectUnregisteredNonPrivateCardUsers", new PropertyMappingValue(new List<string>() { "RejectUnregisteredNonPrivateCardUsers" } )},
               { "RejectUnregisteredPrivateCardUsers", new PropertyMappingValue(new List<string>() { "RejectUnregisteredPrivateCardUsers" } )},
               { "CummulativeRiskAmount", new PropertyMappingValue(new List<string>() { "CummulativeRiskAmount" } )},
               { "MaxiumumOfflineAmount", new PropertyMappingValue(new List<string>() { "MaxiumumOfflineAmount" } )},
               { "OfflineSalesOption", new PropertyMappingValue(new List<string>() { "OfflineSalesOption" } )},
               { "AmountOfFillupKey", new PropertyMappingValue(new List<string>() { "AmountOfFillupKey" } )},
               { "MaximumSaleAmount", new PropertyMappingValue(new List<string>() { "MaximumSaleAmount" } )},
               { "TaxTableId", new PropertyMappingValue(new List<string>() { "TaxTableId" } )},
               { "DisableFillupKey", new PropertyMappingValue(new List<string>() { "DisableFillupKey" } )},
               { "ProprietaryCardCode", new PropertyMappingValue(new List<string>() { "ProprietaryCardCode" } )},
               { "QTGatewayTerminalId", new PropertyMappingValue(new List<string>() { "QTGatewayTerminalId" } )},
               { "MSATerminalId", new PropertyMappingValue(new List<string>() { "MSATerminalId" } )},
               { "MSAUserName", new PropertyMappingValue(new List<string>() { "MSAUserName" } )},
               { "MSTSMerchantAccountID", new PropertyMappingValue(new List<string>() { "MSTSMerchantAccountID" } )},
               { "HeartlandCompanyId", new PropertyMappingValue(new List<string>() { "HeartlandCompanyId" } )},
               { "HeartlandDeviceId", new PropertyMappingValue(new List<string>() { "HeartlandDeviceId" } )},
               { "TargetNetworkEnvironment", new PropertyMappingValue(new List<string>() { "TargetNetworkEnvironment" } )},
               { "QTGatewayAccessCode", new PropertyMappingValue(new List<string>() { "QTGatewayAccessCode" } )},
               { "MSAMechantNumber", new PropertyMappingValue(new List<string>() { "MSAMechantNumber" } )},
               { "MSAPassword", new PropertyMappingValue(new List<string>() { "MSAPassword" } )},
               { "MSTSMerchantJobberId", new PropertyMappingValue(new List<string>() { "MSTSMerchantJobberId" } )},
               { "HeartlandTerminalLocationId", new PropertyMappingValue(new List<string>() { "HeartlandTerminalLocationId" } )},
               { "DPIPort", new PropertyMappingValue(new List<string>() { "DPIPort" } )},
               { "PIEPort", new PropertyMappingValue(new List<string>() { "PIEPort" } )},
               { "HVDPOrt", new PropertyMappingValue(new List<string>() { "HVDPOrt" } )},
               { "BarcodeReaderPort", new PropertyMappingValue(new List<string>() { "BarcodeReaderPort" } )},
               { "AutoTankGuaging", new PropertyMappingValue(new List<string>() { "AutoTankGuaging" } )},
               { "ATGPort", new PropertyMappingValue(new List<string>() { "ATGPort" } )},
               { "ATGBaudRate", new PropertyMappingValue(new List<string>() { "ATGBaudRate" } )},
               { "ATGParity", new PropertyMappingValue(new List<string>() { "ATGParity" } )},
               { "ATGStopBits", new PropertyMappingValue(new List<string>() { "ATGStopBits" } )},
               { "ATGDataBits", new PropertyMappingValue(new List<string>() { "ATGDataBits" } )},
               { "ATGHandshake", new PropertyMappingValue(new List<string>() { "ATGHandshake" } )},
               { "ATGSecurityCode", new PropertyMappingValue(new List<string>() { "ATGSecurityCode" } )},
               { "ForceRegistry", new PropertyMappingValue(new List<string>() { "ForceRegistry" } )},
               { "RegistryMinLength", new PropertyMappingValue(new List<string>() { "RegistryMinLength" } )},
               { "RegistryMaxLength", new PropertyMappingValue(new List<string>() { "RegistryMaxLength" } )},
               { "RegistryLabelAs", new PropertyMappingValue(new List<string>() { "RegistryLabelAs" } )}
        };

        private Dictionary<string, PropertyMappingValue> _contactPropertyMapping =
         new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
         {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Type", new PropertyMappingValue(new List<string>() { "Type" } )},
               { "Title", new PropertyMappingValue(new List<string>() { "Title" } )},
               { "CountryCode", new PropertyMappingValue(new List<string>() { "CountryCode" })},
               { "Phone", new PropertyMappingValue(new List<string>() { "Phone" })},
               { "Mobile", new PropertyMappingValue(new List<string>() { "Mobile" })},
               { "Fax", new PropertyMappingValue(new List<string>() { "Fax" })},
               { "Email", new PropertyMappingValue(new List<string>() { "Email" })},
               { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" })}


         };

        private Dictionary<string, PropertyMappingValue> _accountPropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Type", new PropertyMappingValue(new List<string>() { "Type" } )},
               { "Title", new PropertyMappingValue(new List<string>() { "Title" } )},
               { "CountryCode", new PropertyMappingValue(new List<string>() { "CountryCode" })},
               { "Phone", new PropertyMappingValue(new List<string>() { "Phone" })},
               { "Mobile", new PropertyMappingValue(new List<string>() { "Mobile" })},
               { "Fax", new PropertyMappingValue(new List<string>() { "Fax" })},
               { "Email", new PropertyMappingValue(new List<string>() { "Email" })},
               { "Name", new PropertyMappingValue(new List<string>() { "FirstName", "LastName" })}
        };

        private Dictionary<string, PropertyMappingValue> _fueltypePropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Name" } )},
               { "Description", new PropertyMappingValue(new List<string>() { "Description" })},
        };

        private Dictionary<string, PropertyMappingValue> _fuelPropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Name" } )},
               { "Description", new PropertyMappingValue(new List<string>() { "Description" })},
        };

        private Dictionary<string, PropertyMappingValue> _dispenserPropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Name" } )},
               { "DispenserId", new PropertyMappingValue(new List<string>() { "DispenserId" } )},
               { "DispenserType", new PropertyMappingValue(new List<string>() { "DispenserType" } )},
               { "HVDisplayNumber", new PropertyMappingValue(new List<string>() { "HVDisplayNumber" })},
               { "TotalizerReading", new PropertyMappingValue(new List<string>() { "TotalizerReading" })},
               { "MaxTotalizerDigits", new PropertyMappingValue(new List<string>() { "MaxTotalizerDigits" })},
               { "VolumeUnit", new PropertyMappingValue(new List<string>() { "VolumeUnit" })},
        };
        private Dictionary<string, PropertyMappingValue> _devicePropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "DeviceName", new PropertyMappingValue(new List<string>() { "DeviceName" } )},
               { "ModelName", new PropertyMappingValue(new List<string>() { "ModelName" } )},
               { "SerialNumber", new PropertyMappingValue(new List<string>() { "SerialNumber" })},
               { "Description", new PropertyMappingValue(new List<string>() { "Description" })},
        };

        private Dictionary<string, PropertyMappingValue> _cardTypePropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Name" } )},
               { "Description", new PropertyMappingValue(new List<string>() { "Description" })},
               { "IsPrivateAccount", new PropertyMappingValue(new List<string>() { "IsPrivateAccount" })},

        };

        private Dictionary<string, PropertyMappingValue> _userAccountPropertyMapping =
        new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
        {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "UserName", new PropertyMappingValue(new List<string>() { "UserName" } )},
               { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" })},
               { "LastName", new PropertyMappingValue(new List<string>() { "LastName" })},
               { "Email", new PropertyMappingValue(new List<string>() { "Email" })},
               { "Password", new PropertyMappingValue(new List<string>() { "Password" })},

        };

        private Dictionary<string, PropertyMappingValue> _customerAccountPropertyMapping =
       new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
       {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "AccountType", new PropertyMappingValue(new List<string>() { "AccountType" } )},
               { "PaymentType", new PropertyMappingValue(new List<string>() { "PaymentType" })},
               { "CreditLimit", new PropertyMappingValue(new List<string>() { "CreditLimit" })},
               { "CustomerName", new PropertyMappingValue(new List<string>() { "CustomerName" })},
               { "TailNumber", new PropertyMappingValue(new List<string>() { "TailNumber" })},
               { "AccountNumber", new PropertyMappingValue(new List<string>() { "AccountNumber" })},
               { "ExternalAccountReference", new PropertyMappingValue(new List<string>() { "ExternalAccountReference" })},
               { "Identifer", new PropertyMappingValue(new List<string>() { "Identifer" })},
               { "Balance", new PropertyMappingValue(new List<string>() { "Balance" })},
               { "AccountingCodeRequired", new PropertyMappingValue(new List<string>() { "AccountingCodeRequired" })},
               { "CardTypeID", new PropertyMappingValue(new List<string>() { "CardTypeID" })},
               { "EmbossedNumber", new PropertyMappingValue(new List<string>() { "EmbossedNumber" })},
               { "CardHolderName", new PropertyMappingValue(new List<string>() { "CardHolderName" })},
               { "Status", new PropertyMappingValue(new List<string>() { "Status" })},
               { "AccountName", new PropertyMappingValue(new List<string>() { "AccountName" })},
               { "ContactName", new PropertyMappingValue(new List<string>() { "ContactName" })},
               { "EmailAddress", new PropertyMappingValue(new List<string>() { "EmailAddress" })},
               { "Address1", new PropertyMappingValue(new List<string>() { "Address1" })},
               { "Address2", new PropertyMappingValue(new List<string>() { "Address2" })},
               { "City", new PropertyMappingValue(new List<string>() { "City" })},
               { "State", new PropertyMappingValue(new List<string>() { "State" })},
               { "ZipCode", new PropertyMappingValue(new List<string>() { "ZipCode" })},
               { "Phone", new PropertyMappingValue(new List<string>() { "Phone" })},
               { "Fax", new PropertyMappingValue(new List<string>() { "Fax" })},
               { "DiscountPerGallon", new PropertyMappingValue(new List<string>() { "DiscountPerGallon" })},
               { "PinCode", new PropertyMappingValue(new List<string>() { "PinCode" })},
               { "CompanyId", new PropertyMappingValue(new List<string>() { "CompanyId" })},
       };

        private Dictionary<string, PropertyMappingValue> _tankPropertyMapping =
       new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
       {
               { "Id", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Name", new PropertyMappingValue(new List<string>() { "Id" } ) },
               { "Description", new PropertyMappingValue(new List<string>() { "UserName" } )},
               { "Size", new PropertyMappingValue(new List<string>() { "FirstName" })},
               { "Level", new PropertyMappingValue(new List<string>() { "LastName" })},
               { "AlarmPoint", new PropertyMappingValue(new List<string>() { "Email" })},
               { "TerminalId", new PropertyMappingValue(new List<string>() { "Password" })},
               { "DispenserId", new PropertyMappingValue(new List<string>() { "Password" })},
               { "FuelTypeId", new PropertyMappingValue(new List<string>() { "Password" })},

       };

        private IList<IPropertyMapping> _companyPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _siteTypePropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _sitePropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _terminalPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _terminalSettingsPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _contactPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _accountPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _fueltypePropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _fuelPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _dispenserPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _devicePropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _cardTypePropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _userAccountPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _customerAccountPropertyMappings = new List<IPropertyMapping>();
        private IList<IPropertyMapping> _tankPropertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _companyPropertyMappings.Add(new PropertyMapping<CompanyDto, Company>(_companyPropertyMapping));
            _siteTypePropertyMappings.Add(new PropertyMapping<SiteTypeDto, SiteType>(_sitePropertyMapping));
            _sitePropertyMappings.Add(new PropertyMapping<SiteDto, Site>(_sitePropertyMapping));
            _terminalPropertyMappings.Add(new PropertyMapping<TerminalDto, Terminal>(_terminalPropertyMapping));
            _terminalSettingsPropertyMappings.Add(new PropertyMapping<TerminalSettingsDto, TerminalSettings>(_terminalSettingsPropertyMapping));
            _contactPropertyMappings.Add(new PropertyMapping<ContactDto, Contact>(_contactPropertyMapping));
            _accountPropertyMappings.Add(new PropertyMapping<CustomerAccountDto, Entities.CustomerAccount>(_accountPropertyMapping));
            _fueltypePropertyMappings.Add(new PropertyMapping<FuelTypeDto, FuelType>(_fueltypePropertyMapping));
            _fuelPropertyMappings.Add(new PropertyMapping<FuelDto, Fuel>(_fuelPropertyMapping));
            _dispenserPropertyMappings.Add(new PropertyMapping<DispenserDto, Dispenser>(_dispenserPropertyMapping));
            _devicePropertyMappings.Add(new PropertyMapping<DevicesDto, Device>(_devicePropertyMapping));
            _cardTypePropertyMappings.Add(new PropertyMapping<CardTypeDto, CardType>(_cardTypePropertyMapping));
            _userAccountPropertyMappings.Add(new PropertyMapping<UserAccountDto, UserAccount>(_userAccountPropertyMapping));
            _customerAccountPropertyMappings.Add(new PropertyMapping<CustomerAccountDto, Entities.CustomerAccount>(_customerAccountPropertyMapping));
            _tankPropertyMappings.Add(new PropertyMapping<CustomerAccountDto, Entities.Tank>(_tankPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetCompanyPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _companyPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetSiteTypePropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _siteTypePropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetSitePropertyMapping
         <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _sitePropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetTerminalPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _terminalPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetTerminalSettingsPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _terminalSettingsPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetContactPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _contactPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetDevicePropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _devicePropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetFuelTypePropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _fueltypePropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetFuelPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _fuelPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetDispenserPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _dispenserPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }
        public Dictionary<string, PropertyMappingValue> GetCardTypePropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _cardTypePropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }
        public Dictionary<string, PropertyMappingValue> GetUserAccountPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _userAccountPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }
        public Dictionary<string, PropertyMappingValue> GetCustomerAccountPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _customerAccountPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public Dictionary<string, PropertyMappingValue> GetTankPropertyMapping
        <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = _tankPropertyMappings
                .OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance " +
                $"for <{typeof(TSource)},{typeof(TDestination)}");
        }



        public bool ValidCompanyMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetCompanyPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidSiteTypeMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetSiteTypePropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidSiteMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetSitePropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidTerminalMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetTerminalPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidTerminalSettingsMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetTerminalSettingsPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidContactMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetContactPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidDeviceMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetDevicePropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidFuelTypeMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetFuelTypePropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidFuelMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetFuelPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidDispenserMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetDispenserPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidCardTypeMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetCardTypePropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidUserAccountMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetUserAccountPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidCustomerAccountMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetCustomerAccountPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

        public bool ValidTankMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetTankPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is seperated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();

                // remove everyhting after the first "," - if the fields
                // are coming from an orderBy string , this is part must be 
                // ignored
                var indexofFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexofFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexofFirstSpace);

                //find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
