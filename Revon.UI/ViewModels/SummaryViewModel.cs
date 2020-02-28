using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Caliburn.Micro;
using Revon.UI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revon.UI.ViewModels
{
    public partial class SummaryViewModel : PropertyChangedBase
    {


        private Document doc;
        public SortedList<string, RevonCategory> RevonCategories { get; set; }

        ShellViewModel _shell;
        public SummaryViewModel(ShellViewModel shell, UIApplication uiapp)
        {
            _shell = shell;
            doc = uiapp.ActiveUIDocument.Document;

            GetCategories();
        }

        private void GetCategories()
        {

            FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id);

            //get distinct categories of elements in the active view
            var categories = (from f in allElementsInView
                              select f.Category)
                              .Distinct(new CategoryEqualityComparer())
                                .ToList();

            // SortedList<string, Category> myCategories = new SortedList<string, Category>();
            RevonCategories = new SortedList<string, RevonCategory>();

            if (categories == null)
                return;
                        
            BuiltInCategory bic;

            foreach (Category c in categories)
            {
                bic = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), c.Id.ToString());

                RevonCategories.Add(c.Name, new RevonCategory
                {
                    CatID = c.Id.IntegerValue,
                    CatName = c.Name,
                    CatNameType = c.CategoryType.ToString(),
                    Families = GetFamiliesOfCategory(bic)
                });
            }
        }

        IList<Family> GetFamiliesOfCategory(BuiltInCategory bic)
        {
            IEnumerable<Family> families = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where<Family>(f => FamilyFirstSymbolCategoryEquals(f, bic));

            return families.ToList();
        }

        bool FamilyFirstSymbolCategoryEquals(Family f, BuiltInCategory bic)
        {
           
            ISet<ElementId> ids = f.GetFamilySymbolIds();

            Category cat = (0 == ids.Count)
              ? null
              : doc.GetElement(ids.First<ElementId>()).Category;

            return null != cat
              && cat.Id.IntegerValue.Equals((int)bic);
        }




        private void GetFamilies(string category)
        {

       
            FilteredElementCollector elementCollector = new FilteredElementCollector(doc)
                .OfClass(typeof(Family));

            //ICollection<Element> elements = collector.OfClass(typeof(Family)).ToElements();

            //StringBuilder sb = new StringBuilder();

            //foreach (Element el in elements)
            //{
            //    sb.AppendLine(el.Name);
            //}



            /////////////////////////////////////////////////////////////////
            ///
            /*
             Array bics = Enum.GetValues( 
    typeof( BuiltInCategory ) );
 
  n = bics.Length;
 
  Debug.Print( "{0} built-in categories and the "
    + "corresponding document ones:", n );
 
  Category cat;
  string s;
 
  foreach( BuiltInCategory bic in bics )
  {
    try
    {
      cat = categories.get_Item( bic );
 
      s = (null == cat)
        ? "<none>"
        : string.Format( "--> {0} ({1}", 
          cat.Name, cat.Id.IntegerValue );
    }
    catch( Exception ex )
    {
      s = ex.GetType().Name + " " + ex.Message;
    }
    Debug.Print( "  {0} {1}", bic.ToString(), s );
  }
             */



            //int num = 0;
            //using (IEnumerator<Element> enumerator = elementCollector.GetEnumerator())
            //{
            //    while (((IEnumerator)enumerator).MoveNext())
            //    {
            //        Family current = (Family)enumerator.Current;
            //        string name = ((Element)current).get_Name();
            //        try
            //        {
            //            Document document = this.doc.EditFamily(current);
            //            document.SaveAs(str + name + ".rfa");
            //            document.Close(false);
            //            this.secondBxUpdates.Items.Add((object)("\n" + ((Element)current).get_Name() + ":   Successfully saved to the folder"));
            //            this.secondBxUpdates.TopIndex = this.lstBxUpdates.Items.Count - 1;
            //            if (++num > 20)
            //                break;
            //        }
            //        catch (Exception ex)
            //        {
            //            this.failures.Add(((Element)current).get_Name() + " System Families not editable: " + ex.Message);
            //            this.secondBxUpdates.Items.Add((object)("\n" + ((Element)current).get_Name() + ":   System Families not editable"));
            //            this.secondBxUpdates.TopIndex = this.lstBxUpdates.Items.Count - 1;
            //            ++this.failed;
            //        }
            //    }

        }

        IList<Category> CategoriesToExport;
        private void SelectCategoryToExport(string category)
        {

        }

        public void ExportFamilies()
        {
            if (CategoriesToExport.Count == 0)
            {
                // export all
            }
            else
            {
                // export selected categories
                // CategoriesToExport.Export
            }
        }

        private void ExportSummary(string exportType)
        {
            switch (exportType)
            {
                case "excel":
                    break;
                case "pdf":
                    break;
            }
        }

        //private void ExportFamilies()
        //{
        //    string str = this.directoryPath1.ToString() + "/";
        //    if (string.IsNullOrEmpty(this.directoryPath1))
        //    {
        //        TaskDialog.Show("Message", "Please select a folder....");
        //    }
        //    else
        //    {
        //        FilteredElementCollector elementCollector = new FilteredElementCollector(this.doc);
        //        elementCollector.OfClass(typeof(Family));
        //        int num = 0;
        //        using (IEnumerator<Element> enumerator = elementCollector.GetEnumerator())
        //        {
        //            while (((IEnumerator)enumerator).MoveNext())
        //            {
        //                Family current = (Family)enumerator.Current;
        //                string name = ((Element)current).get_Name();
        //                try
        //                {
        //                    Document document = this.doc.EditFamily(current);
        //                    document.SaveAs(str + name + ".rfa");
        //                    document.Close(false);
        //                    this.secondBxUpdates.Items.Add((object)("\n" + ((Element)current).get_Name() + ":   Successfully saved to the folder"));
        //                    this.secondBxUpdates.TopIndex = this.lstBxUpdates.Items.Count - 1;
        //                    if (++num > 20)
        //                        break;
        //                }
        //                catch (Exception ex)
        //                {
        //                    this.failures.Add(((Element)current).get_Name() + " System Families not editable: " + ex.Message);
        //                    this.secondBxUpdates.Items.Add((object)("\n" + ((Element)current).get_Name() + ":   System Families not editable"));
        //                    this.secondBxUpdates.TopIndex = this.lstBxUpdates.Items.Count - 1;
        //                    ++this.failed;
        //                }
        //            }
        //        }
        //    }

        //}

    }

    #region Implementation of IEqualityComparer<in Category>

    class CategoryEqualityComparer : IEqualityComparer<Category>
    {
        public bool Equals(Category x, Category y)
        {
            if (x == null || y == null) return false;

            // Two items are equal if their keys are equal.
            return x.Id.Equals(y.Id);
            // return x.Id == y.Id;
        }

        public int GetHashCode(Category obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    #endregion
}
