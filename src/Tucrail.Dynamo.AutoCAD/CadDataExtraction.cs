using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Xsl;
using Autodesk.AutoCAD.DataExtraction;
using Autodesk.DesignScript.Runtime;
using Dynamo.Graph.Nodes;


public class CadDataExtraction
{
    private CadDataExtraction()
    {
    }

    /// <summary>
    /// Use a DXE file to perform a data extraction
    /// </summary>
    [MultiReturn("Columns", "Data", "Handles")]
    public static Dictionary<string, object> GetData(string dxeFile)
    {
        if (string.IsNullOrEmpty(dxeFile)) return null;
        if (!File.Exists(dxeFile)) return null;

        var extractionSettings = DxExtractionSettings.FromFile(dxeFile);
        var dwgExtractor = extractionSettings.DrawingDataExtractor;
        dwgExtractor.OnError += (s, e) => Debug.WriteLine($"Error: {string.Join(",", e.Filenames)}");

        var succes = dwgExtractor.ExtractData(dxeFile);
        var table = dwgExtractor.ExtractedData;

        var data = new List<List<string>>();
        var columns = new List<string>();
        var handles = new List<string>();

        for (var i = 0; i <= table.Rows.Count - 1; i++)
        {
            data.Add(new List<string>());
            handles.Add(table.Rows[i][0].ToString());

            for (var j = 2; j <= table.Columns.Count - 1; j++)
                data[i].Add(table.Rows[i][j].ToString());
        }

        for (var j = 2; j <= table.Columns.Count - 1; j++)
            columns.Add(table.Columns[j].ColumnName);

        return new Dictionary<string, object>
        {
            { "Columns", columns }, 
            { "Data", data }, 
            { "Handles", handles }
        };
    }
}
