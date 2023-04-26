using static System.Runtime.InteropServices.Marshal;
using static SteamDeckControllerMapper.HidApi;
using WindowsInput;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

const double padToMouseFactor = 1 / 150.0;
const double padToScrollFactor = 1 / 500.0;

start:
try {
  // open steam controller device
  var deviceHandle = IntPtr.Zero;
  IntPtr deviceInfoPtr = hid_enumerate(SteamDeckController.vendorId, SteamDeckController.productId);
  while (deviceInfoPtr != IntPtr.Zero) {
    hid_device_info deviceInfo = PtrToStructure<hid_device_info>(deviceInfoPtr);
    string path = PtrToStringAnsi(deviceInfo.path)!;
    deviceHandle = hid_open_path(path);
    if (deviceHandle != IntPtr.Zero) break;

    deviceInfoPtr = deviceInfo.next;
  }

  if (deviceHandle == IntPtr.Zero) {
    Console.WriteLine("Steam Deck Controller not found!");
    Console.ReadKey();
    return;
  }

  void RequestFeatureReport(byte[] request) {
    if (request.Length > SteamDeckController.inputBufferLen)
      throw new ArgumentException("request length is greater than input buffer length.");

    byte[] request_full = new byte[SteamDeckController.inputBufferLen + 1];
    Array.Copy(request, 0, request_full, 1, request.Length);
    byte[] response = new byte[SteamDeckController.inputBufferLen + 1];

    int err = hid_send_feature_report(deviceHandle, request_full, (uint)(SteamDeckController.inputBufferLen + 1));
    if (err < 0) {
      throw new Exception($"Could not send report to hid device. Error: {err}");
    }
    err = hid_get_feature_report(deviceHandle, response, (uint)(SteamDeckController.inputBufferLen + 1));
    if (err < 0) {
      throw new Exception($"Could not get report from hid device. Error: {err}");
    }
  }
  /** 
   * Disable the default mouse emulation (a.k.s lizard mode). I needs to be called periodically
   * since otherwise it will return to lizard mode after a few seconds by itself.
*/
  void DisableLizardMode() {
    RequestFeatureReport(new byte[] { 0x87, 0x03, 0x08, 0x07 });
    RequestFeatureReport(new byte[] { 0x81, 0x00 });
  }

  System.Timers.Timer disableLizardModeTimer = new System.Timers.Timer(1000);
  disableLizardModeTimer.Elapsed += (s, e) => DisableLizardMode();
  disableLizardModeTimer.Start();

  // initialize input simulator
  InputSimulator inputSimulator = new InputSimulator();

  // initialize controller simulator
  var vigEmclient = new ViGEmClient();
  var xboxController = vigEmclient.CreateXbox360Controller();
  xboxController.Connect();

  // read input
  var buffer = new byte[64];
  var prevInputState = new SteamDeckController.InputState();

  byte ScaleInt16ToByte(Int16 value) {
    // First, we'll scale the Int16 range to a float range between 0 and 1
    float scaledFloat = (value - Int16.MinValue) / (float)(Int16.MaxValue - Int16.MinValue);

    // Next, we'll convert the float value to a byte by scaling it to the byte range
    byte scaledByte = (byte)(scaledFloat * byte.MaxValue);

    return scaledByte;
  }

  while (true) {
    int bytesRead = hid_read_timeout(deviceHandle, buffer, 64, 100);
    if (bytesRead > 0) {
      var inputState = new SteamDeckController.InputState(buffer);

      // simulate mouse clicks
      if (inputState.rightPadPress && !prevInputState.rightPadPress) {
        inputSimulator.Mouse.RightButtonDown();
      }
      if (!inputState.rightPadPress && prevInputState.rightPadPress) {
        inputSimulator.Mouse.RightButtonUp();
      }
      if (inputState.leftPadPress && !prevInputState.leftPadPress) {
        inputSimulator.Mouse.LeftButtonDown();
      }
      if (!inputState.leftPadPress && prevInputState.leftPadPress) {
        inputSimulator.Mouse.LeftButtonUp();
      }

      // simulate mouse movement
      if ((inputState.rightPadX != 0 && prevInputState.rightPadY != 0) ||
          (inputState.rightPadY != 0 && prevInputState.rightPadY != 0)) {
        var xDelta = inputState.rightPadX - prevInputState.rightPadX;
        var yDelta = -(inputState.rightPadY - prevInputState.rightPadY);
        inputSimulator.Mouse.MoveMouseBy(
          (int)(xDelta * padToMouseFactor),
          (int)(yDelta * padToMouseFactor)
        );
      }

      // simulate mouse scroll
      if (inputState.leftPadX != 0 && prevInputState.leftPadX != 0) {
        var xDelta = (inputState.leftPadX - prevInputState.leftPadX);
        inputSimulator.Mouse.HorizontalScroll((int)(xDelta * padToScrollFactor));
      }
      if (inputState.leftPadY != 0 && prevInputState.leftPadY != 0) {
        var yDelta = (inputState.leftPadY - prevInputState.leftPadY);
        inputSimulator.Mouse.VerticalScroll((int)(yDelta * padToScrollFactor));
      }

      // Update Xbox controller button states
      xboxController.SetButtonState(Xbox360Button.A, inputState.buttonA);
      xboxController.SetButtonState(Xbox360Button.B, inputState.buttonB);
      xboxController.SetButtonState(Xbox360Button.X, inputState.buttonX);
      xboxController.SetButtonState(Xbox360Button.Y, inputState.buttonY);

      xboxController.SetButtonState(Xbox360Button.LeftShoulder, inputState.buttonL1);
      xboxController.SetButtonState(Xbox360Button.RightShoulder, inputState.buttonR1);

      xboxController.SetButtonState(Xbox360Button.Back, inputState.buttonMenu);
      xboxController.SetButtonState(Xbox360Button.Start, inputState.buttonOptions);
      xboxController.SetButtonState(Xbox360Button.Guide, inputState.buttonSteam);

      xboxController.SetButtonState(Xbox360Button.LeftThumb, inputState.leftStickPress);
      xboxController.SetButtonState(Xbox360Button.RightThumb, inputState.rightStickPress);

      // Update Xbox controller D-Pad states
      xboxController.SetButtonState(Xbox360Button.Up, inputState.buttonDpadUp);
      xboxController.SetButtonState(Xbox360Button.Right, inputState.buttonDpadRight);
      xboxController.SetButtonState(Xbox360Button.Down, inputState.buttonDpadDown);
      xboxController.SetButtonState(Xbox360Button.Left, inputState.buttonDpadLeft);

      // Update Xbox controller trigger states
      xboxController.SetSliderValue(Xbox360Slider.LeftTrigger, ScaleInt16ToByte(inputState.leftTrigger));
      xboxController.SetSliderValue(Xbox360Slider.RightTrigger, ScaleInt16ToByte(inputState.rightTrigger));

      // Update Xbox controller thumbstick states
      xboxController.SetAxisValue(Xbox360Axis.LeftThumbX, inputState.leftStickX);
      xboxController.SetAxisValue(Xbox360Axis.LeftThumbY, inputState.leftStickY);
      xboxController.SetAxisValue(Xbox360Axis.RightThumbX, inputState.rightStickX);
      xboxController.SetAxisValue(Xbox360Axis.RightThumbY, inputState.rightStickY);

      prevInputState = inputState;
    }
  }

}
catch (Exception ex) {
  Console.WriteLine(ex.Message);
  Console.WriteLine(ex.StackTrace);
  goto start;
}
