namespace Gavlar50.Umbraco.MediaTracker.Reports.Models
{
   public class UnusedMediaModel
   {
      private string _size; // Size is string to cater for null images - an image record where no file was selected

      public string Name { get; set; }
      public string Media { get; set; }

      public string Size
      {
         get { return _size; }

         set
         {
            if (string.IsNullOrEmpty(value))
            {
               _size = "0";
            }
            else
            {
               var nSize = 0;
               int.TryParse(value, out nSize);
               _size = nSize.ToString();
            }
         }
      }
   }
}