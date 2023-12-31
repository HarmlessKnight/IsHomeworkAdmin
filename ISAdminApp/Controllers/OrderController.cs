﻿using ClosedXML.Excel;
using GemBox.Document;
using ISAdminApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ISAdminApp.Controllers
{
    public class OrderController : Controller
    {

        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            string Url = "https://localhost:44378/api/admin/GetAllActiveOrders";

            HttpResponseMessage response = client.GetAsync(Url).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;

            return View(data);
        }

        public IActionResult Details(int orderId)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44378/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = orderId,
            };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;


            var Data = response.Content.ReadAsAsync<Order>().Result;

            return View(Data);

        }

        public async Task<FileResult> SavePdfAsync(int orderId)
        {
            HttpClient client = new HttpClient();
            string url = "https://localhost:44378/api/Admin/GetDetailsForOrder";

            var model = new
            {
                Id = orderId,
            };
            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Headers: {response.Headers}");
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");



            var result = response.Content.ReadAsAsync<Order>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");

            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", orderId.ToString());

            document.Content.Replace("{{UserName}}", result.OrderedBy.Name.ToString());

            StringBuilder stringBuilder = new StringBuilder();
            double totalPrice = 0.0;

            foreach (var item in result.Tickets)
            {
                totalPrice += item.Quantity * item.Ticket.Price;
                stringBuilder.AppendLine(item.Ticket.MovieTitle + ",Quantity:" + item.Quantity + " price: $" + item.Ticket.Price * item.Quantity);
            }

            document.Content.Replace("{{TicketList}}", stringBuilder.ToString());

            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString());

            var stream = new MemoryStream();
            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");

        }

        [HttpGet]
        public IActionResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Email";

                HttpClient client = new HttpClient();

                string Url = "https://localhost:44378/api/admin/GetAllActiveOrders";

                HttpResponseMessage response = client.GetAsync(Url).Result;

                var data = response.Content.ReadAsAsync<List<Order>>().Result;

                for (int i = 1; i <= data.Count; i++)
                {
                    var item = data[i];
                    worksheet.Cell(i+1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.OrderedBy.Email;

                    for (int p = 0; p< item.Tickets.Count; p++)
                    {
                        worksheet.Cell(1, p + 3).Value = "Ticket - " + (p + 1);
                        worksheet.Cell(i + 1, p + 3).Value = item.Tickets.ElementAt(p).Ticket.MovieTitle;
                    }
                }
                using (var stream = new MemoryStream()) 
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
                
            }
            
        }

    }
}
