using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace kur
{
    class anaClass
    {
        private readonly System.Timers.Timer timer;
        string str = @"Data Source=DESKTOP-NU88O7L\SQLEXPRESS01;Initial Catalog=Kur;Integrated Security=True";
        string file = @"C:\Users\lenovo\Desktop\Kur\kur\test.txt";
        public DateTime sonGuncellemeKur;
        private List<kur> kurList;

        public void DosyayaYaz(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
        public anaClass()
        {
            timer = new System.Timers.Timer(25 * 10 * 1000) { AutoReset = true };
            timer.Elapsed += timerKur;
        }
        private void timerKur(object sender, ElapsedEventArgs e)
        {
            string[] lines = new string[] { DateTime.Now.ToString() + " => Kur Çekme İşlemi Çalışıyor." };
            File.AppendAllLines(file, lines);

            try
            {
                if (DateTime.Now.Hour == 10 || DateTime.Now.Hour == 18)
                {
                    if (sonGuncellemeKur == null || DateTime.Now.Day != sonGuncellemeKur.Day)
                    {
                        try
                        {
                            kurList = new List<kur>();
                            string today = "http://www.tcmb.gov.tr/kurlar/today.xml";
                            var xmlDoc = new XmlDocument();
                            xmlDoc.Load(today);
                            Task.Run(() =>
                            {
                                SqlConnection con = new SqlConnection(str);
                                try
                                {
                                    string sql = "";
                                    XmlNodeList secilen = xmlDoc.DocumentElement.SelectNodes("Currency");
                                    foreach (XmlNode ekle in secilen)
                                    {
                                        string Kod = ekle.Attributes["Kod"].Value;
                                        string Unit = ekle.SelectSingleNode("Unit").InnerText;
                                        string Currency_Name = ekle.SelectSingleNode("CurrencyName").InnerText;
                                        string ForexBuying = ekle.SelectSingleNode("ForexBuying").InnerText;
                                        string ForexSelling = ekle.SelectSingleNode("ForexSelling").InnerText;
                                        string BanknoteBuying = ekle.SelectSingleNode("BanknoteBuying").InnerText;
                                        string BanknoteSelling = ekle.SelectSingleNode("BanknoteSelling").InnerText;
                                        string Date = DateTime.Now.ToString();
                                        kurList.Add(new kur
                                        {
                                            Kod = Kod,
                                            Unit = Unit,
                                            Currency_Name = Currency_Name,
                                            ForexBuying = ForexBuying,
                                            ForexSelling = ForexSelling,
                                            BanknoteBuying = BanknoteBuying,
                                            BanknoteSelling = BanknoteSelling,
                                            Date = Date
                                        });
                                    }
                                    foreach (var item in kurList)
                                    {
                                        try
                                        {
                                            var Kod = item.Kod;
                                            var Unit = item.Unit;
                                            var Currency_Name = item.Currency_Name;
                                            var ForexBuying = item.ForexBuying;
                                            var ForexSelling = item.ForexSelling;
                                            var BanknoteBuying = item.BanknoteBuying;
                                            var BanknoteSelling = item.BanknoteSelling;
                                            var Date = item.Date;
                                            // insert işlemi yapılabilir.
                                            //sql = "INSERT INTO tblDovizKur1 VALUES ('" +
                                            //                    Kod + "','" +
                                            //                    Unit.Replace("'", "''") + "','" +
                                            //                    Currency_Name.Replace("'", "''") + "','" +
                                            //                    ForexBuying + "','" +
                                            //                    ForexSelling + "','" +
                                            //                    BanknoteBuying + "','" +
                                            //                    BanknoteSelling + "','" +
                                            //                    Date + "');";
                                            //SqlCommand cmd = new SqlCommand(sql, conn);
                                            //cmd.ExecuteNonQuery();
                                            //Console.WriteLine("veritabanına kayıt edildi.");
                                            sql = "update tblDovizKur1 set Kod=@Kod, Unit=@Unit, Currency_Name=@Currency_Name, ForexBuying=@ForexBuying, ForexSelling=@ForexSelling, BanknoteBuying=@BanknoteBuying, BanknoteSelling=@BanknoteSelling, Date=@Date where Kod=@Kod ";

                                            SqlCommand cmd = new SqlCommand(sql, con);
                                            cmd.Parameters.AddWithValue("@Kod", Kod);
                                            cmd.Parameters.AddWithValue("@Unit", Unit);
                                            cmd.Parameters.AddWithValue("@Currency_Name", Currency_Name);
                                            cmd.Parameters.AddWithValue("@ForexBuying", ForexBuying);
                                            cmd.Parameters.AddWithValue("@ForexSelling", ForexSelling);
                                            cmd.Parameters.AddWithValue("@BanknoteBuying", BanknoteBuying);
                                            cmd.Parameters.AddWithValue("@BanknoteSelling", BanknoteSelling);
                                            cmd.Parameters.AddWithValue("@Date", Date);
                                            cmd.ExecuteNonQuery();
                                            Console.WriteLine("veritabanı güncellendi");
                                        }
                                        catch (Exception ex)
                                        {
                                            DosyayaYaz(ex.Message);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    DosyayaYaz(ex.Message + DateTime.Now);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            DosyayaYaz(ex.Message + DateTime.Now);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DosyayaYaz(ex.Message + DateTime.Now);
            }
        }
        public void start()
        {
            timer.Start();
        }
        public void stop()
        {
            timer.Stop();
        }
    }
}
