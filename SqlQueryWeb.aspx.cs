using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace Index
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SqlQuery
    {
        public static string conn = "Data Source=LAPTOP-AT4UKP9G;Initial Catalog=User;Integrated Security=True";

        public static bool IsUsername(string username)
        {
            List<User> userList = SqlQuery.Select($"SELECT * FROM [User].[dbo].[User] WHERE [username] = '{username}'");

            return userList.Count > 0;
        }

        public static bool IsId(int id)
        {
            List<User> userList = SqlQuery.Select($"SELECT * FROM [User].[dbo].[User] WHERE [id] = {id};");

            return userList.Count > 0;
        }

        public static bool CheckPassword(string username, string password)
        {
            List<User> userList = SqlQuery.Select($"SELECT * FROM [User].[dbo].[User] WHERE [username] = '{username}'");

            if (userList.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (User user in userList)
                {
                    if (user.Password == password)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static List<User> Select(string cmdtxt)
        {
            using (SqlConnection myConn = new SqlConnection(conn))
            {
                myConn.Open();
                List<User> listUser = new List<User>();
                using (SqlCommand myCmd = new SqlCommand(cmdtxt, myConn))
                {
                    using (SqlDataReader myDataReader = myCmd.ExecuteReader())
                    {
                        while (myDataReader.Read())
                        {
                            User user = new User()
                            {
                                Id = myDataReader.GetInt32(0),
                                Username = myDataReader.GetString(1),
                                Password = myDataReader.GetString(2)
                            };

                            listUser.Add(user);
                        }
                    }
                }
                return listUser;
            }
        }

        public static bool Add(string username, string password)
        {
            //sử lý đầu vào //người dùng đã tồn tại
            if (SqlQuery.IsUsername(username)) { return  false; }
            //nếu username và password là trống
            if (username.Equals("") || password.Equals("")) { return false; }

            using (SqlConnection myConn = new SqlConnection(conn))
            {
                myConn.Open();
                List<User> listUser = new List<User>();
                using (SqlCommand myCmd = new SqlCommand($"INSERT INTO [User].[dbo].[User] ([username], [password]) VALUES ('{username}','{password}');", myConn))
                {
                    using (SqlDataReader dataReader = myCmd.ExecuteReader())
                    {
                        return !dataReader.Read();
                    }
                    
                }
            }
        }

        public static bool Delete(int id)
        {
            //sử lý đầu vào //người dùng không đã tồn tại
            if (!SqlQuery.IsId(id)) { return false; }

            using (SqlConnection myConn = new SqlConnection(conn))
            {
                myConn.Open();
                List<User> listUser = new List<User>();
                using (SqlCommand myCmd = new SqlCommand($"DELETE FROM [User].[dbo].[User] WHERE [id] = {id};", myConn))
                {
                    using (SqlDataReader dataReader = myCmd.ExecuteReader())
                    {
                        return !dataReader.Read();
                    }

                }
            }
        }

        public static bool Update(int id, string username, string password)
        {
            //sử lý đầu vào 
            //người dùng không đã tồn tại
            if (!SqlQuery.IsId(id)) { return false; }

            //nếu username và password là trống
            if (username.Equals("") || password.Equals("")) { return false; }

            //nếu không thay đổi gì
            if (SqlQuery.CheckPassword(username, password)) { return false; }

            //người dùng đã tồn tại
            if (SqlQuery.IsUsername(username)) { return false; }

            using (SqlConnection myConn = new SqlConnection(conn))
            {
                myConn.Open();
                List<User> listUser = new List<User>();
                using (SqlCommand myCmd = new SqlCommand($"UPDATE [User].[dbo].[User] SET [username] = '{username}', [password] = '{password}' WHERE [id] = {id};", myConn))
                {
                    using (SqlDataReader dataReader = myCmd.ExecuteReader())
                    {
                        return !dataReader.Read();
                    }

                }
            }
        }
    }

    public partial class SqlQueryWeb : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        protected void btSelectAll_Click(object sender, EventArgs e)
        {
            List<User> userList = SqlQuery.Select("SELECT * FROM [User].[dbo].[User]");

            GridViewUsers.DataSource = userList;
            GridViewUsers.DataBind();
        }

        protected void btSelectID_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(txtID.Text);
                List<User> userList = SqlQuery.Select($"SELECT * FROM [User].[dbo].[User] WHERE [id] = {id};");

                GridViewUsers.DataSource = userList;
                GridViewUsers.DataBind();
            }
            catch
            {

            }
        }

        protected void btSelectUsername_Click(object sender, EventArgs e)
        {
            try
            {
               
                string username = txtUsername.Text;
                List<User> userList = SqlQuery.Select($"SELECT * FROM [User].[dbo].[User] WHERE [username] = '{username}';");

                GridViewUsers.DataSource = userList;
                GridViewUsers.DataBind();
            }
            catch
            {

            }
        }

        protected void btAdd_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            if (SqlQuery.Add(username, password) )
            {
                MessageNotify("Bạn đã thêm user thành công!");
            }
            else
            {
                MessageNotify("Bạn đã thêm user không thành công!");
            }
        }

        protected void btDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(txtID.Text);
                if (SqlQuery.Delete(id))
                {
                    MessageNotify("Bạn đã xóa user thành công!");
                }
                else
                {
                    MessageNotify("Bạn đã xóa user không thành công!");
                }
            }
            catch { }
        }

        protected void btUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(txtID.Text);
                string username = txtUsername.Text;
                string password = txtPassword.Text;
                if (SqlQuery.Update(id, username, password))
                {
                    MessageNotify("Bạn đã cập nhật user thành công!");
                }
                else
                {
                    MessageNotify("Bạn đã cập nhật user không thành công!");
                }
            }
            catch { }
        }

        private void MessageNotify(string message)
        {
            string script = $"<script type='text/javascript'>alert('{message}');</script>";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script);
        }
    }
}