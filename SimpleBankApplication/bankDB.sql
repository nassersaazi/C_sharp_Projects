USE [master]
GO
/****** Object:  Database [TestBank]    Script Date: 25-Jun-20 10:46:33 ******/
CREATE DATABASE [TestBank]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TestBank', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\TestBank.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TestBank_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\TestBank_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [TestBank] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TestBank].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TestBank] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TestBank] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TestBank] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TestBank] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TestBank] SET ARITHABORT OFF 
GO
ALTER DATABASE [TestBank] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [TestBank] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TestBank] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TestBank] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TestBank] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TestBank] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TestBank] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TestBank] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TestBank] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TestBank] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TestBank] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TestBank] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TestBank] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TestBank] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TestBank] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TestBank] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TestBank] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TestBank] SET RECOVERY FULL 
GO
ALTER DATABASE [TestBank] SET  MULTI_USER 
GO
ALTER DATABASE [TestBank] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TestBank] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TestBank] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TestBank] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TestBank] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'TestBank', N'ON'
GO
ALTER DATABASE [TestBank] SET QUERY_STORE = OFF
GO
USE [TestBank]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[AccountNo] [varchar](100) NOT NULL,
	[AccountType] [varchar](50) NULL,
	[CreationDate] [date] NULL,
	[AccountBalance] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientsTB]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientsTB](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[AccountNo] [varchar](250) NULL,
	[Name] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[Lastname] [varchar](50) NULL,
	[PhoneNumber] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ClientTB_2]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientTB_2](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Email] [varchar](50) NULL,
	[AccountNo] [varchar](100) NULL,
	[AccountBalance] [varchar](250) NULL,
	[NotificationStatus] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DepositsTB]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DepositsTB](
	[RecordId] [int] IDENTITY(1,1) NOT NULL,
	[AccountNo] [varchar](100) NULL,
	[DepositAmount] [varchar](50) NULL,
	[DepositDate] [date] NULL,
	[NotificationStatus] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ErrorLogs]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLogs](
	[RecordId] [bigint] IDENTITY(1,1) NOT NULL,
	[LogDate] [datetime] NOT NULL,
	[ErrorMessage] [varchar](max) NOT NULL,
	[TranId] [varchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GeneralLedger]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralLedger](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[AccountNo] [varchar](100) NULL,
	[TransRef] [varchar](50) NULL,
	[TransType] [varchar](100) NULL,
	[AccountFrom] [varchar](50) NULL,
	[AccountTo] [varchar](50) NULL,
	[TransactionAmount] [int] NULL,
	[TransactionDate] [datetime] NULL,
	[Naration] [varchar](250) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestLogs]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestLogs](
	[RequestId] [bigint] IDENTITY(1,1) NOT NULL,
	[Content] [nvarchar](max) NULL,
	[LogDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionsTB]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionsTB](
	[TransID] [int] IDENTITY(1,1) NOT NULL,
	[AccountNo] [varchar](50) NULL,
	[TransType] [varchar](15) NULL,
	[TransDate] [datetime] NULL,
	[Status] [bit] NULL,
	[Amount] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserTB]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTB](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Email] [varchar](50) NULL,
	[Password] [varchar](50) NULL,
	[UserType] [varchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WithdrawTB]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WithdrawTB](
	[RecordID] [int] IDENTITY(1,1) NOT NULL,
	[AccountNo] [varchar](100) NULL,
	[WithdrawAmount] [varchar](100) NULL,
	[WithdrawDate] [date] NULL,
	[NotificationStatus] [bit] NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ErrorLogs] ADD  DEFAULT (getdate()) FOR [LogDate]
