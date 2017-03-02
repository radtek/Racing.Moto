using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Racing.Moto.Services
{
    /// <summary>
    /// 报表
    /// </summary>
    public class ReportService : BaseServcice
    {
        public void GetReports(ReportSearchModel model)
        {
            var sql = new StringBuilder();
            sql.AppendLine("");

            /*
             
             SELECT [B].BetId, [B].UserId, [B].PKId
		    ,[BI].BetItemId, [BI].[Rank], [BI].Num, [BI].Amount, [BI].CreateTime 
		    ,ISNULL([PKB].BonusType, 1) AS BonusType, ISNULL([PKB].Amount, 0) AS BonusAmount
            FROM [dbo].[Bet] [B]
            INNER JOIN [dbo].[BetItem] [BI] ON [B].BetId = [BI].BetId
            LEFT JOIN [dbo].[PKBonus] [PKB] ON [B].PKId = [PKB].PKId AND [BI].[Rank] = [PKB].[Rank] AND [BI].Num = [PKB].Num AND [PKB].BonusType = 1 --奖金

            INNER JOIN (
	            -- General Agent
	            SELECT * FROM [dbo].[User] WHERE ParentUserId = 1
	            UNION
	            -- Agent
	            SELECT [U1].* FROM [dbo].[User] [U1] 
	            INNER JOIN  [dbo].[User] [U2] ON [U1].ParentUserId = [U2].UserId
	            WHERE [U2].ParentUserId = 1
	            UNION
	            -- Member
	            SELECT [U1].* FROM [dbo].[User] [U1] 
	            INNER JOIN  [dbo].[User] [U2] ON [U1].ParentUserId = [U2].UserId
	            INNER JOIN  [dbo].[User] [U3] ON [U2].ParentUserId = [U3].UserId
	            WHERE [U3].ParentUserId = 1
            ) AS [U] ON [B].UserId = [U].UserId
             
             */
        }
    }
}
