using ClosedXML.Excel;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using MovieTickets.Domain.DomainModels;
using MovieTickets.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieTickets.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            this._orderService = orderService;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult Index()
        {
            var orderList = this._orderService.getAllOrders();
            return View(orderList);
        }
        public IActionResult Details(BaseEntity model)
        {
            Order selectedOrder = this._orderService.getOrderDetails(model);
            return View(selectedOrder);
        }
        public FileContentResult CreateInvoice(BaseEntity model)
        {
            var result = this._orderService.getOrderDetails(model);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{UserName}}", result.User.UserName);

            StringBuilder sb = new StringBuilder();

            var totalPrice = 0.0;

            foreach (var item in result.Tickets)
            {
                totalPrice += item.Quantity * item.SelectedTicket.Price;
                sb.AppendLine(item.SelectedTicket.MovieName + " with quantity of: " + item.Quantity + " and price of: " + item.SelectedTicket.Price + "$");
            }


            document.Content.Replace("{{TicketList}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", totalPrice.ToString() + "$");


            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }

        public FileContentResult ExportAllOrders()
        {
            string fileName = "Orders.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("All Orders");

                worksheet.Cell(1, 1).Value = "Order Id";
                worksheet.Cell(1, 2).Value = "Costumer Email";


                var result = this._orderService.getAllOrders();

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.User.Email;

                    for (int p = 0; p < item.Tickets.Count(); p++)
                    {
                        worksheet.Cell(1, p + 3).Value = "Ticket-" + (p + 1);
                        worksheet.Cell(i + 1, p + 3).Value = item.Tickets.ElementAt(p).SelectedTicket.MovieName;
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
