using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using log4net;

namespace BR.EzTravel.Web.Helpers
{
    public static class CommonExtensions
    {
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType.Name);

        /// <summary>
        ///     A DbEntityValidationException extension method that formates validation errors to string.
        /// </summary>
        public static string DbEntityValidationExceptionToString(this DbEntityValidationException e)
        {
            var validationErrors = e.DbEntityValidationResultToString();
            var exceptionMessage = string.Format("{0}{1}Validation errors:{1}{2}", e, Environment.NewLine, validationErrors);
            return exceptionMessage;
        }

        /// <summary>
        ///     A DbEntityValidationException extension method that aggregate database entity validation results to string.
        /// </summary>
        public static string DbEntityValidationResultToString(this DbEntityValidationException e)
        {
            return e.EntityValidationErrors
                    .Select(dbEntityValidationResult => dbEntityValidationResult.DbValidationErrorsToString(dbEntityValidationResult.ValidationErrors))
                    .Aggregate(string.Empty, (current, next) => string.Format("{0}{1}{2}", current, Environment.NewLine, next));
        }

        /// <summary>
        ///     A DbEntityValidationResult extension method that to strings database validation errors.
        /// </summary>
        public static string DbValidationErrorsToString(this DbEntityValidationResult dbEntityValidationResult, IEnumerable<DbValidationError> dbValidationErrors)
        {
            var entityName = string.Format("[{0}]", dbEntityValidationResult.Entry.Entity.GetType().Name);
            const string indentation = "\t - ";
            var aggregatedValidationErrorMessages = dbValidationErrors.Select(error => string.Format("[{0} - {1}]", error.PropertyName, error.ErrorMessage))
                                                   .Aggregate(string.Empty, (current, validationErrorMessage) => current + (Environment.NewLine + indentation + validationErrorMessage));
            return string.Format("{0}{1}", entityName, aggregatedValidationErrorMessages);
        }

        public static void Log(this Exception ex)
        {
            var exString = "";

            try
            {
                if (ex is DbEntityValidationException)
                    exString = (ex as DbEntityValidationException).DbEntityValidationExceptionToString();
                else
                    exString = ex.ToString();

                logger.Error(exString);
            }
            catch (Exception iex)
            {
                using (var eventLog = new EventLog("Application"))
                {
                    eventLog.WriteEntry(iex.ToString(), EventLogEntryType.Error);
                }
            }
        }

        public static void LogObject(this Object obj)
        {
            var json = new JavaScriptSerializer().Serialize(obj);

            logger.Info(json);
        }

        public static bool IsEmpty<T>(this IEnumerable<T> value)
        {
            if (value == null || value.Count() == 0)
                return true;

            return false;
        }

        public static bool IsStringEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string ToString2(this object value)
        {
            return value == null ? "" : value.ToString();
        }

        public static int ToInt(this object value)
        {
            return value.ToString2() == "" ? 0 : Convert.ToInt32(value);
        }

        public static string CleanData(this string value)
        {
            value =
                value
                    .Replace("\r", "")
                    .Replace("\n", "")
                    .Replace("\t", "")
                    .Trim();

            while (value.IndexOf("  ") > 0)
                value = value.Replace("  ", " ");

            return value.Trim();
        }

        public static string ToFormatString(this DateTime? target, string format)
        {
            if (!target.HasValue || target.Value == DateTime.MinValue)
                return "";

            return target.Value.ToString(format);
        }

        public static string ToFormatString(this DateTime target, string format)
        {
            DateTime? temp = target;
            return temp.ToFormatString(format);
        }
    }
}
