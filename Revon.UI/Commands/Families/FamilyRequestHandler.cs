using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Revon.UI.ViewModels;

namespace Revon.UI
{
    /// <summary>
    ///   A class with methods to execute requests made by the dialog user.
    /// </summary>
    /// 
    public static class FamilyRequestHandler
    {
        // A trivial delegate, but handy
        private delegate void DoorOperation(FamilyInstance e);

        public static string familyFile { get; set; }

        public static string Message { get; set; }

        /// <summary>
        ///   The top function that distributes requests to individual methods. 
        /// </summary>
        /// 
        public static void Execute(UIApplication uiapp, RequestId reqest)
        {
            switch (reqest)
            {
                case RequestId.None:
                    {
                        return;  // no request at this time -> we can leave immediately
                    }
                case RequestId.Delete:
                    {
                        ModifySelectedDoors(uiapp, "Delete doors", e => e.Document.Delete(e.Id));
                        break;
                    }
                case RequestId.Create:
                    {
                        CreateFamily(uiapp, "Create Family", e => e.flipHand());
                        break;
                    }
                default:
                    {
                        // some kind of a warning here should
                        // notify us about an unexpected request 
                        break;
                    }
            }

            return;
        }

        private static void CreateFamily(UIApplication uiapp, String text, DoorOperation operation)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;

            if ((uidoc != null))
            {
                Family family = null;

                // Load family from file:
                using (Transaction trans = new Transaction(uidoc.Document))
                {

                    if (trans.Start(text) == TransactionStatus.Started)
                    {
                        bool isload = uidoc.Document.LoadFamily(familyFile, out family);
                        if (!isload)
                            throw new Exception("Unable to load family.");


                        //loop trough door symbols and add a new table
                        ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();
                        double x = 0.0, y = 0.0;
                        foreach (ElementId id in familySymbolIds)
                        {
                            FamilySymbol symbol = family.Document.GetElement(id) as FamilySymbol;
                            XYZ location = new XYZ(x, y, 0);

                            if (!symbol.IsActive)
                            { symbol.Activate(); uidoc.Document.Regenerate(); }

                            FamilyInstance instance = uidoc.Document.Create.NewFamilyInstance(location, symbol, StructuralType.NonStructural);

                        }

                    }

                    trans.Commit();
                }

            }
        }


        /// <summary>
        ///   The main door-modification subroutine - called from every request 
        /// </summary>
        /// <remarks>
        ///   It searches the current selection for all doors
        ///   and if it finds any it applies the requested operation to them
        /// </remarks>
        /// <param name="uiapp">The Revit application object</param>
        /// <param name="text">Caption of the transaction for the operation.</param>
        /// <param name="operation">A delegate to perform the operation on an instance of a door.</param>
        /// 
        private static void ModifySelectedDoors(UIApplication uiapp, String text, DoorOperation operation)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;

            // check if there is anything selected in the active document

            if ((uidoc != null) && (uidoc.Selection != null))
            {
                ICollection<ElementId> selElements = uidoc.Selection.GetElementIds();
                if (selElements.Count > 0)
                {
                    // Filter out all doors from the current selection

                    FilteredElementCollector collector = new FilteredElementCollector(uidoc.Document, selElements);
                    ICollection<Element> doorset = collector.OfCategory(BuiltInCategory.OST_Doors).ToElements();

                    if (doorset != null)
                    {
                        // Since we'll modify the document, we need a transaction
                        // It's best if a transaction is scoped by a 'using' block
                        using (Transaction trans = new Transaction(uidoc.Document))
                        {
                            // The name of the transaction was given as an argument

                            if (trans.Start(text) == TransactionStatus.Started)
                            {
                                // apply the requested operation to every door

                                foreach (FamilyInstance door in doorset)
                                {
                                    operation(door);
                                }

                                trans.Commit();
                            }
                        }
                    }
                }
            }
        }

    }  // class

}  // namespace
