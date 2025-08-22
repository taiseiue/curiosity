using Android;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Java.IO;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using System.Threading.Tasks;
using AndroidApp = Android.App.Application;



public class CameraService
{
    private CameraManager _cameraManager;
    private string _cameraId;
    private CameraDevice _cameraDevice;
    private CameraCaptureSession _captureSession;
    private ImageReader _imageReader;
    private HandlerThread _backgroundThread;
    private Handler _backgroundHandler;

    public bool IsCameraAvailable => _cameraManager != null && !string.IsNullOrEmpty(_cameraId);

    public event EventHandler<byte[]> PhotoCaptured;

    public CameraService()
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        try
        {
            var context = AndroidApp.Context;
            _cameraManager = (CameraManager)context.GetSystemService(Context.CameraService);

            var cameraIds = _cameraManager.GetCameraIdList();
            if (cameraIds.Length > 0)
            {
                _cameraId = cameraIds[0]; // 背面カメラを使用
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Camera initialization error: {ex.Message}");
        }
    }


    public async Task<byte[]> CapturePhotoAsync()
    {
        try
        {
            if (!IsCameraAvailable)
            {
                throw new InvalidOperationException("Camera is not available");
            }


            return await CapturePhotoInternal();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Capture photo error: {ex.Message}");
            throw;
        }
    }

    private async Task<byte[]> CapturePhotoInternal()
    {
        var tcs = new TaskCompletionSource<byte[]>();

        try
        {
            StartBackgroundThread();

            // ImageReaderを設定
            _imageReader = ImageReader.NewInstance(1920, 1080, ImageFormatType.Jpeg, 1);

            var readerListener = new ImageAvailableListener();
            readerListener.ImageCaptured += (sender, imageData) =>
            {
                PhotoCaptured?.Invoke(this, imageData);
                tcs.TrySetResult(imageData);
            };

            _imageReader.SetOnImageAvailableListener(readerListener, _backgroundHandler);

            // カメラデバイスを開く
            var stateCallback = new CameraStateCallback();
            stateCallback.OnOpenedHandler = async (camera) =>
            {
                _cameraDevice = camera;
                await CreateCaptureSession();
            };

            stateCallback.OnErrorHandler = (camera, error) =>
            {
                tcs.TrySetException(new Exception($"Camera error: {error}"));
            };

            _cameraManager.OpenCamera(_cameraId, stateCallback, _backgroundHandler);

            return await tcs.Task;
        }
        catch (Exception ex)
        {
            tcs.TrySetException(ex);
            return await tcs.Task;
        }
    }

    private async Task CreateCaptureSession()
    {
        try
        {
            var surfaces = new List<Surface> { _imageReader.Surface };

            var sessionCallback = new CameraCaptureSessionCallback();
            sessionCallback.OnConfiguredHandler = async (session) =>
            {
                _captureSession = session;
                await CaptureStillPicture();
            };

            _cameraDevice.CreateCaptureSession(surfaces, sessionCallback, _backgroundHandler);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Create capture session error: {ex.Message}");
            throw;
        }
    }

    private async Task CaptureStillPicture()
    {
        try
        {
            var reader = _imageReader;
            var captureBuilder = _cameraDevice.CreateCaptureRequest(CameraTemplate.StillCapture);
            captureBuilder.AddTarget(reader.Surface);

            //captureBuilder.Set(CaptureRequest.ControlAfMode, (int)ControlAfMode.ContinuousPicture);
            //captureBuilder.Set(CaptureRequest.ControlAeMode, (int)ControlAeMode.On);

            var captureCallback = new CameraCaptureCallback();

            _captureSession.Capture(captureBuilder.Build(), captureCallback, _backgroundHandler);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Capture still picture error: {ex.Message}");
            throw;
        }
    }

    public async Task<string> SavePhotoAsync(byte[] photoData)
    {
        try
        {
            var fileName = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            var filePath = System.IO.Path.Combine(FileSystem.AppDataDirectory, fileName);

            await System.IO.File.WriteAllBytesAsync(filePath, photoData);
            return filePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save photo error: {ex.Message}");
            throw;
        }
    }

    private void StartBackgroundThread()
    {
        _backgroundThread = new HandlerThread("CameraBackground");
        _backgroundThread.Start();
        _backgroundHandler = new Handler(_backgroundThread.Looper);
    }

    private void StopBackgroundThread()
    {
        try
        {
            _backgroundThread?.QuitSafely();
            _backgroundThread?.Join();
            _backgroundThread = null;
            _backgroundHandler = null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Stop background thread error: {ex.Message}");
        }
    }

    public void Dispose()
    {
        try
        {
            _captureSession?.Close();
            _captureSession = null;

            _cameraDevice?.Close();
            _cameraDevice = null;

            _imageReader?.Close();
            _imageReader = null;

            StopBackgroundThread();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Dispose error: {ex.Message}");
        }
    }
}

// Helper classes
public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
{
    public event EventHandler<byte[]> ImageCaptured;

    public void OnImageAvailable(ImageReader reader)
    {
        try
        {
            using var image = reader.AcquireLatestImage();
            if (image != null)
            {
                var buffer = image.GetPlanes()[0].Buffer;
                var bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);

                ImageCaptured?.Invoke(this, bytes);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Image available error: {ex.Message}");
        }
    }
}

public class CameraStateCallback : CameraDevice.StateCallback
{
    public Action<CameraDevice> OnOpenedHandler;
    public Action<CameraDevice, int> OnErrorHandler;

    public override void OnOpened(CameraDevice camera)
    {
        OnOpenedHandler?.Invoke(camera);
    }


    public override void OnDisconnected(CameraDevice camera)
    {
        camera?.Close();
    }

    public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
    {
        OnErrorHandler?.Invoke(camera, (int)error);
    }
}

public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
{
    public Action<CameraCaptureSession> OnConfiguredHandler;

    public override void OnConfigured(CameraCaptureSession session)
    {
        OnConfiguredHandler?.Invoke(session);
    }

    public override void OnConfigureFailed(CameraCaptureSession session)
    {
        System.Diagnostics.Debug.WriteLine("Camera capture session configuration failed");
    }
}

public class CameraCaptureCallback : CameraCaptureSession.CaptureCallback
{
    public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
    {
        base.OnCaptureCompleted(session, request, result);
    }
}
