// filepath: c:\Users\palle\source\repos\DMA-P4\ReservationSystemWebAPI\ReservationSystemWebAPI\Controllers\QRCodeController.cs
using Microsoft.AspNetCore.Mvc;
using ZXing;
using System.Drawing;
using ZXing.Windows.Compatibility;

[ApiController]
[Route("api/[controller]")]
public class QRCodeController : ControllerBase
{
    [HttpPost("decode")]
    public IActionResult DecodeQRCode([FromBody] QRCodeRequest request)
    {
        try
        {
            var barcodeReader = new BarcodeReader();
            var imageBytes = Convert.FromBase64String(request.QRCodeData);
            using var ms = new MemoryStream(imageBytes);
            using var bitmap = new Bitmap(ms);
            var result = barcodeReader.Decode(bitmap);

            if (result != null)
            {
                return Ok(new { text = result.Text });
            }

            return BadRequest("No QR code detected.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error decoding QR code: {ex.Message}");
        }
    }
}

public class QRCodeRequest
{
    public string QRCodeData { get; set; }
}