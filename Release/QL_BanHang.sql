USE [master]
GO
-- database quản lý bán hàng
CREATE DATABASE QL_BanHang
GO

USE QL_BanHang
GO
--bảng users
CREATE TABLE Users
(
	UserID int IDENTITY(1,1) NOT NULL,
	UserName nvarchar(45) null,
	Password nvarchar(20) NULL,
	UserType bit null,
	Constraint PK_Users primary key (UserID)
)
--bảng category
CREATE TABLE Category
(
	CategoryID int IDENTITY(1,1) NOT NULL,
	CategoryName nvarchar(45) null,
	Constraint PK_Category primary key (CategoryID)
)
--bảng product
CREATE TABLE Product
(
	ProductID int IDENTITY(1,1) NOT NULL,
	CategoryID int null,
	ProductName nvarchar(45) null,
	GiaGoc float null,
	Constraint PK_Product primary key (ProductID)
)
--bang thong tin product
CREATE TABLE ProductInfo
(
	ProductInfoID int IDENTITY(1,1) NOT NULL,
	ProductID int  NULL,
	ManHinh nvarchar(75) null,
	HDH nvarchar(75) null,
	CameraTruoc nvarchar(75) null,
	CameraSau nvarchar(75) null,
	CPU nvarchar(75) null,
	RAM nvarchar(75) null,
	ROM nvarchar(75) null,
	SIM nvarchar(75) null,
	PIN nvarchar(75) null,
	Constraint PK_ProductInfo primary key (ProductInfoID)
)
--bảng Customers
CREATE TABLE Customer
(
	CustomerID int IDENTITY(1,1) NOT NULL,
	Ten nvarchar(45) null,
	NgaySinh date null,
	SDT nvarchar(20) null,
	DiaChi nvarchar(100) null,
	Cmnd nvarchar(20) null,
	Constraint PK_Customer primary key (CustomerID)
)
--bảng phiếu xuất
CREATE TABLE PhieuBanHang
(
	PhieuBanHangID int IDENTITY(1,1) NOT NULL,
	ProductID int null,
	CustomerID int null,
	TenNguoiBan nvarchar(45) null,
	NgayBan datetime null,
	GiaBan float null,
	BaoHanh nvarchar(50) null,
	Constraint PK_PhieuBanHangID primary key (PhieuBanHangID)
)
--thêm các khóa ngoại
ALTER TABLE Product ADD CONSTRAINT FK_Category_Product FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
ALTER TABLE ProductInfo ADD CONSTRAINT FK_ProductInfo_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
ALTER TABLE PhieuBanHang ADD CONSTRAINT FK_PhieuBanHang_Product FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
ALTER TABLE PhieuBanHang ADD CONSTRAINT FK_PhieuBanHang_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)

--thêm một số cơ sở dữ liệu giả lập

