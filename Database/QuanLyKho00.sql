--create database QuanLyKho00
--go

use QuanLyKho00
go

--ALTER TABLE OutputInfo
-- ADD SumPrice float default 0;

create table Unit
(
	Id int identity(1,1) primary key,
	DisplayName nvarchar(max)
)
go

SET IDENTITY_INSERT [dbo].[Unit] ON 
insert into Unit(Id, DisplayName) values('1', N'Viên')
go
insert into Unit(Id, DisplayName) values('2', N'Kg')
go
insert into Unit(Id, DisplayName) values('3', N'Bao')
go
SET IDENTITY_INSERT [dbo].[Unit] OFF 

create table Supplier
(
	Id int identity(1,1) primary key,
	DisplayName nvarchar(max),
	Address nvarchar(max),
	Phone varchar(11),
	Email nvarchar(200),
	MoreInfo nvarchar(max),
	ContractDate DateTime
)
go

SET IDENTITY_INSERT [dbo].[Supplier] ON 
insert into Supplier(Id, DisplayName, Address, Phone, Email, MoreInfo, ContractDate) values('1', N'Nguyen Van A', N'HCM', '0911234567', N'nguyenvana@gmail.com', 'chuyen nhanh', CAST(N'2022-11-19T00:00:00.000' AS DateTime))
go
insert into Supplier(Id, DisplayName, Address, Phone, Email, MoreInfo, ContractDate) values('2', N'Đào Thị B', N'Ha Noi', '0916569424', N'daothib@gmail.com', 'bth', CAST(N'2022-11-21T00:00:00.000' AS DateTime))
go 
SET IDENTITY_INSERT [dbo].[Supplier] OFF

create table Customer
(
	Id int identity(1,1) primary key,
	DisplayName nvarchar(max),
	Address nvarchar(max),
	Phone varchar(11),
	Email nvarchar(200),
	MoreInfo nvarchar(max),
	ContractDate DateTime
)
go

SET IDENTITY_INSERT [dbo].Customer ON 
insert into Customer(Id, DisplayName, Address, Phone, Email, MoreInfo, ContractDate) values('1', N'Nguyễn A', N'HCM', '0914265198', N'nguyenvana@gmail.com', 'ok', CAST(N'2022-11-24T00:00:00.000' AS DateTime))
go
insert into Customer(Id, DisplayName, Address, Phone, Email, MoreInfo, ContractDate) values('2', N'Lê B', N'Ha Noi', '0946294645', N'lethib@gmail.com', 'tot', CAST(N'2022-11-23T00:00:00.000' AS DateTime))
go
SET IDENTITY_INSERT [dbo].Customer OFF 

create table Object
(
	Id nvarchar(128) primary key,
	DisplayName nvarchar(max),
	IdUnit int not null,
	IdSupplier int not null,
	QRCode nvarchar(max),
	BarCode nvarchar(max)

	foreign key(IdUnit) references Unit(Id),
	foreign key(IdSupplier) references Supplier(Id),
)
go

insert into Object(Id, DisplayName, IdUnit, IdSupplier, QRCode, BarCode) values(N'1', 'Gạch', '1', '1', 'abc', 'xyz')
go
insert into Object(Id, DisplayName, IdUnit, IdSupplier, QRCode, BarCode) values(N'2', 'Thép', '2', '2', '123', '456')
go
insert into Object(Id, DisplayName, IdUnit, IdSupplier, QRCode, BarCode) values(N'3', 'Xi măng', '3', '3', '1a', '2b')
go

create table Promotion
(	
	Id int identity(1,1) primary key,
	DisplayName nvarchar(max),
	StartDate Datetime,
	EndDate Datetime,
	PromotionalValue float default 0,
)

SET IDENTITY_INSERT [dbo].Promotion ON 
insert into Promotion(Id, DisplayName, StartDate, EndDate, PromotionalValue) values('1', N'Giảm giá 15%', CAST(N'2022-11-23T00:00:00.000' AS DateTime), CAST(N'2022-12-23T00:00:00.000' AS DateTime), '0.1' )
go 
SET IDENTITY_INSERT [dbo].Promotion OFF 

create table UserRole
(
	Id int identity(1,1) primary key,
	DisplayName nvarchar(max)
)
go

insert into UserRole(DisplayName) values(N'Admin')
go
insert into UserRole(DisplayName) values(N'Nhân viên')
go

