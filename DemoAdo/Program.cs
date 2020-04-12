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
using System.Data.Entity;
using System.Runtime.InteropServices;

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
            //TestOutputParameter();
            //TestInputOutParameter();
            //TestReturnParameter();
            //TestDataReader();
            //TestDataSetRealtions();
            #endregion


            Console.ReadKey();

        }

        private static void TestDataSetRealtions()
        {
            ///概念 DataSet数据在内存中的缓存 --- 内存中的数据库，DataTable内存数据库中的一个表，
            ///     Ado.Net的核心组件
            ///成员 一组DataTable的组成。DataRelation 相互关联，一起实施了数据的完整性。
            /// 
            ///应用 三种，结合DataAdapter使用
            ///一 DataAdapter讲数据填充到DataSet中
            ///二 DataAdapter 将DataSet中的更改提交到数据库
            ///三 XML文档或文本加载到DataSet中
            ///
            ///作用：Ds将数据加载到内存中来执行，提高了数据访问的速度，提到硬盘数据的安全性，程序运行的速度和稳定性。
            ///
            ///特性：独立性，不依赖与任何数据库、离线和连接。数据视图  数据操作：灵活
            ///
            ///创建 DataSet() DataSet(名称)

            //1.
            //DataSet ds = new DataSet();
            //ds.DataSetName = "ds1"; //默认名称 NewDataSet

            //2. 
            DataSet ds = new DataSet("ds1");

            //常用属性
            //DataSetName ds名称
            DataTable dt1 = new DataTable("User");
            DataTable dt2 = new DataTable("Dept");
            //Tables DataTable集合
            ds.Tables.Add(dt1); //添加dt1到ds中
            ds.Tables.Add(dt2); //添加dt2到ds中
            //DataTable dt = ds.Tables[0];//获取表
            dt1.Columns.Add("UserId", typeof(int));
            dt1.Columns.Add("UserName", typeof(string));
            dt1.Columns.Add("Age", typeof(int));
            dt1.Columns.Add("DeptId", typeof(int));

            dt2.Columns.Add("DeptId", typeof(int));
            dt2.Columns.Add("DeptName", typeof(string));

            //dt1.PrimaryKey = new DataColumn[] { dt1.Columns[0] };//主键 --铸件约束
            //dt2.Constraints.Add(new UniqueConstraint("uc", dt2.Columns[1]));//添加一个唯一约束，限制数据 规则，阻止

            //dt1.Constraints.Add(new ForeignKeyConstraint("fk", dt2.Columns[0], dt1.Columns[3]));//外键约束
            //默认情况下，建立关系，就会自动为附表中建立唯一约束，子表中外键建立一个外键约束
            DataRelation relation = new DataRelation("relation", dt2.Columns[0], dt1.Columns[3], true);//关系 相互读取
            ds.Relations.Add(relation);//添加到ds.Relations中

            InitData(dt1, dt2);
            //通过关系，相互读取数据
            foreach (DataRow dr in dt2.Rows)
            {
                DataRow[] rows = dr.GetChildRows(relation);
                foreach (DataRow r in rows)
                {
                    Console.WriteLine($"UserId:{r[0].ToString()},UserName:{r[1].ToString()},Age:{r[2].ToString()},DeptId:{r[3].ToString()}");
                }
            }

            //子表读取父表中的数据

            DataRow row = dt1.Rows[1].GetParentRow(relation);
            Console.WriteLine($"DeptId:{row[0].ToString()},DeptName:{row[1].ToString()}");

            ////方法
            //ds.AcceptChanges();// 提交
            //ds.RejectChanges();//回滚
            //ds.Clear();//清楚所有表中的所有行的数据
            //ds.Copy();//复制结构和数据
            //ds.Clone();//复制架构，不包含数据
            ////ds.Merge(rows/Datatable/dataset); 合并
            //ds.Reset();//
            ////ds.Load(IDataReader);
        }

        static public void InitData(DataTable dt1, DataTable dt2)
        {
            DataRow dr2 = dt2.NewRow();
            dr2["DeptId"] = 1;
            dr2["DeptName"] = "人事部";
            dt2.Rows.Add(dr2);

            dr2 = dt2.NewRow();
            dr2["DeptId"] = 2;
            dr2["DeptName"] = "管理部";
            dt2.Rows.Add(dr2);

            dr2 = dt2.NewRow();
            dr2["DeptId"] = 3;
            dr2["DeptName"] = "销售部";
            dt2.Rows.Add(dr2);

            DataRow dr1 = dt1.NewRow();
            dr1["UserId"] = 1;
            dr1["UserName"] = "李明";
            dr1["Age"] = 22;
            dr1["DeptId"] = 3;
            dt1.Rows.Add(dr1);

            dr1 = dt1.NewRow();
            dr1["UserId"] = 2;
            dr1["UserName"] = "刘丽";
            dr1["Age"] = 24;
            dr1["DeptId"] = 1;
            dt1.Rows.Add(dr1);

            dr1 = dt1.NewRow();
            dr1["UserId"] = 3;
            dr1["UserName"] = "王力";
            dr1["Age"] = 23;
            dr1["DeptId"] = 3;
            dt1.Rows.Add(dr1);

        }
        private static void TestDataReader()
        {
            // SqlDataReader 从sql server数据库中杜宇只进的行流的方式
            ///特点：快速的、轻量级，只读的，遍历访问每一行数据的数据流，向一个方向，一行一行的，不能向后读取，不能修改数据
            ///缺点：不灵活，只适合数据小的情况，读取数据， 一直占用链接
            ///读取方式:Read() 获取第一行的数据，再次调用Read()方法的，
            ///         当调用Read()方法返回False时，就表示不再有数据行。
            /// 注意：
            ///     链接对象一直保持Open状态，如果链接关闭，是不能读取数据的。使用完成过后，应该马上调用Close()关闭，不然Reader对象会一直占用链接
            /// 创建方式，是不能直接构造的，cmd.ExecuteReader()来创建。
            /// cmd.ExecuteReader(CommandBehaviour.CloseConnection) --好处：关闭Reader对象时，就会自动关闭链接
            ///     读取时金亮使用与数据库字段类型相匹配的方法来取得对应值，会减少因类型不一致增加类型转换操作性能损耗
            ///     没有读取到末尾就要关闭reader对象时，先调用cmd.Cancel(),然后在调用Reader.CLose()
            ///     cmd.ExecuteReader()获取储存过程的返回值或输出参数，先调用reader.close(),然后才能获取参数值。
            /// 常用属性:
            ///     Connection:获取与Reader对象相关的SqlConnection
            ///     FieldCount:当前行中的列数
            ///     HasRows:Reader是否宝行一行还是多行
            ///     IsClose:reader对象人是否已经关闭 true false
            ///     Item[int]: 列序号，给定序列号的情况，获取指定列的值dr[1] object
            ///     Item[String]: 列明，后去指定列的值
            /// 常用方法：
            ///     Close() 关闭dr
            ///     GetInt32(列序号) --根据数据类型相匹配的方法
            ///     GetFieldType(i) 获取数据类型的Type
            ///     GetName(序列号) 获取指定列的列明
            ///     GetOrdinal(列明) 获取指定列名的序号
            ///     Read() 使dr前进到下一条记录
            ///     NextResult() 使dr前进到下一条记录
            string connstr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                string sql = "SELECT FUserID,FUserName,FPassword FROM tblUser";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();//必须在执行之前
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);//创建
                DataTable dt = new DataTable();
                //dt.Load(dr);
                List<tblUser> users = new List<tblUser>();

                if (dr.HasRows)
                {
                    int passWord = dr.GetOrdinal("FPassword");
                    int indexId = dr.GetOrdinal("FUserID");
                    int indexUserName = dr.GetOrdinal("FUserName");
                    while (dr.Read())//检测是否有数据
                    {
                        tblUser user = new tblUser();
                        //int userId = (int)dr[0]; //装箱拆箱性能损耗
                        user.FUserID = dr.GetInt32(indexId);
                        //dr.GetName(0);//获取之地当列序号的列名
                        user.FUserName = dr.GetString(indexUserName); //列名读取
                        string iDName = dr.GetName(0); //获取指定学好的列名
                        //Console.WriteLine(userId + " " + userName + " " + iDName) ;
                        users.Add(user);
                    }
                }
                dr.Close();
            }
        }

        private static void TestReturnParameter()
        {
            string connstr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                SqlCommand cmd = new SqlCommand("GetUserID", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                //输出参数
                SqlParameter paraId = new SqlParameter("@UserName", SqlDbType.NVarChar, 50);
                SqlParameter[] paras = {
                    new SqlParameter("@UserName", "admin"),
                    new SqlParameter("@reValue",SqlDbType.Int,4) };
                paras[1].Direction = ParameterDirection.ReturnValue;
                //paraId.Value = "admin";
                //paraId.Direction = ParameterDirection.Input;
                //cmd.Parameters.Add(paraId);//添加灿哥参数
                cmd.Parameters.AddRange(paras);
                conn.Open();
                cmd.ExecuteScalar();
                Console.WriteLine(paras[1].Value.ToString());
            }
        }

        private static void TestInputOutParameter()
        {
            //输入输出参数,（双向参数）
            string connstr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                SqlCommand cmd = new SqlCommand("GetDeptName", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                //输出参数
                SqlParameter paraName = new SqlParameter("@DeptName", SqlDbType.NVarChar, 50);
                paraName.Value = "财";
                paraName.Direction = ParameterDirection.InputOutput;
                cmd.Parameters.Add(paraName);//添加灿哥参数

                conn.Open();
                object o = cmd.ExecuteScalar();
                conn.Close();
                Console.WriteLine(paraName.Value.ToString());
            }
        }

        private static void TestOutputParameter()
        {
            //输出参数的使用
            string connstr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connstr))
            {
                SqlCommand cmd = new SqlCommand("GetDept", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paraId = new SqlParameter("@DeptId", 2);
                cmd.Parameters.Add(paraId);//添加单个参数
                //输出参数
                SqlParameter paraName = new SqlParameter("@DeptName", SqlDbType.NVarChar, 50);
                paraName.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(paraName);//添加灿哥参数

                conn.Open();
                object o = cmd.ExecuteScalar();
                conn.Close();
                Console.WriteLine(paraName.Value.ToString());
            }
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
