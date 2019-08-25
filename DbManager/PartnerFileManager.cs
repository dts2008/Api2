using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
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

            updateAction = (newItem, currentItem) => 
            {
                newItem.added = currentItem.added;
                newItem.name = currentItem.name;
                newItem.size = currentItem.size;
                newItem.added = currentItem.added;
                newItem.partnerId = currentItem.partnerId;
                newItem.fileToken = currentItem.fileToken;
            };

            var pfInfo = new PartnerFileInfo();

            pfInfo.fileToken = "80f0f214f3d74cb6b2c92395bf13e2ba.png";

            string path = Path.Combine(resourcePath, pfInfo.fileToken);

            pfInfo.id = 1;
            pfInfo.added = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            pfInfo.name = "test001.png";
            pfInfo.size = new FileInfo(path).Length;

            commonItems.Add(pfInfo);
        }

        public override async Task<int> Upload(string fileName, Stream stream, string item)
        {
            try
            {
                var pfInfo = new PartnerFileInfo();
                var info = Tools.Deserialize<PartnerFileInfo>(item);

                if (info.partnerId == 0)
                    return 0;

                pfInfo.fileToken = Guid.NewGuid().ToString("N") + Path.GetExtension(fileName);
                string path = Path.Combine(resourcePath, pfInfo.fileToken);

                using (var file = new FileStream(path, FileMode.Create))
                {
                    await stream.CopyToAsync(file);
                }

                pfInfo.id = commonItems.Count > 0 ? commonItems.Max(i => i.id) + 1 : 1;
                pfInfo.added = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
                pfInfo.name = fileName;
                pfInfo.size = new FileInfo(path).Length;

                
                pfInfo.partnerId = info.partnerId;
                pfInfo.description = info.description;

                if (info.id != 0)
                {
                    pfInfo.id = info.id;

                    int index = commonItems.FindIndex(k => k.id == info.id);
                    if (index == -1) commonItems.Add(pfInfo);
                    else
                    {
                        File.Delete(Path.Combine(resourcePath, commonItems[index].fileToken));
                        commonItems[index] = pfInfo;
                    }
                }
                else commonItems.Add(pfInfo);

                return pfInfo.id;
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return 0;
        }

        public override string Download(int id)
        {
            int index = commonItems.FindIndex(k => k.id == id);
            if (index == -1) return string.Empty;

            return Path.Combine(resourcePath, commonItems[index].fileToken);
        }
    }
}