create table Users
(
	Id int identity(1,1) primary key,
	DisplayName nvarchar(max),
	UserName nvarchar(100),
	Password nvarchar(max),
	IdRole int not null

	foreign key (IdRole) references UserRole(Id)
)
go

insert into Users(DisplayName, Username, Password, IdRole) values(N'Admin', N'admin', N'db69fc039dcbd2962cb4d28f5891aae1', 1)
go
insert into Users(DisplayName, Username, Password, IdRole) values(N'Nhân viên', N'staff', N'978aae9bb6bee8fb75de3e4830a1be46', 2)
go
insert into Users(DisplayName, Username, Password, IdRole) values(N'Fuwa', N'admin', N'c0f3ef0aa02edc226c397c08bee3379b', 1)
go
insert into Users(DisplayName, Username, Password, IdRole) values(N'hiep', N'hiep123', N'29ca6a2f513d0839f918ba099a020ba7', 1)
go
insert into Users(DisplayName, Username, Password, IdRole) values(N'khai', N'khai12', N'76ff7543584e0401d2036c9e3c96c96b', 1)
go


create table Input
(
	Id nvarchar(128) primary key,
	DateInput Datetime
)
go

insert into Input(Id, DateInput) values(N'1', CAST(N'2022-12-05T00:00:00.000' AS DateTime))
go
insert into Input(Id, DateInput) values(N'2', CAST(N'2022-11-28T00:00:00.000' AS DateTime))
go

create table InputInfo
(
	Id nvarchar(128) primary key,
	IdObject nvarchar(128) not null,
	IdInput nvarchar(128) not null,
	Count int,
	InputPrice float default 0,
	OutputPrice float default 0,
	Status nvarchar(max),

	foreign key (IdObject) references Object(Id),
	foreign key (IdInput) references Input(Id)
)
go

insert into InputInfo(Id, IdObject, IdInput, Count, InputPrice, OutputPrice, Status) values(N'1', '1', '1', 50, 15000, 40000, 'first')
go
insert into InputInfo(Id, IdObject, IdInput, Count, InputPrice, OutputPrice, Status) values(N'2', '2', '2', 20, 40000, 70000, 'two')
go

create table Output
(
	Id nvarchar(128) primary key,
	IdCustomer int not null,
	IdUser int not null,
	IdPromotion int,
	DateOutput Datetime,
	Status nvarchar(max),
	Total float default 0,
	
	foreign key (IdCustomer) references Customer(Id),	
	foreign key (IdUser) references Users(Id),
	foreign key (IdPromotion) references Promotion(Id),
)
go

insert into Output(Id, IdCustomer, IdUser, IdPromotion, DateOutput, Status, Total) values (N'1', '1', '1', '1', CAST(N'2022-11-23T00:00:00.000' AS DateTime),'firstOutput', 10000)
go
insert into Output(Id, IdCustomer, IdUser, IdPromotion, DateOutput, Status, Total) values (N'2', '2', '1', '1', CAST(N'2022-11-23T00:00:00.000' AS DateTime),'twoOutput', 5000)
go

create table OutputInfo
(
	Id nvarchar(128) primary key,
	IdOutput nvarchar(128) not null,
	IdObject nvarchar(128) not null,
	IdInputInfo nvarchar(128) not null,
	Count int,
	Status nvarchar(max),
	SumPrice float default 0,

	foreign key (IdOutput) references Output(Id),
	foreign key (IdObject) references Object(Id),
	foreign key (IdInputInfo) references InputInfo(Id),
)
go

insert into OutputInfo(Id, IdOutput, IdInputInfo, IdObject, Count, Status, SumPrice) values(N'1', '1', '1', '1', 5, N'1', 2000)
go
insert into OutputInfo(Id, IdOutput, IdInputInfo, IdObject, Count, Status, SumPrice) values(N'2', '1', '2', '2', 5, N'2', 3000)
go
insert into OutputInfo(Id, IdOutput, IdInputInfo, IdObject, Count, Status, SumPrice) values(N'3', '1', '1', '2', 5, N'1', 4000)
go
insert into OutputInfo(Id, IdOutput, IdInputInfo, IdObject, Count, Status, SumPrice) values(N'4', '2', '2', '2', 5, N'2', 5000)
go