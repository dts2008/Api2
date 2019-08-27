using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api2
{
    public class Api2Engine
    {
        #region Class & Type

        public enum Api2Options
        {
            None = 0,
            Token = 1,
            Post = 2,
            Get = 3
        }

        public delegate Task ApiFunc(HttpContext context, string cmd, UserItem userItem);

        public class UserItem
        {
            public int uid;
        }

        #endregion

        #region Field(s)

        public Dictionary<string, Tuple<ApiFunc, Api2Options>> _apiMethods = new Dictionary<string, Tuple<ApiFunc, Api2Options>>();

        private static Api2Engine _instance = new Api2Engine();

        public static Api2Engine Instance
        {
            get { return _instance; }
        }

        private static TimeoutDataBuffer<string, UserItem> _activeUsers = new TimeoutDataBuffer<string, UserItem>(3600L * 10000000);

        #endregion

        #region

        public enum Api2Error
        {
            Parameters = 1,
            Authorization = 2,
            WrongTime = 3,
            WrongHash = 4,
            WrongGroupId = 5,
            WrongGameId = 6,
            WrongBets = 7,
            WrongPayout = 8,
            WrongId = 9,
            WrongLogin = 10,
            WrongRequest = 11,
            Token = 12,
            Internal = 100
        }


        #endregion

        #region Field(s)

        public static Dictionary<Api2Error, string> errorList = new Dictionary<Api2Error, string>()
        {
            { Api2Error.Parameters, "Parameters issue. Please, check request parameters." },
            { Api2Error.Authorization, "Authorization issue. Please, check login or password." },
            { Api2Error.WrongTime, "Authorization issue. Please, check time of request." },
            { Api2Error.WrongHash, "Authorization issue. Please, check hash." },
            { Api2Error.WrongGroupId, "Parameters issue. Please, check group_id." },
            { Api2Error.WrongGameId, "Parameters issue. Please, check game_id." },
            { Api2Error.WrongBets, "Parameters issue. Please, check bets." },
            { Api2Error.WrongPayout, "Parameters issue. Please, check payout." },
            { Api2Error.WrongId, "Parameters issue. Wrong Id." },
            { Api2Error.WrongLogin, "User with same Login alrady axist." },
            { Api2Error.Token, "Wrong or expired token." },
            { Api2Error.Internal, "Internal error." },
        };

        public static string GetError(Api2Error error)
        {
            var result = new Api2Result("error");

            result["error_id"] = ((int)error).ToString();
            result["error_description"] = errorList[error];
            result["status"] = Api2Result.status_error;

            return JsonConvert.SerializeObject(result);
        }

        #endregion

        #region Public 

        public async Task Process(HttpContext context, string cmd)
        {
            if (!_apiMethods.TryGetValue(cmd, out var apiFunc))
                return;

            UserItem userItem = null;
            if ((apiFunc.Item2 & Api2Options.Token) != 0)
            {
                string token = context.Request.Query["token"];

                if (!_activeUsers.GetRenew(token, out userItem) || userItem == null)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }
                // check tokens
            }

            await apiFunc.Item1(context, cmd, userItem);
        }

        public bool IsApi(string cmd)
        {
            return _apiMethods.ContainsKey(cmd);
        }

        #endregion

        #region Private method(s)

        private Api2Engine()
        {
            _apiMethods["auth"] = new Tuple<ApiFunc, Api2Options>(Auth, Api2Options.None);
            _apiMethods["get"] = new Tuple<ApiFunc, Api2Options>(Get, Api2Options.Token);
            _apiMethods["delete"] = new Tuple<ApiFunc, Api2Options>(Delete, Api2Options.Token | Api2Options.Post);
            _apiMethods["update"] = new Tuple<ApiFunc, Api2Options>(Update, Api2Options.Token | Api2Options.Post);
            _apiMethods["upload"] = new Tuple<ApiFunc, Api2Options>(Upload, Api2Options.Token | Api2Options.Post);
            _apiMethods["download"] = new Tuple<ApiFunc, Api2Options>(Download, Api2Options.Token | Api2Options.Get);
        }

        #region Api

        private async Task Auth(HttpContext context, string cmd, UserItem userItem)
        {
            if (!DBManager.Instance.Get("userinfo", out IManager manager))
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                return;
            }

            string login = context.Request.Query["login"];
            string password = context.Request.Query["password"];

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                return;
            }

            var user = manager.Get("login", login) as UserInfo;
            if (user == null)
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                return;
            }

            if (string.Compare(user.password, password, true) != 0)
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Authorization)));
                return;
            }

            var result = new Api2Result(cmd);
            string token = Guid.NewGuid().ToString("N");
            result["token"] = token;

            _activeUsers.Set(token, new UserItem() { uid = user.id });

            await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
        }

        private async Task Get(HttpContext context, string cmd, UserItem userItem)
        {
            try
            {
                string type = context.Request.Query["type"];

                int.TryParse(context.Request.Query["current_page"], out int current_page);
                int.TryParse(context.Request.Query["page_size"], out int page_size);

                string sort_by = context.Request.Query["sort_by"];
                bool.TryParse(context.Request.Query["descending"], out bool descending);

                bool dependence = Tools.ToInt(context.Request.Query["dependence"].ToString()) == 1;
                

                if (current_page <= 0) current_page = 1;
                if (page_size <= 0) page_size = 5;

                if (!DBManager.Instance.Get(type, out IManager manager))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                string filter = context.Request.Query["filter"];

                var filterList = Tools.Deserialize<List<FilterItem>>(filter);

                var items = manager.Get(current_page, page_size, out int total_items, sort_by, descending, filterList);

                var result = new Api2Result(cmd);

                var data = new Dictionary<string, object>()
                        {
                            { "total_items", total_items },
                            { "current_page", current_page },
                            { "page_size", page_size },
                            { "items", items },
                        };

                result["data"] = data;

                if (dependence)
                {
                    var dep = manager.Dependence(items);
                    if (dep != null)
                        data["dependence"] = dep;
                }


                //var result = new Dictionary<string, object>()
                //        {
                //            { "error_code", 0 },
                //            { "data", data },
                //        };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));

            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
            }
        }

        private async Task Delete(HttpContext context, string cmd, UserItem userItem)
        {
            try
            {
                string body;
                using (var reader = new StreamReader(context.Request.Body))
                {
                    body = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(body))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                string type = context.Request.Query["type"];
                // check type

                if (!DBManager.Instance.Get(type, out IManager manager))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                var del = Tools.Deserialize<Api2Delete>(body);
                if (del == null || del.id <= 0 || !manager.Delete(del.id))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.WrongId)));
                    return;
                }

                var result = new Api2Result(cmd);
                result["id"] = del.id;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
            }
        }

        private async Task Update(HttpContext context, string cmd, UserItem userItem)
        {
            try
            {
                string body;
                using (var reader = new StreamReader(context.Request.Body))
                {
                    body = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(body))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                string type = context.Request.Query["type"];
                // check type

                if (!DBManager.Instance.Get(type, out IManager manager))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                if (!manager.Update(body, out int id))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                var result = new Api2Result(cmd);
                result["id"] = id;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
            }
        }

        private async Task Upload(HttpContext context, string cmd, UserItem userItem)
        {
            try
            {
                var boundary = context.Request.GetMultipartBoundary();

                if (string.IsNullOrWhiteSpace(boundary))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.WrongRequest)));
                    return;
                }

                string type = context.Request.Query["type"];
                string item = context.Request.Query["item"];

                // check type
                //Upload(string fileName, string description, Stream stream, string options)

                if (!DBManager.Instance.Get(type, out IManager manager))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                var reader = new MultipartReader(boundary, context.Request.Body);
                var section = await reader.ReadNextSectionAsync();

                if (section == null)
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                var fileSection = section.AsFileSection();
                var fileName = fileSection.FileName;

                int id = await manager.Upload(fileSection.FileName, fileSection.FileStream, item);

                
                var result = new Api2Result(cmd);
                result["id"] = id;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Internal)));
            }
        }

        private async Task Download(HttpContext context, string cmd, UserItem userItem)
        {
            try
            {
                string type = context.Request.Query["type"];
                int id = Tools.ToInt((string)context.Request.Query["id"]);

                // check type
                //Upload(string fileName, string description, Stream stream, string options)

                if (!DBManager.Instance.Get(type, out IManager manager))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                string fileName = manager.Download(id);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Parameters)));
                    return;
                }

                await context.Response.SendFileAsync(fileName);
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
                await context.Response.WriteAsync(JsonConvert.SerializeObject(GetError(Api2Error.Internal)));
            }
        }

        #endregion

        #endregion
    }
}
