using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Gavlar50.Umbraco.MediaTracker.Models
{
   [TableName("MediaTracker")]
   public class MediaTrackerModel
   {
      [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
      public int Id { get; set; }
      /// <summary>
      /// Link to CmsPropertyData for media
      /// </summary>
      public int MediaId { get; set; }
      /// <summary>
      /// Link to CmsPropertyData for content
      /// </summary>
      public int ContentId { get; set; }
      /// <summary>
      /// Link to CmsPropertyType Id. Used with MediaId and ContentId to retrieve content values.
      /// </summary>
      public int PropertyId { get; set; }
   }
}
