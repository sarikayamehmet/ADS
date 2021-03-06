﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;
using System.Data;

namespace WalmartFinancialPredictionSite
{
    public partial class DefineProfitLoss : System.Web.UI.Page
    {
        Classificationcode c = new Classificationcode();
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void btn_Submit_Click(object sender, EventArgs e)
		{
			String store = txt_month.Text;
			String dept = txt_DeptNo.Text;
			String month = txt_month.Text;
			String year = txt_Year.Text;
			Classificationcode c=new Classificationcode(store,dept,month,year);
			Label1.Text = c.Result;
		}

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string FileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                string Extension = Path.GetExtension(FileUpload1.PostedFile.FileName);
                string FolderPath = ConfigurationManager.AppSettings["FolderPath"];

                string FilePath = Server.MapPath(FolderPath + FileName);
                FileUpload1.SaveAs(FilePath);
                DataTable dt = c.Import_To_Grid(FilePath, Extension, rbHDR.SelectedItem.Text);

            }
        }

        protected void PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
            string FileName = GridView1.Caption;
            string Extension = Path.GetExtension(FileName);
            string FilePath = Server.MapPath(FolderPath + FileName);

            DataTable dt=c.Import_To_Grid(FilePath, Extension, rbHDR.SelectedItem.Text);
            GridView1.Caption = Path.GetFileName(FilePath);
            GridView1.DataSource = dt;
            GridView1.DataBind();
           
        }

		protected void UploadButton_Click(object sender, EventArgs e)
{
    if (FileUpload1.HasFile)            
        try
        {
            FileUpload1.SaveAs(Server.MapPath("~/uploads/") +
                 FileUpload1.FileName);
			String fileName = FileUpload1.PostedFile.FileName;
			var filePath = Path.Combine(Server.MapPath("~/uploads"), fileName);
            FileUploadedLabel.Text = "File name: " +
                 fileName + "<br>" +
                 FileUpload1.PostedFile.ContentLength + " kb<br>" +
                 "Content type: " + FileUpload1.PostedFile.ContentType;

			if (!string.IsNullOrEmpty(filePath))
			{
				StringBuilder strbuild = new StringBuilder();
				Classificationcode c=new Classificationcode();
				using (StreamReader sr = new StreamReader(Path.Combine(Server.MapPath("~/uploads"), fileName)))
				{
					while (sr.Peek() >= 0)
					{
						strbuild.AppendFormat(sr.ReadLine());
					}
				}
				Label1.Text = strbuild.ToString();
				//c.ReadFileForBatch(strbuild);
			}
        }
        catch (Exception ex)
        {
            FileUploadedLabel.Text = "ERROR: " + ex.Message.ToString();
        }
    else
    {
        FileUploadedLabel.Text = "You have not specified a file.";
    }
}
	
    }
}