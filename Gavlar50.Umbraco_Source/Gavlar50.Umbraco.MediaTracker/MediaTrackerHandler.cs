using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Gavlar50.Umbraco.MediaTracker.Models;
using Gavlar50.Umbraco.MediaTracker.DbProviders;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;

namespace Gavlar50.Umbraco.MediaTracker
{
   public class MediaTrackerHandler : IApplicationEventHandler
   {
      List<string> _trackedProperties = new List<string>();
      readonly Regex _imgtagRegex = new Regex("<img.*?(/>|</img>)");
      readonly Regex _imgsrcRegex = new Regex("<img.+?src=[\"'](?<src>.+?)[\"'?].*?>");
      readonly Regex _dataidRegex = new Regex("<img.+?data-id=[\"'](?<dataid>.+?)[\"'].*?>");
      private bool _verbose;

      public IMediaTrackerDbProvider DbProvider { get; set; }

      public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication,
         ApplicationContext applicationContext)
      {
         if (!applicationContext.DatabaseContext.Database.TableExist("MediaTracker"))
         {
            applicationContext.DatabaseContext.Database.CreateTable<MediaTrackerModel>();
         }
      }

      public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
      {

      }

      public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
      {
         if (ConfigurationSettings.AppSettings["MediaTracker.Properties"] == null)
         {
            LogHelper.Warn<MediaTrackerHandler>("MediaTracker.Properties is not set in your web.config. MediaTracker is not tracking anything!");
            return;
         }
         _verbose = Convert.ToBoolean(ConfigurationSettings.AppSettings["MediaTracker.VerboseLogging"]);
         DbProvider = new UmbracoProvider();
         _trackedProperties = ConfigurationManager.AppSettings["MediaTracker.Properties"].Split(new[] { ',' }).ToList();
         ContentService.Published += ContentServiceOnPublished;
         ContentService.Deleted += ContentServiceOnDeleted;
         LogHelper.Info<MediaTrackerHandler>("MediaTracker initialised and tracking the following properties: " + string.Join(",", _trackedProperties));
      }

      /// <summary>
      /// Called when user empties the recycle bin, or an item in the recycle bin is permanently deleted
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="deleteEventArgs"></param>
      private void ContentServiceOnDeleted(IContentService sender, DeleteEventArgs<IContent> deleteEventArgs)
      {
         if (!deleteEventArgs.DeletedEntities.Any()) return;
         foreach (var entity in deleteEventArgs.DeletedEntities)
         {
            DbProvider.RemoveMediaTrack(entity.Id);
            if (_verbose)
            {
               LogHelper.Info<MediaTrackerHandler>(string.Format("All tracking for the page '{0}' was removed because the page was moved to the recycle bin.", entity.Name));
            }
         }
      }

      /// <summary>
      /// Called when an item is published
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="publishEventArgs"></param>
      private void ContentServiceOnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> publishEventArgs)
      {
         if (!publishEventArgs.PublishedEntities.Any()) return;
         foreach (var entity in publishEventArgs.PublishedEntities)
         {
            DbProvider.RemoveMediaTrack(entity.Id);
            if (_verbose)
            {
               LogHelper.Info<MediaTrackerHandler>(string.Format("All previous tracking for the page '{0}' was removed because the page is about to be published.", entity.Name));
            }
            var content = ApplicationContext.Current.Services.ContentService.GetById(entity.Id);
            var validProperties = content.Properties.Where(x => _trackedProperties.Contains(x.Alias)).ToList();
            foreach (var property in validProperties)
            {
               var propertyType = entity.PropertyTypes.Single(x => x.Alias == property.Alias);
               var editorType = GetInstanceField(typeof (PropertyType), propertyType, "PropertyEditorAlias").ToString();
               var propertyVal = property.Value.ToString();
               if (string.IsNullOrEmpty(propertyVal)) continue;

               switch (editorType)
               {
                  case "Umbraco.MediaPicker":
                     CreateMediaRecord(
                        EnsureModel(content.Id, Convert.ToInt32(propertyVal), propertyType.Id)
                        , content, property);
                     break;
                  case "Umbraco.MultipleMediaPicker":
                     var mediaIds = propertyVal.Split(new[] {','});
                     foreach (var mediaId in mediaIds)
                     {
                        CreateMediaRecord(
                           EnsureModel(content.Id, Convert.ToInt32(mediaId), propertyType.Id)
                           , content, property);
                     }
                     break;
                  default: // assume content with embedded image links
                     var matches = _imgtagRegex.Matches(propertyVal);
                     foreach (Match m in matches)
                     {
                        var imgTag = m.Value;
                        var src = _imgsrcRegex.Match(imgTag).Groups["src"].Value;
                        var id = _dataidRegex.Match(imgTag).Groups["dataid"].Value;
                        CreateMediaRecord(
                           EnsureModel(content.Id, EvaluateDataId(id, src), propertyType.Id)
                           , content, property);
                     }
                     break;
               }
            }
         }
      }

      /// <summary>
      /// Extract media Id from url
      /// </summary>
      /// <param name="dataId"></param>
      /// <param name="url"></param>
      /// <returns></returns>
      private int EvaluateDataId(string dataId, string url)
      {
         return dataId == "" ?
            DbProvider.GetMediaIdFromUrl(url.TrimEnd("\"").TrimEnd("?")) :
            Convert.ToInt32(dataId);
      }

      /// <summary>
      /// Obtains access to non-public properties
      /// </summary>
      /// <param name="type">Object type</param>
      /// <param name="instance">Object instance</param>
      /// <param name="fieldName">Required field</param>
      /// <returns></returns>
      private object GetInstanceField(Type type, object instance, string fieldName)
      {
         BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
         var field = type.GetProperty(fieldName, bindFlags);
         return field.GetValue(instance, null);
      }

      /// <summary>
      /// Creates database entry for media track
      /// </summary>
      /// <param name="model">The MediaTrackerModel</param>
      /// <param name="content">The umbraco content item</param>
      /// <param name="property">The Umbraco property being tracked</param>
      private void CreateMediaRecord(MediaTrackerModel model, IContent content, Property property)
      {
         DbProvider.AddMediaTrack(model);
         if (_verbose)
         {
            var media = ApplicationContext.Current.Services.MediaService.GetById(model.MediaId);
            LogHelper.Info<MediaTrackerHandler>(
               string.Format(
                  "The media item '{0}' is now being tracked in the property '{1}' on the page '{2}'",
                  media.Name, property.Alias, content.Name));
         }
      }

      /// <summary>
      /// Creates a new mediatracker model object
      /// </summary>
      /// <param name="contentId">Id of content page</param>
      /// <param name="mediaId">Id of media item</param>
      /// <param name="propertyId">Id of umbraco property</param>
      /// <returns>MediaTrackerModel object</returns>
      private MediaTrackerModel EnsureModel(int contentId, int mediaId, int propertyId)
      {
         return new MediaTrackerModel
         {
            ContentId = contentId,
            MediaId = mediaId,
            PropertyId = propertyId
         };
      }
   }
}