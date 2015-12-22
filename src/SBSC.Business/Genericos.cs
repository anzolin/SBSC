using SBSC.Lib.Helpers;
using SBSC.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace SBSC.Business
{
    public class Genericos
    {
        public static IEnumerable<SelectListItem> GetListItemSimOuNao(int? selectValue)
        {
            return EnumsHelper.GetSelectListItems(typeof(Enumerations.Generico.SimOuNao), selectValue);
        }

        public static DateTime GetDateTimeFromBrazil()
        {
            var ptBR = new CultureInfo("pt-BR", true);

            Thread.CurrentThread.CurrentCulture = ptBR;
            Thread.CurrentThread.CurrentUICulture = ptBR;

            IFormatProvider culture = new CultureInfo("pt-BR", true);

            //DateTime date = DateTime.Now;

            //DateTime dt = DateTime.Parse(date.ToString(), ptBR);

            //return dt;

            DateTime dt = DateTime.Now;

            //var dateTime = dt.ToString(ptBR);

            //var xxxx = DateTime.Parse(dateTime, culture, DateTimeStyles.AssumeLocal);

            return dt;
        }
    }
}
