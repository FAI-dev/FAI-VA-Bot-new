﻿using CoreBot.models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace SNOW.snowLogger
{

    public class SNOWLogger
    {
        public SNOWLogger(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        protected readonly IConfiguration Configuration;

        //public string str { get; private set; }

        public string CreateIncidentServiceNow(string shortDescription, string description, string incpriority)
        {
            string incNumber = "";
            try
            {


                string username = "VABot";
                string password = "vaboT12345@";
                string url = "https://dev58462.service-now.com/api/now/table/";
                //string url = "https://dev77188.service-now.com/api/now/table/";
                //string username = Configuration["ServiceNowUserName"];
                //string password = Configuration["ServiceNowPassword"];
                //string url = Configuration["ServiceNowUrl"];

                //foreach (string url in File.ReadLines("URLconfig.txt"))
                //{
                //string str = url.ToString();


                //}
                string routingAlgoURL = "https://faitrs.azurewebsites.net/";
                HttpWebRequest request1 = WebRequest.Create(routingAlgoURL + "category") as HttpWebRequest;              
                request1.ContentType = "application/json";
                request1.Method = "Post";

                using (var streamWriter = new StreamWriter(request1.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        ticketDescription = shortDescription + " " + description
                    });

                    streamWriter.Write(json);
                }


                using (HttpWebResponse response = request1.GetResponse() as HttpWebResponse)
                {
                    var res = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    JObject joResponse = JObject.Parse(res.ToString());
                    JValue ojObject = (JValue)joResponse["resolver group"];
                    string resGrp = ojObject.Value.ToString();

                    if (null == resGrp)
                        resGrp = "Service Desk";

                    var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));
                   
                    HttpWebRequest request = WebRequest.Create(url + "incident") as HttpWebRequest;
                    request.Headers.Add("Authorization", auth);
                    request.Method = "Post";

                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(new
                        {
                            description = description,
                            short_description = shortDescription, //Configuration["ServiceNowTicketShortDescription"],
                                                             // contact_type = Configuration["ServiceNowContactType"],
                                                             // category = Configuration["ServiceNowCategory"],
                                                             //subcategory = Configuration["ServiceNowSubCategory"],
                            assignment_group = resGrp, //Default assignee grp & validation 
                            impact = incpriority, //["ServiceNowIncidentImpact"],
                            priority = incpriority,//Configuration["ServiceNowIncidentPriority"],
                            caller_id = "anirudharpatil@gmail.com"//Configuration["ServiceNowCallerId"],
                                                                  //cmdb_ci = Configuration["ServiceNowCatalogueName"],
                                                                  //comments = Configuration["ServiceNowTicketShortDescription"]


                        });

                        streamWriter.Write(json);
                    }
                    using (HttpWebResponse response1 = request.GetResponse() as HttpWebResponse)
                    {
                        var res1 = new StreamReader(response1.GetResponseStream()).ReadToEnd();

                        JObject joResponse1 = JObject.Parse(res1.ToString());
                        JObject ojObject1 = (JObject)joResponse1["result"];
                        incNumber = ((JValue)ojObject1.SelectToken("number")).Value.ToString();

                        return incNumber + "," + resGrp;
                    }

                }
          
            }
            catch
            {
                //return ex.Message; 
                // Error message to be handled by AdapterWithErrorHandler. This does not work if we return string value.
                return incNumber;
            }
        }


        public apiresult KBSearchServiceNow(string shortDescription)
        {
            try
            {

                string username = Configuration["ServiceNowUserName"];
                string password = Configuration["ServiceNowPassword"];
                string url = Configuration["ServiceNowUrl"];
                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                //DataSet ds = null;
                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri("https://dev77188.service-now.com");
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(username, password)

                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "admin", "Passw0rd!"))));


                //HttpResponseMessage response1 = client.GetAsync(string.Format("api/now/table/kb_knowledge?sysparm_query=GOTOshort_description>=laptop&sysparm_fields=number,short_description,text&sysparm_limit=10")).Result;
                //if (response1.IsSuccessStatusCode)
                //{
                //    var json = response1.Content.ReadAsAsync<dynamic>().Result;
                //    ds = JsonConvert.DeserializeObject<DataSet>(json);


                //}




                //HttpWebRequest request = WebRequest.Create(url+"kb_knowledge?") as HttpWebRequest;
                HttpWebRequest request = WebRequest.Create("https://dev58462.service-now.com/api/now/table/kb_knowledge?sysparm_query=" + shortDescription + "&sysparm_fields=number,short_description,text&sysparm_limit=10") as HttpWebRequest;

                //https://dev77188.service-now.com/api/now/table/kb_knowledge?sysparm_query=GOTO123TEXTQUERY321%3Dlaptop&sysparm_limit=10
                request.Headers.Add("Authorization", auth);
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Accept", "application/json");

                request.Method = "Get";


                //using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                // {
                //   string json = JsonConvert.SerializeObject(new
                // {
                // description = shortDescription + Environment.NewLine + Environment.NewLine + description,
                //short_description = Configuration["ServiceNowTicketShortDescription"],
                //contact_type = Configuration["ServiceNowContactType"],
                //category = Configuration["ServiceNowCategory"],
                // subcategory = Configuration["ServiceNowSubCategory"],
                // assignment_group = Configuration["ServiceNowAssignmentGroup"],
                // impact = Configuration["ServiceNowIncidentImpact"],
                // priority = Configuration["ServiceNowIncidentPriority"],
                // caller_id = Configuration["ServiceNowCallerId"],
                // cmdb_ci = Configuration["ServiceNowCatalogueName"],
                //comments = Configuration["ServiceNowTicketShortDescription"]
                //   sysparm_query = shortDescription,
                //    sysparm_fields = "number,short_description,text",
                //    sysparm_limit = "10"
                // });

                //                    streamWriter.Write(json);
                //              }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var res = new StreamReader(response.GetResponseStream()).ReadToEnd();


                    apiresult rootObject = JsonConvert.DeserializeObject<apiresult>(res);


                    JObject joResponse = JObject.Parse(res.ToString());
                    //     joResponse.GetValue("text");

                    //    var fdate = JObject.Parse(res)["text"];

                    //        string incNumber = joResponse.SelectToken("result").ToString();
                    //      JObject ojObject = (JObject)joResponse["result"];
                    //   string incNumber = ((JValue)ojObject.SelectToken("number")).Value.ToString();

                    //     string incNumber= "text" ;
                    //     return incNumber;

                    //    JObject myResult = GetMyResult();
                    //  returnObject.Id = myResult["string here"]["id"];
                    //   var incNumber = joResponse["string here"]["id"];


                    //    var json = joResponse.Content.ReadAsAsync<dynamic>().Result;

                    //   var ds = JsonConvert.DeserializeObject<DataSet>(json);



                    //     List<apiresult> apiresult = new List<apiresult>();


                    //var incNumber = joResponse.SelectToken("result[0].short_description");
                    //var incNumber1 = joResponse.SelectToken("result");
                    //int i = 10;



                    //List<Result> result = new  Result();
                    //   r

                    //   foreach (var item in incNumber)
                    //   {
                    //       Result result = new Result();

                    //     result.text = item.v
                    //           resu
                    //   }
                    //     JsonSerializer serializer = new JsonSerializer();
                    //     JObject o = (JObject)serializer.Deserialize(joResponse);

                    //   MyAccount.EmployeeID = (string)o["employeeid"][0];

                    //  Console.WriteLine("t = " + obj["t"]);
                    // Console.WriteLine("l = " + obj["l"]);

                    //    JObject joResponse = JObject.Parse(res.ToString());

                    //      var obj = JsonConvert.DeserializeObject<JArray>(res).ToObject<List<JObject>>().ToArray();
                    //obj.GetValue(0);

                    //      JObject ojObject = (JObject)joResponse["result"];
                    //  string incNumber = ((JValue)ojObject.SelectToken("number")).Value.ToString();
                    //string incNumber = "hmm";

                    // dynamic jo = JObject.Parse(joResponse);

                    return rootObject;


                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal apiresult KBSearchByNumber(string kbnumber)
        {
            try
            {




               // string username = Configuration["ServiceNowUserName"];
                //string password = Configuration["ServiceNowPassword"];
                //string url = Configuration["ServiceNowUrl"];
                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("VABot" + ":" + "vaboT12345@"));


                HttpWebRequest request = WebRequest.Create("https://dev58462.service-now.com/api/now/table/kb_knowledge?number=" + kbnumber.ToUpper() + "&sysparm_fields=number,short_description,text&sysparm_limit=10") as HttpWebRequest;

                //https://dev77188.service-now.com/api/now/table/kb_knowledge?sysparm_query=GOTO123TEXTQUERY321%3Dlaptop&sysparm_limit=10
                request.Headers.Add("Authorization", auth);
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Accept", "application/json");

                request.Method = "Get";





                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var res = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    apiresult kbnumberobj = JsonConvert.DeserializeObject<apiresult>(res);
                    JObject joResponse = JObject.Parse(res.ToString());




                    return kbnumberobj;


                }



            }
            catch (Exception)
            {
                return null;
            }
        }






        internal Incident_api_result GetIncident(string INCnumber)
        {
            try
            {
                //string username = Configuration["ServiceNowUserName"];
                //string password = Configuration["ServiceNowPassword"];
                //string url = Configuration["ServiceNowUrl"];
                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("VABot" + ":" + "vaboT12345@"));


                HttpWebRequest request = WebRequest.Create("https://dev58462.service-now.com/api/now/table/incident?sysparm_query=GOTOnumber=" + INCnumber + "&sysparm_limit=1") as HttpWebRequest;

                //https://dev77188.service-now.com/api/now/table/kb_knowledge?sysparm_query=GOTO123TEXTQUERY321%3Dlaptop&sysparm_limit=1
                request.Headers.Add("Authorization", auth);
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Accept", "application/json");

                request.Method = "Get";
                //   CoreBot.models.Incident_api_result incident_Api_Result = null;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var resforinc = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    apiresult kbnumberobj = JsonConvert.DeserializeObject<apiresult>(resforinc);
                    JObject joResponse = JObject.Parse(resforinc.ToString());

                    //CoreBot.models.Incident_api_result incident_api = null;

                    if (kbnumberobj.result != null)
                    {
                        if (kbnumberobj.result.Count != 0)
                        {
                            if ((kbnumberobj.result[0].sys_id != null) && !(kbnumberobj.result[0].sys_id.Equals("")))
                            {


                                Incident_api_result incident_Api_Result = GetIncidentipdate(kbnumberobj.result[0].sys_id);


                                return incident_Api_Result;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;

                        }
                    }
                    else
                    {
                        return null;
                    }
                    // return kbnumberobj;


                }



                Incident_api_result GetIncidentipdate(string sys_id)
                {
                    //  string username = Configuration["ServiceNowUserName"];
                    //string password = Configuration["ServiceNowPassword"];
                    //string url = Configuration["ServiceNowUrl"];
                    try
                    {
                        var auth1 = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("VABot" + ":" + "vaboT12345@"));

                        HttpWebRequest requestforIncstatus = WebRequest.Create("https://dev58462.service-now.com/api/now/table/sys_journal_field?sysparm_query=element_id=" + sys_id + "&sysparm_limit=1") as HttpWebRequest;

                        requestforIncstatus.Headers.Add("Authorization", auth1);
                        requestforIncstatus.Headers.Add("Content-Type", "application/json");
                        requestforIncstatus.Headers.Add("Accept", "application/json");

                        request.Method = "Get";


                        using (HttpWebResponse response = requestforIncstatus.GetResponse() as HttpWebResponse)
                        {
                            var resfrominc = new StreamReader(response.GetResponseStream()).ReadToEnd();

                            Incident_api_result kbnumberobj = JsonConvert.DeserializeObject<Incident_api_result>(resfrominc);
                            JObject joResponse = JObject.Parse(resfrominc.ToString());



                            return kbnumberobj;


                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }


                }




                // return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

















        internal string CommentONIncident(string INCnumber,string comment)
        {
            try
            {
                //string username = Configuration["ServiceNowUserName"];
                //string password = Configuration["ServiceNowPassword"];
                //string url = Configuration["ServiceNowUrl"];
                var auth = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("VABot" + ":" + "vaboT12345@"));


                HttpWebRequest request = WebRequest.Create("https://dev58462.service-now.com/api/now/table/incident?sysparm_query=GOTOnumber=" + INCnumber + "&sysparm_limit=1") as HttpWebRequest;

                //https://dev77188.service-now.com/api/now/table/kb_knowledge?sysparm_query=GOTO123TEXTQUERY321%3Dlaptop&sysparm_limit=1
                request.Headers.Add("Authorization", auth);
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Accept", "application/json");

                request.Method = "Get";
                //   CoreBot.models.Incident_api_result incident_Api_Result = null;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var resforinc = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    apiresult kbnumberobj = JsonConvert.DeserializeObject<apiresult>(resforinc);
                    JObject joResponse = JObject.Parse(resforinc.ToString());

                    //CoreBot.models.Incident_api_result incident_api = null;

                    if (kbnumberobj.result != null)
                    {
                        if (kbnumberobj.result.Count != 0)
                        {
                            if ((kbnumberobj.result[0].sys_id != null) && !(kbnumberobj.result[0].sys_id.Equals("")))
                            {


                                string incident_Api_Result = CommentIncidentipdate(kbnumberobj.result[0].sys_id,comment);


                                return incident_Api_Result;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;

                        }
                    }
                    else
                    {
                        return null;
                    }
                    // return kbnumberobj;


                }



                string CommentIncidentipdate(string sys_id, string commentit)
                {
                    //  string username = Configuration["ServiceNowUserName"];
                    //string password = Configuration["ServiceNowPassword"];
                    //string url = Configuration["ServiceNowUrl"];
                    try
                    {

                        string username = "VABot"; //Configuration["ServiceNowUserName"];

                        string password = "vaboT12345@"; // Configuration["ServiceNowPassword"];
                        string url = "https://dev58462.service-now.com/api/now/table/incident/"; // Configuration["ServiceNowUrl"];

                        var auth1 = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(username + ":" + password));

                        HttpWebRequest request1 = WebRequest.Create(url + sys_id) as HttpWebRequest;
                        request1.Headers.Add("Authorization", auth1);
                        request1.Method = "patch";

                        using (var streamWriter = new StreamWriter(request1.GetRequestStream()))
                        {
                            string json = JsonConvert.SerializeObject(new
                            {
                                //description = shortDescription + Environment.NewLine + Environment.NewLine + description,
                                //short_description = description, //Configuration["ServiceNowTicketShortDescription"],
                                //                                 // contact_type = Configuration["ServiceNowContactType"],
                                //                                 // category = Configuration["ServiceNowCategory"],
                                //                                 //subcategory = Configuration["ServiceNowSubCategory"],
                                //                                 //assignment_group = Configuration["ServiceNowAssignmentGroup"],
                                //impact = incpriority, //["ServiceNowIncidentImpact"],
                                //priority = incpriority,//Configuration["ServiceNowIncidentPriority"],
                                //caller_id = "anirudharpatil@gmail.com"//Configuration["ServiceNowCallerId"],
                                //                                      //cmdb_ci = Configuration["ServiceNowCatalogueName"],
                                //                                      //comments = Configuration["ServiceNowTicketShortDescription"]
                                comments = commentit

                            });

                            streamWriter.Write(json);
                        }

                        using (HttpWebResponse response = request1.GetResponse() as HttpWebResponse)
                        {
                            var res = new StreamReader(response.GetResponseStream()).ReadToEnd();

                            JObject joResponse = JObject.Parse(res.ToString());
                            JObject ojObject = (JObject)joResponse["result"];
                            string incNumber = ((JValue)ojObject.SelectToken("number")).Value.ToString();

                            return incNumber;
                        }
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }


                }




                // return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
















        internal apiresult GetIncidentDetails(string INCnumber)
        {
            try
            {
             //  string username = Configuration["ServiceNowUserName"];
              //string password = Configuration["ServiceNowPassword"];
               //string url = Configuration["ServiceNowUrl"];
                var auth2 = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("VABot" + ":" + "vaboT12345@"));


                HttpWebRequest request = WebRequest.Create("https://dev58462.service-now.com/api/now/table/incident?sysparm_query=GOTOnumber=" + INCnumber + "&sysparm_limit=1") as HttpWebRequest;

                //https://dev77188.service-now.com/api/now/table/kb_knowledge?sysparm_query=GOTO123TEXTQUERY321%3Dlaptop&sysparm_limit=1
                request.Headers.Add("Authorization", auth2);
                request.Headers.Add("Content-Type", "application/json");
                request.Headers.Add("Accept", "application/json");

                request.Method = "Get";
                //   CoreBot.models.Incident_api_result incident_Api_Result = null;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var resforinc = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    apiresult kbnumberobj = JsonConvert.DeserializeObject<apiresult>(resforinc);
                    JObject joResponse = JObject.Parse(resforinc.ToString());

                    //CoreBot.models.Incident_api_result incident_api = null;

                    return kbnumberobj;


                }


            }
            catch (Exception)
            {
                return null;
            }






        }



    }
}