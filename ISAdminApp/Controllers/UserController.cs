using ExcelDataReader;
using ISAdminApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace ISAdminApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult ImportUsers(IFormFile file)
        {
            string pathToUpload = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";
            using (FileStream filestream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(filestream);
                filestream.Flush();
            }
            List<User> users = getAllUsersFromFile(file.FileName);

            HttpClient client = new HttpClient();

            string Url = "https://localhost:44378/api/admin/ImportAllUsers";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(users),Encoding.UTF8,"application/json");

            HttpResponseMessage response = client.PostAsync(Url,content).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;

            return RedirectToAction("Index","Order");

        }



        private List<User> getAllUsersFromFile(string fileName)
        {
            
            string path = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";
            List<User> users = new List<User>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while(reader.Read())
                    {
                        users.Add(new Models.User
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(2).ToString(),    
                        });
                    }
                }
            }
            return users;
        }
    }
}
