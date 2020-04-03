using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Siteminder.API.Migrations
{
    public partial class InitialSiteminderDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CardTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 1500, nullable: false),
                    IsPrivateAccount = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<string>(maxLength: 30, nullable: false),
                    Address1 = table.Column<string>(maxLength: 50, nullable: false),
                    Address2 = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    State = table.Column<string>(maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyCards",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(nullable: false),
                    CardId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCards", x => new { x.CompanyId, x.CardId });
                });

            migrationBuilder.CreateTable(
                name: "CompanyContacts",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(nullable: false),
                    ContactId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyContacts", x => new { x.CompanyId, x.ContactId });
                });

            migrationBuilder.CreateTable(
                name: "ContactType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DeviceName = table.Column<string>(maxLength: 50, nullable: false),
                    ModelName = table.Column<string>(maxLength: 50, nullable: false),
                    SerialNumber = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 1500, nullable: false),
                    TerminalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DispenserTankAssociations",
                columns: table => new
                {
                    DispenserId = table.Column<Guid>(nullable: false),
                    TankId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispenserTankAssociations", x => new { x.DispenserId, x.TankId });
                });

            migrationBuilder.CreateTable(
                name: "FuelBlocking",
                columns: table => new
                {
                    FuelId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelBlocking", x => new { x.FuelId, x.AccountId });
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteContacts",
                columns: table => new
                {
                    SiteId = table.Column<Guid>(nullable: false),
                    ContactId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteContacts", x => new { x.SiteId, x.ContactId });
                });

            migrationBuilder.CreateTable(
                name: "SiteType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TankFuelAssociations",
                columns: table => new
                {
                    TankId = table.Column<Guid>(nullable: false),
                    FuelId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TankFuelAssociations", x => new { x.TankId, x.FuelId });
                });

            migrationBuilder.CreateTable(
                name: "TerminalContacts",
                columns: table => new
                {
                    TerminalId = table.Column<Guid>(nullable: false),
                    ContactId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminalContacts", x => new { x.TerminalId, x.ContactId });
                });

            migrationBuilder.CreateTable(
                name: "TerminalDispenserTankAssociations",
                columns: table => new
                {
                    DispenserId = table.Column<Guid>(nullable: false),
                    TerminalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminalDispenserTankAssociations", x => new { x.TerminalId, x.DispenserId });
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(maxLength: 20, nullable: false),
                    FirstName = table.Column<string>(maxLength: 30, nullable: false),
                    LastName = table.Column<string>(maxLength: 30, nullable: false),
                    Email = table.Column<string>(maxLength: 30, nullable: false),
                    Password = table.Column<string>(maxLength: 30, nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizedCard",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    HoldAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizedCard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizedCard_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanySiteTypes",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(nullable: false),
                    SiteTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySiteTypes", x => new { x.CompanyId, x.SiteTypeId });
                    table.ForeignKey(
                        name: "FK_CompanySiteTypes_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountType = table.Column<string>(nullable: false),
                    PaymentType = table.Column<string>(nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerName = table.Column<string>(nullable: false),
                    TailNumber = table.Column<string>(nullable: false),
                    AccountNumber = table.Column<string>(nullable: false),
                    ExternalAccountReference = table.Column<string>(nullable: true),
                    Identifer = table.Column<string>(nullable: true),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AccountingCodeRequired = table.Column<bool>(nullable: false),
                    CardTypeID = table.Column<string>(nullable: false),
                    EmbossedNumber = table.Column<string>(nullable: true),
                    CardHolderName = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AccountName = table.Column<string>(nullable: false),
                    ContactName = table.Column<string>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: false),
                    Address1 = table.Column<string>(nullable: false),
                    Address2 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: false),
                    State = table.Column<string>(maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(maxLength: 10, nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Fax = table.Column<string>(nullable: true),
                    DiscountPerGallon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PinCode = table.Column<int>(nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAccounts_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    Title = table.Column<string>(maxLength: 30, nullable: false),
                    CountryCode = table.Column<string>(maxLength: 2, nullable: false),
                    Phone = table.Column<string>(maxLength: 12, nullable: false),
                    Mobile = table.Column<string>(maxLength: 12, nullable: true),
                    Fax = table.Column<string>(maxLength: 12, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    CompanyId = table.Column<Guid>(nullable: false),
                    ContactTypeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contact_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contact_ContactType_ContactTypeId",
                        column: x => x.ContactTypeId,
                        principalTable: "ContactType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SiteName = table.Column<string>(maxLength: 50, nullable: false),
                    Address1 = table.Column<string>(maxLength: 50, nullable: false),
                    Address2 = table.Column<string>(maxLength: 50, nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    State = table.Column<string>(maxLength: 2, nullable: false),
                    ZipCode = table.Column<string>(maxLength: 10, nullable: false),
                    CompanyId = table.Column<Guid>(nullable: false),
                    SiteTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Site_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Site_SiteType_SiteTypeId",
                        column: x => x.SiteTypeId,
                        principalTable: "SiteType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignedTerminals",
                columns: table => new
                {
                    TerminalId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CustomerAccountId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedTerminals", x => new { x.TerminalId, x.AccountId });
                    table.ForeignKey(
                        name: "FK_AssignedTerminals_CustomerAccounts_CustomerAccountId",
                        column: x => x.CustomerAccountId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    TransactionDate = table.Column<DateTime>(nullable: false),
                    SortableDate = table.Column<string>(nullable: true),
                    BatchNumber = table.Column<string>(nullable: true),
                    ApprovalCode = table.Column<string>(nullable: false),
                    AuthorizedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TerminalName = table.Column<string>(nullable: true),
                    AuthResponseData = table.Column<string>(nullable: true),
                    CustomTicketMessage = table.Column<string>(nullable: true),
                    IssuerResponseCode = table.Column<string>(nullable: true),
                    ResponseMessage = table.Column<string>(nullable: true),
                    ResponseText = table.Column<string>(nullable: true),
                    CaptureResponseMessage = table.Column<string>(nullable: true),
                    SettlementResponseText = table.Column<string>(nullable: true),
                    ResponseTimestamp = table.Column<DateTime>(nullable: false),
                    TraceNumber = table.Column<int>(nullable: false),
                    ApprovingNetworkName = table.Column<string>(nullable: true),
                    AuthorizationTimeout = table.Column<string>(nullable: true),
                    CardType = table.Column<string>(nullable: true),
                    ComdataID = table.Column<string>(nullable: true),
                    DriverID = table.Column<string>(nullable: true),
                    EntryMode = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    NameOnCard = table.Column<string>(nullable: true),
                    PaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Zipcode = table.Column<string>(nullable: false),
                    VehicleID = table.Column<string>(nullable: true),
                    NetworkType = table.Column<string>(nullable: true),
                    Processor = table.Column<string>(nullable: true),
                    SettlementResponseCode = table.Column<string>(nullable: true),
                    SettlementResponseID = table.Column<string>(nullable: true),
                    CancelResponseMessage = table.Column<string>(nullable: true),
                    CancelResponseText = table.Column<string>(nullable: true),
                    InvoiceNumber = table.Column<string>(nullable: true),
                    FlightNumber = table.Column<string>(nullable: true),
                    TailNumber = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StartTotalizer = table.Column<string>(nullable: true),
                    EndTotalizer = table.Column<string>(nullable: true),
                    DispenserID = table.Column<string>(nullable: true),
                    DispenserName = table.Column<string>(nullable: true),
                    ProductID = table.Column<string>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    TotalSaleAmount = table.Column<string>(nullable: true),
                    AvgPricePerUnit = table.Column<string>(nullable: true),
                    VolumeUnitName = table.Column<string>(nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VolumeGross = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    VolumeNet = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Temperature = table.Column<string>(nullable: true),
                    CustomerAccountNumber = table.Column<string>(nullable: true),
                    CustomerName = table.Column<string>(nullable: true),
                    ReceiptAvailable = table.Column<string>(nullable: true),
                    AccountId = table.Column<Guid>(nullable: false),
                    CustomerAccountId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transaction_CustomerAccounts_CustomerAccountId",
                        column: x => x.CustomerAccountId,
                        principalTable: "CustomerAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Gallons = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    SiteId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discount_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Terminal",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TerminalName = table.Column<string>(maxLength: 50, nullable: false),
                    SiteId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terminal_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dispensers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    DispenserId = table.Column<int>(maxLength: 2, nullable: false),
                    DispenserType = table.Column<string>(nullable: false),
                    PulserType = table.Column<string>(nullable: false),
                    HVDisplayNumber = table.Column<int>(nullable: false),
                    TotalizerReading = table.Column<int>(nullable: false),
                    MaxTotalizerDigits = table.Column<int>(nullable: false),
                    VolumeUnit = table.Column<Guid>(nullable: false),
                    TerminalId = table.Column<Guid>(nullable: false),
                    FuelId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dispensers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dispensers_Terminal_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "Terminal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ScheduleName = table.Column<string>(nullable: false),
                    TerminalId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedule_Terminal_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "Terminal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TerminalSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PrintPvtAccountBalanceOnReceipt = table.Column<bool>(nullable: false),
                    BatchCloseTime = table.Column<DateTime>(nullable: false),
                    TimeZoneId = table.Column<string>(nullable: true),
                    CCPlatform = table.Column<string>(nullable: true),
                    RejectUnregisteredNonPrivateCardUsers = table.Column<bool>(nullable: false),
                    RejectUnregisteredPrivateCardUsers = table.Column<bool>(nullable: false),
                    CummulativeRiskAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxiumumOfflineAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OfflineSalesOption = table.Column<bool>(nullable: false),
                    AmountOfFillupKey = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumSaleAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxTableId = table.Column<string>(nullable: true),
                    DisableFillupKey = table.Column<bool>(nullable: false),
                    ProprietaryCardCode = table.Column<int>(nullable: false),
                    QTGatewayTerminalId = table.Column<string>(nullable: true),
                    MSATerminalId = table.Column<string>(nullable: true),
                    MSAUserName = table.Column<string>(nullable: true),
                    MSTSMerchantAccountID = table.Column<string>(nullable: true),
                    HeartlandCompanyId = table.Column<string>(nullable: true),
                    HeartlandDeviceId = table.Column<string>(nullable: true),
                    TargetNetworkEnvironment = table.Column<string>(nullable: true),
                    QTGatewayAccessCode = table.Column<string>(nullable: true),
                    MSAMechantNumber = table.Column<string>(nullable: true),
                    MSAPassword = table.Column<string>(nullable: true),
                    MSTSMerchantJobberId = table.Column<string>(nullable: true),
                    HeartlandTerminalLocationId = table.Column<string>(nullable: true),
                    DPIPort = table.Column<string>(nullable: true),
                    PIEPort = table.Column<string>(nullable: true),
                    HVDPOrt = table.Column<string>(nullable: true),
                    BarcodeReaderPort = table.Column<string>(nullable: true),
                    AutoTankGuaging = table.Column<string>(nullable: true),
                    ATGPort = table.Column<string>(nullable: true),
                    ATGBaudRate = table.Column<int>(nullable: false),
                    ATGParity = table.Column<string>(nullable: true),
                    ATGStopBits = table.Column<string>(nullable: true),
                    ATGDataBits = table.Column<string>(nullable: true),
                    ATGHandshake = table.Column<string>(nullable: true),
                    ATGSecurityCode = table.Column<string>(nullable: true),
                    ForceRegistry = table.Column<bool>(nullable: false),
                    RegistryMinLength = table.Column<int>(nullable: false),
                    RegistryMaxLength = table.Column<int>(nullable: false),
                    RegistryLabelAs = table.Column<string>(nullable: true),
                    TerminalId = table.Column<Guid>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TerminalSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TerminalSettings_Terminal_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "Terminal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DispenserType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Description = table.Column<string>(maxLength: 1500, nullable: false),
                    DispenserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispenserType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispenserType_Dispensers_DispenserId",
                        column: x => x.DispenserId,
                        principalTable: "Dispensers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tank",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<int>(maxLength: 1500, nullable: false),
                    Size = table.Column<int>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    AlarmPoint = table.Column<int>(nullable: false),
                    TerminalId = table.Column<Guid>(nullable: false),
                    DispenserId = table.Column<Guid>(nullable: false),
                    FuelTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tank", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tank_Dispensers_DispenserId",
                        column: x => x.DispenserId,
                        principalTable: "Dispensers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DayOfWeek = table.Column<string>(nullable: false),
                    OpenTime = table.Column<DateTime>(nullable: false),
                    CloseTime = table.Column<DateTime>(nullable: false),
                    ScheduleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleDetail_Schedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VolumeUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Description = table.Column<string>(maxLength: 1500, nullable: false),
                    TankId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolumeUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolumeUnits_Tank_TankId",
                        column: x => x.TankId,
                        principalTable: "Tank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FuelPump",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 1500, nullable: false),
                    TerminalId = table.Column<Guid>(nullable: true),
                    FuelId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelPump", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelPump_Terminal_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "Terminal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FuelType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FuelPumpId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelType_FuelPump_FuelPumpId",
                        column: x => x.FuelPumpId,
                        principalTable: "FuelPump",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fuel",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FuelTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fuel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fuel_FuelType_FuelTypeId",
                        column: x => x.FuelTypeId,
                        principalTable: "FuelType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("d8dd0fdb-8550-43de-acda-b1bdad9ecd9c"), "Proprietary card type", "Proprietary" },
                    { new Guid("bfc9d58f-b8e9-465a-85d5-d4fce918ad44"), "Credit card type", "Credit" }
                });

            migrationBuilder.InsertData(
                table: "CardTypes",
                columns: new[] { "Id", "Description", "IsPrivateAccount", "Name" },
                values: new object[,]
                {
                    { new Guid("4e173d24-3094-4853-a8ed-01c54068c8d2"), "Mastercard card type", false, "Mastercard" },
                    { new Guid("77eb2e55-7e74-4288-9f10-28d4035a3081"), "Visa card Type", false, "Visa" },
                    { new Guid("d38c2488-bc3f-44fb-9725-b3e6cafbc4e0"), "ComData Card Type", false, "ComData" },
                    { new Guid("b93cf5d1-a662-48db-8351-be0c0888cc0e"), "Discover Card Type", false, "Discover" },
                    { new Guid("6bb4854a-81de-41a1-b1c7-cc1864a2d0d6"), "American Express Card Type", false, "American Express" },
                    { new Guid("f31ebc17-09a0-4ce8-873f-9195420dd4da"), "Proprietary Card Type", false, "Proprietary" }
                });

            migrationBuilder.InsertData(
                table: "Company",
                columns: new[] { "Id", "Address1", "Address2", "City", "Name", "State", "Type", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b41"), "701 Main Street", "", "Boulder", "Dallas' Fuel", "CO", "Airport", "77777-77" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b40"), "601 Main Street", "", "New York", "Greg's Fuel", "NY", "Airport", "66666-66" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b39"), "501 Main Street", "", "Parsippany", "Matt's Fuel", "NJ", "Airport", "55555-5555" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b35"), "101 Main Street", "", "Los Angeles", "Andrew's Fuel", "CA", "Airport", "11111" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b37"), "301 Main Street", "", "Detroit", "Jacks Fuel", "MI", "Airport", "33333" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b36"), "201 Main Street", "", "Oklahoma City", "Danny's Fuel", "OK", "Airport", "22222" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b42"), "801 Main Street", "", "Port Jefferson", "Chris'' Fuel", "NY", "Airport", "11777" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b38"), "401 Main Street", "", "Cleveland", "Dan's Fuel", "OH", "Airport", "44444-4444" }
                });

            migrationBuilder.InsertData(
                table: "ContactType",
                columns: new[] { "Id", "Description", "Type" },
                values: new object[,]
                {
                    { new Guid("5c80c4c0-9100-4972-8d93-958c4bf101ff"), "Site contact type", "Site" },
                    { new Guid("43e40f5e-698b-4621-ad07-6d73cea9c624"), "Site contact type", "Terminal" },
                    { new Guid("ab406afc-4b10-4d9c-b2d6-13791af12af5"), "Company contact type", "Company" }
                });

            migrationBuilder.InsertData(
                table: "DispenserTankAssociations",
                columns: new[] { "DispenserId", "TankId" },
                values: new object[] { new Guid("06c6d1a3-91a6-44ec-82a2-a29d4e3ec81b"), new Guid("d741b92c-9383-4124-ab35-00aac3e0c738") });

            migrationBuilder.InsertData(
                table: "DispenserType",
                columns: new[] { "Id", "Description", "DispenserId", "Name" },
                values: new object[,]
                {
                    { new Guid("3e214f64-5c9a-4249-9328-f76d330f98fa"), "Dispenser type PIE Box", null, "PIE Box" },
                    { new Guid("a1c09103-55ec-461e-af62-d2dfe5046a17"), "Dispenser type Dresser-Wayne", null, "Dresser-Wayne" },
                    { new Guid("5f348e7f-c0d0-45fc-a2f5-798b0f1e2bcf"), "Dispenser type Tokheim", null, "Tokheim" },
                    { new Guid("af990478-970d-48bb-a863-bf374b64402a"), "Dispenser type DPI", null, "DPI" }
                });

            migrationBuilder.InsertData(
                table: "FuelType",
                columns: new[] { "Id", "Description", "FuelPumpId", "Name" },
                values: new object[,]
                {
                    { new Guid("6513569b-e36b-413f-921e-c56d04011d14"), "Jet Fuel type", null, "Jet" },
                    { new Guid("5fbf8365-ef8a-4975-b939-90d5f507a1d1"), "AVSFuel fuel Type", null, "AVSFuel" },
                    { new Guid("8a4a97f0-df35-4d4a-be7e-ba40590b432b"), "Unleaded fuel Type", null, "Unleaded" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("273e1214-1742-4d01-b4d9-382da2548947"), "Administrator" },
                    { new Guid("7bc3e576-874f-4257-9f62-11c6e097fbb5"), "QT Administrator" },
                    { new Guid("195073e4-3503-40a1-ba0d-126811b44566"), "User" }
                });

            migrationBuilder.InsertData(
                table: "SiteType",
                columns: new[] { "Id", "Description", "Type" },
                values: new object[,]
                {
                    { new Guid("e28888e9-2ba9-473a-a40f-e38cb54f9b35"), "Airport site type", "Airport" },
                    { new Guid("e28888e9-2ba9-473a-a40f-e38cb54f9b36"), "Marina site type", "Marina" },
                    { new Guid("e28888e9-2ba9-473a-a40f-e38cb54f9b37"), "Taxi site type", "Taxi" }
                });

            migrationBuilder.InsertData(
                table: "TankFuelAssociations",
                columns: new[] { "TankId", "FuelId" },
                values: new object[] { new Guid("d741b92c-9383-4124-ab35-00aac3e0c738"), new Guid("6513569b-e36b-413f-921e-c56d04011d14") });

            migrationBuilder.InsertData(
                table: "VolumeUnits",
                columns: new[] { "Id", "Description", "Name", "TankId" },
                values: new object[,]
                {
                    { new Guid("6b97f87e-f113-45fe-8d55-12c4f8633746"), "Volume units for Imperial Gallons", "Imperial Gallons", null },
                    { new Guid("89dfe007-4503-408a-8adb-8b19a91cf290"), "Volume units for US Gallons", "US Gallons", null },
                    { new Guid("b88e6b95-caaf-4b15-9118-76c7bf9fe9c0"), "Volume units for liters", "Liters", null }
                });

            migrationBuilder.InsertData(
                table: "CompanySiteTypes",
                columns: new[] { "CompanyId", "SiteTypeId" },
                values: new object[] { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b42"), new Guid("e28888e9-2ba9-473a-a40f-e38cb54f9b35") });

            migrationBuilder.InsertData(
                table: "Fuel",
                columns: new[] { "Id", "Description", "FuelTypeId", "Name" },
                values: new object[,]
                {
                    { new Guid("6513569b-e36b-413f-921e-c56d04011d14"), "This is the Jet A Fuel", new Guid("6513569b-e36b-413f-921e-c56d04011d14"), "Jet A" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b41"), "This is the Jet B Fuel", new Guid("6513569b-e36b-413f-921e-c56d04011d14"), "Jet B" },
                    { new Guid("6513569b-e36b-413f-921e-c56d04011d15"), "This is the AVS Fuel A Fuel", new Guid("5fbf8365-ef8a-4975-b939-90d5f507a1d1"), "AVS Fuel A" },
                    { new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b45"), "This is the Regular Unleaded Fuel", new Guid("8a4a97f0-df35-4d4a-be7e-ba40590b432b"), "Regular Unleaded" },
                    { new Guid("0ae33d16-253f-4436-808c-a797b1d5e389"), "This is the Premium Unleaded Fuel", new Guid("8a4a97f0-df35-4d4a-be7e-ba40590b432b"), "Premium Unleaded" }
                });

            migrationBuilder.InsertData(
                table: "Site",
                columns: new[] { "Id", "Address1", "Address2", "City", "CompanyId", "SiteName", "SiteTypeId", "State", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("4facfa8e-0008-450b-8022-abe7566dc50f"), "110 Main Street", null, "Port Jefferson", new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b42"), "Port Jefferson Marina", new Guid("e28888e9-2ba9-473a-a40f-e38cb54f9b36"), "NY", "11778" },
                    { new Guid("c45028a6-9887-481f-8167-79518dccc886"), "110 Broadway", null, "Denver", new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b41"), "Denver Marina Marina", new Guid("e28888e9-2ba9-473a-a40f-e38cb54f9b36"), "CO", "80028" },
                    { new Guid("5f9d7f91-2a5d-4424-bf81-4ee0e0a6dbde"), "500 50 Acre Rd", null, "Smithtown", new Guid("d28888e9-2ba9-473a-a40f-e38cb54f9b40"), "Smithtown Marina", new Guid("e28888e9-2ba9-473a-a40f-e38cb54f9b36"), "NY", "11787" }
                });

            migrationBuilder.InsertData(
                table: "Terminal",
                columns: new[] { "Id", "SiteId", "TerminalName" },
                values: new object[] { new Guid("08a028cb-4318-4555-9dea-17ec0ad31b76"), new Guid("c45028a6-9887-481f-8167-79518dccc886"), "Port Jefferson Terminal" });

            migrationBuilder.InsertData(
                table: "Terminal",
                columns: new[] { "Id", "SiteId", "TerminalName" },
                values: new object[] { new Guid("691f7447-ab56-4821-a193-46e6a055192f"), new Guid("5f9d7f91-2a5d-4424-bf81-4ee0e0a6dbde"), "Smithtown Terminal" });

            migrationBuilder.InsertData(
                table: "Dispensers",
                columns: new[] { "Id", "DispenserId", "DispenserType", "FuelId", "HVDisplayNumber", "MaxTotalizerDigits", "Name", "PulserType", "ScheduleId", "TerminalId", "TotalizerReading", "VolumeUnit" },
                values: new object[,]
                {
                    { new Guid("3e214f64-5c9a-4249-9328-f76d330f98fa"), 1, "PIE Box", new Guid("6513569b-e36b-413f-921e-c56d04011d14"), 0, 9, "100 LL", "Default", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("08a028cb-4318-4555-9dea-17ec0ad31b76"), 1000, new Guid("89dfe007-4503-408a-8adb-8b19a91cf290") },
                    { new Guid("06c6d1a3-91a6-44ec-82a2-a29d4e3ec81b"), 2, "PIE Box", new Guid("6513569b-e36b-413f-921e-c56d04011d15"), 0, 9, "Jet A", "Default", new Guid("00000000-0000-0000-0000-000000000000"), new Guid("08a028cb-4318-4555-9dea-17ec0ad31b76"), 1000, new Guid("89dfe007-4503-408a-8adb-8b19a91cf290") }
                });

            migrationBuilder.InsertData(
                table: "Schedule",
                columns: new[] { "Id", "ScheduleName", "TerminalId" },
                values: new object[] { new Guid("352485aa-2436-4912-85e8-32d124374eb8"), "Weekly", new Guid("08a028cb-4318-4555-9dea-17ec0ad31b76") });

            migrationBuilder.InsertData(
                table: "TerminalSettings",
                columns: new[] { "Id", "ATGBaudRate", "ATGDataBits", "ATGHandshake", "ATGParity", "ATGPort", "ATGSecurityCode", "ATGStopBits", "AmountOfFillupKey", "AutoTankGuaging", "BarcodeReaderPort", "BatchCloseTime", "CCPlatform", "CummulativeRiskAmount", "DPIPort", "DisableFillupKey", "ForceRegistry", "HVDPOrt", "HeartlandCompanyId", "HeartlandDeviceId", "HeartlandTerminalLocationId", "MSAMechantNumber", "MSAPassword", "MSATerminalId", "MSAUserName", "MSTSMerchantAccountID", "MSTSMerchantJobberId", "MaximumSaleAmount", "MaxiumumOfflineAmount", "OfflineSalesOption", "PIEPort", "PrintPvtAccountBalanceOnReceipt", "ProprietaryCardCode", "QTGatewayAccessCode", "QTGatewayTerminalId", "RegistryLabelAs", "RegistryMaxLength", "RegistryMinLength", "RejectUnregisteredNonPrivateCardUsers", "RejectUnregisteredPrivateCardUsers", "ScheduleId", "TargetNetworkEnvironment", "TaxTableId", "TerminalId", "TimeZoneId" },
                values: new object[,]
                {
                    { new Guid("805774c4-8b08-4055-9eb9-25f8cce4c1ee"), 0, null, null, null, null, null, null, 500.00m, null, null, new DateTime(2020, 1, 11, 0, 0, 0, 0, DateTimeKind.Local), "Phillips66", 250.00m, null, false, false, null, null, null, null, null, null, null, null, null, null, 500.00m, 250.00m, true, null, false, 0, null, null, null, 0, 0, true, true, new Guid("2f126d7c-bb47-4cb6-b30e-b601d286df6a"), null, "1", new Guid("08a028cb-4318-4555-9dea-17ec0ad31b76"), "MST" },
                    { new Guid("b03a284e-df40-42aa-9be8-b1f91c23d3cc"), 0, null, null, null, null, null, null, 500.00m, null, null, new DateTime(2020, 1, 11, 0, 0, 0, 0, DateTimeKind.Local), "Phillips66", 250.00m, null, false, false, null, null, null, null, null, null, null, null, null, null, 500.00m, 250.00m, true, null, false, 0, null, null, null, 0, 0, true, true, new Guid("2f126d7c-bb47-4cb6-b30e-b601d286df6a"), null, "1", new Guid("691f7447-ab56-4821-a193-46e6a055192f"), "MST" }
                });

            migrationBuilder.InsertData(
                table: "ScheduleDetail",
                columns: new[] { "Id", "CloseTime", "DayOfWeek", "OpenTime", "ScheduleId" },
                values: new object[,]
                {
                    { new Guid("78414ab8-da92-44f1-b191-c61849cb3563"), new DateTime(2020, 1, 10, 20, 0, 0, 0, DateTimeKind.Unspecified), "Monday", new DateTime(2020, 1, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("352485aa-2436-4912-85e8-32d124374eb8") },
                    { new Guid("f5d408ff-d24f-40a1-85f5-8f6bf4ddcd15"), new DateTime(2020, 1, 10, 20, 0, 0, 0, DateTimeKind.Unspecified), "Tuesday", new DateTime(2020, 1, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("352485aa-2436-4912-85e8-32d124374eb8") },
                    { new Guid("f861f7c0-055a-49c6-b3bc-12a6f662222a"), new DateTime(2020, 1, 10, 20, 0, 0, 0, DateTimeKind.Unspecified), "Wednesday", new DateTime(2020, 1, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("352485aa-2436-4912-85e8-32d124374eb8") },
                    { new Guid("0e798f1a-3c56-4efe-adcf-a98eeefa2c79"), new DateTime(2020, 1, 10, 20, 0, 0, 0, DateTimeKind.Unspecified), "Thursday", new DateTime(2020, 1, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("352485aa-2436-4912-85e8-32d124374eb8") },
                    { new Guid("da4bf274-87e1-464a-8272-d6db715012c2"), new DateTime(2020, 1, 10, 20, 0, 0, 0, DateTimeKind.Unspecified), "Friday", new DateTime(2020, 1, 10, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("352485aa-2436-4912-85e8-32d124374eb8") },
                    { new Guid("64efec51-6d9e-4f04-b1da-96f2a10e5d95"), new DateTime(2020, 1, 10, 22, 0, 0, 0, DateTimeKind.Unspecified), "Saturday", new DateTime(2020, 1, 10, 6, 0, 0, 0, DateTimeKind.Unspecified), new Guid("352485aa-2436-4912-85e8-32d124374eb8") },
                    { new Guid("39b1e940-5eb8-418d-8067-7f1b4f54c736"), new DateTime(2020, 1, 10, 22, 0, 0, 0, DateTimeKind.Unspecified), "Sunday", new DateTime(2020, 1, 10, 6, 0, 0, 0, DateTimeKind.Unspecified), new Guid("352485aa-2436-4912-85e8-32d124374eb8") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignedTerminals_CustomerAccountId",
                table: "AssignedTerminals",
                column: "CustomerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedCard_CompanyId",
                table: "AuthorizedCard",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CardTypes_Name",
                table: "CardTypes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Company_Name",
                table: "Company",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_CompanyId",
                table: "Contact",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ContactTypeId",
                table: "Contact",
                column: "ContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAccounts_CompanyId",
                table: "CustomerAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_SiteId",
                table: "Discount",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Dispensers_Name",
                table: "Dispensers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Dispensers_TerminalId",
                table: "Dispensers",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_DispenserType_DispenserId",
                table: "DispenserType",
                column: "DispenserId");

            migrationBuilder.CreateIndex(
                name: "IX_Fuel_FuelTypeId",
                table: "Fuel",
                column: "FuelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPump_FuelId",
                table: "FuelPump",
                column: "FuelId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPump_TerminalId",
                table: "FuelPump",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_FuelType_FuelPumpId",
                table: "FuelType",
                column: "FuelPumpId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_TerminalId",
                table: "Schedule",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetail_ScheduleId",
                table: "ScheduleDetail",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Site_CompanyId",
                table: "Site",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Site_SiteTypeId",
                table: "Site",
                column: "SiteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tank_DispenserId",
                table: "Tank",
                column: "DispenserId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminal_SiteId",
                table: "Terminal",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TerminalSettings_TerminalId",
                table: "TerminalSettings",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_CustomerAccountId",
                table: "Transaction",
                column: "CustomerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VolumeUnits_TankId",
                table: "VolumeUnits",
                column: "TankId");

            migrationBuilder.AddForeignKey(
                name: "FK_FuelPump_Fuel_FuelId",
                table: "FuelPump",
                column: "FuelId",
                principalTable: "Fuel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Site_Company_CompanyId",
                table: "Site");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminal_Site_SiteId",
                table: "Terminal");

            migrationBuilder.DropForeignKey(
                name: "FK_FuelPump_Terminal_TerminalId",
                table: "FuelPump");

            migrationBuilder.DropForeignKey(
                name: "FK_Fuel_FuelType_FuelTypeId",
                table: "Fuel");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "AssignedTerminals");

            migrationBuilder.DropTable(
                name: "AuthorizedCard");

            migrationBuilder.DropTable(
                name: "CardTypes");

            migrationBuilder.DropTable(
                name: "CompanyCards");

            migrationBuilder.DropTable(
                name: "CompanyContacts");

            migrationBuilder.DropTable(
                name: "CompanySiteTypes");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "DispenserTankAssociations");

            migrationBuilder.DropTable(
                name: "DispenserType");

            migrationBuilder.DropTable(
                name: "FuelBlocking");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ScheduleDetail");

            migrationBuilder.DropTable(
                name: "SiteContacts");

            migrationBuilder.DropTable(
                name: "TankFuelAssociations");

            migrationBuilder.DropTable(
                name: "TerminalContacts");

            migrationBuilder.DropTable(
                name: "TerminalDispenserTankAssociations");

            migrationBuilder.DropTable(
                name: "TerminalSettings");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "VolumeUnits");

            migrationBuilder.DropTable(
                name: "ContactType");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "CustomerAccounts");

            migrationBuilder.DropTable(
                name: "Tank");

            migrationBuilder.DropTable(
                name: "Dispensers");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.DropTable(
                name: "SiteType");

            migrationBuilder.DropTable(
                name: "Terminal");

            migrationBuilder.DropTable(
                name: "FuelType");

            migrationBuilder.DropTable(
                name: "FuelPump");

            migrationBuilder.DropTable(
                name: "Fuel");
        }
    }
}
