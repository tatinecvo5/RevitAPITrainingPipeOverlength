using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingPipeOverlength
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var selectedRef = uidoc.Selection.PickObject(ObjectType.Element, "Выберите трубу");
            var selectedElement = doc.GetElement(selectedRef);

            if (selectedElement is Pipe)
            {
                Parameter lengthParameter = selectedElement.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH);
                if (lengthParameter.StorageType == StorageType.Double)
                {
                    double lengthValue = UnitUtils.ConvertFromInternalUnits(lengthParameter.AsDouble(), UnitTypeId.Millimeters);
                    double overLength = lengthValue * 1.1;

                    using (Transaction ts = new Transaction(doc, "Set parameters"))
                {
                    ts.Start();
                    var pipe = selectedElement as Pipe;
                    Parameter commentParameter = pipe.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);
                    commentParameter.Set($"Длина трубы с запасом 10% - {overLength.ToString()}");
                    ts.Commit();
                }

                        TaskDialog.Show("Длина трубы с запасом 10%", overLength.ToString());
                                   }
            }

            return Result.Succeeded;
        }
    }
}