GO
/****** Object:  StoredProcedure [dbo].[AccountStatement_1]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[AccountStatement_1]
 @AccountNo varchar(50),@FromDate datetime,@ToDate datetime
--set @AccountNo = 'AC0017'
--set @FromDate = '2020-06-16 00:36:10.787'
--set @ToDate = '2020-06-16 00:54:48.610'
AS
SELECT 
      AccountNo, 
	  TransactionDate,
	  TransactionAmount,
	  Naration  
FROM GeneralLedger 
WHERE TransactionDate BETWEEN @FromDate AND @ToDate
AND ((AccountNo = @AccountNo) OR (@AccountNo = '0'))

GO
/****** Object:  StoredProcedure [dbo].[CheckBalance]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[CheckBalance](
@AccountNumber varchar(50)
)
AS
BEGIN

	SELECT AccountBalance FROM Accounts 

			--WHERE AccountNo = @AccountNumber

END
      



GO
/****** Object:  StoredProcedure [dbo].[CheckBalanceNew]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[CheckBalanceNew](
@AccountNumber varchar(50)
)
AS
BEGIN
if (@AccountNumber='All')
 begin

	select bk.AccountNo ,Sa.Name,bk.AccountBalance from ClientsTB as Sa
		inner join Accounts as bk on bk.AccountNo=Sa.AccountNo
	    --where bk.AccountNo = @AccountNumber
 end
 else
 begin

 select bk.AccountNo ,Sa.Name,bk.AccountBalance from ClientsTB as Sa
		inner join Accounts as bk on bk.AccountNo=Sa.AccountNo
	    where bk.AccountNo = @AccountNumber
 end

END
 

 --select * from GeneralLedger where AccountNo = 'BAC001'

 --select * from Accounts
GO
/****** Object:  StoredProcedure [dbo].[CreateCustomerAccount]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[CreateCustomerAccount](
@AccountNo varchar(350),
@AccountType varchar(350),
@AccountBalance MONEY,
@CreationDate varchar(350)
)
AS

BEGIN
 INSERT INTO Accounts(AccountNo, AccountType,AccountBalance,CreationDate) VALUES(@AccountNo,@AccountType,@AccountBalance,GetDate())
END


GO
/****** Object:  StoredProcedure [dbo].[DepositFunds]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROC [dbo].[DepositFunds](
@AccountNumber varchar(50),
@Amount int,
@Ref varchar(50)

)
AS
declare @TotalBank int = (select AccountBalance from Accounts where AccountNo = 'BAC001')
declare @TotalClient int = (select AccountBalance from Accounts where AccountNo = @AccountNumber)
declare @balanceBank int
declare @balanceClient int

-- DEBIT BANK
BEGIN
--INSERT INTO TransactionsTB(AccountNo,TransType,TransDate,Amount,Status) VALUES (@AccountNumber,'DEPOSIT',GETDATE(),@Amount,0)


		--begin
			
			set @balanceBank = (@TotalBank - @Amount)
			
			update Accounts set AccountBalance = @balanceBank where AccountNo = 'BAC001'

			INSERT INTO GeneralLedger(AccountNo,TransRef,TransType,AccountTo,TransactionAmount,TransactionDate) 
			  VALUES ('BAC001',@Ref,'DEBIT',@AccountNumber,-@Amount,GETDATE())

			  --CREDIT CLIENT
			INSERT INTO GeneralLedger(AccountNo,TransRef,TransType,AccountFrom,TransactionAmount,TransactionDate) 
			  VALUES (@AccountNumber,@Ref ,'CREDIT','BAC001',@Amount,GETDATE())

			set @balanceClient = (@TotalClient + @Amount)
			
			update Accounts set AccountBalance = @balanceClient where AccountNo = @AccountNumber

			INSERT INTO TransactionsTB(AccountNo,TransType,TransDate,Amount,Status) VALUES (@AccountNumber,'DEPOSIT',GETDATE(),@Amount,0)
	--	end

END
GO
/****** Object:  StoredProcedure [dbo].[DepositNotifier]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[DepositNotifier]
AS
BEGIN
   SELECT * FROM TransactionsTB WHERE TransType = 'DEPOSIT' AND Status=0
END
GO
/****** Object:  StoredProcedure [dbo].[Get_OpeningAndRunningBal]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[Get_OpeningAndRunningBal](
@AccountNumber varchar(100),
@FromDate DateTime,
@ToDate DateTime
)
AS
DECLARE @RunningBalance money,@TransType VARCHAR(50),@OpeningBal money
Declare @CreditsByDate decimal(18,2);
Declare @DebitsByDate decimal(18,2);
Declare @BalanceByDate decimal(18,2);
Declare @CreditsByEndDate decimal(18,2);
Declare @DebitsByEndDate decimal(18,2);
Declare @BalanceByEndDate decimal(18,2);
--set @AccountNumber = 'AC0014'
--set @FromDate = '2020-06-01'
--set @ToDate = '2020-06-16'

SET @CreditsByDate= (select SUM(TransactionAmount) FROM GeneralLedger WHERE AccountTo=@AccountNumber AND Convert(Date,TransactionDate)<=Convert(Date,@FromDate));

SET @DebitsByDate= (select SUM(TransactionAmount) FROM GeneralLedger WHERE AccountFrom=@AccountNumber AND   Convert(Date,TransactionDate)<=Convert(Date,@FromDate));

SET @CreditsByEndDate= (select SUM(TransactionAmount) FROM GeneralLedger WHERE AccountTo=@AccountNumber AND Convert(Date,TransactionDate)<=Convert(Date,@ToDate));

SET @DebitsByEndDate= (select SUM(TransactionAmount) FROM GeneralLedger WHERE AccountFrom=@AccountNumber AND  Convert(Date,TransactionDate)<=Convert(Date,@ToDate));

SET @BalanceByEndDate=SUM(isnull(@CreditsByEndDate,0)+ISNULL(@DebitsByEndDate,0));

SET @BalanceByDate=SUM(isnull(@CreditsByDate,0)+ISNULL(@DebitsByDate,0));

SET @BalanceByEndDate=SUM(isnull(@CreditsByEndDate,0)+ISNULL(@DebitsByEndDate,0));

SET @BalanceByDate=SUM(isnull(@CreditsByDate,0)+ISNULL(@DebitsByDate,0));

SELECT 
	@AccountNumber AS AccountNumber,
	isnull(@CreditsByDate,0) AS CREDITS,
	isnull(@DebitsByDate,0) AS DEBITS,
	@BalanceByDate AS OPENINGBALANCE,
	@BalanceByEndDate AS CLOSINGBALANCE
FROM GeneralLedger 
WHERE (AccountNo=@AccountNumber or @AccountNumber = '')
AND (TransactionDate BETWEEN @FromDate AND @ToDate)
group by AccountNo
GO
/****** Object:  StoredProcedure [dbo].[GetAccount]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


create PROC [dbo].[GetAccount](
@AccountNumber varchar(50)
)
AS
BEGIN

	SELECT AccountNo FROM ClientsTB 

			WHERE AccountNo = @AccountNumber

END

GO
/****** Object:  StoredProcedure [dbo].[GetAccounts]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





create procedure [dbo].[GetAccounts]
as begin
select * from Accounts where AccountType = 'CLIENT'
end 
GO
/****** Object:  StoredProcedure [dbo].[GetClientCount]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create Procedure [dbo].[GetClientCount]
AS BEGIN
  SELECT Count(*) CustomerCount from ClientsTB
END
GO
/****** Object:  StoredProcedure [dbo].[GetEmail]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[GetEmail](
@AccountNumber varchar(50)
)
AS
BEGIN

	SELECT Email FROM ClientsTB 

			WHERE AccountNo = @AccountNumber

END



GO
/****** Object:  StoredProcedure [dbo].[GetStatement]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[GetStatement](
@AccountNumber varchar(50),
@FromDate date,
@ToDate date

)
AS

BEGIN
	select 
		TransactionDate,
		TransactionAmount,
		TransRef,
		TransType,SUM(TransactionAmount) OVER ( ORDER BY RecordId ) AS RunningBalance
		
	from GeneralLedger 
	where AccountNo = @AccountNumber
	AND (TransactionDate between @FromDate and @ToDate)
		
END
GO
/****** Object:  StoredProcedure [dbo].[GetUnnotifiedTransactions]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[GetUnnotifiedTransactions]

as
select * from TransactionsTB
where Status=0 

order by TransDate 



GO
/****** Object:  StoredProcedure [dbo].[GetUser]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create Procedure [dbo].[GetUser](
@email varchar(350),
@password varchar(340)
)
AS BEGIN
SELECT * FROM UserTB WHERE Email=@email and Password=@password
END
GO
/****** Object:  StoredProcedure [dbo].[GetUserLogin]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROC [dbo].[GetUserLogin]

@Email varchar(50),
@Password varchar(50)

AS
 SELECT * FROM UserTB WHERE Email = @Email AND Password = @Password


GO
/****** Object:  StoredProcedure [dbo].[InsertIntoClientTable]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertIntoClientTable] 
(
  
  @firstname varchar (50),
  @lastname varchar (50),
  @phone varchar (50),
  @email varchar (50)
)
as
Declare @recordid int
Declare @record varchar(50)
Declare @number varchar(50)
Declare @result varchar(50)
begin

		insert into ClientsTB(Name, Email,LastName,PhoneNumber)
		values ( @firstname, @email,@lastname,@phone)

		begin
			set @recordid = @@identity 
			set @record = CONVERT(VARCHAR(50), @recordId)
			set @number = 'AC00' +@record
			update ClientsTB set AccountNo = @number where RecordID = @recordid
		end

		
  insert into Accounts (AccountNo,AccountType,CreationDate,AccountBalance)
  values (@number, 'CLIENT',GETDATE(), 0)

		
		
end
GO
/****** Object:  StoredProcedure [dbo].[InsertIntoUserTable]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[InsertIntoUserTable] 
(  
  @name varchar (50),
  @email varchar (50),
  @password varchar(50)
)
as

begin
		insert into UserTB(Name, Email,Password)
		values ( @name, @email,@password)
end
GO
/****** Object:  StoredProcedure [dbo].[LogError]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[LogError]

@Date datetime,
@Msg varchar(MAX),
@Code varchar(50)
AS
INSERT INTO ErrorLogs
                      (LogDate,ErrorMessage,TranId)
VALUES     (@Date,@Msg,@Code)

GO
/****** Object:  StoredProcedure [dbo].[RegisterUser]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[RegisterUser](
@Name varchar(50), 
@Email varchar(50),
@Password varchar(50)

)
AS 
Begin 
Insert into UserTB(Name,Email,Password) values(@Name,@Email,@Password)
END
GO
/****** Object:  StoredProcedure [dbo].[SaveRequestLogs]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SaveRequestLogs]

@message varchar(max),
@TranDate datetime
as
insert into RequestLogs
(Content,LogDate)
values(@message,@TranDate)

GO
/****** Object:  StoredProcedure [dbo].[UpdateDepositStatus]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[UpdateDepositStatus](
@TransID INT
)
AS
BEGIN
UPDATE TransactionsTB SET Status=1 WHERE TransID=@TransID AND Status=0
END


GO
/****** Object:  StoredProcedure [dbo].[UpdateTransactionStatus]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[UpdateTransactionStatus](
@AccountNumber varchar(50)

)
AS
BEGIN
UPDATE TransactionsTB SET Status=1 WHERE AccountNo = @AccountNumber  AND Status=0
END


GO
/****** Object:  StoredProcedure [dbo].[UpdateWithdrawStatus]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[UpdateWithdrawStatus](
@TransID INT
)
AS
BEGIN
UPDATE TransactionsTB SET Status=1 WHERE TransID=@TransID AND Status=0
END


GO
/****** Object:  StoredProcedure [dbo].[VerifyClient]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[VerifyClient](
@AccountNumber varchar(50),
@Email varchar(50)
)
AS
BEGIN

	SELECT AccountNo FROM ClientsTB 

			WHERE AccountNo = @AccountNumber AND Email=@Email

END
GO
/****** Object:  StoredProcedure [dbo].[WithdrawFunds]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[WithdrawFunds](
@AccountNumber varchar(50),
@Amount int,

@Ref varchar(50)

)
AS
declare @TotalBank int = (select AccountBalance from Accounts where AccountNo = 'BAC001')
declare @TotalClient int = (select AccountBalance from Accounts where AccountNo = @AccountNumber)
declare @balanceBank int
declare @balanceClient int

--declare @Total int = (select AccountBalance from ClientsTB where AccountNo = @AccountNumber)
--declare @balance int

BEGIN
--INSERT INTO TransactionsTB(AccountNo,TransType,TransDate,Amount,Status) VALUES (@AccountNumber,'DEPOSIT',GETDATE(),@Amount,0)


		--begin
			
			set @balanceClient = (@TotalClient - @Amount)
			
			update Accounts set AccountBalance = @balanceClient where AccountNo = @AccountNumber

			INSERT INTO GeneralLedger(AccountNo,TransRef,TransType,AccountTo,TransactionAmount,TransactionDate) 
			  VALUES (@AccountNumber,@Ref,'DEBIT','BAC001',-@Amount,GETDATE())

			  --CREDIT CLIENT
			INSERT INTO GeneralLedger(AccountNo,TransRef,TransType,AccountFrom,TransactionAmount,TransactionDate) 
			  VALUES ('BAC001',@Ref,'CREDIT',@AccountNumber,@Amount,GETDATE())

			set @balanceBank = (@TotalBank + @Amount)
			
			update Accounts set AccountBalance = @balanceBank where AccountNo = 'BAC001'

			INSERT INTO TransactionsTB(AccountNo,TransType,TransDate,Amount,Status) VALUES (@AccountNumber,'WITHDRAW',GETDATE(),@Amount,0)
	--	end

END
GO
/****** Object:  StoredProcedure [dbo].[WithdrawNotifier]    Script Date: 25-Jun-20 10:46:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROC [dbo].[WithdrawNotifier]
AS
BEGIN
   SELECT * FROM TransactionsTB WHERE TransType = 'WITHDRAW' AND Status=0
END
GO
USE [master]
GO
ALTER DATABASE [TestBank] SET  READ_WRITE 
GO