SET IDENTITY_INSERT [dbo].[Category] ON 
--Bảng category
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (1, N'Samsung')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (2, N'iPhone')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (3, N'Oppo')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (4, N'Nokia')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (5, N'Huawei')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (6, N'Xiaomi')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (7, N'Vsmart')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (8, N'Realme')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (9, N'Vivo')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (10, N'Philips')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (11, N'Mobell')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (12, N'Mobistar')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (13, N'Htc')
INSERT [dbo].[Category] ([CategoryID], [CategoryName]) VALUES (14, N'Asus')
SET IDENTITY_INSERT [dbo].[Category] OFF
SET IDENTITY_INSERT [dbo].[Product] ON 
--Bảng product
INSERT [dbo].[Product] ([ProductID], [CategoryID], [ProductName], [GiaGoc]) VALUES (1, 2, N'iPhone Xs Max 256GB', 37990000)
INSERT [dbo].[Product] ([ProductID], [CategoryID], [ProductName], [GiaGoc]) VALUES (2, 1, N'Samsung Galaxy Note 9 512GB', 28490000)
INSERT [dbo].[Product] ([ProductID], [CategoryID], [ProductName], [GiaGoc]) VALUES (3, 1, N'Samsung Galaxy Note 9', 22990000)
INSERT [dbo].[Product] ([ProductID], [CategoryID], [ProductName], [GiaGoc]) VALUES (4, 4, N'Nokia 7 plus', 6990000)
INSERT [dbo].[Product] ([ProductID], [CategoryID], [ProductName], [GiaGoc]) VALUES (5, 4, N'Nokia 6.1 Plus', 5990000)
INSERT [dbo].[Product] ([ProductID], [CategoryID], [ProductName], [GiaGoc]) VALUES (6, 3, N'OPPO Find X', 20990000)
SET IDENTITY_INSERT [dbo].[Product] OFF
SET IDENTITY_INSERT [dbo].[ProductInfo] ON 
--Bảng ProductInfo
INSERT [dbo].[ProductInfo] ([ProductInfoID], [ProductID], [ManHinh], [HDH], [CameraTruoc], [CameraSau], [CPU], [RAM], [ROM], [SIM], [PIN]) VALUES (1, 1, N'	OLED, 6.5", Super Retina', N'iOS 12', N'7 MP', N'2 camera 12 MP', N'	Apple A12 Bionic 6 nhân', N'	4 GB', N'256 GB', N'Nano SIM & eSIM, Hỗ trợ 4G', N'	3174 mAh, có sạc nhanh')
INSERT [dbo].[ProductInfo] ([ProductInfoID], [ProductID], [ManHinh], [HDH], [CameraTruoc], [CameraSau], [CPU], [RAM], [ROM], [SIM], [PIN]) VALUES (2, 2, N'Super AMOLED, 6.4", Quad HD+ (2K+)', N'Android 8.1 (Oreo)', N'8 MP', N'2 camera 12 MP', N'Exynos 9810 8 nhân 64-bit', N'8 GB', N'512 GB', N'2 SIM Nano (SIM 2 chung khe thẻ nhớ), Hỗ trợ 4G', N'4000 mAh, sạc nhanh')
INSERT [dbo].[ProductInfo] ([ProductInfoID], [ProductID], [ManHinh], [HDH], [CameraTruoc], [CameraSau], [CPU], [RAM], [ROM], [SIM], [PIN]) VALUES (3, 3, N'Super AMOLED, 6.4", Quad HD+ (2K+)', N'	Android 8.1 (Oreo)', N'8 MP', N'2 camera 12 MP', N'	Exynos 9810 8 nhân 64-bit', N'6 GB', N'128 GB', N'2 SIM Nano (SIM 2 chung khe thẻ nhớ), Hỗ trợ 4G', N'4000 mAh, có sạc nhanh')
INSERT [dbo].[ProductInfo] ([ProductInfoID], [ProductID], [ManHinh], [HDH], [CameraTruoc], [CameraSau], [CPU], [RAM], [ROM], [SIM], [PIN]) VALUES (4, 4, N'IPS LCD, 6", Full HD+', N'Android 8.0 (Oreo)', N'16 MP', N'	12 MP và 13 MP (2 camera)', N'	Qualcomm Snapdragon 660 8 nhân', N'4 GB', N'64 GB', N'2 SIM Nano (SIM 2 chung khe thẻ nhớ), Hỗ trợ 4G', N'3800 mAh, có sạc nhanh')
INSERT [dbo].[ProductInfo] ([ProductInfoID], [ProductID], [ManHinh], [HDH], [CameraTruoc], [CameraSau], [CPU], [RAM], [ROM], [SIM], [PIN]) VALUES (5, 5, N'IPS LCD, 5.8", Full HD+', N' Android 8 Oreo (Android One)', N'16 MP và 5 MP (2 camera)', N' 16 MP', N' Qualcomm Snapdragon 636 8 nhân', N' 4 GB', N' 64 GB', N'2 SIM Nano (SIM 2 chung khe thẻ nhớ), Hỗ trợ 4G', N' 3060 mAh, có sạc nhanh')
INSERT [dbo].[ProductInfo] ([ProductInfoID], [ProductID], [ManHinh], [HDH], [CameraTruoc], [CameraSau], [CPU], [RAM], [ROM], [SIM], [PIN]) VALUES (6, 6, N'AMOLED, 6.42", Full HD+', N'Android 8.1 (Oreo)', N' 25 MP', N'20 MP và 16 MP (2 camera)', N' Snapdragon 845 8 nhân', N' 8 GB', N' 256 GB', N'2 Nano Sim, Hỗ trợ 4G', N' 3730 mAh, có sạc nhanh')
SET IDENTITY_INSERT [dbo].[ProductInfo] OFF
SET IDENTITY_INSERT [dbo].[Customer] ON 
--Bảng khách hàng
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (1, N'Nguyễn Văn Nhất', CAST(N'1998-03-02' AS Date), N'0978654535', N'Bình Dương', N'287463536')
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (2, N'Lâm Quốc Đạt', CAST(N'1994-08-23' AS Date), N'0984746363', N'Đồng Tháp', N'283746445')
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (3, N'Trần Văn Nam', CAST(N'1989-03-23' AS Date), N'0984763737', N'An Giang', N'283646474')
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (4, N'Trần Văn Quý', CAST(N'1974-03-14' AS Date), N'0937464631', N'Hà Nội', N'283645360')
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (5, N'Trần Minh Cường', CAST(N'1997-12-25' AS Date), N'0975206769', N'Bình Phước', N'284764232')
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (6, N'Nguyễn Thành Danh', CAST(N'1997-12-01' AS Date), N'0972657456', N'Đồng Nai', N'286745453')
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (7, N'Ngô Văn Tạo', CAST(N'1974-08-01' AS Date), N'0987654321', N'Cà Mau', N'287463537')
INSERT [dbo].[Customer] ([CustomerID], [Ten], [NgaySinh], [SDT], [DiaChi], [Cmnd]) VALUES (9, N'Nguyễn Thế Lữ', CAST(N'1997-06-27' AS Date), N'0987564675', N'Cái Bè', N'286574654')
SET IDENTITY_INSERT [dbo].[Customer] OFF
SET IDENTITY_INSERT [dbo].[PhieuBanHang] ON 
--Bảng Hóa đơn bán hàng
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (1, 3, 5, N'admin', CAST(N'2019-01-02 17:05:28.887' AS DateTime), 22990000, N'1 năm')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (2, 1, 5, N'admin', CAST(N'2019-01-03 18:06:58.767' AS DateTime), 37990000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (3, 1, 1, N'admin', CAST(N'2019-01-04 18:07:05.400' AS DateTime), 37990000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (4, 1, 4, N'admin', CAST(N'2019-01-04 18:07:08.567' AS DateTime), 37990000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (5, 1, 6, N'admin', CAST(N'2019-01-04 18:07:11.493' AS DateTime), 37990000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (6, 5, 6, N'admin', CAST(N'2019-01-05 18:07:14.550' AS DateTime), 5990000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (7, 6, 6, N'admin', CAST(N'2019-01-10 18:07:18.183' AS DateTime), 20990000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (8, 2, 6, N'admin', CAST(N'2019-01-11 18:07:21.840' AS DateTime), 28490000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (9, 2, 3, N'admin', CAST(N'2019-01-20 18:07:29.077' AS DateTime), 28490000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (10, 2, 5, N'admin', CAST(N'2019-01-19 18:07:34.007' AS DateTime), 28490000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (11, 6, 3, N'admin', CAST(N'2019-01-11 20:27:11.057' AS DateTime), 21990000, N'1 năm')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (12, 2, 7, N'admin', CAST(N'2019-01-12 19:49:51.343' AS DateTime), 29490000, N'6 tháng')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (13, 3, 5, N'admin', CAST(N'2019-01-12 19:52:24.233' AS DateTime), 23990000, N'1 năm')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (14, 4, 3, N'admin', CAST(N'2019-01-12 19:54:28.120' AS DateTime), 7990000, N'1 năm')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (15, 6, 9, N'admin', CAST(N'2019-01-12 19:55:19.113' AS DateTime), 22990000, N'2 năm')
INSERT [dbo].[PhieuBanHang] ([PhieuBanHangID], [ProductID], [CustomerID], [TenNguoiBan], [NgayBan], [GiaBan], [BaoHanh]) VALUES (16, 2, 7, N'admin', CAST(N'2019-01-12 20:01:15.610' AS DateTime), 27490000, N'2 năm')
SET IDENTITY_INSERT [dbo].[PhieuBanHang] OFF
SET IDENTITY_INSERT [dbo].[Users] ON 
--Bảng tài khoản
INSERT [dbo].[Users] ([UserID], [UserName], [Password], [UserType]) VALUES (1, N'admin', N'admin', 1)
INSERT [dbo].[Users] ([UserID], [UserName], [Password], [UserType]) VALUES (2, N'quanly', N'1234', 0)
INSERT [dbo].[Users] ([UserID], [UserName], [Password], [UserType]) VALUES (3, N'minhcuong', N'2612', 0)
INSERT [dbo].[Users] ([UserID], [UserName], [Password], [UserType]) VALUES (4, N'cuongthuy', N'123', 1)
SET IDENTITY_INSERT [dbo].[Users] OFF