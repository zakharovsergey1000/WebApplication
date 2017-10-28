using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using WebApplication.Models;
using WebApplication.Models.CodeFirst;
//
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication.Utils
{
    public class PerformanceMonitorModule : IHttpModule
    {
        private ApplicationDbContext db;

        public void Init(HttpApplication httpApp)
        {
            httpApp.BeginRequest += OnBeginRequest;
            httpApp.EndRequest += OnEndRequest;
            httpApp.PreSendRequestHeaders += OnHeaderSent;
            db = new ApplicationDbContext();
        }

        public void OnHeaderSent(object sender, EventArgs e)
        {
            var httpApp = (HttpApplication)sender;
            httpApp.Context.Items["HeadersSent"] = true;
        }

        // Record the time of the begin request event.
        public void OnBeginRequest(Object sender, EventArgs e)
        {
            var httpApp = (HttpApplication)sender;
            if (httpApp.Request.Path.StartsWith("/media/")) return;
            var timer = new Stopwatch();
            httpApp.Context.Items["Timer"] = timer;
            httpApp.Context.Items["HeadersSent"] = false;
            timer.Start();
            //using (var dbTransaction = db.Database.BeginTransaction())
            //{
            //if (httpApp.Context.Session != null)
            //{
            //    if (httpApp.Context.Session.IsNewSession)
            //    {
            try
            {
                //
                if (false)
                {
                    List<Visitor> visitors =
                                    db.Visitors.Where(b => (
                                    (
                                    b.Url != null && (
                                    b.Url.StartsWith("http://206.221.223.103") ||
                                    b.Url.StartsWith("https://localhost:44304/") ||
                                    b.Url.StartsWith("http://advancedcalendar.biz/cgi-bin/") ||
                                    b.Url.StartsWith("http://advancedcalendar.biz/robots.txt") ||
                                    b.Url.StartsWith("https://advancedcalendar.biz/robots.txt") ||
                                    b.Url.StartsWith("http://www.advancedcalendar.biz/robots.txt") ||
                                    b.Url.StartsWith("https://www.advancedcalendar.biz/robots.txt") ||
                                    b.Url.StartsWith("https://advancedcalendar.biz/Content/css?v=") ||
                                    b.Url.StartsWith("https://www.advancedcalendar.biz/Content/css?v=") ||
                                    b.Url.StartsWith("https://advancedcalendar.biz/bundles/modernizr?v=") ||
                                    b.Url.StartsWith("https://www.advancedcalendar.biz/bundles/modernizr?v=") ||
                                    b.Url.StartsWith("https://advancedcalendar.biz/bundles/jquery?v=") ||
                                    b.Url.StartsWith("https://www.advancedcalendar.biz/bundles/jquery?v=") ||
                                    b.Url.StartsWith("https://advancedcalendar.biz/bundles/bootstrap?v=") ||
                                    b.Url.StartsWith("https://www.advancedcalendar.biz/bundles/bootstrap?v=") ||
                                    b.Url.StartsWith("http://advancedcalendar.biz/bundles/jqueryval?v=") ||
                                    b.Url.StartsWith("http://www.advancedcalendar.biz/bundles/jqueryval?v=")
                                    )
                                    ) || 
                                    (
                                    b.UrlReferrer != null && (
                                    b.UrlReferrer.Equals("http://best-seo-offer.com/try.php?u=http://advancedcalendar.biz") ||
                                    b.UrlReferrer.Equals("http://buttons-for-your-website.com/") ||
                                    b.UrlReferrer.Equals("http://www.netcraft.com/survey/")
                                    )
                                    ) ||
                                    (
                                    b.IpString != null && (
                                    b.IpString.StartsWith("66.249.64.") ||
                                    b.IpString.StartsWith("66.249.65.1") ||
                                    b.IpString.StartsWith("66.249.67") ||
                                    b.IpString.StartsWith("66.249.73.2") ||
                                    b.IpString.StartsWith("66.249.79.") ||
                                    b.IpString.StartsWith("68.180.229.31") ||
                                    b.IpString.StartsWith("69.58.178.57") ||
                                    b.IpString.StartsWith("101.226.16") ||
                                    b.IpString.StartsWith("120.141.114.151") ||
                                    b.IpString.StartsWith("123.125.71.") ||
                                    b.IpString.StartsWith("139.0.32.30") ||
                                    b.IpString.StartsWith("157.55.39.71") ||
                                    b.IpString.StartsWith("177.33.172.233") ||
                                    b.IpString.StartsWith("179.178.255.177") ||
                                    b.IpString.StartsWith("179.172.53.1") ||
                                    b.IpString.StartsWith("182.118.") ||
                                    b.IpString.StartsWith("186.214.187.106") ||
                                    b.IpString.StartsWith("186.236.67.74") ||
                                    b.IpString.StartsWith("187.56.225.185") ||
                                    b.IpString.StartsWith("188.138.17.205") ||
                                    b.IpString.StartsWith("189.47.69.67") ||
                                    b.IpString.StartsWith("192.99.107.") ||
                                    b.IpString.StartsWith("207.46.13.") ||
                                    b.IpString.StartsWith("208.115.111.69") ||
                                    b.IpString.StartsWith("220.181.108.") 
                                    )
                                    )
                                    )).ToList();
                    foreach (Visitor visitor in visitors)
                    {
                        VisitorBot newEntity = new VisitorBot();
                        newEntity.DateTime = visitor.DateTime;
                        newEntity.IP = visitor.IP;
                        newEntity.Url = visitor.Url;
                        newEntity.UrlReferrer = visitor.UrlReferrer;
                        newEntity.IpString = visitor.IpString;

                        db.VisitorBots.Add(newEntity);
                        db.Visitors.Remove(visitor);
                        db.SaveChanges();
                    }
                }
                //
                try
                {
                    string ipString = httpApp.Context.Request.UserHostAddress;

                    long IP = BitConverter.ToInt32(IPAddress.Parse(ipString).GetAddressBytes(), 0);
                    string url = "";
                    try
                    {
                        url = httpApp.Context.Request.Url.ToString();
                    }
                    catch (Exception e6) { }

                    string urlReferrer = "";
                    try
                    {
                        urlReferrer = httpApp.Context.Request.UrlReferrer.ToString();
                    }
                    catch (Exception e7) { }

                    if (

                                        // ipString != null && ipString.Equals("http://206.221.223.103/")

                                        (
                                        url != null && (
                                        url.StartsWith("http://206.221.223.103") ||
                                        url.StartsWith("https://localhost:44304/") ||
                                        url.StartsWith("http://advancedcalendar.biz/cgi-bin/") ||
                                        url.StartsWith("http://advancedcalendar.biz/robots.txt") ||
                                        url.StartsWith("https://advancedcalendar.biz/robots.txt") ||
                                        url.StartsWith("http://www.advancedcalendar.biz/robots.txt") ||
                                        url.StartsWith("https://www.advancedcalendar.biz/robots.txt") ||
                                        url.StartsWith("https://advancedcalendar.biz/Content/css?v=") ||
                                        url.StartsWith("https://www.advancedcalendar.biz/Content/css?v=") ||
                                        url.StartsWith("https://advancedcalendar.biz/bundles/modernizr?v=") ||
                                        url.StartsWith("https://www.advancedcalendar.biz/bundles/modernizr?v=") ||
                                        url.StartsWith("https://advancedcalendar.biz/bundles/jquery?v=") ||
                                        url.StartsWith("https://www.advancedcalendar.biz/bundles/jquery?v=") ||
                                        url.StartsWith("https://advancedcalendar.biz/bundles/bootstrap?v=") ||
                                        url.StartsWith("https://www.advancedcalendar.biz/bundles/bootstrap?v=") ||
                                        url.StartsWith("http://advancedcalendar.biz/bundles/jqueryval?v=") ||
                                        url.StartsWith("http://www.advancedcalendar.biz/bundles/jqueryval?v=")
                                        )
                                        ) ||
                                        (
                                        urlReferrer != null && (
                                        urlReferrer.Equals("http://best-seo-offer.com/try.php?u=http://advancedcalendar.biz") ||
                                        urlReferrer.Equals("http://buttons-for-your-website.com/") ||
                                        urlReferrer.Equals("http://www.netcraft.com/survey/")
                                        )
                                        ) ||
                                        (
                                        ipString != null && (
                                                      ipString.StartsWith("66.249.64.") ||
                                            ipString.StartsWith("66.249.65.1") ||
                                            ipString.StartsWith("66.249.67") ||
                                            ipString.StartsWith("66.249.73.2") ||
                                            ipString.StartsWith("66.249.79.") ||
                                            ipString.StartsWith("68.180.229.31") ||
                                            ipString.StartsWith("69.58.178.57") ||
                                            ipString.StartsWith("101.226.16") ||
                                            ipString.StartsWith("120.141.114.151") ||
                                            ipString.StartsWith("123.125.71.") ||
                                            ipString.StartsWith("139.0.32.30") ||
                                            ipString.StartsWith("157.55.39.71") ||
                                            ipString.StartsWith("177.33.172.233") ||
                                            ipString.StartsWith("179.178.255.177") ||
                                            ipString.StartsWith("179.172.53.1") ||
                                            ipString.StartsWith("182.118.") ||
                                            ipString.StartsWith("186.214.187.106") ||
                                            ipString.StartsWith("186.236.67.74") ||
                                            ipString.StartsWith("187.56.225.185") ||
                                            ipString.StartsWith("188.138.17.205") ||
                                            ipString.StartsWith("189.47.69.67") ||
                                            ipString.StartsWith("192.99.107.") ||
                                            ipString.StartsWith("207.46.13.") ||
                                            ipString.StartsWith("208.115.111.69") ||
                                            ipString.StartsWith("220.181.108.")
                                        )
                                        )

                        )
                    {
                        VisitorBot newEntity = new VisitorBot();
                        newEntity.IP = IP;
                        newEntity.IpString = ipString;
                        newEntity.DateTime = DateTimeOffset.Now;
                        newEntity.Url = url;
                        newEntity.UrlReferrer = urlReferrer;

                        db.VisitorBots.Add(newEntity);
                        db.SaveChanges();

                    }
                    else
                    {
                        DateTimeOffset now = DateTimeOffset.Now;
                        Visitor existingEntity = db.Visitors.Where(b => (b.IP == IP && b.Url.Equals(url))).OrderByDescending(visitor => visitor.DateTime).FirstOrDefault();
                        bool isNeedToWrite = false;
                        if (existingEntity != null)
                        {
                            TimeSpan ts = new TimeSpan(now.UtcTicks - existingEntity.DateTime.UtcTicks);
                            if (ts.TotalMinutes >= 1)
                            {
                                isNeedToWrite = true;
                            }
                        }
                        else { isNeedToWrite = true; }

                        if (isNeedToWrite)
                        {
                            Visitor newEntity = new Visitor();
                            newEntity.IP = IP;
                            newEntity.IpString = ipString;
                            newEntity.DateTime = DateTimeOffset.Now;
                            try
                            {
                                newEntity.Url = httpApp.Context.Request.Url.ToString();
                            }
                            catch (Exception e2) { }

                            try
                            {
                                newEntity.UrlReferrer = httpApp.Context.Request.UrlReferrer.ToString();
                            }
                            catch (Exception e3) { }
                            db.Visitors.Add(newEntity);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception e4) { }

            }
            catch (Exception e1) { }
            //    }
            //}
            //}
        }

        public void OnEndRequest(Object sender, EventArgs e)
        {
            var httpApp = (HttpApplication)sender;
            if (httpApp.Request.Path.StartsWith("/media/")) return;
            var timer = (Stopwatch)httpApp.Context.Items["Timer"];

            if (timer != null)
            {
                timer.Stop();
                if (!(bool)httpApp.Context.Items["HeadersSent"])
                {
                    httpApp.Context.Response.AppendHeader("ProcessTime",
                                                          ((double)timer.ElapsedTicks / Stopwatch.Frequency) * 1000 +
                                                          " ms.");
                }
            }

            httpApp.Context.Items.Remove("Timer");
            httpApp.Context.Items.Remove("HeadersSent");

        }

        public void Dispose() { /* Not needed */ }
    }
}