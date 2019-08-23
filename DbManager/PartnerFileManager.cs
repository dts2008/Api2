using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api2.DbManager
{
    public class PartnerFileManager : CommonManager<PartnerFileInfo>
    {
        public static string resourcePath = string.Empty;
        public override void Init()
        {
            resourcePath = AppSettings.Instance.GetSection("settings").GetValue<string>("resourcepath");

            if (string.IsNullOrEmpty(resourcePath))
            {
                resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resource");
            }

            if (!Directory.Exists(resourcePath))
                Directory.CreateDirectory(resourcePath);
        }

        public override async Task<int> Upload(string fileName, Stream stream, string options)
        {
            try
            {
                var item = new PartnerFileInfo();
                var info = Tools.Deserialize<(int partnerId, string description)>(options);

                if (info.partnerId == 0)
                    return 0;

                item.fileToken = Guid.NewGuid().ToString("N") + "." + Path.GetExtension(fileName);
                string path = Path.Combine(resourcePath, item.fileToken);

                using (var file = new FileStream(path, FileMode.Create))
                {
                    await stream.CopyToAsync(stream);
                }

                item.id = commonItems.Count > 0 ? commonItems.Max(i => i.id) + 1 : 1;
                item.added = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
                item.name = fileName;
                item.size = new FileInfo(path).Length;

                
                item.partnerId = info.partnerId;
                item.description = info.description;

                commonItems.Add(item);

                return item.id;
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return 0;
        }
    }
}
