USE [HeThongGiaoHang]
GO
/****** Object:  Table [dbo].[Admin]    Script Date: 18/07/2024 2:11:19 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admin](
	[AdminID] [int] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[MatKhau] [nvarchar](255) NOT NULL,
	[Quyen] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[AdminID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ChiTietDonHang]    Script Date: 18/07/2024 2:11:19 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ChiTietDonHang](
	[ChiTietDonHangID] [int] IDENTITY(1,1) NOT NULL,
	[DonHangID] [int] NOT NULL,
	[TenHangHoa] [nvarchar](255) NOT NULL,
	[TienThuHoCOD] [decimal](18, 2) NULL,
	[IsDeleted] [bit] NULL,
	[TienVanChuyen] [decimal](18, 2) NULL,
	[KhoiLuong] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ChiTietDonHangID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DonHang]    Script Date: 18/07/2024 2:11:19 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DonHang](
	[DonHangID] [int] IDENTITY(1,1) NOT NULL,
	[KhachHangID] [int] NOT NULL,
	[ShipperID] [int] NULL,
	[TrangThaiID] [int] NOT NULL,
	[NgayTao] [datetime] NULL,
	[NgayCapNhat] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[DiaChiNhanHang] [nvarchar](255) NULL,
	[DiaChiGiaoHang] [nvarchar](255) NULL,
	[SoDienThoaiNguoiNhan] [nvarchar](50) NULL,
	[SoDienThoaiNguoiGui] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[DonHangID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KhachHang]    Script Date: 18/07/2024 2:11:19 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KhachHang](
	[KhachHangID] [int] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[MatKhau] [nvarchar](255) NOT NULL,
	[Quyen] [nvarchar](50) NOT NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[KhachHangID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LyDoHoanHang]    Script Date: 18/07/2024 2:11:19 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LyDoHoanHang](
	[LyDoHoanHangID] [int] IDENTITY(1,1) NOT NULL,
	[DonHangID] [int] NOT NULL,
	[LyDo] [nvarchar](255) NOT NULL,
	[NgayTao] [datetime] NULL,
	[AdminID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[LyDoHoanHangID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Shipper]    Script Date: 18/07/2024 2:11:19 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shipper](
	[ShipperID] [int] IDENTITY(1,1) NOT NULL,
	[HoTen] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NULL,
	[MatKhau] [nvarchar](255) NULL,
	[Quyen] [nvarchar](50) NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK__Shipper__1F8AFFB91DAA6DCA] PRIMARY KEY CLUSTERED 
(
	[ShipperID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TrangThaiDonHang]    Script Date: 18/07/2024 2:11:19 SA ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrangThaiDonHang](
	[TrangThaiID] [int] IDENTITY(1,1) NOT NULL,
	[MoTaTrangThai] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[TrangThaiID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Admin] ON 

INSERT [dbo].[Admin] ([AdminID], [HoTen], [Email], [MatKhau], [Quyen], [IsDeleted]) VALUES (3, N'admin', N'admin@gmail.com', N'123', N'admin', 0)
SET IDENTITY_INSERT [dbo].[Admin] OFF
GO
SET IDENTITY_INSERT [dbo].[ChiTietDonHang] ON 

INSERT [dbo].[ChiTietDonHang] ([ChiTietDonHangID], [DonHangID], [TenHangHoa], [TienThuHoCOD], [IsDeleted], [TienVanChuyen], [KhoiLuong]) VALUES (1014, 1023, N'string', CAST(20.00 AS Decimal(18, 2)), 0, CAST(700.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)))
INSERT [dbo].[ChiTietDonHang] ([ChiTietDonHangID], [DonHangID], [TenHangHoa], [TienThuHoCOD], [IsDeleted], [TienVanChuyen], [KhoiLuong]) VALUES (1015, 1025, N'1', CAST(20.00 AS Decimal(18, 2)), 0, CAST(700.00 AS Decimal(18, 2)), CAST(20.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[ChiTietDonHang] OFF
GO
SET IDENTITY_INSERT [dbo].[DonHang] ON 

INSERT [dbo].[DonHang] ([DonHangID], [KhachHangID], [ShipperID], [TrangThaiID], [NgayTao], [NgayCapNhat], [IsDeleted], [DiaChiNhanHang], [DiaChiGiaoHang], [SoDienThoaiNguoiNhan], [SoDienThoaiNguoiGui]) VALUES (1023, 6, 4, 6, CAST(N'2024-07-18T01:28:50.943' AS DateTime), CAST(N'2024-07-18T02:02:53.007' AS DateTime), 0, N'a', N'b', N'1', N'2')
INSERT [dbo].[DonHang] ([DonHangID], [KhachHangID], [ShipperID], [TrangThaiID], [NgayTao], [NgayCapNhat], [IsDeleted], [DiaChiNhanHang], [DiaChiGiaoHang], [SoDienThoaiNguoiNhan], [SoDienThoaiNguoiGui]) VALUES (1025, 6, 4, 4, CAST(N'2024-07-18T01:40:15.510' AS DateTime), CAST(N'2024-07-18T02:01:50.450' AS DateTime), 0, N'a', N'b', N'1', N'2')
SET IDENTITY_INSERT [dbo].[DonHang] OFF
GO
SET IDENTITY_INSERT [dbo].[KhachHang] ON 

INSERT [dbo].[KhachHang] ([KhachHangID], [HoTen], [Email], [MatKhau], [Quyen], [IsDeleted]) VALUES (6, N'khach hang', N'kh@gmail.com', N'123', N'khachhang', 0)
SET IDENTITY_INSERT [dbo].[KhachHang] OFF
GO
SET IDENTITY_INSERT [dbo].[LyDoHoanHang] ON 

INSERT [dbo].[LyDoHoanHang] ([LyDoHoanHangID], [DonHangID], [LyDo], [NgayTao], [AdminID]) VALUES (2, 1023, N'string', CAST(N'2024-07-18T01:36:58.323' AS DateTime), 3)
INSERT [dbo].[LyDoHoanHang] ([LyDoHoanHangID], [DonHangID], [LyDo], [NgayTao], [AdminID]) VALUES (3, 1025, N'không thích', CAST(N'2024-07-18T01:41:37.020' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[LyDoHoanHang] OFF
GO
SET IDENTITY_INSERT [dbo].[Shipper] ON 

INSERT [dbo].[Shipper] ([ShipperID], [HoTen], [Email], [MatKhau], [Quyen], [IsDeleted]) VALUES (4, N'shipper 1', N'shiper@gmail.com', N'123', N'shipper', 0)
SET IDENTITY_INSERT [dbo].[Shipper] OFF
GO
SET IDENTITY_INSERT [dbo].[TrangThaiDonHang] ON 

INSERT [dbo].[TrangThaiDonHang] ([TrangThaiID], [MoTaTrangThai], [IsDeleted]) VALUES (1, N'Chờ duyệt', 0)
INSERT [dbo].[TrangThaiDonHang] ([TrangThaiID], [MoTaTrangThai], [IsDeleted]) VALUES (2, N'Đã Duyệt', 0)
INSERT [dbo].[TrangThaiDonHang] ([TrangThaiID], [MoTaTrangThai], [IsDeleted]) VALUES (3, N'Đang Giao Hàng', 0)
INSERT [dbo].[TrangThaiDonHang] ([TrangThaiID], [MoTaTrangThai], [IsDeleted]) VALUES (4, N'Đã Giao', 0)
INSERT [dbo].[TrangThaiDonHang] ([TrangThaiID], [MoTaTrangThai], [IsDeleted]) VALUES (5, N'Hoàn hàng đang xử lý', 0)
INSERT [dbo].[TrangThaiDonHang] ([TrangThaiID], [MoTaTrangThai], [IsDeleted]) VALUES (6, N'Hoàn hàng đã hoàn tất', 0)
SET IDENTITY_INSERT [dbo].[TrangThaiDonHang] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Admin__A9D1053466D66D49]    Script Date: 18/07/2024 2:11:19 SA ******/
ALTER TABLE [dbo].[Admin] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__NguoiDun__A9D105345CA8D750]    Script Date: 18/07/2024 2:11:19 SA ******/
ALTER TABLE [dbo].[KhachHang] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Admin] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ChiTietDonHang] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DonHang] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[DonHang] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[KhachHang] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[LyDoHoanHang] ADD  DEFAULT (getdate()) FOR [NgayTao]
GO
ALTER TABLE [dbo].[Shipper] ADD  CONSTRAINT [DF__Shipper__IsDelet__49C3F6B7]  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[TrangThaiDonHang] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[ChiTietDonHang]  WITH CHECK ADD FOREIGN KEY([DonHangID])
REFERENCES [dbo].[DonHang] ([DonHangID])
GO
ALTER TABLE [dbo].[DonHang]  WITH CHECK ADD FOREIGN KEY([KhachHangID])
REFERENCES [dbo].[KhachHang] ([KhachHangID])
GO
ALTER TABLE [dbo].[DonHang]  WITH CHECK ADD FOREIGN KEY([TrangThaiID])
REFERENCES [dbo].[TrangThaiDonHang] ([TrangThaiID])
GO
ALTER TABLE [dbo].[DonHang]  WITH CHECK ADD  CONSTRAINT [FK_DonHang_Shipper] FOREIGN KEY([ShipperID])
REFERENCES [dbo].[Shipper] ([ShipperID])
GO
ALTER TABLE [dbo].[DonHang] CHECK CONSTRAINT [FK_DonHang_Shipper]
GO
ALTER TABLE [dbo].[LyDoHoanHang]  WITH CHECK ADD FOREIGN KEY([DonHangID])
REFERENCES [dbo].[DonHang] ([DonHangID])
GO
ALTER TABLE [dbo].[LyDoHoanHang]  WITH CHECK ADD  CONSTRAINT [FK_LyDoHoanHang_Admin] FOREIGN KEY([AdminID])
REFERENCES [dbo].[Admin] ([AdminID])
GO
ALTER TABLE [dbo].[LyDoHoanHang] CHECK CONSTRAINT [FK_LyDoHoanHang_Admin]
GO
