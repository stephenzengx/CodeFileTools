using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

namespace CodeFileTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class DateTimeJsonConverter : JsonConverter<DateTime>
        {
            //JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            //options.Converters.Add(new DateTimeJsonConverter());
            //var hisOut = JsonSerializer.Deserialize<FileTypeEnum>("", options: options);

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.Parse(reader.GetString());  //反序列化
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss")); //序列化
            }
        }

        private void BtnStartCreate_Click(object sender, EventArgs e)
        {
            Utils.StartCreateFile(TxtDirPath.Text);
        }
    }
}
