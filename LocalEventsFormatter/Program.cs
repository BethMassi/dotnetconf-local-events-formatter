using ConvertToHTML;
using Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalEventsFormatter
{
    class Program
    {
        static void Main(string[] args)
        {

            List<MyEvent> events = new List<MyEvent>();
            string path = null;
            string outputFile = null;

            if (args.Length == 1) path = args[0];
            if (args.Length == 2)
            {
                path = args[0];
                outputFile = args[1];
            }

            if (path == null)
            {
                var defaultFileName = Environment.CurrentDirectory + "\\Events.xlsx";
                if (System.IO.File.Exists(defaultFileName)) path = defaultFileName;
            }
            if (path != null)
            {
                var excelData = new ExcelData(path);
                var allEvents = excelData.getData("Sheet");
                foreach (var row in allEvents)
                {
                    var url = row["URL"].ToString();
                    if (!url.ToLower().Contains("do not publish"))
                    {
                        var theevent = new MyEvent()
                        {
                            TheDate = ExtractShortDate(row["Date / Time"].ToString()),
                            City = GetCity(row),
                            Url = url
                        };
                        if (theevent.City != "")
                        {
                            events.Add(theevent);
                        }                        
                    }
                }
            }
            var convert = new ConvertToHTML.ConvertToHTML();

            try
            {
                string bingKey = ConfigurationManager.AppSettings["BingKey"];
                convert.BingMapsKey = bingKey;

                convert.Convert(events, outputFile);

            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        static string GetCity(DataRow row)
        {
            var city = row["City"].ToString().Trim();
            var state = row["State"].ToString().Trim();
            var country = row["Country"].ToString().Trim();
            if (state != "" && state != city)
            {
                city = string.Format("{0}, {1}", city, state);
            }
            if (country != "")
            {
                city += ", "  + country;
            }
            if (city.StartsWith(", "))
            {
                city = city.Substring(2);
            }
            return city;
        }
               
        static string ExtractShortDate(string dateString)
        {
            var theShortDate = "10/31/2017";        

            if (dateString != "")
            {
                //take the 12AM off 
                try
                {
                    var MyDate = System.Convert.ToDateTime(dateString);
                    theShortDate = MyDate.ToShortDateString();
                }
                catch (Exception)
                {                    
                }
            }
            return theShortDate; 
        }
    }   


    public class ExcelData
    {
        string _path;

        public ExcelData(string path)
        {
            _path = path;
        }


        public IExcelDataReader getExcelReader()
        {
            // ExcelDataReader works with the binary Excel file, so it needs a FileStream
            // to get started. This is how we avoid dependencies on ACE or Interop:
            FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read);

            // We return the interface, so that 
            IExcelDataReader reader = null;
            try
            {
                if (_path.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                if (_path.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                return reader;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<DataRow> getData(string sheet, bool firstRowIsColumnNames = true)
        {
            var reader = this.getExcelReader();
            reader.IsFirstRowAsColumnNames = firstRowIsColumnNames;
            var workSheet = reader.AsDataSet().Tables[sheet];
            var rows = from DataRow a in workSheet.Rows select a;
            return rows;
        }
    }
}
