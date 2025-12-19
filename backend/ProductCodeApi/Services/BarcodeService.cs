using ZXing;
using ZXing.Windows.Compatibility;
using ZXing.Common;

namespace ProductCodeApi.Services;

public class BarcodeService
{
    public byte[] GenerateCode39(string code)
    {
        var writer = new BarcodeWriter
        {
            Format = BarcodeFormat.CODE_39,
            Options = new EncodingOptions
            {
                Width = 600,        // เพิ่มความกว้างของบาร์โค้ด
                Height = 120,
                Margin = 10,
                PureBarcode = true // ไม่แสดงข้อความรหัสบาร์โค้ดด้านล่าง
            }
        };

        using var bitmap = writer.Write(code);
        using var stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        return stream.ToArray();
    }
}