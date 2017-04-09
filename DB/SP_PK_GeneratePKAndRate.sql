/****** Object:  StoredProcedure [dbo].[SP_PK_GeneratePK]    Script Date: 2017/3/4 21:35:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* =============================================
-- Description:	Éú³ÉPK
   exec [dbo].[SP_PK_GeneratePK]
-- ============================================= */
ALTER PROCEDURE [dbo].[SP_PK_GeneratePK]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @Now DATETIME
	SET @Now = GETDATE()


    -- check PK exist or not
	IF(EXISTS(SELECT 1 FROM [dbo].[PK] WHERE BeginTime <= @Now AND @Now<= EndTime))
		BEGIN
			SELECT * FROM [dbo].[PK] WHERE BeginTime <= @Now AND @Now<= EndTime
			RETURN;
		END

	-- insert new pk
	DECLARE @PKId INT,
			@Total_Seconds INT,
			@Opening_Seconds INT,
			@Close_Seconds INT,
			@Game_Seconds INT,
			@Lottery_Seconds INT,
			@End_Time DATETIME
	SELECT @Opening_Seconds = CAST([Value] AS INT) FROM [dbo].[AppConfig] WHERE [Name] = 'Racing_Opening_Seconds'
	SELECT @Close_Seconds = CAST([Value] AS INT) FROM [dbo].[AppConfig] WHERE [Name] = 'Racing_Close_Seconds'
	SELECT @Game_Seconds = CAST([Value] AS INT) FROM [dbo].[AppConfig] WHERE [Name] = 'Racing_Game_Seconds'
	SELECT @Lottery_Seconds = CAST([Value] AS INT) FROM [dbo].[AppConfig] WHERE [Name] = 'Racing_Lottery_Seconds'
	SELECT @Total_Seconds = @Opening_Seconds + @Close_Seconds + @Game_Seconds + @Lottery_Seconds
	SELECT @End_Time = DATEADD(second, @Total_Seconds, @Now)

	INSERT INTO [dbo].[PK] ([BeginTime],[EndTime],[CreateTime],[OpeningSeconds],[CloseSeconds],[GameSeconds],[LotterySeconds],[IsBonused],[IsRebated]) 
	VALUES (@Now, @End_Time, @Now, @Opening_Seconds, @Close_Seconds, @Game_Seconds, @Lottery_Seconds, 0, 0)
	
	SET @PKId=@@IDENTITY
	SELECT * FROM [dbo].[PK] WHERE PKId = @PKId

	-- insert PKRate
	DECLARE @RateId INT,
			@Rank INT,
			@Rate1 DECIMAL(18,2),
			@Rate2 DECIMAL(18,2),
			@Rate3 DECIMAL(18,2),
			@Rate4 DECIMAL(18,2),
			@Rate5 DECIMAL(18,2),
			@Rate6 DECIMAL(18,2),
			@Rate7 DECIMAL(18,2),
			@Rate8 DECIMAL(18,2),
			@Rate9 DECIMAL(18,2),
			@Rate10 DECIMAL(18,2),
			@Rate11 DECIMAL(18,2),
			@Rate12 DECIMAL(18,2),
			@Rate13 DECIMAL(18,2),
			@Rate14 DECIMAL(18,2)

	SELECT  DealFlg = 0,
			RateId, [Rank],
			Rate1, Rate2, Rate3, Rate4, Rate5, 
			Rate6, Rate7, Rate8, Rate9, Rate10, 
			Big, Small, Odd, Even
	INTO #Temp_Rate
	FROM [dbo].[Rate]
	WHERE [RateType] = 0

	SELECT @RateId = MIN(RateId) FROM #Temp_Rate WHERE DealFlg = 0
	WHILE @RateId IS NOT NULL
		BEGIN
			SELECT  @Rank = [Rank],
					@Rate1 = Rate1,
					@Rate2 = Rate2,
					@Rate3 = Rate3,
					@Rate4 = Rate4,
					@Rate5 = Rate5,
					@Rate6 = Rate6,
					@Rate7 = Rate7,
					@Rate8 = Rate8,
					@Rate9 = Rate9,
					@Rate10 = Rate10,
					@Rate11 = Big,
					@Rate12 = Small,
					@Rate13 = Odd,
					@Rate14 = Even
			FROM #Temp_Rate WHERE RateId = @RateId

			-- insert into PKRate
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 1, @Rate1)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 2, @Rate2)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 3, @Rate3)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 4, @Rate4)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 5, @Rate5)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 6, @Rate6)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 7, @Rate7)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 8, @Rate8)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 9, @Rate9)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 10, @Rate10)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 11, @Rate11)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 12, @Rate12)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 13, @Rate13)
			INSERT INTO [dbo].[PKRate] ([PKId], [Rank], [Num], [Rate]) VALUES (@PKId, @Rank, 14, @Rate14)

			-- update DealFlg
			UPDATE #Temp_Rate SET DealFlg = 1 WHERE RateId = @RateId

			-- get next row
			SELECT @RateId = MIN(RateId) FROM #Temp_Rate WHERE DealFlg = 0 AND RateId > @RateId
		END

END
