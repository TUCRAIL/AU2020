using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DynamoApp.Services;
using Autodesk.AutoCAD.DynamoNodes;
using Autodesk.AutoCAD.Geometry;
using Autodesk.DesignScript.Geometry;
using Dynamo.Graph.Nodes;
using DynamoServices;
using Object = Autodesk.AutoCAD.DynamoNodes.Object;


[RegisterForTrace]
public class CadTable : Object
{
    internal CadTable(Table table, bool isDynamoOwned) : base(table, isDynamoOwned)
    {
    }

    /// <summary>
    /// Create an AutoCAD Table
    /// </summary>
    public static CadTable Create(Document document, Point insertPoint, string title, string[] columns, int columnWidth, List<string>[] data)
    {
        if (document == null) return null;
        if (insertPoint == null) return null;
        if (string.IsNullOrEmpty(title)) return null;
        if (columns == null) return null;
        if (data == null) return null;

        var AcDocument = document.AcDocument;
        var db = AcDocument.Database;

        using (var ctx = new DocumentContext(db))
        {
            var id = ElementBinder.GetObjectIdFromTrace(ctx.Database);

            var apply = new Action<Table>(table =>
            {
                table.TableStyle = db.Tablestyle;
                table.SetSize(data.Length + 2, columns.Length);
                table.SetColumnWidth(columnWidth);
                table.Position = new Point3d(insertPoint.X, insertPoint.Y, insertPoint.Z);

                for (var i = 0; i <= data.Length + 1; i++)
                {
                    if (i == 0)
                        table.Cells[i, 0].TextString = title;
                    else if (i == 1)
                        for (var j = 0; j <= columns.Length - 1; j++)
                            table.Cells[i, j].TextString = columns[j];
                    else
                        for (var j = 0; j <= columns.Length - 1; j++)
                            table.Cells[i, j].TextString = data[i - 2][j];
                }
            });

            using (var trans = db.TransactionManager.StartTransaction())
            {
                if (id.IsValid & !id.IsErased)
                {
                    var table = (Table)trans.GetObject(id, OpenMode.ForWrite, false, true);
                    apply(table);
                }
                else
                {
                    var table = new Table();
                    apply(table);
                    table.GenerateLayout();

                    var bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead, false, true);
                    var modelspace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite, false, true);

                    id = modelspace.AppendEntity(table);
                    trans.AddNewlyCreatedDBObject(table, true);
                }

                trans.Commit();
            }

            var tableObj = ctx.GetTransaction().GetObject(id, OpenMode.ForRead, false, true) as Table;
            return tableObj != null ? new CadTable(tableObj, true) : null;
        }
    }

    /// <summary>
    /// Formats an AutoCAD Table column
    /// </summary>
    public static bool FormatColumn(CadTable table, int columnIndex, int[] dataRowIndex, int red, int green, int blue)
    {
        if (table == null) return false;
        if (dataRowIndex == null) return false;

        var db = table.InternalDBObject.Database;

        using (var trans = db.TransactionManager.StartTransaction())
        {
            var formatTable = (Table)trans.GetObject(table.InternalObjectId, OpenMode.ForWrite, false, true);
            var formatTableStyle = (TableStyle)trans.GetObject(formatTable.TableStyle, OpenMode.ForRead, false, true);
            var cellColor = Autodesk.AutoCAD.Colors.Color.FromRgb(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));

            for (var i = 2; i <= formatTable.Rows.Count - 1; i++)
                if (!formatTable.Cells[i, columnIndex].IsBackgroundColorNone == true)
                    formatTable.Cells[i, columnIndex].BackgroundColor = formatTableStyle.BackgroundColor(RowType.DataRow);

            foreach (var Row in dataRowIndex)
                formatTable.Cells[Row + 2, columnIndex].BackgroundColor = cellColor;

            trans.Commit();
        }

        return true;
    }
}
