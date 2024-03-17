create database Storage
go 

use Storage
go

create table ProductTypes
(
Id int identity primary key,
Type nvarchar(100) not null 
)
go 

create table Suppliers
(
Id int identity primary key,
Name nvarchar(255) not null
)
go

create table Products
(
Id int identity primary key,
ProductName nvarchar(100),
TypeId int not null references ProductTypes(Id),
SupplierId int not null references Suppliers(Id),
Quantity int not null check(Quantity >= 0),
Cost money not null check(Cost >= 0),
SupplyDate date not null check(SupplyDate <= GETDATE())
)
go


select * from ProductTypes
select * from Suppliers
select * from Products