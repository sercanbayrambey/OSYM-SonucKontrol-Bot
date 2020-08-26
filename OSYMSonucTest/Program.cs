using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Media;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace OSYMSonucTest
{
    class Program
    {
        const int RequestTime = 30;
        static void Main(string[] args)
        {
            Thread sonucTH = new Thread(CheckWebsite);
            sonucTH.Start();

        }



        public static void CheckWebsite()
        {
            SoundPlayer sp = new SoundPlayer();
            sp.SoundLocation = "notif.wav";
            while (true)
            {
                try
                {
                    Random rnd = new Random();
                    int rndNumber = rnd.Next(0, 1000);
                    var url = $"https://sonuc.osym.gov.tr/?antiCache={rndNumber}";
                    var html = url;
                    Console.WriteLine("\n \n<!> github.com/sercanbayrambey");
                    Console.WriteLine($"<!> [{DateTime.Now.ToString("HH:mm:ss")}] Siteye istek atılıyor ({url})... ");
                    HtmlWeb web = new HtmlWeb();

                    var htmlDoc = web.Load(html);
                    var node = htmlDoc.DocumentNode.SelectSingleNode("//body");
                    var htmlSource = node.InnerHtml;

                    Console.WriteLine($"<!> [{DateTime.Now.ToString("HH:mm:ss")}] Siteye istek başarılı, cevap bekleniyor...");

                    if (htmlSource.Contains("action=\"SonucSec.aspx"))
                    {
                        Console.WriteLine($"<!> [{DateTime.Now.ToString("HH:mm:ss")}] Sonuç henüz açıklanmadı. :(");
                        Console.WriteLine($"<!> [{DateTime.Now.ToString("HH:mm:ss")}] {RequestTime} saniye içinde tekrar denenecek.");
                        Thread.Sleep(RequestTime * 1000);
                    }
                    else
                    {
                        sp.Play();
                        Console.WriteLine($"<!> [{DateTime.Now.ToString("HH:mm:ss")}] Sonuç açıklandı !!!!");
                        Process myProcess = new Process();
                        myProcess.StartInfo.UseShellExecute = true;
                        myProcess.StartInfo.FileName = url;
                        myProcess.Start();
                        SendMail("sercanbayrambeyy@gmail.com");
                        Console.ReadKey();
                        break;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"<!> Bir hata oluştu. {e}");
                    continue;
                }
            }
           
        }

        public static void SendMail(string email)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("comporeport@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Sonuç Açıklandı!!";
                mail.Body = "<h1>ÖSYM Sonucu açıklanmıştır. Bizi tercih ettiğiniz için teşekkür ederiz.</h1>";
                mail.IsBodyHtml = true;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("comporeport@gmail.com", "şifre"),
                    Timeout = 20000
                };

                smtp.Send(mail);
            }
        }
    }
}
