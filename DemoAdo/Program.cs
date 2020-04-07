using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Data;
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
            //TestConnectionPool();

            //TestCreateCommand();
            //TestCommandExecuteNonQuery();
            //TestExecuteScalar();


            //TestExecuteReader();
            #endregion

            Console.ReadKey();

        }

        private static void TestExecuteReader()
        {
            SqlDataReader dr = null;
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            string sql = "SELECT * FROM tblUser";
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);//如果dr关闭，conn也跟着关闭；如果关闭conn,dr也随之关闭
            int userid;
            string userName;

            while (dr.Read())
            {
                userid = int.Parse(dr["FUserID"].ToString());
                userName = dr["FUserName"].ToString();
                Console.WriteLine(userid + userName);
            }
            dr.Close();
            conn.Close();
        }

        private static void TestExecuteScalar()
        {
            //ExecuteScalar方法
            ///执行查询语句或储存过程，返回查询结果集中第一行第一列的值，忽略其他行或列
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            object o = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                //创建命令
                string sql = "SELECT * FROM tblUser";
                string sql1 = "SELECT COUNT(1) FROM tblUser";
                SqlCommand cmd = new SqlCommand(sql1, conn);
                conn.Open();
                ///使用：做查询，返回一个值  记录数 数据运算而出的一个结果
                o = cmd.ExecuteScalar();
            }
            if (o != null)
            {
                Console.WriteLine(o);
            }
        }

        private static void TestCommandExecuteNonQuery()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                //创建命令
                string uName = "Jason";
                string uPwd = "123";
                string uWorkName = "001";
                string sql = $"INSERT INTO tblUser(FUserName,FPassword,FWorkNum) VALUES('{uName}','{uPwd}','{uWorkName}')";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                //1.执行T-SQL语句或储存过程，并返回受影响的行数
                ///命令类型：插入、更新、删除 -----DML
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                {
                    Console.WriteLine("用户信息添加成功！");
                }
                string sql1 = "DeLETE FROM tblUser WHERE FUserID>7";
                SqlCommand cmd1 = new SqlCommand(sql1, conn);
                int count1 = cmd1.ExecuteNonQuery();
                if (count > 0)
                {
                    Console.WriteLine("用户信息删除成功！");
                }
            }
        }

        private static void TestCreateCommand()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    ///创建命令，执行命令的对象 执行命令
                    ///命令 --T-SQL或储存过程 --数据库里面创建
                    ///SqlCommand 对SqlServer数据库执行的一个T-SQL语句
                    ///CommandText:获取或设置要执行的T-SQL语句或储存过程名
                    ///CommandType: CommandType.Text --执行是一个sql语句
                    ///  CommandType.StoreProcedure --执行的是一个储存过程
                    ///Parameters:SqlCommand对象的命令参数集合  空集合
                    ///Transaction:获取设置要在其中执行的事务
                    //第1钟方式
                    string sql = "SELECT * FROM tblMaterial";
                    SqlCommand cmd1 = new SqlCommand();
                    cmd1.Connection = conn;
                    cmd1.CommandText = sql;
                    //cmd1.CommandType = CommandType.Text; //没有必要的
                    //cmd1.CommandType = CommandType.StoredProcedure;//如果是储存过程必须设置
                    //第2种方式
                    SqlCommand cmd2 = new SqlCommand(sql);
                    cmd2.Connection = conn;
                    //第3钟方式
                    SqlCommand cmd3 = new SqlCommand(sql, conn);
                    //第4钟方式
                    SqlCommand cmd4 = conn.CreateCommand();
                    cmd4.CommandText = sql;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 测试连接池
        /// </summary>
        private static void TestConnectionPool()
        {
            //连接池 
            //Ado.Net是默认启用连接池的
            // Max Pool Size:最大连接数:100
            // Min Pool Size:最小连接数:0
            //Pooling 是否启用连接池 true,设置为false后，设置最大连接池数是无效的
            #region 测试连接池的性能测试
            /*
            string connStr = "Server=.;database=HG;uid=sa;pwd=Chendong144216,;max Pool Size=5;Pooling=false";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 100; i++)
            {
                SqlConnection conn = new SqlConnection(connStr);
                conn.Open();
                //Console.WriteLine($"第{i+1}个链接已经打开");
                conn.Close();
            }
            Console.WriteLine($"不启用连接池，耗时:{sw.ElapsedMilliseconds}ms!"); //400+毫秒

            Stopwatch sw1 = new Stopwatch();
            sw1.Stop();
            string connStr1 = "Server=.;database=HG;uid=sa;pwd=Chendong144216,";
            sw1.Start();
            for (int i = 0; i < 100; i++)
            {
                SqlConnection conn1 = new SqlConnection(connStr1);
                conn1.Open();
                //Console.WriteLine($"第{i + 1}个链接已经打开");
                conn1.Close();
            }
            Console.WriteLine($"启用连接池，耗时:{sw1.ElapsedMilliseconds}ms!");//4毫秒
            sw1.Stop();
            */
            #endregion

            #region 链接字符串对连接池的影响
            //链接字符串区分连接池类型
            //connStr1与connStr3一样的，所以他们公用一个连接池，connStr2会单独创建一个连接池，2个连接池
            string connStr1 = "Server=.;database=HG;uid=sa;pwd=Chendong144216,;max Pool Size=5";
            string connStr2 = "Server=.;database=HG; uid=sa;pwd=Chendong144216,;max Pool Size=5";
            string connStr3 = "Server=.;database=HG;uid=sa;pwd=Chendong144216,;max Pool Size=5";
            for (int i = 0; i < 5; i++)
            {
                SqlConnection conn1 = new SqlConnection(connStr1);
                conn1.Open();
                Console.WriteLine($"conn1第{i + 1}个链接打开！");
                SqlConnection conn2 = new SqlConnection(connStr2);
                conn2.Open();
                Console.WriteLine($"conn2第{i + 1}个链接打开！");
                SqlConnection conn3 = new SqlConnection(connStr3);
                conn3.Open();
                Console.WriteLine($"conn3第{i + 1}个链接打开！");
            }
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
