﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data.OleDb;
using System.Data;

namespace WalmartFinancialPredictionSite
{
    public class ClusteringCode
    {
        public string Result { get; private set; }
        public DataTable Import_To_Grid(string FilePath, string Extension, string isHDR)
        {
            string conStr = "";
            switch (Extension)
            {
                case ".xls": //Excel 97-03
                    conStr = ConfigurationManager.ConnectionStrings["Excel03ConString"]
                             .ConnectionString;
                    break;
                case ".xlsx": //Excel 07
                    conStr = ConfigurationManager.ConnectionStrings["Excel07ConString"]
                              .ConnectionString;
                    break;
            }
            conStr = String.Format(conStr, FilePath, isHDR);
            OleDbConnection connExcel = new OleDbConnection(conStr);
            OleDbCommand cmdExcel = new OleDbCommand();
            OleDbDataAdapter oda = new OleDbDataAdapter();
            DataTable dt = new DataTable();
            cmdExcel.Connection = connExcel;

            //Get the name of First Sheet
            connExcel.Open();
            DataTable dtExcelSchema;
            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            string SheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
            connExcel.Close();

            //Read Data from First Sheet
            connExcel.Open();
            cmdExcel.CommandText = "SELECT * From [" + SheetName + "]";
            oda.SelectCommand = cmdExcel;
            oda.Fill(dt);
            connExcel.Close();

            //Bind Data to GridView
            return dt;
        }
        public class StringTable
        {
            public string[] ColumnNames { get; set; }
            public string[,] Values { get; set; }
        }

         async Task InvokeRequestResponseService(String temp, String unemp, String fuelPrice, String cpi, String size)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() { 
                        { 
                            "input1", 
                            new StringTable() 
                            {
                                ColumnNames = new string[] {"Weekly_Sales", "Size", "Temperature", "Fuel_Price", "CPI", "Unemployment"},
                                Values = new string[,] {  { "0", size,temp,fuelPrice,cpi,unemp },  { "0", "0", "0", "0", "value", "value" },  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "BKDUlC/RAhl01cIhrUJbzO1sGZSEkoLN4sDPbFVkmsuWYyrM1XSk2k1HVDDcawGuqnHV6y8OKAk/nkcGTJcEWQ=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/e2f415aa3a044f69a28aaf823039379e/services/1ff860ce10004d928ec43fe6af0aa898/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false); ;

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);
                }
                else
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Result: {0}", result);
                    try
                    {
                        MemoryStream jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
                        JObject jObject = JsonConvert.DeserializeObject<JObject>(result);
                        String rr = jObject["Results"]["output1"]["value"]["Values"][0][6].ToString();
                        double x = Double.Parse(rr);
                        if (x > 0.5)
                        {
                            Result = "In Profit";
                        }
                        else
                        {
                            Result = "May not have sufficient sales..Apply markups to increase sales..!!";
                        }
                    }
                    catch (Exception e)
                    {
                        String msg = e.Message;
                    }
                }
            }
        }
  

        
        public ClusteringCode(String temp, String unemp, String fuelPrice, String cpi,String size)
        {
            InvokeRequestResponseService(temp, unemp, fuelPrice, cpi, size).Wait();
        }
        public ClusteringCode()
        {

        }
    }
}