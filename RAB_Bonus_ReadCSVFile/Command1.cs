#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace RAB_Bonus_ReadCSVFile
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // 1. declare variables
            string levelPath = "C:\\Users\\micha\\OneDrive\\Documents\\Revit Add-in Bootcamp\\RAB_Bonus_Levels.csv";

            // 2. create list of string arrays for CSV data			
            List<string[]> levelData = new List<string[]>();

            // 3. read text file datas
            string[] levelArray = System.IO.File.ReadAllLines(levelPath);

            // 4. loop through file data and put into list
            foreach (string levelString in levelArray)
            {
                string[] rowArray = levelString.Split(',');
                levelData.Add(rowArray);
            }

            // 5. remove header row
            levelData.RemoveAt(0);

            // 6. create transaction
            Transaction t = new Transaction(doc);
            t.Start("Create Levels");

            // 7. loop through level data
            int counter = 0;
            foreach (string[] currentLevelData in levelData)
            {
                //8. create height variables
                double heightFeet = 0;
                double heightMeters = 0;

                //9. get height and convert from string to double
                bool convertFeet = double.TryParse(currentLevelData[1], out heightFeet);
                bool convertMeters = double.TryParse(currentLevelData[2], out heightMeters);

                //10. if using metric, convert meters to feet
                double heightMetersConvert = heightMeters * 3.28084;
                double heightMetersConvert2 = UnitUtils.ConvertToInternalUnits(heightMeters, UnitTypeId.Meters);

                //11. create level and rename
                Level currentLevel = Level.Create(doc, heightFeet);
                currentLevel.Name = currentLevelData[0];

                //12. increment counter
                counter++;
            }

            //13. commit and dispose transaction
            t.Commit();
            t.Dispose();

            //14. tell user what happend
            TaskDialog.Show("Complete", "Created " + counter.ToString() + " levels.");

            return Result.Succeeded;
        }

        public static String GetMethod()
        {
            var method = MethodBase.GetCurrentMethod().DeclaringType?.FullName;
            return method;
        }
    }
}
