using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revon.UI.Models
{
    public class RevonCategory
    {

        public int CatID { set; get; }

        public string CatName { set; get; }

        public List<string> CatType { set; get; }

        public bool IsSchedule { set; get; }

        public string CatNameType { set; get; }

        public string IsAnnotation { set; get; }

        public bool IsSelected { get; set; }

        public bool IsInActiveView { get; set; }

        public IList<Family> Families { get; set; }

    }
}
