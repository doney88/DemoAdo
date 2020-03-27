using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DemoAdo
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 已经学完

            //ConnectDataBase();


            //TryTransactionScope();

            //TestDataTable();

            //事务SqlTransaction
            //TestTrasactionAdo();

            #endregion
        }

        private static void ConnectDataBase()
        {
            SqlConnection conn = new SqlConnection();
            SqlConnectionStringBuilder connStrBuilder = new SqlConnectionStringBuilder();
            connStrBuilder.DataSource = ".";
            connStrBuilder.InitialCatalog = "HG";
            connStrBuilder.UserID = "sa";
            connStrBuilder.Password = "Chendong144216,";
            connStrBuilder.Pooling = false; //禁用连接池
            string Connstring = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
            string connStr = ConfigurationManager.AppSettings["SqlConnectionString"].ToString();
            conn.ConnectionString = "Server=DANLAPTOP\\HG;DataBase=HG;uid=sa;pwd=Chendong144216,;";
            //Sql server 身份验证
            //conn.ConnectionString = "Data Source=.;Initial Catalog=HG;User Id=sa;Password=Chendong144216,;";
            //Windows身份验证
            //conn.ConnectionString = "Server=DANLAPTOP\\HG;DataBase=HG;Integrated Security=SSPI";

            //oracle
            // Data Source User Id Password

            //MySql
            // Data Source Initial Catalog User Id Passsword

            //Access
            //Provider=Microsoft.Jet.OLEDB.4.0;    Data Source=文件的绝对路径  User Id=admin; Password = 

            string dataBaseString = conn.Database;  //要链接的数据库名称（只读属性）
            string dataSuorceString = conn.DataSource; //数据源 Local . Ip,端口号 （只读属性）
            ConnectionState connState = conn.State; //链接状态（只读属性）
            int timeOut = conn.ConnectionTimeout; //链接超时 ，默认是15

            conn.Open();
            conn.Close();   //后还可以用Open()方法重新打开
            conn.Dispose(); //所有属性清空，重新打开需要重新设置链接字符串
        }

        private static void TestTrasactionAdo()
        {
            DbConnection conn = new SqlConnection("Data Source=DANLAPTOP\\HG;Initial Catalog=HG;Persist Security Info=True;User ID=sa;Password=Chendong144216,");
            using (conn)
            {
                DbTransaction tran = null;
                try
                {

                    //条件：链接打开
                    conn.Open();
                    tran = conn.BeginTransaction();//开启一个事务
                    DbCommand cmd = conn.CreateCommand();
                    cmd.Transaction = tran;//设置要执行的事务
                    //定义要执行的操作
                    cmd.CommandText = "INSERT INTO tblCat1(FCat1,FCat1Code) values(@FCat1,@FCat1Code);SELECT @@identity";
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new SqlParameter("@FCat1", "测试"));
                    cmd.Parameters.Add(new SqlParameter("@FCat1Code", "C"));
                    object oId = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    int cat1ID = 0;
                    if (oId != null)
                    {
                        cat1ID = int.Parse(oId.ToString());
                    }
                    cmd.CommandText = "INSERT INTO tblCat2(FCat2,FCat2Code,FCat1ID) VALUES(@FCat2,@FCat2Code,@FCat1ID)";
                    cmd.Parameters.Add(new SqlParameter("@FCat2", "测试"));
                    cmd.Parameters.Add(new SqlParameter("@FCat2Code", "C"));
                    cmd.Parameters.Add(new SqlParameter("@FCat1ID", cat1ID));
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    tran.Dispose();
                    conn.Close();
                }

            }
        }

        private static void TestDataTable()
        {
            //独立创建与使用
            //1.
            //DataTable dt = new DataTable();
            //dt.TableName = "tblOrderSub";
            //2.
            DataTable dt = new DataTable("tblOrderSub");
            DataColumn dc = new DataColumn();
            dc.ColumnName = "FSKUID";
            dc.DataType = typeof(int);
            dt.Columns.Add(dc);
            dt.Columns.Add("FOrderID", typeof(int));
            dt.Columns.Add("FMaterialID", typeof(int));
            dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };  //设置主键
            dt.Constraints.Add(new UniqueConstraint(dt.Columns[1])); //添加唯一约束

            //架构定义好了，添加数据
            DataRow dr = dt.NewRow(); //具有相同的架构
            dr[0] = 1;
            dr["FOrderID"] = 1;
            dr["FMaterialId"] = 1;      //Detached
            //这条数据并没有加到dt表中
            dt.Rows.Add(dr);//添加到dt表中   Added
            //dt.RejectChanges();//回滚
            //dt.AcceptChanges();//提交更改 Unchanged

            dr["FMaterialID"] = 2; ///修改    Modified

            //dr.Delete(); // Deleted

            //dt.AcceptChanges();//Detached
            //dt.Rows.Remove(dr);//Detached
            //DataRow ---RowState:Detached  Added  Unchanged Modifyied Deleted
            //dt.Clear();//清楚数据
            DataTable dt2 = dt.Copy(); //复制架构和数据
            DataTable dt3 = dt.Clone();//支付至架构不保存数据
            DataRow dr1 = dt2.NewRow();
            dr1[0] = 2;
            dr1["FOrderID"] = 2;
            dr1["FMaterialID"] = 3;
            dt2.Rows.Add(dr1);

            dt.Merge(dt2);
            DataRow[] rows = dt.Select();//获取所得的行
            DataRow[] rows1 = dt.Select("FMaterialID=2", "FSKUID desc");//按条件筛选，排序
        }

        private static void TryTransactionScope()
        {
            string sql;
            var option = new TransactionOptions();
            option.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,option))
            {
                SqlConnection conn = new SqlConnection("Data Source=DANLAPTOP\\HG;Initial Catalog=HG;Persist Security Info=True;User ID=sa;Password=Chendong144216,");
                conn.Open();

                tblOrder tblOrder = new tblOrder() { FOrderNum = "12345", FOrderType = "大货", FClientID = 1086, FOrderStatus = "3_待排产" };
                //EFContext context = new EFContext();

                //context.tblOrders.Add(tblOrder);
                //context.SaveChanges();
                int orderID = tblOrder.FOrderID;
                sql = $"INSERT INTO tblOrderSub (FOrderID,FMaterialID,FQty,FQtyShip,FFinishSKU,FModelCust,FColorCust)" +
                                        $" VALUES(@FOrderID,@FMaterialID,@FQty,@FQtyShip,@FFinishSKU,@FModelCust,@FColorCust)";
                SqlParameter[] sqlParameters =
                    {
                        new SqlParameter("@FOrderID",orderID),
                        new SqlParameter("@FMaterialID",1),
                        new SqlParameter("@FQty",1),
                        new SqlParameter("@FQtyShip",1),
                        new SqlParameter("@FFinishSKU",false),
                        new SqlParameter("@FModelCust", "TEst"),
                        new SqlParameter("@FColorCust", "C1")
                    };
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(sqlParameters);
                cmd.ExecuteNonQuery();
                scope.Complete();
            }
        }
    }
}
