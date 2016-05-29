using System;
using System.Linq;
using Gavlar50.Umbraco.MediaTracker.Reports.Models;
using umbraco.NodeFactory;
using UmbracoUserControl = Umbraco.Web.UI.Controls.UmbracoUserControl;

namespace Gavlar50.Umbraco.MediaTracker.Reports.usercontrols
{
    public partial class MediaTrackerReport : UmbracoUserControl
    {
        protected void btnShowUsages_Click(object sender, EventArgs e)
        {
            var imgId = 0;
            Int32.TryParse(txtMediaId.Text, out imgId);
            if (imgId > 0)
            {
                lblMediaError.Visible = false;
                var img = Umbraco.Media(imgId);
                var context = UmbracoContext.Application.DatabaseContext.Database;
                var query = context.Query<MediaUsedModel>(string.Format(
                    "select t.name as Property, " +
                    "mt.ContentId, " +
                    "d.text as ContentName, " +
                    "'' as Url from MediaTracker mt " +
                    "inner join umbracoNode d on mt.ContentId = d.id " +
                    "inner join cmsPropertyType t on mt.PropertyId=t.id " +
                    "where mt.MediaId={0} order by mt.ContentId", imgId)).ToList();
                foreach (var item in query)
                {
                    var page = new Node(item.ContentId);
                    item.Url = page.NiceUrl;
                }
                grdUnused.DataSource = null;
                grdUnused.DataBind();
                grdAllMedia.DataSource = null;
                grdAllMedia.DataBind();
                grdMedia.DataSource = query;
                grdMedia.DataBind();
                lblReportHeading.Text = "<h3>Media Usage For <a href='" + img.Url + "' target='_blank'>" + img.Name +
                                        "</a> [" + txtMediaId.Text + "]</h3>";
            }
            else
            {
                lblMediaError.Visible = true;
            }
        }

        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            var context = UmbracoContext.Application.DatabaseContext.Database;
            var query = context.Query<AllMediaModel>(
               "select t.name as Property, mt.ContentId, d.text as ContentName, '' as Url, p.dataNvarchar as Media from MediaTracker mt " +
               "inner join umbracoNode d on mt.ContentId = d.id " +
               "inner join cmsPropertyType t on mt.PropertyId=t.id " +
               "inner join cmsPropertyData p on mt.MediaId=p.contentNodeId " +
               "inner join cmsPropertyType y on p.propertytypeid=y.id and y.Alias='umbracoFile' " +
               "order by mt.ContentId").ToList();
            foreach (var item in query)
            {
                var page = new Node(item.ContentId);
                item.Url = page.NiceUrl;
            }
            grdUnused.DataSource = null;
            grdUnused.DataBind();
            grdMedia.DataSource = null;
            grdMedia.DataBind();
            grdAllMedia.DataSource = query;
            grdAllMedia.DataBind();
            lblReportHeading.Text = "<h3>All Tracked Media</h3>";
        }

        protected void btnShowUnused_Click(object sender, EventArgs e)
        {
            var context = UmbracoContext.Application.DatabaseContext.Database;
            var query = context.Query<UnusedMediaModel>(
               "select n.text as Name, d.dataNvarchar as Media,p.dataNvarchar as Size from cmspropertydata d " +
               "inner join cmsPropertyType t on d.propertytypeid=t.id " +
               "inner join cmspropertydata p on d.contentnodeid=p.contentnodeid " +
               "inner join cmsPropertyType t2 on p.propertytypeid=t2.id and t2.Alias='umbracoBytes' " +
               "inner join umbracoNode n on p.contentNodeId=n.id " +
               "where t.Alias='umbracoFile' and n.trashed = 0 and d.contentNodeId not in " +
               "(select MediaId from MediaTracker)").ToList();
            grdMedia.DataSource = null;
            grdMedia.DataBind();
            grdAllMedia.DataSource = null;
            grdAllMedia.DataBind();
            grdUnused.DataSource = query;
            grdUnused.DataBind();
            lblReportHeading.Text = "<h3>Unused Media</h3><h4>* Unused according to tracked properties. Media may still be in use by untracked properties.</h4>";
        }

        protected void btnShowSizes_Click(object sender, EventArgs e)
        {
            var context = UmbracoContext.Application.DatabaseContext.Database;
            var query = context.Query<UnusedMediaModel>(
               "select n.text as Name, d.dataNvarchar as Media,p.dataNvarchar as Size from cmspropertydata d " +
               "inner join cmsPropertyType t on d.propertytypeid=t.id " +
               "inner join cmspropertydata p on d.contentnodeid=p.contentnodeid " +
               "inner join cmsPropertyType t2 on p.propertytypeid=t2.id and t2.Alias='umbracoBytes' " +
               "inner join umbracoNode n on p.contentNodeId=n.id " +
               "where t.Alias='umbracoFile' and n.trashed = 0").ToList().OrderByDescending(x => Convert.ToInt32(x.Size));
            grdMedia.DataSource = null;
            grdMedia.DataBind();
            grdAllMedia.DataSource = null;
            grdAllMedia.DataBind();
            grdUnused.DataSource = query;
            grdUnused.DataBind();
            lblReportHeading.Text = "<h3>All Media By Size</h3><h4>* All media in site, tracked and untracked.</h4>";
        }
    }
}