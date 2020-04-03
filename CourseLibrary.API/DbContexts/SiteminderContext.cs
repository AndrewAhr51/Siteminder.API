using Siteminder.API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Siteminder.API.DbContexts
{
    public class SiteminderContext : DbContext
    {
        public SiteminderContext(DbContextOptions<SiteminderContext> options)
           : base(options)
        {
        }
        public DbSet<Company> Company { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<Device> DeviceType { get; set; }
        public DbSet<Dispenser> Dispensers { get; set; }
        public DbSet<Site> Site { get; set; }
        public DbSet<SiteType> SiteType { get; set; }
        public DbSet<CompanySiteTypes> CompanySiteTypes { get; set; }
        public DbSet<ContactType> ContactType { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<FuelType> FuelType { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<Terminal> Terminal { get; set; }
        public DbSet<TerminalSettings> TerminalSettings { get; set; }
        public DbSet<CompanyContacts> CompanyContacts { get; set; }
        public DbSet<SiteContacts>SiteContacts { get; set; }
        public DbSet<TerminalContacts> TerminalContacts { get; set; }
        public DbSet<TerminalDispenserAssociation> TerminalDispenserTankAssociations { get; set; }
        public DbSet<DispenserTankAssociation> DispenserTankAssociations { get; set; }
        public DbSet<TankFuelAssociation> TankFuelAssociations { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Tank> Tanks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed the database with dummy data
            modelBuilder.Entity<Company>()
                .HasIndex(e => e.Name);

            modelBuilder.Entity<CardType>()
               .HasIndex(e => e.Name);

            modelBuilder.Entity<Company>()
               .HasData(
                new Company()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    Name = "Andrew's Fuel",
                    Type = "Airport",
                    Address1 = "101 Main Street",
                    Address2 = "",
                    City = "Los Angeles",
                    State = "CA",
                    ZipCode = "11111"
                },
                new Company()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b36"),
                    Name = "Danny's Fuel",
                    Type = "Airport",
                    Address1 = "201 Main Street",
                    Address2 = "",
                    City = "Oklahoma City",
                    State = "OK",
                    ZipCode = "22222"
                },
                new Company()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b37"),
                    Name = "Jacks Fuel",
                    Type = "Airport",
                    Address1 = "301 Main Street",
                    Address2 = "",
                    City = "Detroit",
                    State = "MI",
                    ZipCode = "33333"
                },
                new Company()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b38"),
                    Name = "Dan's Fuel",
                    Type = "Airport",
                    Address1 = "401 Main Street",
                    Address2 = "",
                    City = "Cleveland",
                    State = "OH",
                    ZipCode = "44444-4444"
                },
                new Company()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b39"),
                    Name = "Matt's Fuel",
                    Type = "Airport",
                    Address1 = "501 Main Street",
                    Address2 = "",
                    City = "Parsippany",
                    State = "NJ",
                    ZipCode = "55555-5555"
                },
                new Company()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b40"),
                    Name = "Greg's Fuel",
                    Type = "Airport",
                    Address1 = "601 Main Street",
                    Address2 = "",
                    City = "New York",
                    State = "NY",
                    ZipCode = "66666-66"
                },
                new Company()
                {
                    Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b41"),
                    Name = "Dallas' Fuel",
                    Type = "Airport",
                    Address1 = "701 Main Street",
                    Address2 = "",
                    City = "Boulder",
                    State = "CO",
                    ZipCode = "77777-77"
                },
                 new Company()
                 {
                     Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b42"),
                     Name = "Chris'' Fuel",
                     Type = "Airport",
                     Address1 = "801 Main Street",
                     Address2 = "",
                     City = "Port Jefferson",
                     State = "NY",
                     ZipCode = "11777"
                 }
            );
            modelBuilder.Entity<SiteType>().HasData(
                new SiteType()
                {
                    Id = Guid.Parse("e28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                    Type = "Airport",
                    Description = "Airport site type"
                },
                new SiteType()
                {
                    Id = Guid.Parse("e28888e9-2ba9-473a-a40f-e38cb54f9b36"),
                    Type = "Marina",
                    Description = "Marina site type"
                },
                new SiteType()
                {
                    Id = Guid.Parse("e28888e9-2ba9-473a-a40f-e38cb54f9b37"),
                    Type = "Taxi",
                    Description = "Taxi site type"
                }
            );

            modelBuilder.Entity<Site>().HasData(
                new Site()
                {
                    Id = Guid.Parse("4FACFA8E-0008-450B-8022-ABE7566DC50F"),
                    SiteName = "Port Jefferson Marina",
                    CompanyId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b42"),
                    SiteTypeId = Guid.Parse("e28888e9-2ba9-473a-a40f-e38cb54f9b36"),
                    Address1 = "110 Main Street",
                    Address2 = null,
                    City = "Port Jefferson",
                    State = "NY",
                    ZipCode = "11778"
                },
                new Site()
                {
                    Id = Guid.Parse("C45028A6-9887-481F-8167-79518DCCC886"),
                    CompanyId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b41"),
                    SiteTypeId = Guid.Parse("e28888e9-2ba9-473a-a40f-e38cb54f9b36"),
                    SiteName = "Denver Marina Marina",
                    Address1 = "110 Broadway",
                    Address2 = null,
                    City = "Denver",
                    State = "CO",
                    ZipCode = "80028"
                },
                new Site()
                {
                    Id = Guid.Parse("5F9D7F91-2A5D-4424-BF81-4EE0E0A6DBDE"),
                    SiteName = "Smithtown Marina",
                    CompanyId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b40"),
                    SiteTypeId = Guid.Parse("e28888e9-2ba9-473a-a40f-e38cb54f9b36"),
                    Address1 = "500 50 Acre Rd",
                    Address2 = null,
                    City = "Smithtown",
                    State = "NY",
                    ZipCode = "11787"
                }
           );

            modelBuilder.Entity<Terminal>().HasData(
                 new Terminal()
                 {
                     Id = Guid.Parse("08A028CB-4318-4555-9DEA-17EC0AD31B76"),
                     SiteId = Guid.Parse("C45028A6-9887-481F-8167-79518DCCC886"),
                     TerminalName = "Port Jefferson Terminal",
                 },
                 new Terminal()
                 {
                     Id = Guid.Parse("691F7447-AB56-4821-A193-46E6A055192F"),
                     SiteId = Guid.Parse("5F9D7F91-2A5D-4424-BF81-4EE0E0A6DBDE"),
                     TerminalName = "Smithtown Terminal",
                 }
            );

            var scheduleGUID = Guid.NewGuid();
            modelBuilder.Entity<Schedule>().HasData(
                new Schedule()
                {
                    Id = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"),
                    ScheduleName = "Weekly",
                    TerminalId = Guid.Parse("08A028CB-4318-4555-9DEA-17EC0AD31B76"),
                }
            );

            modelBuilder.Entity<ScheduleDetail>().HasData(
                new ScheduleDetail() { Id = Guid.NewGuid(), ScheduleId = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"), DayOfWeek = "Monday", OpenTime = Convert.ToDateTime("8:00AM"), CloseTime = Convert.ToDateTime("8:00PM") },
                new ScheduleDetail() { Id = Guid.NewGuid(), ScheduleId = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"), DayOfWeek = "Tuesday", OpenTime = Convert.ToDateTime("8:00AM"), CloseTime = Convert.ToDateTime("8:00PM") },
                new ScheduleDetail() { Id = Guid.NewGuid(), ScheduleId = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"), DayOfWeek = "Wednesday", OpenTime = Convert.ToDateTime("8:00AM"), CloseTime = Convert.ToDateTime("8:00PM") },
                new ScheduleDetail() { Id = Guid.NewGuid(), ScheduleId = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"), DayOfWeek = "Thursday", OpenTime = Convert.ToDateTime("8:00AM"), CloseTime = Convert.ToDateTime("8:00PM") },
                new ScheduleDetail() { Id = Guid.NewGuid(), ScheduleId = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"), DayOfWeek = "Friday", OpenTime = Convert.ToDateTime("8:00AM"), CloseTime = Convert.ToDateTime("8:00PM") },
                new ScheduleDetail() { Id = Guid.NewGuid(), ScheduleId = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"), DayOfWeek = "Saturday", OpenTime = Convert.ToDateTime("6:00AM"), CloseTime = Convert.ToDateTime("10:00PM") },
                new ScheduleDetail() { Id = Guid.NewGuid(), ScheduleId = Guid.Parse("352485AA-2436-4912-85E8-32D124374EB8"), DayOfWeek = "Sunday", OpenTime = Convert.ToDateTime("6:00AM"), CloseTime = Convert.ToDateTime("10:00PM") }
            );

            modelBuilder.Entity<TerminalSettings>().HasData(
                new TerminalSettings()
                {
                    Id = Guid.Parse("805774C4-8B08-4055-9EB9-25F8CCE4C1EE"),
                    PrintPvtAccountBalanceOnReceipt = false,
                    BatchCloseTime = DateTime.Today.AddDays(1),
                    TimeZoneId = "MST",
                    CCPlatform = "Phillips66",
                    RejectUnregisteredNonPrivateCardUsers = true,
                    RejectUnregisteredPrivateCardUsers = true,
                    CummulativeRiskAmount = 250.00m,
                    MaxiumumOfflineAmount = 250.00m,
                    OfflineSalesOption = true,
                    AmountOfFillupKey = 500.00m,
                    MaximumSaleAmount = 500.00m,
                    TaxTableId = "1",
                    DisableFillupKey = false,
                    TerminalId = Guid.Parse("08A028CB-4318-4555-9DEA-17EC0AD31B76"),
                    ScheduleId = scheduleGUID,
                },
                new TerminalSettings()
                {
                    Id = Guid.Parse("B03A284E-DF40-42AA-9BE8-B1F91C23D3CC"),
                    PrintPvtAccountBalanceOnReceipt = false,
                    BatchCloseTime = DateTime.Today.AddDays(1),
                    TimeZoneId = "MST",
                    CCPlatform = "Phillips66",
                    RejectUnregisteredNonPrivateCardUsers = true,
                    RejectUnregisteredPrivateCardUsers = true,
                    CummulativeRiskAmount = 250.00m,
                    MaxiumumOfflineAmount = 250.00m,
                    OfflineSalesOption = true,
                    AmountOfFillupKey = 500.00m,
                    MaximumSaleAmount = 500.00m,
                    TaxTableId = "1",
                    DisableFillupKey = false,
                    TerminalId = Guid.Parse("691F7447-AB56-4821-A193-46E6A055192F"),
                    ScheduleId = scheduleGUID,
                }
             );
            modelBuilder.Entity<ContactType>().HasData(
                 new ContactType()
                 {
                     Id = Guid.Parse("AB406AFC-4B10-4D9C-B2D6-13791AF12AF5"),
                     Type = "Company",
                     Description = "Company contact type"
                 },
                new ContactType()
                {
                    Id = Guid.Parse("5C80C4C0-9100-4972-8D93-958C4BF101FF"),
                    Type = "Site",
                    Description = "Site contact type"
                },
                new ContactType()
                {
                    Id = Guid.Parse("43E40F5E-698B-4621-AD07-6D73CEA9C624"),
                    Type = "Terminal",
                    Description = "Site contact type"
                }
          );
            modelBuilder.Entity<CardType>().HasData(
                   new CardType()
                   {
                       Id = Guid.Parse("4E173D24-3094-4853-A8ED-01C54068C8D2"),
                       Name = "Mastercard",
                       Description = "Mastercard card type"
                   },
                  new CardType()
                  {
                      Id = Guid.Parse("77EB2E55-7E74-4288-9F10-28D4035A3081"),
                      Name = "Visa",
                      Description = "Visa card Type"
                  },
                  new CardType()
                  {
                      Id = Guid.Parse("D38C2488-BC3F-44FB-9725-B3E6CAFBC4E0"),
                      Name = "ComData",
                      Description = "ComData Card Type"
                  },
                   new CardType()
                   {
                       Id = Guid.Parse("B93CF5D1-A662-48DB-8351-BE0C0888CC0E"),
                       Name = "Discover",
                       Description = "Discover Card Type"
                   },
                   new CardType()
                   {
                       Id = Guid.Parse("6BB4854A-81DE-41A1-B1C7-CC1864A2D0D6"),
                       Name = "American Express",
                       Description = "American Express Card Type"
                   },
                   new CardType()
                   {
                       Id = Guid.Parse("F31EBC17-09A0-4CE8-873F-9195420DD4DA"),
                       Name = "Proprietary",
                       Description = "Proprietary Card Type"
                   }
            );

            modelBuilder.Entity<VolumeUnits>().HasData(
                  new VolumeUnits()
                  {
                      Id = Guid.Parse("89DFE007-4503-408A-8ADB-8B19A91CF290"),
                      Name = "US Gallons",
                      Description = "Volume units for US Gallons"
                  },
                 new VolumeUnits()
                 {
                     Id = Guid.Parse("6B97F87E-F113-45FE-8D55-12C4F8633746"),
                     Name = "Imperial Gallons",
                     Description = "Volume units for Imperial Gallons"
                 },
                 new VolumeUnits()
                 {
                     Id = Guid.Parse("B88E6B95-CAAF-4B15-9118-76C7BF9FE9C0"),
                     Name = "Liters",
                     Description = "Volume units for liters"
                 }
            );
            modelBuilder.Entity<FuelType>().HasData(
                   new FuelType()
                   {
                       Id = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D14"),
                       Name = "Jet",
                       Description = "Jet Fuel type"
                   },
                  new FuelType()
                  {
                      Id = Guid.Parse("5FBF8365-EF8A-4975-B939-90D5F507A1D1"),
                      Name = "AVSFuel",
                      Description = "AVSFuel fuel Type"
                  },
                  new FuelType()
                  {
                      Id = Guid.Parse("8A4A97F0-DF35-4D4A-BE7E-BA40590B432B"),
                      Name = "Unleaded",
                      Description = "Unleaded fuel Type"
                  }
            );
            modelBuilder.Entity<Fuel>().HasData(
                  new Fuel()
                  {
                      Id = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D14"),
                      Name = "Jet A",
                      Description = "This is the Jet A Fuel",
                      FuelTypeId = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D14"),

                  },
                 new Fuel()
                 {
                     Id = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D15"),
                     Name = "AVS Fuel A",
                     Description = "This is the AVS Fuel A Fuel",
                     FuelTypeId = Guid.Parse("5FBF8365-EF8A-4975-B939-90D5F507A1D1"),

                 },
                 new Fuel()
                 {
                     Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b41"),
                     Name = "Jet B",
                     Description = "This is the Jet B Fuel",
                     FuelTypeId = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D14"),
                 },
                 new Fuel()
                 {
                     Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b45"),
                     Name = "Regular Unleaded",
                     Description = "This is the Regular Unleaded Fuel",
                     FuelTypeId = Guid.Parse("8A4A97F0-DF35-4D4A-BE7E-BA40590B432B"),
                 },
                 new Fuel()
                 {
                     Id = Guid.Parse("0AE33D16-253F-4436-808C-A797B1D5E389"),
                     Name = "Premium Unleaded",
                     Description = "This is the Premium Unleaded Fuel",
                     FuelTypeId = Guid.Parse("8A4A97F0-DF35-4D4A-BE7E-BA40590B432B"),
                 }
             );
            modelBuilder.Entity<DispenserType>().HasData(
                new DispenserType()
                {
                    Id = Guid.Parse("3E214F64-5C9A-4249-9328-F76D330F98FA"),
                    Name = "PIE Box",
                    Description = "Dispenser type PIE Box"
                },
               new DispenserType()
               {
                   Id = Guid.Parse("A1C09103-55EC-461E-AF62-D2DFE5046A17"),
                   Name = "Dresser-Wayne",
                   Description = "Dispenser type Dresser-Wayne"
               },
               new DispenserType()
               {
                   Id = Guid.Parse("5F348E7F-C0D0-45FC-A2F5-798B0F1E2BCF"),
                   Name = "Tokheim",
                   Description = "Dispenser type Tokheim"
               },
               new DispenserType()
               {
                   Id = Guid.Parse("AF990478-970D-48BB-A863-BF374B64402A"),
                   Name = "DPI",
                   Description = "Dispenser type DPI"
               }
            );

            modelBuilder.Entity<Dispenser>().HasData(
                new Dispenser()
                {
                    Id = Guid.Parse("3E214F64-5C9A-4249-9328-F76D330F98FA"),
                    Name = "100 LL",
                    DispenserId = 1,
                    DispenserType = "PIE Box",
                    PulserType = "Default",
                    TotalizerReading = 1000,
                    MaxTotalizerDigits = 9,
                    VolumeUnit = Guid.Parse("89DFE007-4503-408A-8ADB-8B19A91CF290"),
                    TerminalId = Guid.Parse("08A028CB-4318-4555-9DEA-17EC0AD31B76"),
                    FuelId = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D14")
                },
               new Dispenser()
               {
                   Id = Guid.Parse("06C6D1A3-91A6-44EC-82A2-A29D4E3EC81B"),
                   Name = "Jet A",
                   DispenserId = 2,
                   DispenserType = "PIE Box",
                   PulserType = "Default",
                   HVDisplayNumber = 0,
                   TotalizerReading = 1000,
                   MaxTotalizerDigits = 9,
                   VolumeUnit = Guid.Parse("89DFE007-4503-408A-8ADB-8B19A91CF290"),
                   TerminalId = Guid.Parse("08A028CB-4318-4555-9DEA-17EC0AD31B76"),
                   FuelId = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D15")
               }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    Id = Guid.Parse("273E1214-1742-4D01-B4D9-382DA2548947"),
                    Name = "Administrator"
                },
               new Dispenser()
               {
                   Id = Guid.Parse("7BC3E576-874F-4257-9F62-11C6E097FBB5"),
                   Name = "QT Administrator"
               },
                new Dispenser()
                {
                    Id = Guid.Parse("195073E4-3503-40A1-BA0D-126811B44566"),
                    Name = "User"
                }
            );

            modelBuilder.Entity<AccountType>().HasData(
               new AccountType()
               {
                   Id = Guid.Parse("D8DD0FDB-8550-43DE-ACDA-B1BDAD9ECD9C"),
                   Name = "Proprietary",
                   Description = "Proprietary card type"
               },
                 new AccountType()
                 {
                     Id = Guid.Parse("BFC9D58F-B8E9-465A-85D5-D4FCE918AD44"),
                     Name = "Credit",
                     Description = "Credit card type"
                 }
            );

           modelBuilder.Entity<CompanyContacts>()
             .HasKey(c => new { c.CompanyId, c.ContactId });

            modelBuilder.Entity<SiteContacts>()
            .HasKey(c => new { c.SiteId, c.ContactId });

            modelBuilder.Entity<TerminalContacts>()
            .HasKey(c => new { c.TerminalId, c.ContactId });

            modelBuilder.Entity<CompanyCards>()
            .HasKey(c => new { c.CompanyId, c.CardId });

            modelBuilder.Entity<CompanySiteTypes>()
            .HasKey(c => new { c.CompanyId, c.SiteTypeId });

            modelBuilder.Entity<FuelBlocking>()
            .HasKey(c => new { c.FuelId, c.AccountId });
            
            modelBuilder.Entity<DispenserTankAssociation>()
            .HasKey(c => new { c.DispenserId, c.TankId });

            modelBuilder.Entity<TerminalDispenserAssociation>()
           .HasKey(c => new { c.TerminalId, c.DispenserId });

            modelBuilder.Entity<TankFuelAssociation>()
            .HasKey(c => new { c.TankId, c.FuelId });

            modelBuilder.Entity<AssignedTerminals>()
           .HasKey(c => new { c.TerminalId, c.AccountId });

            modelBuilder.Entity<CompanySiteTypes>().HasData(
                new CompanySiteTypes()
                {
                    CompanyId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b42"),
                    SiteTypeId = Guid.Parse("e28888e9-2ba9-473a-a40f-e38cb54f9b35")
                }
            );

            modelBuilder.Entity<DispenserTankAssociation>().HasData(
               new DispenserTankAssociation()
               {
                   DispenserId = Guid.Parse("06C6D1A3-91A6-44EC-82A2-A29D4E3EC81B"),
                   TankId = Guid.Parse("D741B92C-9383-4124-AB35-00AAC3E0C738"),
               }
            );

            modelBuilder.Entity<TankFuelAssociation>().HasData(
              new TankFuelAssociation()
              {
                  TankId = Guid.Parse("D741B92C-9383-4124-AB35-00AAC3E0C738"),
                  FuelId = Guid.Parse("6513569B-E36B-413F-921E-C56D04011D14"),
              }
           );

            modelBuilder.Entity<TerminalSettings>()
                .HasIndex(e => e.TerminalId);

            modelBuilder.Entity<Dispenser>()
               .HasIndex(e => e.Name);

            base.OnModelCreating(modelBuilder);
        }

    }
}
