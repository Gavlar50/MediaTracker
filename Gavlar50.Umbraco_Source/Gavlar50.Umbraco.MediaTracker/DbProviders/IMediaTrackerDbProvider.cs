using Gavlar50.Umbraco.MediaTracker.Models;

namespace Gavlar50.Umbraco.MediaTracker.DbProviders
{
   public interface IMediaTrackerDbProvider
   {
      string DbConnString { get; set; }
      void AddMediaTrack(MediaTrackerModel track);
      void RemoveMediaTrack(int contentId);
      int GetMediaIdFromUrl(string url);
   }
}
