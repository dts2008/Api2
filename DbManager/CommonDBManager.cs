using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Api2
{
    public class CommonDBManager<T> : IManager where T : CommonInfo
    {
        public static string typeName;

        public FieldInfo[] typeFields;

        //protected Action<T, T> updateAction = null;

        private static string connectionString;

        static CommonDBManager()
        {
            connectionString = AppSettings.Instance.GetSection("ConnectionStrings").GetValue<string>("MySQLConnection");
        }

        public CommonDBManager()
        {
            typeName = typeof(T).Name.ToLower();

            var t = typeof(T);
            typeFields = t.GetFields();

            Init();
        }

        public virtual Type ItemType()
        {
            return typeof(T);
        }

        public virtual bool UpdateItem(T newValue, T oldValue)
        {
            return true;
        }

        public virtual bool InsertItem(T newValue)
        {
            return true;
        }

        public virtual void Init()
        {
            //for (int i = 0; i < 30; ++i)
            //{
            //    var item = new UserInfo();

            //    item.id = i + 1;
            //    item.login = $"login{i + 1}";
            //    item.name = $"Name {i + 1}";
            //    item.role = sequence.Next(3) + 1;
            //    item.partners = sequence.Next(50);
            //    item.activity = (int)((DateTimeOffset)DateTime.UtcNow.AddHours(-sequence.Next(72))).ToUnixTimeSeconds();
            //    item.contacts = "";

            //    userItems.Add(item);
            //}
        }

        public virtual bool Delete(int id)
        {
            return DBCommand((IDbConnection db) =>
            {
                db.Execute($"delete {typeName} where id = {id}");
            }
            );
        }

        public bool Update(string data, out int id)
        {
            var item = Tools.Deserialize<T>(data);
            id = 0;

            if (item == null) return false;

            if (item.id > 0)
            {
                //if (updateAction != null)
                //{
                //    var currentItem = Get(item.id) as T;

                //    if (currentItem == null) return false;

                //    updateAction.Invoke(item, currentItem);
                //}

                var currentItem = Get(item.id) as T;
                if (currentItem == null) return false;

                if (!UpdateItem(item, currentItem))
                    return false;

                if (!DBCommand((IDbConnection db) => {
                    db.Update(item);
                })) return false;

                return true;
            }

            long result = 0;
            T ci = item as T;
            if (!InsertItem(ci))
                return false;

            if (!DBCommand((IDbConnection db) => {
                result = db.Insert(item as T);
            })) return false;

            id = (int)result;

            return true;
        }

        public virtual async Task<int> Upload(string fileName, Stream stream, string item)
        {
            await Task<int>.CompletedTask;

            return 0;
        }

        public virtual string Download(int id)
        {
            return string.Empty;
        }

        public List<CommonInfo> Get(int page, int pageSize, out int total_items, string sort_by, bool descending, List<FilterItem> filterList)
        {
            total_items = 0;
            string desc = descending ? "DESC" : string.Empty;
            var where = new StringBuilder();

            if (filterList != null && filterList.Count > 0)
            {
                
                foreach (var filters in filterList)
                {
                    var field = typeFields.FirstOrDefault(f => f.Name == filters.name);
                    if (field == null) continue;

                    if (where.Length > 0) where.Append("AND ");
                        where.Append($"{filters.name} {GetFilterOperator(filters.type)} {filters.value}");

                    GetFilterOperator(filters.type);
                }

                if (where.Length > 0) where.Insert(0, " WHERE ");
            }

            string query = pageSize != -1 ?
                $"SELECT * FROM {typeName} {where.ToString()} ORDER BY {sort_by} {desc} LIMIT {(page - 1) * pageSize}, {pageSize} " :
                $"SELECT * FROM {typeName} {where.ToString()} ORDER BY {sort_by} {desc} ";

            IEnumerable<T> result = null;
            int result_count = 0;

            if (!DBCommand((IDbConnection db) => {
                result = db.Query<T>(query);
                result_count = db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {typeName}");
            })) return null;

            total_items = result_count;
            return new List<CommonInfo>(result);
        }

        public virtual CommonInfo Get(int id)
        {
            CommonInfo result = null;

            DBCommand((IDbConnection db) =>
            {
                result = db.QueryFirstOrDefault<T>($"select * from {typeName} where id = {id}");
            }
            );

            return result;
        }

        public virtual CommonInfo Get(string field, object value)
        {
            CommonInfo result = null;

            DBCommand((IDbConnection db) =>
                {
                    result = db.QueryFirstOrDefault<T>($"select * from {typeName} where {field} = '{value}'");
                }
            );
            
            return result;
        }

        public virtual Dictionary<string, List<CommonInfo>> Dependence(List<CommonInfo> origin)
        {
            return null;
        }

        #region Private method(s)

        #region Compare Methods

        protected static bool DBCommand(Action<IDbConnection> action)
        {
            try
            {
                using (IDbConnection db = new MySqlConnection(connectionString))
                {
                    db.Open();

                    action(db);
                }

                return true;
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return false;
        }

        private static string GetFilterOperator(FilterType filterType)
        {
            switch (filterType)
            {
                case FilterType.Equal:
                    return " = ";
                case FilterType.MoreOrEqual:
                    return " >= ";
                case FilterType.LessOrEqual:
                    return " <= ";
                case FilterType.More:
                    return " > ";
                case FilterType.Less:
                    return " < ";
                case FilterType.In:
                    return " in ";
                default:
                    return "";
            }
        }

        #endregion Compare Methods

        #endregion
    }
}
