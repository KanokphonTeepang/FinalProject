using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace FinalProject.Pages
{
    public class ComposeModel : PageModel
    {
        public EmailInfo emailInfo = new EmailInfo();
        public String errorMessage = "";
        public String successMessage = "";
        public void OnGet()
        {
        }

        public void OnPost()
        {
            emailInfo.EmailReceiver = Request.Form["emailreceiver"];
            emailInfo.EmailSubject = Request.Form["emailsubject"];
            emailInfo.EmailMessage = Request.Form["emailmessage"];

            if (emailInfo.EmailReceiver.Length == 0 || emailInfo.EmailSubject.Length == 0 ||
               emailInfo.EmailMessage.Length == 0)
            {
                errorMessage = "All the fields are required";
                return;
            }

            try
            {
                String connectionString = "Server=tcp:kanokphonteep.database.windows.net,1433;Initial Catalog=finaldb;Persist Security Info=False;User ID=Kanokphon;Password=cs436_227G;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // ดึงชื่อผู้ใช้ที่ login อยู่
                    string username = User.Identity.Name ?? "";

                    String sql = "INSERT INTO emails" +
                                    "(emailreceiver, emailsubject, emailmessage, emailisread, emailsender, emaildate) VALUES " +
                                    "(@emailreceiver, @emailsubject, @emailmessage, @emailisread, @emailsender, GETDATE());";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@emailreceiver", emailInfo.EmailReceiver);
                        command.Parameters.AddWithValue("@emailsubject", emailInfo.EmailSubject);
                        command.Parameters.AddWithValue("@emailmessage", emailInfo.EmailMessage);
                        command.Parameters.AddWithValue("@emailisread", Convert.ToInt32(emailInfo.EmailIsRead));
                        command.Parameters.AddWithValue("@emailsender", username);  // ให้ emailsender เป็น username ของคนที่ login
                        command.Parameters.AddWithValue("@emaildate", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            emailInfo.EmailReceiver = "";
            emailInfo.EmailSubject = "";
            emailInfo.EmailMessage = "";
            successMessage = "Compose new email successfully";

            Response.Redirect("Index");
        }
    }
}