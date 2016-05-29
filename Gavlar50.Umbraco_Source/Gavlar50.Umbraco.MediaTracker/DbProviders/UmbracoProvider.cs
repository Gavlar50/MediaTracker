using System;
using System.Linq;
using Gavlar50.Umbraco.MediaTracker.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Web;

namespace Gavlar50.Umbraco.MediaTracker.DbProviders
{
   public class UmbracoProvider : IMediaTrackerDbProvider
   {
      public string DbConnString { get; set; }

      readonly Database _context = UmbracoContext.Current.Application.DatabaseContext.Database;

      public void AddMediaTrack(MediaTrackerModel track)
      {
         try
         {
            var query =
               _context.Query<MediaTrackerModel>(
                  string.Format("select * from MediaTracker where MediaId={0} and ContentId={1} and PropertyId={2}",
                     track.MediaId, track.ContentId, track.PropertyId));
            if (query.Any()) return;
            _context.Insert("MediaTracker", "Id", track);
         }
         catch (Exception ex)
         {
            LogHelper.Error<Exception>(ex.Message, ex);
         }
      }

      public void RemoveMediaTrack(int contentId)
      {
         try
         {
            _context.Delete(string.Format(
               "delete from MediaTracker where ContentId={0}",
               contentId));
         }
         catch (Exception ex)
         {
            LogHelper.Error<Exception>(ex.Message, ex);
         }
      }

      public int GetMediaIdFromUrl(string url)
      {
         var id = 0;
         try
         {
            var sql = string.Format(
               "select ContentNodeId from cmsPropertyData d inner join cmsPropertyType t on d.propertytypeid=t.id and t.Alias='umbracoFile' and d.dataNvarchar='{0}'",
               url);
            var result = _context.Query<MediaTypeModel>(sql).ToList();
            if (result.Any())
               id = result[0].ContentNodeId;
         }
         catch (Exception ex)
         {
            LogHelper.Error<Exception>(ex.Message, ex);
         }
         return id;
      }
   }
}
