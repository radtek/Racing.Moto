USE [Racing.Moto]
GO

SET IDENTITY_INSERT [dbo].[PK] ON 

GO
Insert Into [dbo].[PK] ([PKId],[BeginTime],[EndTime],[CreateTime],[OpeningSeconds],[CloseSeconds],[GameSeconds],[LotterySeconds],[IsBonused],[IsRebated],[IsRanksSynced])
Values (30000, GETDATE(), GETDATE(), GETDATE(),1,1,1,1,1,1,1)
GO

SET IDENTITY_INSERT [dbo].[PK] OFF
GO

delete from [dbo].[PK] where PKID = 30000