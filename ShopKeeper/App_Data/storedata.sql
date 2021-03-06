USE [master]
GO
/****** Object:  Database [ShopKeeper]    Script Date: 18/01/2015 01:41:24 ******/
CREATE DATABASE [ShopKeeper]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'ShopKeeper', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\ShopKeeper.mdf' , SIZE = 5000KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ShopKeeper_log', FILENAME = N'C:\Program Files (x86)\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\ShopKeeper_log.ldf' , SIZE = 5000KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ShopKeeper] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ShopKeeper].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ShopKeeper] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ShopKeeper] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ShopKeeper] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ShopKeeper] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ShopKeeper] SET ARITHABORT OFF 
GO
ALTER DATABASE [ShopKeeper] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ShopKeeper] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [ShopKeeper] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ShopKeeper] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ShopKeeper] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ShopKeeper] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ShopKeeper] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ShopKeeper] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ShopKeeper] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ShopKeeper] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ShopKeeper] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ShopKeeper] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ShopKeeper] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ShopKeeper] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ShopKeeper] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ShopKeeper] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ShopKeeper] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ShopKeeper] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ShopKeeper] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [ShopKeeper] SET  MULTI_USER 
GO
ALTER DATABASE [ShopKeeper] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ShopKeeper] SET DB_CHAINING OFF 
GO
ALTER DATABASE [ShopKeeper] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [ShopKeeper] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [ShopKeeper]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountGroup]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountGroup](
	[AccountGroupId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AccountGroup] PRIMARY KEY CLUSTERED 
(
	[AccountGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AddressType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AddressType](
	[AddressTypeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](250) NULL,
 CONSTRAINT [PK_AddressType] PRIMARY KEY CLUSTERED 
(
	[AddressTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetRoles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AspNetUserClaims] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserLogins] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.AspNetUsers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[BankAccount]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BankAccount](
	[BankAccountId] [bigint] IDENTITY(1,1) NOT NULL,
	[BankId] [bigint] NOT NULL,
	[AccountName] [nvarchar](250) NOT NULL,
	[AccountNo] [bigint] NOT NULL,
	[Status] [bit] NOT NULL,
	[CustomerId] [bigint] NOT NULL,
 CONSTRAINT [PK_BankAccounts] PRIMARY KEY CLUSTERED 
(
	[BankAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ChartOfAccount]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChartOfAccount](
	[ChartOfAccountId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[AccountGroupId] [int] NOT NULL,
	[AccountType] [nvarchar](150) NOT NULL,
	[AccountCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ChartOfAccount] PRIMARY KEY CLUSTERED 
(
	[ChartOfAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ContactType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContactType](
	[ContactTypeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ContactType] PRIMARY KEY CLUSTERED 
(
	[ContactTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Coupon]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Coupon](
	[CouponId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[PercentageDeduction] [float] NOT NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
	[MinimumOrderAmount] [float] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_Coupon] PRIMARY KEY CLUSTERED 
(
	[CouponId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Customer]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerId] [bigint] IDENTITY(1,1) NOT NULL,
	[ReferredByCustomerId] [bigint] NULL,
	[StoreCustomerTypeId] [int] NOT NULL,
	[StoreOutletId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[FirstPurchaseDate] [datetime] NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DailyInventory]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyInventory](
	[DailyInventoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
	[StockLevel] [int] NOT NULL,
	[DateTaken] [datetime] NOT NULL,
	[DateCreated] [timestamp] NOT NULL,
 CONSTRAINT [PK_DailyInventory] PRIMARY KEY CLUSTERED 
(
	[DailyInventoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DailySale]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailySale](
	[DailySaleId] [bigint] IDENTITY(1,1) NOT NULL,
	[SalesDate] [date] NOT NULL,
	[StoreOutletId] [int] NOT NULL,
	[AmountSold] [float] NOT NULL,
	[LastUpdated] [timestamp] NOT NULL,
 CONSTRAINT [PK_DailySales] PRIMARY KEY CLUSTERED 
(
	[DailySaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Delivery]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Delivery](
	[DeliveryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TrackingNumber] [nvarchar](150) NULL,
	[PurchaseOrderId] [bigint] NOT NULL,
	[DeliveryVesselId] [bigint] NOT NULL,
	[ExpectedDeliveryDate] [datetime] NOT NULL,
	[AcutalDeliveryDate] [datetime] NULL,
	[ReceivedById] [bigint] NOT NULL,
 CONSTRAINT [PK_Deliveries] PRIMARY KEY CLUSTERED 
(
	[DeliveryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DeliveryMethod]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryMethod](
	[DeliveryMethodId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[MethodTitle] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_DeliveryMethods] PRIMARY KEY CLUSTERED 
(
	[DeliveryMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DeliveryVessel]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryVessel](
	[DeliveryVesselId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[DeliveryTypeCode] [nvarchar](50) NULL,
	[RegistrationCode] [nvarchar](150) NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_DeliveryVessel] PRIMARY KEY CLUSTERED 
(
	[DeliveryVesselId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentType](
	[DocumentTypeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TypeName] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](350) NULL,
 CONSTRAINT [PK_DocumentType] PRIMARY KEY CLUSTERED 
(
	[DocumentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Employee]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[EmployeeId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[EmployeeNo] [nvarchar](150) NOT NULL,
	[JobRoleId] [bigint] NOT NULL,
	[DateHired] [date] NOT NULL,
	[DateLeft] [date] NULL,
	[StoreOutletId] [int] NOT NULL,
	[StoreAddressId] [bigint] NOT NULL,
	[StoreDepartmentId] [bigint] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmployeeAssigment]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeAssigment](
	[EmployeeAssignmentId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[JobRoleId] [bigint] NOT NULL,
	[StoreOutletId] [int] NOT NULL,
	[FromDate] [date] NOT NULL,
	[ToDate] [date] NULL,
	[LastUpdate] [timestamp] NOT NULL,
 CONSTRAINT [PK_EmployeeAssigment] PRIMARY KEY CLUSTERED 
(
	[EmployeeAssignmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmployeeDocument]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeDocument](
	[EmployeeDocumentId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[DocumentTypeId] [int] NOT NULL,
	[FilePath] [nvarchar](max) NOT NULL,
	[DateAttached] [timestamp] NOT NULL,
 CONSTRAINT [PK_EmployeeDocuments] PRIMARY KEY CLUSTERED 
(
	[EmployeeDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmployeeSalesLog]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeSalesLog](
	[EmployeeSalesLogId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
	[TotalSales] [float] NOT NULL,
	[SalesDate] [date] NOT NULL,
 CONSTRAINT [PK_EmployeeSalesLog] PRIMARY KEY CLUSTERED 
(
	[EmployeeSalesLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ImageView]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageView](
	[ImageViewId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](350) NULL,
 CONSTRAINT [PK_ImageView] PRIMARY KEY CLUSTERED 
(
	[ImageViewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Invoice]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoice](
	[InvoiceId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ReferenceCode] [nvarchar](150) NULL,
	[PurchaseOrderId] [bigint] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DueDate] [date] NOT NULL,
	[DateSent] [date] NOT NULL,
	[Attachment] [nvarchar](max) NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[InvoiceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IssueType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IssueType](
	[IssueTypeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](350) NULL,
 CONSTRAINT [PK_IssueType] PRIMARY KEY CLUSTERED 
(
	[IssueTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemPrice]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemPrice](
	[ItemPriceId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
	[Price] [float] NOT NULL,
	[MinimumQuantity] [int] NOT NULL,
	[Remark] [nvarchar](250) NULL,
	[UoMId] [bigint] NOT NULL,
 CONSTRAINT [PK_ItemPrice] PRIMARY KEY CLUSTERED 
(
	[ItemPriceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ItemReview]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemReview](
	[ItemReviewId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ReviewerName] [nvarchar](350) NOT NULL,
	[ReviewComment] [nvarchar](max) NOT NULL,
	[Rating] [int] NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
 CONSTRAINT [PK_ItemReview] PRIMARY KEY CLUSTERED 
(
	[ItemReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[JobRole]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobRole](
	[JobRoleId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[JobTitle] [nvarchar](250) NOT NULL,
	[JobDescription] [nvarchar](max) NOT NULL,
	[Responsibilities] [nvarchar](max) NULL,
	[MinQualification] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_JobRoles] PRIMARY KEY CLUSTERED 
(
	[JobRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PaymentType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentType](
	[PaymentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_PaymentOptions_1] PRIMARY KEY CLUSTERED 
(
	[PaymentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Person]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Person](
	[PersonId] [bigint] IDENTITY(1,1) NOT NULL,
	[SalutationId] [int] NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[OtherNames] [nvarchar](200) NOT NULL,
	[Gender] [nvarchar](15) NOT NULL,
	[Birthday] [date] NULL,
	[PhotofilePath] [nvarchar](max) NULL,
	[UserId] [nvarchar](128) NULL,
 CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PersonContact]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonContact](
	[PersonContactId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[ContactTypeId] [int] NOT NULL,
	[ContactValue] [nvarchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[ContactTagName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PersonPhoneContact] PRIMARY KEY CLUSTERED 
(
	[PersonContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrder](
	[PurchaseOrderId] [bigint] NOT NULL,
	[StoreOutletId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[SupplierId] [bigint] NOT NULL,
	[StatusCode] [int] NOT NULL,
	[DateTimePlaced] [datetime] NOT NULL,
	[DerivedTotalCost] [float] NULL,
	[GeneratedById] [bigint] NOT NULL,
	[ExpectedDeliveryDate] [datetime] NOT NULL,
	[ActualDeliveryDate] [datetime] NULL,
 CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrderItem]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderItem](
	[PurchaseOrderItemId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[PurchaseOrderId] [bigint] NOT NULL,
	[StoreItemId] [bigint] NOT NULL,
	[StoreOutletId] [int] NOT NULL,
	[SerialNumber] [nchar](100) NOT NULL,
	[QuantityOrdered] [int] NOT NULL,
	[QuantityDelivered] [int] NOT NULL,
	[StatusCode] [int] NOT NULL,
 CONSTRAINT [PK_PurchaseOrderItem_1] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrderItemDelivery]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderItemDelivery](
	[PurchaseOrderItemDeliveryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[DeliveryId] [bigint] NOT NULL,
	[PurchaseOrderItemId] [bigint] NOT NULL,
	[QuantityDelivered] [int] NOT NULL,
	[DateDelivered] [datetime] NOT NULL,
 CONSTRAINT [PK_PurchaseOrderItemDelivery] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderItemDeliveryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseOrderPayment]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrderPayment](
	[PurchaseOrderPaymentId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StoreTransactionId] [bigint] NOT NULL,
	[PurchaseOrderId] [bigint] NOT NULL,
	[AmountPaid] [float] NULL,
	[DateMade] [datetime] NOT NULL,
	[InvoiceFilePath] [nchar](10) NULL,
	[Remark] [nvarchar](350) NULL,
 CONSTRAINT [PK_Purchases] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderPaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Register]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Register](
	[RegisterId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[CurrentOutletId] [int] NOT NULL,
 CONSTRAINT [PK_Register] PRIMARY KEY CLUSTERED 
(
	[RegisterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReturnedProduct]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReturnedProduct](
	[ReturnedProductId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[IssueTypeId] [int] NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
	[SalesId] [bigint] NOT NULL,
	[DateReturned] [datetime] NOT NULL,
 CONSTRAINT [PK_ReturnedProduct] PRIMARY KEY CLUSTERED 
(
	[ReturnedProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sale]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sale](
	[SaleId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[AmountDue] [float] NOT NULL,
	[Status] [int] NOT NULL,
	[Date] [date] NOT NULL,
	[RegisterId] [int] NOT NULL,
	[EmployeeId] [bigint] NOT NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[SaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SalePayment]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SalePayment](
	[SalePaymentId] [bigint] IDENTITY(1,1) NOT NULL,
	[SaleId] [bigint] NOT NULL,
	[AmountPaid] [float] NOT NULL,
	[DatePaid] [datetime] NOT NULL,
 CONSTRAINT [PK_OrderPayments] PRIMARY KEY CLUSTERED 
(
	[SalePaymentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SaleTransaction]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleTransaction](
	[SaleTransactionId] [bigint] IDENTITY(1,1) NOT NULL,
	[SaleId] [bigint] NOT NULL,
	[StoreTransactionId] [bigint] NOT NULL,
 CONSTRAINT [PK_SaleTransaction] PRIMARY KEY CLUSTERED 
(
	[SaleTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Salutation]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Salutation](
	[SalutationId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Salutation] PRIMARY KEY CLUSTERED 
(
	[SalutationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ShopingCartItem]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShopingCartItem](
	[ShopingCartItemId] [bigint] IDENTITY(1,1) NOT NULL,
	[ShopingCartId] [bigint] NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
	[UnitPrice] [float] NOT NULL,
	[QuantityOrdered] [int] NOT NULL,
	[UoMId] [bigint] NOT NULL,
	[Discount] [nvarchar](50) NULL,
 CONSTRAINT [PK_OrderProducts] PRIMARY KEY CLUSTERED 
(
	[ShopingCartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ShoppingCart]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShoppingCart](
	[ShoppingCartId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [bigint] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_ShoppingCart] PRIMARY KEY CLUSTERED 
(
	[ShoppingCartId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StockUpload]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockUpload](
	[StockUploadId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
	[ImageViewId] [int] NOT NULL,
	[ImagePath] [nvarchar](max) NOT NULL,
	[LastUpdated] [date] NOT NULL,
 CONSTRAINT [PK_StockUpload] PRIMARY KEY CLUSTERED 
(
	[StockUploadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreAddress]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreAddress](
	[StoreAddressId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StreetNo] [nvarchar](max) NOT NULL,
	[StoreCityId] [bigint] NOT NULL,
 CONSTRAINT [PK_Addresses] PRIMARY KEY CLUSTERED 
(
	[StoreAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreBank]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreBank](
	[BankId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SortCode] [nvarchar](50) NULL,
	[ShortName] [nvarchar](50) NULL,
	[FullName] [nvarchar](250) NOT NULL,
	[LogoPath] [nvarchar](max) NULL,
	[LastUpdated] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Banks] PRIMARY KEY CLUSTERED 
(
	[BankId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreCity]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreCity](
	[StoreCityId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[StoreStateId] [bigint] NOT NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[StoreCityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreContact]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreContact](
	[StoreContactId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[StoreDepartmentId] [bigint] NOT NULL,
	[DateCreated] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_StoreContact] PRIMARY KEY CLUSTERED 
(
	[StoreContactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreContactTag]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreContactTag](
	[ContactTagId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PhoneTag] PRIMARY KEY CLUSTERED 
(
	[ContactTagId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreCountry]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreCountry](
	[StoreCountryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[StoreCountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreCurrency]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreCurrency](
	[StoreCurrencyId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Symbol] [nvarchar](20) NOT NULL,
	[StoreCountryId] [bigint] NOT NULL,
 CONSTRAINT [PK_StoreCurrency] PRIMARY KEY CLUSTERED 
(
	[StoreCurrencyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreCustomerType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreCustomerType](
	[StoreCustomerTypeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_CustomerType] PRIMARY KEY CLUSTERED 
(
	[StoreCustomerTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreDepartment]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreDepartment](
	[StoreDepartmentId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_CompanyDepartments] PRIMARY KEY CLUSTERED 
(
	[StoreDepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItem]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItem](
	[StoreItemId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StoreItemBrandId] [bigint] NOT NULL,
	[StoreItemTypeId] [bigint] NOT NULL,
	[StoreItemCategoryId] [bigint] NOT NULL,
	[ChartOfAccountId] [int] NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](350) NULL,
	[ParentItemId] [bigint] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[StoreItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemBrand]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemBrand](
	[StoreItemBrandId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[LastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Brands] PRIMARY KEY CLUSTERED 
(
	[StoreItemBrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemCategory]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemCategory](
	[StoreItemCategoryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParentCategoryId] [bigint] NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[ImagePath] [nvarchar](max) NULL,
	[LastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_ProductCategories] PRIMARY KEY CLUSTERED 
(
	[StoreItemCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemIssue]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemIssue](
	[StoreItemIssueId] [bigint] NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
	[IssueTypeId] [int] NOT NULL,
	[Reason] [nvarchar](550) NOT NULL,
	[IssueDate] [datetime] NOT NULL,
	[ResolutionStatus] [int] NOT NULL,
 CONSTRAINT [PK_StoreItemIssue] PRIMARY KEY CLUSTERED 
(
	[StoreItemIssueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemSold]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemSold](
	[StoreItemSoldId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StoreItemStockId] [bigint] NOT NULL,
	[SaleId] [bigint] NOT NULL,
	[QuantitySold] [int] NOT NULL,
	[AmountSold] [float] NOT NULL,
	[UoMId] [int] NOT NULL,
	[DateSold] [datetime] NOT NULL,
 CONSTRAINT [PK_StoreItemSold] PRIMARY KEY CLUSTERED 
(
	[StoreItemSoldId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemStock]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemStock](
	[StoreItemStockId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StoreOutletId] [int] NOT NULL,
	[SKU] [nvarchar](50) NOT NULL,
	[StoreItemVariationId] [int] NULL,
	[StoreItemVariationValueId] [int] NULL,
	[StoreItemId] [bigint] NOT NULL,
	[QuantityInStock] [int] NOT NULL,
	[CostPrice] [float] NULL,
	[ReorderLevel] [int] NULL,
	[ReorderQuantity] [int] NULL,
	[LastUpdated] [datetime] NOT NULL,
	[ShelfLocation] [nvarchar](150) NULL,
	[ExpirationDate] [datetime] NULL,
	[TotalQuantityAlreadySold] [int] NULL,
	[StoreCurrencyId] [bigint] NOT NULL,
 CONSTRAINT [PK_ProductsInventory] PRIMARY KEY CLUSTERED 
(
	[StoreItemStockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemSupplier]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemSupplier](
	[SupplierId] [bigint] NOT NULL,
	[StoreItemId] [bigint] NOT NULL,
	[MinSupplyQuantity] [int] NOT NULL,
	[MaxSupplyQuantity] [int] NOT NULL,
	[UnitOfMeasurement] [int] NULL,
	[Rating] [float] NULL,
	[StandardPrice] [nvarchar](150) NOT NULL,
	[ApplicableDiscount] [float] NOT NULL,
	[Comment] [nvarchar](350) NULL,
 CONSTRAINT [PK_ProductSupplier] PRIMARY KEY CLUSTERED 
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemType](
	[StoreItemTypeId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[SampleImagePath] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProductTypes] PRIMARY KEY CLUSTERED 
(
	[StoreItemTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemVariation]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemVariation](
	[StoreItemVariationId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[VariationProperty] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_StoreItemVariantion] PRIMARY KEY CLUSTERED 
(
	[StoreItemVariationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreItemVariationValue]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreItemVariationValue](
	[StoreItemVariationValueId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Value] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_StoreItemVariationValues] PRIMARY KEY CLUSTERED 
(
	[StoreItemVariationValueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreOutlet]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreOutlet](
	[StoreOutletId] [int] IDENTITY(1,1) NOT NULL,
	[OutletName] [nvarchar](200) NOT NULL,
	[StoreAddressId] [bigint] NOT NULL,
	[IsMainOutlet] [bit] NOT NULL,
	[DefaultTax] [float] NULL,
	[IsOperational] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[FacebookHandle] [nvarchar](100) NULL,
	[TwitterHandle] [nvarchar](100) NULL,
 CONSTRAINT [PK_StoreOutlets] PRIMARY KEY CLUSTERED 
(
	[StoreOutletId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreOutletCoupon]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreOutletCoupon](
	[StoreOutletCouponId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[StoreOutletId] [int] NOT NULL,
	[CouponId] [bigint] NOT NULL,
 CONSTRAINT [PK_OutletCoupon] PRIMARY KEY CLUSTERED 
(
	[StoreOutletCouponId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StorePaymentGateway]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StorePaymentGateway](
	[StorePaymentGatewayId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GatewayName] [nvarchar](250) NOT NULL,
	[LogoPath] [nvarchar](250) NULL,
 CONSTRAINT [PK_StorePaymentGateways_1] PRIMARY KEY CLUSTERED 
(
	[StorePaymentGatewayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StorePaymentMethod]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StorePaymentMethod](
	[StorePaymentMethodId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_PaymentOptions] PRIMARY KEY CLUSTERED 
(
	[StorePaymentMethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreState]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreState](
	[StoreStateId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](250) NOT NULL,
	[StoreCountryId] [bigint] NOT NULL,
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[StoreStateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreTransaction]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreTransaction](
	[StoreTransactionId] [bigint] IDENTITY(1,1) NOT NULL,
	[StoreTransactionTypeId] [int] NOT NULL,
	[StorePaymentMethodId] [int] NOT NULL,
	[EffectedByEmployeeId] [bigint] NOT NULL,
	[TransactionAmount] [float] NOT NULL,
	[TransactionDate] [datetime] NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[StoreOutletId] [int] NOT NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[StoreTransactionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[StoreTransactionType]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreTransactionType](
	[StoreTransactionTypeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Action] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_StoreTransactionTypes] PRIMARY KEY CLUSTERED 
(
	[StoreTransactionTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SubscriptionSetting]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubscriptionSetting](
	[StoreId] [bigint] NOT NULL,
	[SecreteKey] [nvarchar](350) NOT NULL,
	[DatabaseSpace] [float] NOT NULL,
	[FileStorageSpace] [float] NOT NULL,
	[Url] [nvarchar](max) NOT NULL,
	[DateSubscribed] [date] NOT NULL,
	[ExpiryDate] [date] NOT NULL,
	[SubscriptionStatus] [int] NOT NULL,
 CONSTRAINT [PK_SubscriptionSetting] PRIMARY KEY CLUSTERED 
(
	[StoreId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Supplier](
	[SupplierId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[DateJoined] [date] NOT NULL,
	[LastSupplyDate] [date] NULL,
	[Note] [nchar](10) NULL,
	[CompanyName] [nvarchar](100) NOT NULL,
	[TIN] [nvarchar](50) NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[SupplierId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SupplierAddress]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierAddress](
	[SupplierAddressId] [bigint] NOT NULL,
	[SupplierId] [bigint] NOT NULL,
	[StoreAddressId] [bigint] NOT NULL,
	[DateFrom] [datetime] NOT NULL,
	[DateTo] [datetime] NOT NULL,
	[AddressTypeId] [int] NOT NULL,
 CONSTRAINT [PK_SupplierAddress] PRIMARY KEY CLUSTERED 
(
	[SupplierAddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UnitsOfMeasurement]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnitsOfMeasurement](
	[UnitOfMeasurementId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UoMCode] [nvarchar](50) NOT NULL,
	[UoMDescription] [nvarchar](350) NULL,
 CONSTRAINT [PK_UnitsOfMeasurement_1] PRIMARY KEY CLUSTERED 
(
	[UnitOfMeasurementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Warehouse]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Warehouse](
	[WarehouseId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[StoreAddressId] [bigint] NOT NULL,
	[CompanyId] [bigint] NOT NULL,
 CONSTRAINT [PK_Warehouse] PRIMARY KEY CLUSTERED 
(
	[WarehouseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WayBill]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WayBill](
	[WayBillId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[AuthorisedById] [bigint] NOT NULL,
	[SourceId] [bigint] NOT NULL,
	[DeliveryId] [bigint] NOT NULL,
	[TotalItems] [int] NOT NULL,
	[DatePacked] [datetime] NOT NULL,
	[PackedById] [bigint] NOT NULL,
	[CheckedById] [bigint] NOT NULL,
 CONSTRAINT [PK_WayBill] PRIMARY KEY CLUSTERED 
(
	[WayBillId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WayBillItem]    Script Date: 18/01/2015 01:41:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WayBillItem](
	[WayBillItemId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[WayBillId] [bigint] NOT NULL,
	[StoreItemId] [bigint] NOT NULL,
	[Quantity] [int] NOT NULL,
	[Remark] [nvarchar](max) NULL,
 CONSTRAINT [PK_WayBillItem] PRIMARY KEY CLUSTERED 
(
	[WayBillItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[AccountGroup] ON 

INSERT [dbo].[AccountGroup] ([AccountGroupId], [Name]) VALUES (1, N'Phones & Accessories')
INSERT [dbo].[AccountGroup] ([AccountGroupId], [Name]) VALUES (2, N'Televisions & Projectors')
INSERT [dbo].[AccountGroup] ([AccountGroupId], [Name]) VALUES (3, N'Clothes')
INSERT [dbo].[AccountGroup] ([AccountGroupId], [Name]) VALUES (4, N'Footwears')
SET IDENTITY_INSERT [dbo].[AccountGroup] OFF
SET IDENTITY_INSERT [dbo].[ChartOfAccount] ON 

INSERT [dbo].[ChartOfAccount] ([ChartOfAccountId], [AccountGroupId], [AccountType], [AccountCode]) VALUES (1, 1, N'Smart Phone Purchases', N'PHS1002')
INSERT [dbo].[ChartOfAccount] ([ChartOfAccountId], [AccountGroupId], [AccountType], [AccountCode]) VALUES (2, 2, N'Television Sales', N'TVS1001')
INSERT [dbo].[ChartOfAccount] ([ChartOfAccountId], [AccountGroupId], [AccountType], [AccountCode]) VALUES (3, 2, N'Television Purchases', N'TVS1003')
SET IDENTITY_INSERT [dbo].[ChartOfAccount] OFF
SET IDENTITY_INSERT [dbo].[DeliveryMethod] ON 

INSERT [dbo].[DeliveryMethod] ([DeliveryMethodId], [MethodTitle], [Description]) VALUES (1, N'Catch ''n'' Carry', N'Catch ''n'' Carry')
INSERT [dbo].[DeliveryMethod] ([DeliveryMethodId], [MethodTitle], [Description]) VALUES (2, N'Door-to-Door', N'Door-to-Door')
SET IDENTITY_INSERT [dbo].[DeliveryMethod] OFF
SET IDENTITY_INSERT [dbo].[DocumentType] ON 

INSERT [dbo].[DocumentType] ([DocumentTypeId], [TypeName], [Description]) VALUES (1, N'Curriculum Vitae', N'Curriculum Vitae')
INSERT [dbo].[DocumentType] ([DocumentTypeId], [TypeName], [Description]) VALUES (2, N'Referece', NULL)
SET IDENTITY_INSERT [dbo].[DocumentType] OFF
SET IDENTITY_INSERT [dbo].[Employee] ON 

INSERT [dbo].[Employee] ([EmployeeId], [PersonId], [EmployeeNo], [JobRoleId], [DateHired], [DateLeft], [StoreOutletId], [StoreAddressId], [StoreDepartmentId], [Status]) VALUES (1, 1, N'0001', 4, CAST(0x2B380B00 AS Date), NULL, 1, 10002, 1, 1)
SET IDENTITY_INSERT [dbo].[Employee] OFF
SET IDENTITY_INSERT [dbo].[ImageView] ON 

INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (1, N'Side View', NULL)
INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (2, N'Back View', NULL)
INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (3, N'Top View', NULL)
INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (4, N'Bottom View', NULL)
INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (5, N'Front View', NULL)
INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (6, N'Front View', NULL)
INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (7, N'Front View', NULL)
INSERT [dbo].[ImageView] ([ImageViewId], [Name], [Description]) VALUES (8, N'Top View', NULL)
SET IDENTITY_INSERT [dbo].[ImageView] OFF
SET IDENTITY_INSERT [dbo].[ItemPrice] ON 

INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (1, 10006, 56545, 1, NULL, 2)
INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (2, 10007, 43500, 1, NULL, 2)
INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (3, 10007, 35600, 2, NULL, 2)
INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (4, 10007, 24300, 10, NULL, 2)
INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (5, 10009, 47800, 1, NULL, 2)
INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (6, 10010, 47800, 1, NULL, 2)
INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (7, 10011, 32700, 1, NULL, 2)
INSERT [dbo].[ItemPrice] ([ItemPriceId], [StoreItemStockId], [Price], [MinimumQuantity], [Remark], [UoMId]) VALUES (8, 10012, 189000, 1, NULL, 2)
SET IDENTITY_INSERT [dbo].[ItemPrice] OFF
SET IDENTITY_INSERT [dbo].[JobRole] ON 

INSERT [dbo].[JobRole] ([JobRoleId], [JobTitle], [JobDescription], [Responsibilities], [MinQualification]) VALUES (4, N'Cashier                                                                                             ', N'POS Operator', N'Collect payment for sales', N'SSCE      ')
INSERT [dbo].[JobRole] ([JobRoleId], [JobTitle], [JobDescription], [Responsibilities], [MinQualification]) VALUES (6, N'Sales Attendant                                                                                     ', N'Provide Sales Support to customers', N'Monitor roles', N'SSCE      ')
INSERT [dbo].[JobRole] ([JobRoleId], [JobTitle], [JobDescription], [Responsibilities], [MinQualification]) VALUES (7, N'Manager                                                                                             ', N'Manage the daily activities of the store', N'Oversee operations', N'HND       ')
SET IDENTITY_INSERT [dbo].[JobRole] OFF
SET IDENTITY_INSERT [dbo].[Person] ON 

INSERT [dbo].[Person] ([PersonId], [SalutationId], [LastName], [OtherNames], [Gender], [Birthday], [PhotofilePath], [UserId]) VALUES (1, 1, N'Okafor', N'Anthony Chibuike', N'Male', CAST(0x0C1B0B00 AS Date), NULL, NULL)
INSERT [dbo].[Person] ([PersonId], [SalutationId], [LastName], [OtherNames], [Gender], [Birthday], [PhotofilePath], [UserId]) VALUES (2, 3, N'Azubuike', N'Alex Chinaza', N'Male', CAST(0xF6140B00 AS Date), NULL, NULL)
SET IDENTITY_INSERT [dbo].[Person] OFF
SET IDENTITY_INSERT [dbo].[Register] ON 

INSERT [dbo].[Register] ([RegisterId], [Name], [CurrentOutletId]) VALUES (6, N'PO001', 1)
INSERT [dbo].[Register] ([RegisterId], [Name], [CurrentOutletId]) VALUES (7, N'PO002', 1)
INSERT [dbo].[Register] ([RegisterId], [Name], [CurrentOutletId]) VALUES (9, N'PO003', 1)
INSERT [dbo].[Register] ([RegisterId], [Name], [CurrentOutletId]) VALUES (10, N'PO004', 1)
SET IDENTITY_INSERT [dbo].[Register] OFF
SET IDENTITY_INSERT [dbo].[Sale] ON 

INSERT [dbo].[Sale] ([SaleId], [AmountDue], [Status], [Date], [RegisterId], [EmployeeId]) VALUES (3, 219745, 1, CAST(0x74390B00 AS Date), 6, 1)
SET IDENTITY_INSERT [dbo].[Sale] OFF
SET IDENTITY_INSERT [dbo].[SalePayment] ON 

INSERT [dbo].[SalePayment] ([SalePaymentId], [SaleId], [AmountPaid], [DatePaid]) VALUES (2, 3, 219745, CAST(0x0000A419011FCE91 AS DateTime))
SET IDENTITY_INSERT [dbo].[SalePayment] OFF
SET IDENTITY_INSERT [dbo].[SaleTransaction] ON 

INSERT [dbo].[SaleTransaction] ([SaleTransactionId], [SaleId], [StoreTransactionId]) VALUES (2, 3, 2)
SET IDENTITY_INSERT [dbo].[SaleTransaction] OFF
SET IDENTITY_INSERT [dbo].[Salutation] ON 

INSERT [dbo].[Salutation] ([SalutationId], [Name]) VALUES (1, N'Mr.')
INSERT [dbo].[Salutation] ([SalutationId], [Name]) VALUES (2, N'Mrs.')
INSERT [dbo].[Salutation] ([SalutationId], [Name]) VALUES (3, N'Ms.')
INSERT [dbo].[Salutation] ([SalutationId], [Name]) VALUES (4, N'Dr.')
INSERT [dbo].[Salutation] ([SalutationId], [Name]) VALUES (5, N'Chief')
INSERT [dbo].[Salutation] ([SalutationId], [Name]) VALUES (6, N'Engr.')
INSERT [dbo].[Salutation] ([SalutationId], [Name]) VALUES (7, N'Barr.')
SET IDENTITY_INSERT [dbo].[Salutation] OFF
SET IDENTITY_INSERT [dbo].[StockUpload] ON 

INSERT [dbo].[StockUpload] ([StockUploadId], [StoreItemStockId], [ImageViewId], [ImagePath], [LastUpdated]) VALUES (6, 10006, 1, N'~/Images/StoreItemStock/20141220080219159_9f9f02f8-26c7-4772-8eda-08dcc684948c.jpg', CAST(0x62390B00 AS Date))
INSERT [dbo].[StockUpload] ([StockUploadId], [StoreItemStockId], [ImageViewId], [ImagePath], [LastUpdated]) VALUES (7, 10007, 1, N'~/Images/StoreItemStock/20141211193001170_6a472281-9635-40a9-b30e-a93c32ef7d82.jpg', CAST(0x00000000 AS Date))
INSERT [dbo].[StockUpload] ([StockUploadId], [StoreItemStockId], [ImageViewId], [ImagePath], [LastUpdated]) VALUES (8, 10006, 7, N'~/Images/StoreItemStock/20141220080642613_7c325c85-7d38-41c9-9ec9-8b1efa9d39ae.png', CAST(0x62390B00 AS Date))
INSERT [dbo].[StockUpload] ([StockUploadId], [StoreItemStockId], [ImageViewId], [ImagePath], [LastUpdated]) VALUES (9, 10006, 8, N'~/Images/StoreItemStock/20141220080704121_e3d4d299-7494-4c56-806f-79e798287442.jpeg', CAST(0x62390B00 AS Date))
SET IDENTITY_INSERT [dbo].[StockUpload] OFF
SET IDENTITY_INSERT [dbo].[StoreAddress] ON 

INSERT [dbo].[StoreAddress] ([StoreAddressId], [StreetNo], [StoreCityId]) VALUES (2, N'14, Kingsley Odo street, Lekki Phase 1', 1)
INSERT [dbo].[StoreAddress] ([StoreAddressId], [StreetNo], [StoreCityId]) VALUES (10002, N'Suite J251, Ikota Shopping Complex', 1)
SET IDENTITY_INSERT [dbo].[StoreAddress] OFF
SET IDENTITY_INSERT [dbo].[StoreBank] ON 

INSERT [dbo].[StoreBank] ([BankId], [SortCode], [ShortName], [FullName], [LogoPath], [LastUpdated]) VALUES (1, N'5546771233', N'MSB', N'Main Street Bank Plc', NULL, N'2014/40/12 05:40:59 PM')
INSERT [dbo].[StoreBank] ([BankId], [SortCode], [ShortName], [FullName], [LogoPath], [LastUpdated]) VALUES (2, N'3785', N'GTB', N'GTBank Plc', N'~/Logo/Banks/20141220124858057_0928705f-87c3-4d8c-b242-455114aa1ab9.jpg', N'2014/48/20 12:48:57 PM')
SET IDENTITY_INSERT [dbo].[StoreBank] OFF
SET IDENTITY_INSERT [dbo].[StoreCity] ON 

INSERT [dbo].[StoreCity] ([StoreCityId], [Name], [StoreStateId]) VALUES (1, N'Lekki', 1)
INSERT [dbo].[StoreCity] ([StoreCityId], [Name], [StoreStateId]) VALUES (2, N'Ikoyi', 1)
INSERT [dbo].[StoreCity] ([StoreCityId], [Name], [StoreStateId]) VALUES (3, N'Nsukka', 2)
INSERT [dbo].[StoreCity] ([StoreCityId], [Name], [StoreStateId]) VALUES (4, N'Umuahia', 3)
SET IDENTITY_INSERT [dbo].[StoreCity] OFF
SET IDENTITY_INSERT [dbo].[StoreCountry] ON 

INSERT [dbo].[StoreCountry] ([StoreCountryId], [Name]) VALUES (1, N'Nigeria')
INSERT [dbo].[StoreCountry] ([StoreCountryId], [Name]) VALUES (2, N'Ghannah')
INSERT [dbo].[StoreCountry] ([StoreCountryId], [Name]) VALUES (3, N'Algeria')
INSERT [dbo].[StoreCountry] ([StoreCountryId], [Name]) VALUES (4, N'South Africa')
INSERT [dbo].[StoreCountry] ([StoreCountryId], [Name]) VALUES (5, N'USA')
SET IDENTITY_INSERT [dbo].[StoreCountry] OFF
SET IDENTITY_INSERT [dbo].[StoreCurrency] ON 

INSERT [dbo].[StoreCurrency] ([StoreCurrencyId], [Name], [Symbol], [StoreCountryId]) VALUES (1, N'Naira', N'#', 1)
INSERT [dbo].[StoreCurrency] ([StoreCurrencyId], [Name], [Symbol], [StoreCountryId]) VALUES (2, N'Dollar', N'$', 5)
SET IDENTITY_INSERT [dbo].[StoreCurrency] OFF
SET IDENTITY_INSERT [dbo].[StoreCustomerType] ON 

INSERT [dbo].[StoreCustomerType] ([StoreCustomerTypeId], [Code], [Name], [Description]) VALUES (1, N'PG0001', N'Premium/Gold', N'Has highest level of commitment and patronage')
SET IDENTITY_INSERT [dbo].[StoreCustomerType] OFF
SET IDENTITY_INSERT [dbo].[StoreDepartment] ON 

INSERT [dbo].[StoreDepartment] ([StoreDepartmentId], [Name], [Description]) VALUES (1, N'Sales', N'Sales Department')
INSERT [dbo].[StoreDepartment] ([StoreDepartmentId], [Name], [Description]) VALUES (2, N'Delivery', N'Delivery')
INSERT [dbo].[StoreDepartment] ([StoreDepartmentId], [Name], [Description]) VALUES (3, N'Stocking $ Warehousing', N'Stocking $ Warehousing')
INSERT [dbo].[StoreDepartment] ([StoreDepartmentId], [Name], [Description]) VALUES (4, N'Advertising/Marketting', N'Advertising/Marketting')
SET IDENTITY_INSERT [dbo].[StoreDepartment] OFF
SET IDENTITY_INSERT [dbo].[StoreItem] ON 

INSERT [dbo].[StoreItem] ([StoreItemId], [StoreItemBrandId], [StoreItemTypeId], [StoreItemCategoryId], [ChartOfAccountId], [Name], [Description], [ParentItemId]) VALUES (1, 3, 1, 1, 1, N'Nokia Lumia', N'Purchase of 10 units of Nokia Lumia', NULL)
INSERT [dbo].[StoreItem] ([StoreItemId], [StoreItemBrandId], [StoreItemTypeId], [StoreItemCategoryId], [ChartOfAccountId], [Name], [Description], [ParentItemId]) VALUES (2, 2, 1, 1, 1, N'Nokia X7', N'Nokia X7 windows mobile, Dual Sim', 1)
INSERT [dbo].[StoreItem] ([StoreItemId], [StoreItemBrandId], [StoreItemTypeId], [StoreItemCategoryId], [ChartOfAccountId], [Name], [Description], [ParentItemId]) VALUES (4, 1, 1, 1, 3, N'Nokia G75', NULL, 2)
INSERT [dbo].[StoreItem] ([StoreItemId], [StoreItemBrandId], [StoreItemTypeId], [StoreItemCategoryId], [ChartOfAccountId], [Name], [Description], [ParentItemId]) VALUES (5, 4, 10002, 2, 2, N'Samsung XDS3', N'Samsung Smart flat screen Television', NULL)
SET IDENTITY_INSERT [dbo].[StoreItem] OFF
SET IDENTITY_INSERT [dbo].[StoreItemBrand] ON 

INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (1, N'Nokia', N'Nokia', CAST(0x0000A3E200FD2123 AS DateTime))
INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (2, N'Siemens', N'Siemens', CAST(0x0000A3E200FD2F5A AS DateTime))
INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (3, N'Sony', NULL, CAST(0x0000A3E200FD3A0F AS DateTime))
INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (4, N'Samsung', NULL, CAST(0x0000A3E200FD4845 AS DateTime))
INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (5, N'LG', NULL, CAST(0x0000A3E200FD4F7E AS DateTime))
INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (6, N'Tecno', NULL, CAST(0x0000A3E200FD5969 AS DateTime))
INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (7, N'Infinix', NULL, CAST(0x0000A3E200FD6B82 AS DateTime))
INSERT [dbo].[StoreItemBrand] ([StoreItemBrandId], [Name], [Description], [LastUpdated]) VALUES (8, N'Huawei', NULL, CAST(0x0000A3E200FD85C1 AS DateTime))
SET IDENTITY_INSERT [dbo].[StoreItemBrand] OFF
SET IDENTITY_INSERT [dbo].[StoreItemCategory] ON 

INSERT [dbo].[StoreItemCategory] ([StoreItemCategoryId], [ParentCategoryId], [Name], [Description], [ImagePath], [LastUpdated]) VALUES (1, 0, N'Windows Phone', N'Windows Phone - Nokia Lumia', NULL, CAST(0x0000A3E2017B0E69 AS DateTime))
INSERT [dbo].[StoreItemCategory] ([StoreItemCategoryId], [ParentCategoryId], [Name], [Description], [ImagePath], [LastUpdated]) VALUES (2, 0, N'Flat Screen Television', N'Flat Screen Televisions', NULL, CAST(0x0000A3E9010E506F AS DateTime))
INSERT [dbo].[StoreItemCategory] ([StoreItemCategoryId], [ParentCategoryId], [Name], [Description], [ImagePath], [LastUpdated]) VALUES (4, 0, N'Cathod ray Television', N'Cathode Ray TVs', NULL, CAST(0x0000A3E90107E36F AS DateTime))
INSERT [dbo].[StoreItemCategory] ([StoreItemCategoryId], [ParentCategoryId], [Name], [Description], [ImagePath], [LastUpdated]) VALUES (5, NULL, N'Cathode Ray Monitor', N'Cathode Ray Monitors', NULL, CAST(0x0000A3E9010D8FC1 AS DateTime))
INSERT [dbo].[StoreItemCategory] ([StoreItemCategoryId], [ParentCategoryId], [Name], [Description], [ImagePath], [LastUpdated]) VALUES (6, NULL, N'Flat Screen Monitor', N'Flat Screen Monitor', NULL, CAST(0x0000A3E9010E6D25 AS DateTime))
SET IDENTITY_INSERT [dbo].[StoreItemCategory] OFF
SET IDENTITY_INSERT [dbo].[StoreItemSold] ON 

INSERT [dbo].[StoreItemSold] ([StoreItemSoldId], [StoreItemStockId], [SaleId], [QuantitySold], [AmountSold], [UoMId], [DateSold]) VALUES (1, 10006, 3, 1, 56545, 2, CAST(0x0000A419012016FC AS DateTime))
INSERT [dbo].[StoreItemSold] ([StoreItemSoldId], [StoreItemStockId], [SaleId], [QuantitySold], [AmountSold], [UoMId], [DateSold]) VALUES (2, 10007, 3, 3, 130500, 2, CAST(0x0000A41901204F70 AS DateTime))
INSERT [dbo].[StoreItemSold] ([StoreItemSoldId], [StoreItemStockId], [SaleId], [QuantitySold], [AmountSold], [UoMId], [DateSold]) VALUES (3, 10011, 3, 1, 32700, 2, CAST(0x0000A4190120609F AS DateTime))
SET IDENTITY_INSERT [dbo].[StoreItemSold] OFF
SET IDENTITY_INSERT [dbo].[StoreItemStock] ON 

INSERT [dbo].[StoreItemStock] ([StoreItemStockId], [StoreOutletId], [SKU], [StoreItemVariationId], [StoreItemVariationValueId], [StoreItemId], [QuantityInStock], [CostPrice], [ReorderLevel], [ReorderQuantity], [LastUpdated], [ShelfLocation], [ExpirationDate], [TotalQuantityAlreadySold], [StoreCurrencyId]) VALUES (10006, 1, N'112', 2, 2, 1, 5436, 0, 125, 3000, CAST(0x0000A407011D0D35 AS DateTime), N'45TR', NULL, NULL, 1)
INSERT [dbo].[StoreItemStock] ([StoreItemStockId], [StoreOutletId], [SKU], [StoreItemVariationId], [StoreItemVariationValueId], [StoreItemId], [QuantityInStock], [CostPrice], [ReorderLevel], [ReorderQuantity], [LastUpdated], [ShelfLocation], [ExpirationDate], [TotalQuantityAlreadySold], [StoreCurrencyId]) VALUES (10007, 1, N'113', 1, 3, 1, 419, 0, 50, 300, CAST(0x0000A3FE01415C3D AS DateTime), N'34RT', NULL, NULL, 1)
INSERT [dbo].[StoreItemStock] ([StoreItemStockId], [StoreOutletId], [SKU], [StoreItemVariationId], [StoreItemVariationValueId], [StoreItemId], [QuantityInStock], [CostPrice], [ReorderLevel], [ReorderQuantity], [LastUpdated], [ShelfLocation], [ExpirationDate], [TotalQuantityAlreadySold], [StoreCurrencyId]) VALUES (10009, 1, N'121', 2, 2, 2, 523, NULL, 50, 200, CAST(0x0000A40700A2EC29 AS DateTime), N'4R7', NULL, NULL, 1)
INSERT [dbo].[StoreItemStock] ([StoreItemStockId], [StoreOutletId], [SKU], [StoreItemVariationId], [StoreItemVariationValueId], [StoreItemId], [QuantityInStock], [CostPrice], [ReorderLevel], [ReorderQuantity], [LastUpdated], [ShelfLocation], [ExpirationDate], [TotalQuantityAlreadySold], [StoreCurrencyId]) VALUES (10010, 1, N'122', 2, 1, 2, 430, NULL, 60, 220, CAST(0x0000A40700A7A468 AS DateTime), N'4R8', NULL, NULL, 1)
INSERT [dbo].[StoreItemStock] ([StoreItemStockId], [StoreOutletId], [SKU], [StoreItemVariationId], [StoreItemVariationValueId], [StoreItemId], [QuantityInStock], [CostPrice], [ReorderLevel], [ReorderQuantity], [LastUpdated], [ShelfLocation], [ExpirationDate], [TotalQuantityAlreadySold], [StoreCurrencyId]) VALUES (10011, 1, N'141', 2, 2, 4, 244, NULL, 30, 120, CAST(0x0000A40700A7ED2D AS DateTime), N'2SW', NULL, NULL, 1)
INSERT [dbo].[StoreItemStock] ([StoreItemStockId], [StoreOutletId], [SKU], [StoreItemVariationId], [StoreItemVariationValueId], [StoreItemId], [QuantityInStock], [CostPrice], [ReorderLevel], [ReorderQuantity], [LastUpdated], [ShelfLocation], [ExpirationDate], [TotalQuantityAlreadySold], [StoreCurrencyId]) VALUES (10012, 1, N'251', 2, 2, 5, 731, NULL, 10, 50, CAST(0x0000A40700A83954 AS DateTime), N'1WS', NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[StoreItemStock] OFF
SET IDENTITY_INSERT [dbo].[StoreItemType] ON 

INSERT [dbo].[StoreItemType] ([StoreItemTypeId], [Name], [Description], [SampleImagePath]) VALUES (1, N'Smart Phone', N'Smart Phone', N'/Images/StoreItemTypes/20141113142719231_e99a0f12-55fc-43f4-996c-9b39687f3a2d.jpeg')
INSERT [dbo].[StoreItemType] ([StoreItemTypeId], [Name], [Description], [SampleImagePath]) VALUES (2, N'Head Phoneses', N'Wireless/Wired Head Phone', N'/Images/StoreItemTypes/20141113150041218_97b7ebd4-d3f5-4637-a7e3-3cfde469d0d1.jpg')
INSERT [dbo].[StoreItemType] ([StoreItemTypeId], [Name], [Description], [SampleImagePath]) VALUES (10002, N'Smart Displays', N'Televisions with internet capability, smart processing functions, HDD, etc', NULL)
INSERT [dbo].[StoreItemType] ([StoreItemTypeId], [Name], [Description], [SampleImagePath]) VALUES (10004, N'Cathode Ray Displays', N'Cathode Ray Displays', NULL)
INSERT [dbo].[StoreItemType] ([StoreItemTypeId], [Name], [Description], [SampleImagePath]) VALUES (10005, N'Services', NULL, NULL)
SET IDENTITY_INSERT [dbo].[StoreItemType] OFF
SET IDENTITY_INSERT [dbo].[StoreItemVariation] ON 

INSERT [dbo].[StoreItemVariation] ([StoreItemVariationId], [VariationProperty]) VALUES (1, N'Size')
INSERT [dbo].[StoreItemVariation] ([StoreItemVariationId], [VariationProperty]) VALUES (2, N'Color')
INSERT [dbo].[StoreItemVariation] ([StoreItemVariationId], [VariationProperty]) VALUES (3, N'Weight')
SET IDENTITY_INSERT [dbo].[StoreItemVariation] OFF
SET IDENTITY_INSERT [dbo].[StoreItemVariationValue] ON 

INSERT [dbo].[StoreItemVariationValue] ([StoreItemVariationValueId], [Value]) VALUES (1, N'Blue')
INSERT [dbo].[StoreItemVariationValue] ([StoreItemVariationValueId], [Value]) VALUES (2, N'Black')
INSERT [dbo].[StoreItemVariationValue] ([StoreItemVariationValueId], [Value]) VALUES (3, N'15Kg')
SET IDENTITY_INSERT [dbo].[StoreItemVariationValue] OFF
SET IDENTITY_INSERT [dbo].[StoreOutlet] ON 

INSERT [dbo].[StoreOutlet] ([StoreOutletId], [OutletName], [StoreAddressId], [IsMainOutlet], [DefaultTax], [IsOperational], [DateCreated], [FacebookHandle], [TwitterHandle]) VALUES (1, N'Lekki Phase 1', 2, 1, 4500, 0, CAST(0x0000A3EB011CB375 AS DateTime), N'http://facebook.com/kmartng/lekki-phase-1', NULL)
SET IDENTITY_INSERT [dbo].[StoreOutlet] OFF
SET IDENTITY_INSERT [dbo].[StorePaymentGateway] ON 

INSERT [dbo].[StorePaymentGateway] ([StorePaymentGatewayId], [GatewayName], [LogoPath]) VALUES (1, N'Interswitch', N'/Images/StorePaymentGateways/20141113122913376_32ad4ae4-bc04-4bd9-9aa9-3e05571ad1ec.gif')
INSERT [dbo].[StorePaymentGateway] ([StorePaymentGatewayId], [GatewayName], [LogoPath]) VALUES (2, N'QuickTeller', N'~/Images/StorePaymentGateways/20141113122953968_0e72ba98-8f55-4c00-bea7-8b9f5d0837b4.png')
INSERT [dbo].[StorePaymentGateway] ([StorePaymentGatewayId], [GatewayName], [LogoPath]) VALUES (3, N'MasterCard', N'~/Images/StorePaymentGateways/20141113123010882_c4b0f59e-7d94-4833-a727-a90c900d03b2.jpg')
INSERT [dbo].[StorePaymentGateway] ([StorePaymentGatewayId], [GatewayName], [LogoPath]) VALUES (4, N'VisaCard', NULL)
SET IDENTITY_INSERT [dbo].[StorePaymentGateway] OFF
SET IDENTITY_INSERT [dbo].[StorePaymentMethod] ON 

INSERT [dbo].[StorePaymentMethod] ([StorePaymentMethodId], [Name], [Description]) VALUES (1, N'Cash', N'Cash')
INSERT [dbo].[StorePaymentMethod] ([StorePaymentMethodId], [Name], [Description]) VALUES (2, N'POS', N'POS')
INSERT [dbo].[StorePaymentMethod] ([StorePaymentMethodId], [Name], [Description]) VALUES (3, N'Split', N'Split')
SET IDENTITY_INSERT [dbo].[StorePaymentMethod] OFF
SET IDENTITY_INSERT [dbo].[StoreState] ON 

INSERT [dbo].[StoreState] ([StoreStateId], [Name], [StoreCountryId]) VALUES (1, N'Lagos', 1)
INSERT [dbo].[StoreState] ([StoreStateId], [Name], [StoreCountryId]) VALUES (2, N'Enugu', 1)
INSERT [dbo].[StoreState] ([StoreStateId], [Name], [StoreCountryId]) VALUES (3, N'Abia', 1)
INSERT [dbo].[StoreState] ([StoreStateId], [Name], [StoreCountryId]) VALUES (4, N'Georgia', 5)
INSERT [dbo].[StoreState] ([StoreStateId], [Name], [StoreCountryId]) VALUES (5, N'Pennsylvania', 5)
INSERT [dbo].[StoreState] ([StoreStateId], [Name], [StoreCountryId]) VALUES (6, N'Johannesburg', 4)
INSERT [dbo].[StoreState] ([StoreStateId], [Name], [StoreCountryId]) VALUES (7, N'Algiers', 3)
SET IDENTITY_INSERT [dbo].[StoreState] OFF
SET IDENTITY_INSERT [dbo].[StoreTransaction] ON 

INSERT [dbo].[StoreTransaction] ([StoreTransactionId], [StoreTransactionTypeId], [StorePaymentMethodId], [EffectedByEmployeeId], [TransactionAmount], [TransactionDate], [Remark], [StoreOutletId]) VALUES (2, 1, 1, 1, 219745, CAST(0x0000A419011FAD57 AS DateTime), NULL, 1)
SET IDENTITY_INSERT [dbo].[StoreTransaction] OFF
SET IDENTITY_INSERT [dbo].[StoreTransactionType] ON 

INSERT [dbo].[StoreTransactionType] ([StoreTransactionTypeId], [Name], [Description], [Action]) VALUES (1, N'Credit', N'Credit Store Account', N'Credit Store Account')
INSERT [dbo].[StoreTransactionType] ([StoreTransactionTypeId], [Name], [Description], [Action]) VALUES (2, N'Debit', N'Debit Store Account', N'Debit Store Account')
SET IDENTITY_INSERT [dbo].[StoreTransactionType] OFF
SET IDENTITY_INSERT [dbo].[Supplier] ON 

INSERT [dbo].[Supplier] ([SupplierId], [DateJoined], [LastSupplyDate], [Note], [CompanyName], [TIN]) VALUES (1, CAST(0x3E390B00 AS Date), NULL, N'Kmart USA ', N'KMart', NULL)
SET IDENTITY_INSERT [dbo].[Supplier] OFF
SET IDENTITY_INSERT [dbo].[UnitsOfMeasurement] ON 

INSERT [dbo].[UnitsOfMeasurement] ([UnitOfMeasurementId], [UoMCode], [UoMDescription]) VALUES (1, N'Kg', N'Kilogram')
INSERT [dbo].[UnitsOfMeasurement] ([UnitOfMeasurementId], [UoMCode], [UoMDescription]) VALUES (2, N'Pcs', N'Pieces')
INSERT [dbo].[UnitsOfMeasurement] ([UnitOfMeasurementId], [UoMCode], [UoMDescription]) VALUES (3, N'Ltr', N'Litre')
SET IDENTITY_INSERT [dbo].[UnitsOfMeasurement] OFF
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[BankAccount]  WITH CHECK ADD  CONSTRAINT [FK_BankAccount_Bank] FOREIGN KEY([BankId])
REFERENCES [dbo].[StoreBank] ([BankId])
GO
ALTER TABLE [dbo].[BankAccount] CHECK CONSTRAINT [FK_BankAccount_Bank]
GO
ALTER TABLE [dbo].[BankAccount]  WITH CHECK ADD  CONSTRAINT [FK_BankAccount_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[BankAccount] CHECK CONSTRAINT [FK_BankAccount_Customer]
GO
ALTER TABLE [dbo].[ChartOfAccount]  WITH CHECK ADD  CONSTRAINT [FK_ChartOfAccount_AccountGroup] FOREIGN KEY([AccountGroupId])
REFERENCES [dbo].[AccountGroup] ([AccountGroupId])
GO
ALTER TABLE [dbo].[ChartOfAccount] CHECK CONSTRAINT [FK_ChartOfAccount_AccountGroup]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_Person]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_StoreCustomerType] FOREIGN KEY([StoreCustomerTypeId])
REFERENCES [dbo].[StoreCustomerType] ([StoreCustomerTypeId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_StoreCustomerType]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_StoreOutlet]
GO
ALTER TABLE [dbo].[DailyInventory]  WITH CHECK ADD  CONSTRAINT [FK_DailyInventory_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[DailyInventory] CHECK CONSTRAINT [FK_DailyInventory_StoreItemStock]
GO
ALTER TABLE [dbo].[DailySale]  WITH CHECK ADD  CONSTRAINT [FK_DailySale_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[DailySale] CHECK CONSTRAINT [FK_DailySale_StoreOutlet]
GO
ALTER TABLE [dbo].[Delivery]  WITH CHECK ADD  CONSTRAINT [FK_Delivery_DeliveryVessel] FOREIGN KEY([DeliveryVesselId])
REFERENCES [dbo].[DeliveryVessel] ([DeliveryVesselId])
GO
ALTER TABLE [dbo].[Delivery] CHECK CONSTRAINT [FK_Delivery_DeliveryVessel]
GO
ALTER TABLE [dbo].[Delivery]  WITH CHECK ADD  CONSTRAINT [FK_Delivery_Employee] FOREIGN KEY([ReceivedById])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[Delivery] CHECK CONSTRAINT [FK_Delivery_Employee]
GO
ALTER TABLE [dbo].[Delivery]  WITH CHECK ADD  CONSTRAINT [FK_Delivery_PurchaseOrder] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrder] ([PurchaseOrderId])
GO
ALTER TABLE [dbo].[Delivery] CHECK CONSTRAINT [FK_Delivery_PurchaseOrder]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_JobRole] FOREIGN KEY([JobRoleId])
REFERENCES [dbo].[JobRole] ([JobRoleId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_JobRole]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Person]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_StoreAddress] FOREIGN KEY([StoreAddressId])
REFERENCES [dbo].[StoreAddress] ([StoreAddressId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_StoreAddress]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_StoreDepartment] FOREIGN KEY([StoreDepartmentId])
REFERENCES [dbo].[StoreDepartment] ([StoreDepartmentId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_StoreDepartment]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_StoreOutlet]
GO
ALTER TABLE [dbo].[EmployeeAssigment]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAssigment_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[EmployeeAssigment] CHECK CONSTRAINT [FK_EmployeeAssigment_Employee]
GO
ALTER TABLE [dbo].[EmployeeAssigment]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAssigment_JobRole] FOREIGN KEY([JobRoleId])
REFERENCES [dbo].[JobRole] ([JobRoleId])
GO
ALTER TABLE [dbo].[EmployeeAssigment] CHECK CONSTRAINT [FK_EmployeeAssigment_JobRole]
GO
ALTER TABLE [dbo].[EmployeeAssigment]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeAssigment_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[EmployeeAssigment] CHECK CONSTRAINT [FK_EmployeeAssigment_StoreOutlet]
GO
ALTER TABLE [dbo].[EmployeeDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocument_DocumentType] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[DocumentType] ([DocumentTypeId])
GO
ALTER TABLE [dbo].[EmployeeDocument] CHECK CONSTRAINT [FK_EmployeeDocument_DocumentType]
GO
ALTER TABLE [dbo].[EmployeeDocument]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocument_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[EmployeeDocument] CHECK CONSTRAINT [FK_EmployeeDocument_Employee]
GO
ALTER TABLE [dbo].[EmployeeSalesLog]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeSalesLog_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[EmployeeSalesLog] CHECK CONSTRAINT [FK_EmployeeSalesLog_Employee]
GO
ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_PurchaseOrder] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrder] ([PurchaseOrderId])
GO
ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_PurchaseOrder]
GO
ALTER TABLE [dbo].[ItemPrice]  WITH CHECK ADD  CONSTRAINT [FK_ItemPrice_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[ItemPrice] CHECK CONSTRAINT [FK_ItemPrice_StoreItemStock]
GO
ALTER TABLE [dbo].[ItemPrice]  WITH CHECK ADD  CONSTRAINT [FK_ItemPrice_UnitsOfMeasurement] FOREIGN KEY([UoMId])
REFERENCES [dbo].[UnitsOfMeasurement] ([UnitOfMeasurementId])
GO
ALTER TABLE [dbo].[ItemPrice] CHECK CONSTRAINT [FK_ItemPrice_UnitsOfMeasurement]
GO
ALTER TABLE [dbo].[ItemReview]  WITH CHECK ADD  CONSTRAINT [FK_ItemReview_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[ItemReview] CHECK CONSTRAINT [FK_ItemReview_StoreItemStock]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_AspNetUsers] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_AspNetUsers]
GO
ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_Salutation] FOREIGN KEY([SalutationId])
REFERENCES [dbo].[Salutation] ([SalutationId])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_Salutation]
GO
ALTER TABLE [dbo].[PersonContact]  WITH CHECK ADD  CONSTRAINT [FK_PersonContact_ContactType] FOREIGN KEY([ContactTypeId])
REFERENCES [dbo].[ContactType] ([ContactTypeId])
GO
ALTER TABLE [dbo].[PersonContact] CHECK CONSTRAINT [FK_PersonContact_ContactType]
GO
ALTER TABLE [dbo].[PersonContact]  WITH CHECK ADD  CONSTRAINT [FK_PersonContact_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[PersonContact] CHECK CONSTRAINT [FK_PersonContact_Person]
GO
ALTER TABLE [dbo].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Employee] FOREIGN KEY([GeneratedById])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_Employee]
GO
ALTER TABLE [dbo].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_StoreOutlet]
GO
ALTER TABLE [dbo].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_Supplier]
GO
ALTER TABLE [dbo].[PurchaseOrderItem]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderItem_PurchaseOrder] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrder] ([PurchaseOrderId])
GO
ALTER TABLE [dbo].[PurchaseOrderItem] CHECK CONSTRAINT [FK_PurchaseOrderItem_PurchaseOrder]
GO
ALTER TABLE [dbo].[PurchaseOrderItem]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderItem_StoreItem] FOREIGN KEY([StoreItemId])
REFERENCES [dbo].[StoreItem] ([StoreItemId])
GO
ALTER TABLE [dbo].[PurchaseOrderItem] CHECK CONSTRAINT [FK_PurchaseOrderItem_StoreItem]
GO
ALTER TABLE [dbo].[PurchaseOrderItem]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderItem_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[PurchaseOrderItem] CHECK CONSTRAINT [FK_PurchaseOrderItem_StoreOutlet]
GO
ALTER TABLE [dbo].[PurchaseOrderItemDelivery]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderItemDelivery_Delivery] FOREIGN KEY([DeliveryId])
REFERENCES [dbo].[Delivery] ([DeliveryId])
GO
ALTER TABLE [dbo].[PurchaseOrderItemDelivery] CHECK CONSTRAINT [FK_PurchaseOrderItemDelivery_Delivery]
GO
ALTER TABLE [dbo].[PurchaseOrderItemDelivery]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderItemDelivery_PurchaseOrderItem] FOREIGN KEY([PurchaseOrderItemId])
REFERENCES [dbo].[PurchaseOrderItem] ([PurchaseOrderItemId])
GO
ALTER TABLE [dbo].[PurchaseOrderItemDelivery] CHECK CONSTRAINT [FK_PurchaseOrderItemDelivery_PurchaseOrderItem]
GO
ALTER TABLE [dbo].[PurchaseOrderPayment]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderPayment_PurchaseOrder] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrder] ([PurchaseOrderId])
GO
ALTER TABLE [dbo].[PurchaseOrderPayment] CHECK CONSTRAINT [FK_PurchaseOrderPayment_PurchaseOrder]
GO
ALTER TABLE [dbo].[PurchaseOrderPayment]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderPayment_StoreTransaction] FOREIGN KEY([StoreTransactionId])
REFERENCES [dbo].[StoreTransaction] ([StoreTransactionId])
GO
ALTER TABLE [dbo].[PurchaseOrderPayment] CHECK CONSTRAINT [FK_PurchaseOrderPayment_StoreTransaction]
GO
ALTER TABLE [dbo].[Register]  WITH CHECK ADD  CONSTRAINT [FK_Register_StoreOutlet] FOREIGN KEY([CurrentOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[Register] CHECK CONSTRAINT [FK_Register_StoreOutlet]
GO
ALTER TABLE [dbo].[ReturnedProduct]  WITH CHECK ADD  CONSTRAINT [FK_ReturnedProduct_IssueType] FOREIGN KEY([IssueTypeId])
REFERENCES [dbo].[IssueType] ([IssueTypeId])
GO
ALTER TABLE [dbo].[ReturnedProduct] CHECK CONSTRAINT [FK_ReturnedProduct_IssueType]
GO
ALTER TABLE [dbo].[ReturnedProduct]  WITH CHECK ADD  CONSTRAINT [FK_ReturnedProduct_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[ReturnedProduct] CHECK CONSTRAINT [FK_ReturnedProduct_StoreItemStock]
GO
ALTER TABLE [dbo].[Sale]  WITH CHECK ADD  CONSTRAINT [FK_Sale_Employee] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[Sale] CHECK CONSTRAINT [FK_Sale_Employee]
GO
ALTER TABLE [dbo].[Sale]  WITH CHECK ADD  CONSTRAINT [FK_Sale_Register] FOREIGN KEY([RegisterId])
REFERENCES [dbo].[Register] ([RegisterId])
GO
ALTER TABLE [dbo].[Sale] CHECK CONSTRAINT [FK_Sale_Register]
GO
ALTER TABLE [dbo].[SalePayment]  WITH CHECK ADD  CONSTRAINT [FK_SalePayment_Sale] FOREIGN KEY([SaleId])
REFERENCES [dbo].[Sale] ([SaleId])
GO
ALTER TABLE [dbo].[SalePayment] CHECK CONSTRAINT [FK_SalePayment_Sale]
GO
ALTER TABLE [dbo].[SaleTransaction]  WITH CHECK ADD  CONSTRAINT [FK_SaleTransaction_Sale] FOREIGN KEY([SaleId])
REFERENCES [dbo].[Sale] ([SaleId])
GO
ALTER TABLE [dbo].[SaleTransaction] CHECK CONSTRAINT [FK_SaleTransaction_Sale]
GO
ALTER TABLE [dbo].[SaleTransaction]  WITH CHECK ADD  CONSTRAINT [FK_SaleTransaction_StoreTransaction] FOREIGN KEY([StoreTransactionId])
REFERENCES [dbo].[StoreTransaction] ([StoreTransactionId])
GO
ALTER TABLE [dbo].[SaleTransaction] CHECK CONSTRAINT [FK_SaleTransaction_StoreTransaction]
GO
ALTER TABLE [dbo].[ShopingCartItem]  WITH CHECK ADD  CONSTRAINT [FK_ShopingCartItem_ShoppingCart] FOREIGN KEY([ShopingCartId])
REFERENCES [dbo].[ShoppingCart] ([ShoppingCartId])
GO
ALTER TABLE [dbo].[ShopingCartItem] CHECK CONSTRAINT [FK_ShopingCartItem_ShoppingCart]
GO
ALTER TABLE [dbo].[ShopingCartItem]  WITH CHECK ADD  CONSTRAINT [FK_ShopingCartItem_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[ShopingCartItem] CHECK CONSTRAINT [FK_ShopingCartItem_StoreItemStock]
GO
ALTER TABLE [dbo].[ShoppingCart]  WITH CHECK ADD  CONSTRAINT [FK_ShoppingCart_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([CustomerId])
GO
ALTER TABLE [dbo].[ShoppingCart] CHECK CONSTRAINT [FK_ShoppingCart_Customer]
GO
ALTER TABLE [dbo].[StockUpload]  WITH CHECK ADD  CONSTRAINT [FK_StockUpload_ImageView] FOREIGN KEY([ImageViewId])
REFERENCES [dbo].[ImageView] ([ImageViewId])
GO
ALTER TABLE [dbo].[StockUpload] CHECK CONSTRAINT [FK_StockUpload_ImageView]
GO
ALTER TABLE [dbo].[StockUpload]  WITH CHECK ADD  CONSTRAINT [FK_StockUpload_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[StockUpload] CHECK CONSTRAINT [FK_StockUpload_StoreItemStock]
GO
ALTER TABLE [dbo].[StoreAddress]  WITH CHECK ADD  CONSTRAINT [FK_StoreAddress_StoreCity] FOREIGN KEY([StoreCityId])
REFERENCES [dbo].[StoreCity] ([StoreCityId])
GO
ALTER TABLE [dbo].[StoreAddress] CHECK CONSTRAINT [FK_StoreAddress_StoreCity]
GO
ALTER TABLE [dbo].[StoreCity]  WITH CHECK ADD  CONSTRAINT [FK_StoreCity_StoreState] FOREIGN KEY([StoreStateId])
REFERENCES [dbo].[StoreState] ([StoreStateId])
GO
ALTER TABLE [dbo].[StoreCity] CHECK CONSTRAINT [FK_StoreCity_StoreState]
GO
ALTER TABLE [dbo].[StoreContact]  WITH CHECK ADD  CONSTRAINT [FK_StoreContact_Person] FOREIGN KEY([PersonId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[StoreContact] CHECK CONSTRAINT [FK_StoreContact_Person]
GO
ALTER TABLE [dbo].[StoreContact]  WITH CHECK ADD  CONSTRAINT [FK_StoreContact_StoreDepartment] FOREIGN KEY([StoreDepartmentId])
REFERENCES [dbo].[StoreDepartment] ([StoreDepartmentId])
GO
ALTER TABLE [dbo].[StoreContact] CHECK CONSTRAINT [FK_StoreContact_StoreDepartment]
GO
ALTER TABLE [dbo].[StoreCurrency]  WITH CHECK ADD  CONSTRAINT [FK_StoreCurrency_StoreCountry] FOREIGN KEY([StoreCountryId])
REFERENCES [dbo].[StoreCountry] ([StoreCountryId])
GO
ALTER TABLE [dbo].[StoreCurrency] CHECK CONSTRAINT [FK_StoreCurrency_StoreCountry]
GO
ALTER TABLE [dbo].[StoreItem]  WITH CHECK ADD  CONSTRAINT [FK_StoreItem_ChartOfAccount] FOREIGN KEY([ChartOfAccountId])
REFERENCES [dbo].[ChartOfAccount] ([ChartOfAccountId])
GO
ALTER TABLE [dbo].[StoreItem] CHECK CONSTRAINT [FK_StoreItem_ChartOfAccount]
GO
ALTER TABLE [dbo].[StoreItem]  WITH CHECK ADD  CONSTRAINT [FK_StoreItem_StoreItemBrand] FOREIGN KEY([StoreItemBrandId])
REFERENCES [dbo].[StoreItemBrand] ([StoreItemBrandId])
GO
ALTER TABLE [dbo].[StoreItem] CHECK CONSTRAINT [FK_StoreItem_StoreItemBrand]
GO
ALTER TABLE [dbo].[StoreItem]  WITH CHECK ADD  CONSTRAINT [FK_StoreItem_StoreItemCategory] FOREIGN KEY([StoreItemCategoryId])
REFERENCES [dbo].[StoreItemCategory] ([StoreItemCategoryId])
GO
ALTER TABLE [dbo].[StoreItem] CHECK CONSTRAINT [FK_StoreItem_StoreItemCategory]
GO
ALTER TABLE [dbo].[StoreItem]  WITH CHECK ADD  CONSTRAINT [FK_StoreItem_StoreItemType] FOREIGN KEY([StoreItemTypeId])
REFERENCES [dbo].[StoreItemType] ([StoreItemTypeId])
GO
ALTER TABLE [dbo].[StoreItem] CHECK CONSTRAINT [FK_StoreItem_StoreItemType]
GO
ALTER TABLE [dbo].[StoreItemIssue]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemIssue_IssueType] FOREIGN KEY([IssueTypeId])
REFERENCES [dbo].[IssueType] ([IssueTypeId])
GO
ALTER TABLE [dbo].[StoreItemIssue] CHECK CONSTRAINT [FK_StoreItemIssue_IssueType]
GO
ALTER TABLE [dbo].[StoreItemIssue]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemIssue_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[StoreItemIssue] CHECK CONSTRAINT [FK_StoreItemIssue_StoreItemStock]
GO
ALTER TABLE [dbo].[StoreItemSold]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemSold_Sale] FOREIGN KEY([SaleId])
REFERENCES [dbo].[Sale] ([SaleId])
GO
ALTER TABLE [dbo].[StoreItemSold] CHECK CONSTRAINT [FK_StoreItemSold_Sale]
GO
ALTER TABLE [dbo].[StoreItemSold]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemSold_StoreItemStock] FOREIGN KEY([StoreItemStockId])
REFERENCES [dbo].[StoreItemStock] ([StoreItemStockId])
GO
ALTER TABLE [dbo].[StoreItemSold] CHECK CONSTRAINT [FK_StoreItemSold_StoreItemStock]
GO
ALTER TABLE [dbo].[StoreItemStock]  WITH CHECK ADD  CONSTRAINT [FK_ProductStock_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[StoreItemStock] CHECK CONSTRAINT [FK_ProductStock_StoreOutlet]
GO
ALTER TABLE [dbo].[StoreItemStock]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemStock_StoreCurrency] FOREIGN KEY([StoreCurrencyId])
REFERENCES [dbo].[StoreCurrency] ([StoreCurrencyId])
GO
ALTER TABLE [dbo].[StoreItemStock] CHECK CONSTRAINT [FK_StoreItemStock_StoreCurrency]
GO
ALTER TABLE [dbo].[StoreItemStock]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemStock_StoreItem] FOREIGN KEY([StoreItemId])
REFERENCES [dbo].[StoreItem] ([StoreItemId])
GO
ALTER TABLE [dbo].[StoreItemStock] CHECK CONSTRAINT [FK_StoreItemStock_StoreItem]
GO
ALTER TABLE [dbo].[StoreItemStock]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemStock_StoreItemVariation] FOREIGN KEY([StoreItemVariationId])
REFERENCES [dbo].[StoreItemVariation] ([StoreItemVariationId])
GO
ALTER TABLE [dbo].[StoreItemStock] CHECK CONSTRAINT [FK_StoreItemStock_StoreItemVariation]
GO
ALTER TABLE [dbo].[StoreItemStock]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemStock_StoreItemVariationValue] FOREIGN KEY([StoreItemVariationValueId])
REFERENCES [dbo].[StoreItemVariationValue] ([StoreItemVariationValueId])
GO
ALTER TABLE [dbo].[StoreItemStock] CHECK CONSTRAINT [FK_StoreItemStock_StoreItemVariationValue]
GO
ALTER TABLE [dbo].[StoreItemSupplier]  WITH CHECK ADD  CONSTRAINT [FK_StoreItemSupplier_StoreItem] FOREIGN KEY([StoreItemId])
REFERENCES [dbo].[StoreItem] ([StoreItemId])
GO
ALTER TABLE [dbo].[StoreItemSupplier] CHECK CONSTRAINT [FK_StoreItemSupplier_StoreItem]
GO
ALTER TABLE [dbo].[StoreOutlet]  WITH CHECK ADD  CONSTRAINT [FK_StoreOutlet_StoreAddress] FOREIGN KEY([StoreAddressId])
REFERENCES [dbo].[StoreAddress] ([StoreAddressId])
GO
ALTER TABLE [dbo].[StoreOutlet] CHECK CONSTRAINT [FK_StoreOutlet_StoreAddress]
GO
ALTER TABLE [dbo].[StoreOutletCoupon]  WITH CHECK ADD  CONSTRAINT [FK_StoreOutletCoupon_Coupon] FOREIGN KEY([CouponId])
REFERENCES [dbo].[Coupon] ([CouponId])
GO
ALTER TABLE [dbo].[StoreOutletCoupon] CHECK CONSTRAINT [FK_StoreOutletCoupon_Coupon]
GO
ALTER TABLE [dbo].[StoreOutletCoupon]  WITH CHECK ADD  CONSTRAINT [FK_StoreOutletCoupon_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[StoreOutletCoupon] CHECK CONSTRAINT [FK_StoreOutletCoupon_StoreOutlet]
GO
ALTER TABLE [dbo].[StoreState]  WITH CHECK ADD  CONSTRAINT [FK_StoreState_StoreCountry] FOREIGN KEY([StoreCountryId])
REFERENCES [dbo].[StoreCountry] ([StoreCountryId])
GO
ALTER TABLE [dbo].[StoreState] CHECK CONSTRAINT [FK_StoreState_StoreCountry]
GO
ALTER TABLE [dbo].[StoreTransaction]  WITH CHECK ADD  CONSTRAINT [FK_StoreTransaction_Employee] FOREIGN KEY([EffectedByEmployeeId])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[StoreTransaction] CHECK CONSTRAINT [FK_StoreTransaction_Employee]
GO
ALTER TABLE [dbo].[StoreTransaction]  WITH CHECK ADD  CONSTRAINT [FK_StoreTransaction_StoreOutlet] FOREIGN KEY([StoreOutletId])
REFERENCES [dbo].[StoreOutlet] ([StoreOutletId])
GO
ALTER TABLE [dbo].[StoreTransaction] CHECK CONSTRAINT [FK_StoreTransaction_StoreOutlet]
GO
ALTER TABLE [dbo].[StoreTransaction]  WITH CHECK ADD  CONSTRAINT [FK_StoreTransaction_StorePaymentMethod] FOREIGN KEY([StorePaymentMethodId])
REFERENCES [dbo].[StorePaymentMethod] ([StorePaymentMethodId])
GO
ALTER TABLE [dbo].[StoreTransaction] CHECK CONSTRAINT [FK_StoreTransaction_StorePaymentMethod]
GO
ALTER TABLE [dbo].[StoreTransaction]  WITH CHECK ADD  CONSTRAINT [FK_StoreTransaction_StoreTransactionType] FOREIGN KEY([StoreTransactionTypeId])
REFERENCES [dbo].[StoreTransactionType] ([StoreTransactionTypeId])
GO
ALTER TABLE [dbo].[StoreTransaction] CHECK CONSTRAINT [FK_StoreTransaction_StoreTransactionType]
GO
ALTER TABLE [dbo].[SupplierAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierAddress_AddressType] FOREIGN KEY([AddressTypeId])
REFERENCES [dbo].[AddressType] ([AddressTypeId])
GO
ALTER TABLE [dbo].[SupplierAddress] CHECK CONSTRAINT [FK_SupplierAddress_AddressType]
GO
ALTER TABLE [dbo].[SupplierAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierAddress_StoreAddress] FOREIGN KEY([StoreAddressId])
REFERENCES [dbo].[StoreAddress] ([StoreAddressId])
GO
ALTER TABLE [dbo].[SupplierAddress] CHECK CONSTRAINT [FK_SupplierAddress_StoreAddress]
GO
ALTER TABLE [dbo].[SupplierAddress]  WITH CHECK ADD  CONSTRAINT [FK_SupplierAddress_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([SupplierId])
GO
ALTER TABLE [dbo].[SupplierAddress] CHECK CONSTRAINT [FK_SupplierAddress_Supplier]
GO
ALTER TABLE [dbo].[Warehouse]  WITH CHECK ADD  CONSTRAINT [FK_Warehouse_StoreAddress] FOREIGN KEY([StoreAddressId])
REFERENCES [dbo].[StoreAddress] ([StoreAddressId])
GO
ALTER TABLE [dbo].[Warehouse] CHECK CONSTRAINT [FK_Warehouse_StoreAddress]
GO
ALTER TABLE [dbo].[WayBill]  WITH CHECK ADD  CONSTRAINT [FK_WayBill_Delivery] FOREIGN KEY([DeliveryId])
REFERENCES [dbo].[Delivery] ([DeliveryId])
GO
ALTER TABLE [dbo].[WayBill] CHECK CONSTRAINT [FK_WayBill_Delivery]
GO
ALTER TABLE [dbo].[WayBill]  WITH CHECK ADD  CONSTRAINT [FK_WayBill_Employee] FOREIGN KEY([AuthorisedById])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[WayBill] CHECK CONSTRAINT [FK_WayBill_Employee]
GO
ALTER TABLE [dbo].[WayBill]  WITH CHECK ADD  CONSTRAINT [FK_WayBill_Employee1] FOREIGN KEY([CheckedById])
REFERENCES [dbo].[Employee] ([EmployeeId])
GO
ALTER TABLE [dbo].[WayBill] CHECK CONSTRAINT [FK_WayBill_Employee1]
GO
ALTER TABLE [dbo].[WayBillItem]  WITH CHECK ADD  CONSTRAINT [FK_WayBillItem_StoreItem] FOREIGN KEY([StoreItemId])
REFERENCES [dbo].[StoreItem] ([StoreItemId])
GO
ALTER TABLE [dbo].[WayBillItem] CHECK CONSTRAINT [FK_WayBillItem_StoreItem]
GO
ALTER TABLE [dbo].[WayBillItem]  WITH CHECK ADD  CONSTRAINT [FK_WayBillItem_WayBill] FOREIGN KEY([WayBillId])
REFERENCES [dbo].[WayBill] ([WayBillId])
GO
ALTER TABLE [dbo].[WayBillItem] CHECK CONSTRAINT [FK_WayBillItem_WayBill]
GO
USE [master]
GO
ALTER DATABASE [ShopKeeper] SET  READ_WRITE 
GO
