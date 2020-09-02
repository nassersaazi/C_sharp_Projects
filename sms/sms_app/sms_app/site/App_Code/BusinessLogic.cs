using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Drawing;
using System.Data;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

/// <summary>
/// Summary description for BusinessLogic
/// </summary>
public class BusinessLogic
{
	public BusinessLogic()
	{
	}

    public void ExportToExcel(DataTable table, string filename, HttpResponse Response)
    {
        if (table == null)
        {
            return;
        }
        if (table.Rows.Count > 0)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=Report.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            using (StringWriter sw = new StringWriter())
            {
                HtmlTextWriter hw = new HtmlTextWriter(sw);
                GridView grid = new GridView();
                //To Export all pages
                grid.AllowPaging = false;
                grid.DataSource = table;
                grid.DataBind();

                grid.HeaderRow.BackColor = Color.White;
                foreach (TableCell cell in grid.HeaderRow.Cells)
                {
                    cell.BackColor = grid.HeaderStyle.BackColor;
                }
                foreach (GridViewRow row in grid.Rows)
                {
                    row.BackColor = Color.White;
                    foreach (TableCell cell in row.Cells)
                    {
                        if (row.RowIndex % 2 == 0)
                        {
                            cell.BackColor = grid.AlternatingRowStyle.BackColor;
                        }
                        else
                        {
                            cell.BackColor = grid.RowStyle.BackColor;
                        }
                        cell.CssClass = "textmode";
                    }
                }

                grid.RenderControl(hw);

                //style to format numbers to string
                string style = @"<style> .textmode { } </style>";
                Response.Write(style);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
        }
    }

    public void ExportToPdf(DataTable table, string filename, HttpResponse Response)
    {
        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter hw = new HtmlTextWriter(sw))
            {
                //To Export all pages

                GridView gridView = new GridView();
                gridView.DataSource = table;
                gridView.DataBind();
                gridView.AllowPaging = false;

                gridView.RenderControl(hw);
                StringReader sr = new StringReader(sw.ToString());
                iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A2, 10f, 10f, 10f, 0f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.Open();
                htmlparser.Parse(sr);
                pdfDoc.Close();

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=Report.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
            }
        }
    }


}