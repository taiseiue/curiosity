using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Curiosity.Hub
{
    [Route("api/picture")]
    [ApiController]
    public class Picture : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        public Picture()
        {
            // アップロード用ディレクトリを作成
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] string description = "")
        {
            try
            {
                // ファイルのバリデーション
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "ファイルが選択されていません。" });
                }

                // ファイルサイズ制限（例：5MB）
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(new { message = "ファイルサイズが大きすぎます。（最大5MB）" });
                }

                // 画像ファイルのMIMEタイプチェック
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return BadRequest(new { message = "サポートされていないファイル形式です。" });
                }

                // ファイル名の生成（重複を避けるためにGUIDを使用）
                var fileExtension = Path.GetExtension(file.FileName);
                var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_uploadPath, newFileName);

                // ファイルを保存
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // レスポンス
                var response = new
                {
                    message = "ファイルのアップロードが成功しました。",
                    fileName = newFileName,
                    originalFileName = file.FileName,
                    fileSize = file.Length,
                    contentType = file.ContentType,
                    description = description,
                    uploadedAt = DateTime.Now,
                    filePath = $"/uploads/{newFileName}"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"サーバーエラー: {ex.Message}" });
            }
        }

        // 複数ファイル対応版
        [HttpPost("multiple")]
        public async Task<IActionResult> UploadMultiple(List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(new { message = "ファイルが選択されていません。" });
                }

                var uploadedFiles = new List<object>();

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var fileExtension = Path.GetExtension(file.FileName);
                        var newFileName = $"{Guid.NewGuid()}{fileExtension}";
                        var filePath = Path.Combine(_uploadPath, newFileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(stream);

                        uploadedFiles.Add(new
                        {
                            fileName = newFileName,
                            originalFileName = file.FileName,
                            fileSize = file.Length
                        });
                    }
                }

                return Ok(new { message = "複数ファイルのアップロードが成功しました。", files = uploadedFiles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"サーバーエラー: {ex.Message}" });
            }
        }

        // アップロードされた画像を表示するエンドポイント
        [HttpGet("{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_uploadPath, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "ファイルが見つかりません。" });
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var contentType = GetContentType(filePath);

                return File(fileBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"サーバーエラー: {ex.Message}" });
            }
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}
