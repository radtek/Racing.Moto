using Racing.Moto.Core.Extentions;
using Racing.Moto.Data.Entities;
using Racing.Moto.Data.Enums;
using Racing.Moto.Data.Models;
using Racing.Moto.Services.Constants;
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
        //  public void GetReports(ReportSearchModel model)
        //  {
        //      var sql = new StringBuilder();
        //      sql.AppendLine("");

        //      /*

        //       SELECT [B].BetId, [B].UserId, [B].PKId
        //,[BI].BetItemId, [BI].[Rank], [BI].Num, [BI].Amount, [BI].CreateTime 
        //,ISNULL([PKB].BonusType, 1) AS BonusType, ISNULL([PKB].Amount, 0) AS BonusAmount
        //      FROM [dbo].[Bet] [B]
        //      INNER JOIN [dbo].[BetItem] [BI] ON [B].BetId = [BI].BetId
        //      LEFT JOIN [dbo].[PKBonus] [PKB] ON [B].PKId = [PKB].PKId AND [BI].[Rank] = [PKB].[Rank] AND [BI].Num = [PKB].Num AND [PKB].BonusType = 1 --奖金

        //      INNER JOIN (
        //       -- General Agent
        //       SELECT * FROM [dbo].[User] WHERE ParentUserId = 1
        //       UNION
        //       -- Agent
        //       SELECT [U1].* FROM [dbo].[User] [U1] 
        //       INNER JOIN  [dbo].[User] [U2] ON [U1].ParentUserId = [U2].UserId
        //       WHERE [U2].ParentUserId = 1
        //       UNION
        //       -- Member
        //       SELECT [U1].* FROM [dbo].[User] [U1] 
        //       INNER JOIN  [dbo].[User] [U2] ON [U1].ParentUserId = [U2].UserId
        //       INNER JOIN  [dbo].[User] [U3] ON [U2].ParentUserId = [U3].UserId
        //       WHERE [U3].ParentUserId = 1
        //      ) AS [U] ON [B].UserId = [U].UserId

        //       */
        //  }



        /*
            笔数：一场比赛，下注多个盘口，每个盘口算1个笔数
            有效金额：所有下线会员加一起的下注金额总合
            会员输赢：会员输了多少钱，所有的会员组合加一起的输赢（档期都有该数据）（会员输了就是负数，赢了是正数）
            应收下线：下级会员输的金额，应该付给上级多少钱？（会员）
            实占成数：假的，做成0%
            实占注额：假的，做成0
            实占输赢：假的，做成0
            实占退水：假的，做成0
            实占结果：假的，做成0
            赚取水钱：1%的退水，根据后台管理分配的退水比率分配
            赚水后结果：赚取水后，这个没搞明白，。。
            贡献上级：按照有效金额写
            应付上级：应收下线+赚取水钱
         */

        #region General Agent
        public PagerResult<ReportModel> GetGeneralAgentReports(ReportSearchModel model)
        {
            var reports = new PagerResult<ReportModel>();
            reports.Items = new List<ReportModel>();

            var generalSql = GetGeneralUserIdsSql(model);
            var generalQuery = db.Database.SqlQuery<int>(generalSql);
            var generalCount = generalQuery.Count();
            var generalUserIds = generalQuery.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();

            reports.RowCount = generalCount;
            reports.PageCount = (int)Math.Ceiling(generalCount * 1.0 / model.PageSize);

            // 取用户
            var users = db.User.Where(u => generalUserIds.Contains(u.UserId)).ToList();

            // 取下注: 笔数
            var betReportSql = GetGeneralBetReportSql(model);
            var dbBetReports = db.Database.SqlQuery<BetReportModel>(betReportSql).ToList();

            // 取奖金
            var bonusReportSql = GetGeneralBonusReportSql(model);
            var dbBonusReports = db.Database.SqlQuery<BonusReportModel>(bonusReportSql).ToList();


            foreach (var user in users)
            {
                var betReport = dbBetReports.Where(b => b.UserId == user.UserId).FirstOrDefault();
                var bonusReports = dbBonusReports.Where(b => b.UserId == user.UserId).ToList();
                var betAmount = betReport != null ? betReport.Amount : 0;
                var bonusAmount = bonusReports.Count() > 0 ? bonusReports.Sum(b => b.Amount) : 0;
                var rebateAmount = bonusReports.Count() > 0
                    ? bonusReports.Where(b => b.BonusType == BonusType.Rebate && b.UserId == b.GeneralAgentUserId).Sum(b => b.Amount)
                    : 0;

                reports.Items.Add(new ReportModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    BetCount = betReport != null ? betReport.BetCount : 0,
                    BetAmount = betAmount,
                    MemberWinOrLoseAmount = bonusAmount - betAmount,
                    ReceiveAmount = betAmount,
                    RebateAmount = rebateAmount,
                    ContributeHigherLevelAmount = betAmount,
                    PayHigherLevelAmount = betAmount
                });
            }


            return reports;
        }

        private string GetGeneralUserIdsSql(ReportSearchModel model)
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT DISTINCT GeneralAgentUserId");
            sql.AppendLine("FROM [dbo].[Bet]");
            sql.AppendLine(GetWhereSql(model));

            return sql.ToString();
        }

        private string GetGeneralBetReportSql(ReportSearchModel model)
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT GeneralAgentUserId UserId, Count(0) BetCount, Sum(Amount) Amount");
            sql.AppendLine("FROM [dbo].[Bet]");
            sql.AppendLine(GetWhereSql(model));
            sql.AppendLine("GROUP BY GeneralAgentUserId");

            return sql.ToString();
        }

        private string GetGeneralBonusReportSql(ReportSearchModel model)
        {
            var sql = new StringBuilder();

            sql.AppendLine("SELECT [PKB].UserId, GeneralAgentUserId, BonusType, Sum([PKB].Amount) Amount");
            sql.AppendLine("FROM [dbo].[PKBonus] [PKB]");
            sql.AppendLine("INNER JOIN [dbo].[PK] [PK] ON [PK].PKId = [PKB].PKId");
            sql.AppendLine("INNER JOIN [dbo].[UserExtension] [UE] ON [UE].UserId = [PKB].UserId");
            sql.AppendLine(GetWhereSql(model));
            sql.AppendLine("GROUP BY [PKB].UserId, GeneralAgentUserId, [PKB].BonusType");

            return sql.ToString();
        }
        #endregion

        private string GetWhereSql(ReportSearchModel model)
        {
            var sql = new StringBuilder(); sql.AppendLine(string.Format("WHERE IsSettlementDone = {0}", model.SettlementType));

            // 1:按期數, 2:按日期
            if (model.SearchType == 1)
            {
                sql.AppendLine(string.Format("AND PKId = {0}", model.PKId));
            }
            else
            {
                if (model.FromDate.HasValue)
                {
                    sql.AppendLine(string.Format("AND DATEDIFF(DAY, '{0}', CreateTime) >= 0", model.FromDate.Value.ToString("yyyy/MM/dd")));
                }
                if (model.ToDate.HasValue)
                {
                    sql.AppendLine(string.Format("AND DATEDIFF(DAY, '{0}', CreateTime) <= 0", model.ToDate.Value.ToString("yyyy/MM/dd")));
                }
            }

            return sql.ToString();
        }
    }
}
