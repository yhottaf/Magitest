using fantec.Common;
using fantec.Master;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace fantec.Editor
{
    public class ExcelImporter : AssetPostprocessor
    {
        class ExcelAssetInfo
        {
            public Type AssetType { get; set; }
            public ExcelAssetAttribute Attribute { get; set; }
            public string ExcelName => AssetType.Name;
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetsPaths)
        {
            var excelPathes = importedAssets.Where(path => Path.GetExtension(path) == ".xls" || Path.GetExtension(path) == ".xlsx");
            if (excelPathes.Count() == 0) return;

            var masterPathes = excelPathes.Where(path => path.Contains(AssetPath.MasterExcelDataFolderPath) && !path.Contains("~$"));
            if (masterPathes.Count() == 0) return;

            var importedList = new List<String>();

            if (EditorUtility.DisplayDialog("ExcelImporter", "マスターデータの更新を検出しました。\nインポートを開始しますか？", "はい", "いいえ"))
            {
                var cachedInfo = FindExcelAssetInfos();
                string excelName = "";

                foreach (var path in masterPathes)
                {
                    excelName = Path.GetFileNameWithoutExtension(path);

                    switch (EditorUtility.DisplayDialogComplex($"ExcelImporter", $"以下のインポートを実行しますか？\n{path}", "はい", "キャンセル", "いいえ")) // ←ボタンの並び順扱いづらい
                    {
                        case 0:
                            if (path.Contains(AssetPath.MasterExcelDataWaveFolderPath)) excelName = nameof(WaveMaster);
                            if (path.Contains(AssetPath.MasterExcelDataExpFolderPath)) excelName = nameof(ExpMaster);
                            if (path.Contains(AssetPath.MasterExcelDataGrowthFolderPath)) excelName = nameof(GrowthMaster);

                            ExcelAssetInfo info = cachedInfo.Find(i => i.ExcelName == excelName);

                            if (info == null)
                            {
                                continue;
                            }

                            ImportExcel(path, info);
                            importedList.Add(path);
                            break;

                        case 1: goto LOOPEND;   // 終了
                        case 2: break;          // 次へ
                    }
                }

            LOOPEND:

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"インポート完了。\n（クリックで詳細表示）\n{string.Join("\n", importedList)}");
            }
            else
            {
                // 何もしない
            }
        }

        static List<ExcelAssetInfo> FindExcelAssetInfos()
        {
            // NEED:名前空間をフルで取得する方法
            var spaceName = $"{nameof(fantec)}.{nameof(fantec.Master)}";
            var infoList = new List<ExcelAssetInfo>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var attributes = type.GetCustomAttributes(typeof(ExcelAssetAttribute), false);
                    if (attributes.Length == 0) continue;
                    var attribute = (ExcelAssetAttribute)attributes[0];
                    if (string.Equals(type.Namespace, spaceName, StringComparison.Ordinal) == false) continue;
                    var info = new ExcelAssetInfo()
                    {
                        AssetType = type,
                        Attribute = attribute,
                    };
                    infoList.Add(info);
                }
            }
            return infoList;
        }

        static UnityEngine.Object LoadOrCreateAsset(string assetPath, Type assetType)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(assetPath));

            var asset = AssetDatabase.LoadAssetAtPath(assetPath, assetType);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(assetType.Name);
                AssetDatabase.CreateAsset((ScriptableObject)asset, assetPath);
                asset.hideFlags = HideFlags.NotEditable;
            }

            return asset;
        }

        static IWorkbook LoadBook(string excelPath)
        {
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (Path.GetExtension(excelPath) == ".xls") return new HSSFWorkbook(stream);
                else return new XSSFWorkbook(stream);
            }
        }


        static List<IRow> TempActiveRowList = new List<IRow>();
        static List<IRow> GetActiveRowList(ISheet sheet)
        {
            TempActiveRowList.Clear();
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                var cell = row.GetCell(0);

                if (cell != null && cell.CellType != CellType.Blank)
                {
                    if (cell.StringCellValue == "#end") break;
                    if (cell.StringCellValue.StartsWith("#") == true) continue;
                }

                TempActiveRowList.Add(row);
            }

            return TempActiveRowList;
        }

        static object CellToFieldObject(IWorkbook book, ICell cell, FieldInfo fieldInfo, bool isFormulaEvalute = false)
        {
            var type = isFormulaEvalute ? cell.CachedFormulaResultType : cell.CellType;

            switch (type)
            {
                case CellType.String:
                    if (fieldInfo.FieldType.IsEnum) return Enum.Parse(fieldInfo.FieldType, cell.StringCellValue);
                    if (fieldInfo.FieldType.IsArray || IsSupportedList(fieldInfo.FieldType)) return CellToArray(cell, fieldInfo);
                    if (fieldInfo.FieldType == typeof(Vector3))
                    {
                        var vecStr = cell.StringCellValue;
                        var parts = vecStr.Split(',');
                        if (parts.Length == 3)
                        {
                            float x = float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture);
                            return new Vector3(x, y, z);
                        }
                        else
                        {
                            Debug.LogError($"Vector3形式が不正です: {vecStr}");
                            return Vector3.zero;
                        }
                    }
                    else return cell.StringCellValue;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return Convert.ChangeType(cell.NumericCellValue, fieldInfo.FieldType);
                case CellType.Formula:
                    if (isFormulaEvalute) return null;
                    return CellToFieldObject(book, cell, fieldInfo, true);
                default:
                    if (fieldInfo.FieldType.IsValueType) return Activator.CreateInstance(fieldInfo.FieldType);
                    return null;
            }
        }

        static object CellToArray(ICell cell, FieldInfo fieldInfo)
        {
            var a = cell.StringCellValue.Replace("[", "").Replace("]", "");
            var b = a.Replace("\"", "");
            var c = b.Split(',');
            Debug.Log(fieldInfo.FieldType.FullName);
            if (fieldInfo.FieldType == typeof(System.Int32[]))
            {
                return c.Select(int.Parse).ToArray();
            }
            else if (fieldInfo.FieldType == typeof(System.String[]))
            {
                return c;
            }
            else if (fieldInfo.FieldType == typeof(int[]))
            {
                return c.Select(int.Parse).ToArray();
            }
            else if (fieldInfo.FieldType == typeof(List<int>))
            {
                return c.Select(int.Parse).ToList();
            }
            else
            {
                Debug.LogError($"[{fieldInfo.FieldType.FullName}] は未対応です");
                return null;
            }
        }

        static bool IsSupportedList(Type type)
        {
            if (!type.IsGenericType) return false;
            var genericType = type.GetGenericTypeDefinition();
            return genericType == typeof(List<>);
        }

        static object CreateEntityFromRow(IWorkbook book, IRow row, List<string> columNames, Type entityType, string sheetName)
        {
            var entity = Activator.CreateInstance(entityType);

            for (int i = 0; i < columNames.Count; i++)
            {
                if (columNames[i].StartsWith("#")) continue;

                FieldInfo entityField = entityType.GetField(
                    columNames[i],
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

                if (entityField == null) continue;
                if (entityField.IsPublic == false && entityField.GetCustomAttributes(typeof(SerializeField), false).Length == 0) continue;

                ICell cell = row.GetCell(i);
                if (cell == null) continue;

                try
                {
                    object fieldValue = CellToFieldObject(book, cell, entityField);
                    entityField.SetValue(entity, fieldValue);
                }
                catch
                {
                    throw new Exception($"[列 : {row.RowNum + 1}] [行 : {cell.ColumnIndex + 1}] [カラム : {entityField.Name}] [値 : {cell}]");
                }
            }
            return entity;
        }

        static List<string> TempFieldNamesFromSheetHeaderAndIndex = new List<string>();
        static List<string> GetFieldNamesFromSheetHeaderAndIndex(List<IRow> rowList)
        {
            var rowHeader = rowList[0];

            TempFieldNamesFromSheetHeaderAndIndex.Clear();
            for (int i = 0; i < rowHeader.LastCellNum; i++)
            {
                // 先頭はコメント行とする
                if (i == 0)
                {
                    TempFieldNamesFromSheetHeaderAndIndex.Add("#");
                }
                else
                {
                    var cell = rowHeader.GetCell(i);
                    if (cell == null || cell.CellType == CellType.Blank) break;
                    TempFieldNamesFromSheetHeaderAndIndex.Add(cell.StringCellValue);
                }
            }
            return TempFieldNamesFromSheetHeaderAndIndex;
        }

        static object GetEntityListFromSheet(IWorkbook book, ISheet sheet, Type entityType)
        {
            var rowList = GetActiveRowList(sheet);
            var header = GetFieldNamesFromSheetHeaderAndIndex(rowList);

            Type listType = typeof(List<>).MakeGenericType(entityType);
            MethodInfo listAddMethod = listType.GetMethod("Add", new Type[] { entityType });
            object list = Activator.CreateInstance(listType);

            for (int i = 0; i <= rowList.Count() - 1; i++)
            {
                // 先頭はコメント行の為飛ばす
                if (i == 0) continue;

                var row = rowList[i];
                var entity = CreateEntityFromRow(book, row, header, entityType, sheet.SheetName);
                listAddMethod.Invoke(list, new object[] { entity });
            }

            return list;
        }

        static void ImportExcel(string excelPath, ExcelAssetInfo info)
        {
            string assetName = Path.GetFileNameWithoutExtension(excelPath) + ".asset";
            string assetPath = "";

            if (string.IsNullOrEmpty(info.Attribute.AssetPath))
            {
                assetPath = Path.Combine(Path.GetDirectoryName(excelPath), assetName);
            }
            else
            {
                assetPath = Path.Combine(Path.Combine(info.Attribute.AssetPath, assetName));
            }

            UnityEngine.Object asset = LoadOrCreateAsset(assetPath, info.AssetType);

            IWorkbook book = LoadBook(excelPath);
            var assetFields = info.AssetType.GetFields();

            foreach (var assetField in assetFields)
            {
                ISheet sheet = book.GetSheet(assetField.Name);
                if (sheet == null) continue;
                if (sheet.SheetName != "dataList") continue;

                Type fieldType = assetField.FieldType;
                if (fieldType.IsGenericType == false || (fieldType.GetGenericTypeDefinition() != typeof(List<>))) continue;

                Type[] types = fieldType.GetGenericArguments();
                Type entityType = types[0];

                object entitys = GetEntityListFromSheet(book, sheet, entityType);
                assetField.SetValue(asset, entitys);
            }

            EditorUtility.SetDirty(asset);
        }
    }
}